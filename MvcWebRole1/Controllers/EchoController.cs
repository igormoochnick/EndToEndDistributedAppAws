using System.Json;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace MvcWebRole1.Controllers
{
    public class EchoController : ApiController
    {
        // POST /api/echo
        public void Post(string message)
        {            
            // AWS: Get instance public address
            string myAddress = HttpContext.Current.Request.Url.Authority;
            try
            {
                myAddress = Encoding.ASCII.GetString(new WebClient().DownloadData("http://169.254.169.254/latest/meta-data/public-hostname"));
            }
            catch
            {
            }

            var messageJob = new JsonObject();
            messageJob["message"] = message;
            messageJob["sender"] = myAddress;

            // AWS SQS Client
            var sqsClient = new AmazonSQSClient();

            SendMessageRequest request = new SendMessageRequest()
                .WithQueueUrl("https://queue.amazonaws.com/******/codeCampDemo")
                .WithMessageBody(messageJob.ToString());
            sqsClient.SendMessage(request);
        }

    }
}
