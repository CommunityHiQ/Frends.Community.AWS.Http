using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Community.AWS.Http.Tests
{
    [TestFixture]
    class TestClass
    {
        
        public string TestUrl           = Environment.GetEnvironmentVariable("HIQ_AWSGATEWAY_URL");
        public string TestAccessKey     = Environment.GetEnvironmentVariable("HIQ_AWSGATEWAY_ACCESSKEY");
        public string TestSecretKey     = Environment.GetEnvironmentVariable("HIQ_AWSGATEWAY_SECRETKEY");
        public string TestServiceName   = Environment.GetEnvironmentVariable("HIQ_AWSGATEWAY_SERVICENAME");
        public Regions TestRegion       = (Regions)int.Parse(Environment.GetEnvironmentVariable("HIQ_AWSGATEWAY_REGION")); //19 = US EAST 1

        [Test]
        public async Task HttpRequestSuccess()
        {
           
            var input = new Input
            { Method = Method.GET, Url = TestUrl, Headers = new Header[0], Message = "" };
            var options = new Options { ConnectionTimeoutSeconds = 60, AWSConnectionInfo = new AWSConnectionInfo() { AccessKey = TestAccessKey, SecretKey = TestSecretKey, ServiceName = TestServiceName, Region = TestRegion } };

            var result = (HttpResponse)await AWSHttpTasks.HttpRequestWithAWSSigV4(input, options, CancellationToken.None);

            Assert.AreEqual(200, result.StatusCode);
        }

    }
}
