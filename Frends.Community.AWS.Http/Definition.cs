#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace Frends.Community.AWS.Http
{
    public class AWSConnectionInfo
    {
        /// <summary>
        /// AWS_IAM access key
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string AccessKey { get; set; }

        /// <summary>
        /// AWS_IAM secret key
        /// </summary>
        [DisplayFormat(DataFormatString = "Expression")]
        [PasswordPropertyText]
        public string SecretKey { get; set; }

        /// <summary>
        /// AWS region
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public Regions Region { get; set; }

        /// <summary>
        /// Name of the service
        /// </summary>
        [DefaultValue("execute-api")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ServiceName { get; set; }

    }


    public class Input
    {
        /// <summary>
        /// The HTTP Method to be used with the request.
        /// </summary>
        public Method Method { get; set; }

        /// <summary>
        /// The URL with protocol and path. You can include query parameters directly in the url.
        /// </summary>
        [DefaultValue("https://example.org/path/to")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Url { get; set; }

        /// <summary>
        /// The message text to be sent with the request.
        /// </summary>
        [UIHint(nameof(Method), "", Method.POST, Method.DELETE, Method.PATCH, Method.PUT)]
        public string Message { get; set; }

        /// <summary>
        /// List of HTTP headers to be added to the request.
        /// </summary>
        public Header[] Headers { get; set; }
    }

    public enum Method
    {
        GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS, CONNECT
    }

    /// <summary>
    /// Allowed methods for sending content
    /// </summary>
    public enum SendMethod
    {
        POST, PUT, PATCH, DELETE
    }


    public class HttpResponse
    {
        public string Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public int StatusCode { get; set; }
    }
    public class Header
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public interface IHttpClientFactory
    {
        HttpClient CreateClient(Options options);
    }
    public class HttpClientFactory : IHttpClientFactory
    {

        public HttpClient CreateClient(Options options)
        {
            var handler = new HttpClientHandler();
            handler.SetHandlerSettingsBasedOnOptions(options);
            return new HttpClient(handler);
        }
    }
    public enum Regions
    {
        AfSouth1,
        ApEast1,
        ApNortheast1,
        ApNortheast2,
        ApNortheast3,
        ApSouth1,
        ApSoutheast1,
        ApSoutheast2,
        CaCentral1,
        CnNorth1,
        CnNorthWest1,
        EuCentral1,
        EuNorth1,
        EuSouth1,
        EuWest1,
        EuWest2,
        EuWest3,
        MeSouth1,
        SaEast1,
        UsEast1,
        UsEast2,
        UsWest1,
        UsWest2

    }
    public class Options : IEquatable<Options>
    {
        /// <summary>
        /// Method of authenticating request
        /// </summary>
        public AWSConnectionInfo AWSConnectionInfo { get; set; }

        /// <summary>
        /// Timeout in seconds to be used for the connection and operation.
        /// </summary>
        [DefaultValue(30)]
        public int ConnectionTimeoutSeconds { get; set; }

        /// <summary>
        /// If FollowRedirects is set to false, all responses with an HTTP status code from 300 to 399 is returned to the application.
        /// </summary>
        [DefaultValue(true)]
        public bool FollowRedirects { get; set; }

        /// <summary>
        /// Do not throw an exception on certificate error.
        /// </summary>
        public bool AllowInvalidCertificate { get; set; }

        /// <summary>
        /// Some Api's return faulty content-type charset header. This setting overrides the returned charset.
        /// </summary>
        public bool AllowInvalidResponseContentTypeCharSet { get; set; }
        /// <summary>
        /// Throw exception if return code of request is not successfull
        /// </summary>
        public bool ThrowExceptionOnErrorResponse { get; set; }

        /// <summary>
        /// If set to false, cookies must be handled manually. Defaults to true.
        /// </summary>
        [DefaultValue(true)]
        public bool AutomaticCookieHandling { get; set; } = true;

        public bool Equals(Options other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(AWSConnectionInfo.AccessKey, other.AWSConnectionInfo.AccessKey) &&
                   string.Equals(AWSConnectionInfo.SecretKey, other.AWSConnectionInfo.SecretKey) &&
                   ConnectionTimeoutSeconds == other.ConnectionTimeoutSeconds &&
                   FollowRedirects == other.FollowRedirects &&
                   AllowInvalidCertificate == other.AllowInvalidCertificate &&
                   AllowInvalidResponseContentTypeCharSet == other.AllowInvalidResponseContentTypeCharSet &&
                   ThrowExceptionOnErrorResponse == other.ThrowExceptionOnErrorResponse &&
                   AutomaticCookieHandling == other.AutomaticCookieHandling;
        }
    }
}
