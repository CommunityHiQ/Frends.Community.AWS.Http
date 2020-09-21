using System.ComponentModel;
using System.Threading;
using Microsoft.CSharp; // You can remove this if you don't need dynamic type in .Net Standard tasks
using AwsSignatureVersion4;
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Amazon.Runtime;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;


#pragma warning disable 1591

namespace Frends.Community.AWS.Http
{
    public static class AWSHttpTasks
    {
        
        private static readonly ConcurrentDictionary<Options, HttpClient> ClientCache = new ConcurrentDictionary<Options, HttpClient>();

        public static void ClearClientCache()
        {
            ClientCache.Clear();
        }

        public static IHttpClientFactory ClientFactory = new HttpClientFactory();


        /// <summary>
        /// Generic http request task that signs the request using AWS Signature Version 4. Uses 3rd party library AwsSignatureVersion4 to perform the signing.
        /// Documentation: https://github.com/CommunityHiQ/Frends.Community.AWS.Http
        /// </summary>
        /// <param name="input">Input parameters</param>
        /// <param name="options">AWS authentication and optional parameters</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object with the following properties: string Body, Dictionary(string,string) Headers. int StatusCode</returns>   
        public static async Task<object> HttpRequestAWSSigV4([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            var httpClient = GetHttpClientForOptions(options);
            var headers = GetHeaderDictionary(input.Headers);

            using (var content = GetContent(input, headers))
            {
                using (var responseMessage = await GetHttpRequestResponseAsync(
                        httpClient,
                        input.Method.ToString(),
                        input.Url,
                        content,
                        headers,
                        options,
                        cancellationToken)
                    .ConfigureAwait(false))
                {

                    cancellationToken.ThrowIfCancellationRequested();

                    var response = new HttpResponse()
                    {
                        Body = responseMessage.Content != null ? await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) : null,
                        StatusCode = (int)responseMessage.StatusCode,
                        Headers = GetResponseHeaderDictionary(responseMessage.Headers, responseMessage.Content?.Headers)
                    };

                    if (!responseMessage.IsSuccessStatusCode && options.ThrowExceptionOnErrorResponse)
                    {
                        throw new WebException(
                            $"Request to '{input.Url}' failed with status code {(int)responseMessage.StatusCode}. Response body: {response.Body}");
                    }

                    return response;
                }
            }
        }

        private static async Task<HttpResponseMessage> GetHttpRequestResponseAsync(
            HttpClient httpClient, string method, string url,
            HttpContent content, IDictionary<string, string> headers,
            Options options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Only POST, PUT, PATCH and DELETE can have content, otherwise the HttpClient will fail
            var isContentAllowed = Enum.TryParse(method, ignoreCase: true, result: out SendMethod _);

            using (var request = new HttpRequestMessage(new HttpMethod(method), new Uri(url))
            {
                Content = isContentAllowed ? content : null,
            })
            {

                //Clear default headers
                content.Headers.Clear();
                foreach (var header in headers)
                {
                    var requestHeaderAddedSuccessfully = request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    if (!requestHeaderAddedSuccessfully && request.Content != null)
                    {
                        //Could not add to request headers try to add to content headers
                        // this check is probably not needed anymore as the new HttpClient does not seem fail on malformed headers
                        var contentHeaderAddedSuccessfully = content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                        if (!contentHeaderAddedSuccessfully)
                        {
                            Trace.TraceWarning($"Could not add header {header.Key}:{header.Value}");
                        }
                    }
                }

                var credentials = new ImmutableCredentials(options.AWSConnectionInfo.AccessKey, options.AWSConnectionInfo.SecretKey, null);

                string regionName = options.AWSConnectionInfo.Region.ToCodeString();
                string serviceName = options.AWSConnectionInfo.ServiceName;

                HttpResponseMessage response;
                try
                {
                    response = await httpClient.SendAsync(request, cancellationToken, regionName, serviceName, credentials).ConfigureAwait(false);
                }
                catch (TaskCanceledException canceledException)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        // Cancellation is from outside -> Just throw 
                        throw;
                    }

                    // Cancellation is from inside of the request, mostly likely a timeout
                    throw new Exception("HttpRequest was canceled, most likely due to a timeout.", canceledException);
                }

                // this check is probably not needed anymore as the new HttpClient does not fail on invalid charsets
                if (options.AllowInvalidResponseContentTypeCharSet && response.Content.Headers?.ContentType != null)
                {
                    response.Content.Headers.ContentType.CharSet = null;
                }

                return response;
            }
        }
        private static HttpContent GetContent(Input input, IDictionary<string, string> headers)
        {
            //Check if Content-Type exists and is set and valid
            var contentTypeIsSetAndValid = false;
            MediaTypeWithQualityHeaderValue validContentType = null;
            if (headers.TryGetValue("content-type", out string contentTypeValue))
            {
                contentTypeIsSetAndValid = MediaTypeWithQualityHeaderValue.TryParse(contentTypeValue, out validContentType);
            }

            return contentTypeIsSetAndValid
                ? new StringContent(input.Message ?? "", Encoding.GetEncoding(validContentType.CharSet ?? Encoding.UTF8.WebName))
                : new StringContent(input.Message ?? "");
        }
        private static HttpClient GetHttpClientForOptions(Options options)
        {

            return ClientCache.GetOrAdd(options, (opts) =>
            {
                // might get called more than once if e.g. many process instances execute at once,
                // but that should not matter much, as only one client will get cached
                var httpClient = ClientFactory.CreateClient(options);
                httpClient.SetDefaultRequestHeadersBasedOnOptions(opts);

                return httpClient;
            });
        }
        private static IDictionary<string, string> GetHeaderDictionary(IEnumerable<Header> headers)
        {
            //Ignore case for headers and key comparison
            return headers.ToDictionary(key => key.Name, value => value.Value, StringComparer.InvariantCultureIgnoreCase);
        }

        // Combine response- and responsecontent header to one dictionary
        private static Dictionary<string, string> GetResponseHeaderDictionary(HttpResponseHeaders responseMessageHeaders, HttpContentHeaders contentHeaders)
        {
            var responseHeaders = responseMessageHeaders.ToDictionary(h => h.Key, h => string.Join(";", h.Value));
            var allHeaders = contentHeaders?.ToDictionary(h => h.Key, h => string.Join(";", h.Value)) ?? new Dictionary<string, string>();
            responseHeaders.ToList().ForEach(x => allHeaders[x.Key] = x.Value);
            return allHeaders;
        }
    }
}
