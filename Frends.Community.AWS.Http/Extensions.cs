using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Frends.Community.AWS.Http
{
    public static class Extensions
    {
        internal static string ToCodeString(this Regions reg)
        {
            switch (reg)
            {
                case Regions.EuNorth1:
                    return "eu-north-1";
                case Regions.EuWest1:
                    return "eu-west-1";
                case Regions.EuWest2:
                    return "eu-west-2";
                case Regions.EuWest3:
                    return "eu-west-3";
                case Regions.EuCentral1:
                    return "eu-central-1";
                case Regions.EuSouth1:
                    return "eu-south-1";
                case Regions.ApNortheast1:
                    return "ap-northeast-1";
                case Regions.ApNortheast2:
                    return "ap-northeast-2";
                case Regions.ApSouth1:
                    return "ap-south-1";
                case Regions.ApSoutheast1:
                    return "ap-southeast-1";
                case Regions.ApSoutheast2:
                    return "ap-southeast-2";
                case Regions.CaCentral1:
                    return "ca-central-1";
                case Regions.CnNorth1:
                    return "cn-north-1";
                case Regions.SaEast1:
                    return "sa-east-1";
                case Regions.UsEast1:
                    return "us-east-1";
                case Regions.UsEast2:
                    return "us-east-2";
                case Regions.UsWest1:
                    return "us-west-1";
                case Regions.UsWest2:
                    return "us-west-2";
                default:
                    throw new ArgumentException("Unknown region");
            }
        }
        internal static void SetDefaultRequestHeadersBasedOnOptions(this HttpClient httpClient, Options options)
        {
            //Do not automatically set expect 100-continue response header
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json");
            httpClient.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(options.ConnectionTimeoutSeconds));
        }

        internal static void SetHandlerSettingsBasedOnOptions(this HttpClientHandler handler, Options options)
        {

            handler.AllowAutoRedirect = options.FollowRedirects;
            handler.UseCookies = options.AutomaticCookieHandling;

            if (options.AllowInvalidCertificate)
            {
                handler.ServerCertificateCustomValidationCallback = (a, b, c, d) => true;
            }
        }
    }
}
