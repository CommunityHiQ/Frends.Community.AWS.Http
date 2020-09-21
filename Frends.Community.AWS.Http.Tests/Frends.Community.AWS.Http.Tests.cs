using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Community.AWS.Http.Tests
{
    [TestFixture]
    class TestClass
    {

        /******** Fill the variables below to execute the test ********/
        public string TestUrl           = "";
        public string TestAccessKey     = "";
        public string TestSecretKey     = "";
        public string TestServiceName   = "";
        public Regions TestRegion       = Regions.UsEast1;
        /**************************************************************/

        [Test]
        [Ignore("Needs AWS API")]
        public async Task HttpRequestSuccess()
        {
           
            var input = new Input
            { Method = Method.GET, Url = TestUrl, Headers = new Header[0], Message = "" };
            var options = new Options { ConnectionTimeoutSeconds = 60, AWSConnectionInfo = new AWSConnectionInfo() { AccessKey = TestAccessKey, SecretKey = TestSecretKey, ServiceName = TestServiceName, Region = TestRegion } };

            var result = (HttpResponse)await AWSHttpTasks.HttpRequestAWSSigV4(input, options, CancellationToken.None);

            Assert.AreEqual(200, result.StatusCode);
        }

    }
}
