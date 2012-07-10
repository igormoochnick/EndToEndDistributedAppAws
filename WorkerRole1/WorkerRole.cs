using System;
using System.Json;
using System.Net;
using System.Text;
using System.Threading;
using Amazon.SQS;
using Amazon.SQS.Model;
using SignalR.Client.Hubs;

namespace WorkerRole1
{
    class WorkerRole
    {
        static void Main(string[] args)
        {            
            // AWS: Get instance public address
            string myId = "localhost";
            try
            {
                myId = Encoding.ASCII.GetString(new WebClient().DownloadData("http://169.254.169.254/latest/meta-data/public-hostname"));
            }
            catch
            {
            }

            // AWS SQS Client
            var sqsClient = new AmazonSQSClient();

            while (true)
            {
                // Get the next message
                ReceiveMessageRequest request = new ReceiveMessageRequest()
                    .WithQueueUrl("https://queue.amazonaws.com/*****/codeCampDemo")
                    .WithMaxNumberOfMessages(1);
                var response = sqsClient.ReceiveMessage(request);

                foreach (var retrievedMessage in response.ReceiveMessageResult.Message)
                {
                    var messageJson = JsonValue.Parse(retrievedMessage.Body);

                    var message = messageJson["message"].ReadAs<string>();

                    Console.WriteLine(message);

                    message = "Echo: " + message;

                    string address = string.Format("http://{0}", messageJson["sender"].ReadAs<string>());
                    var connection = new HubConnection(address);
                    connection.Start().Wait();

                    IHubProxy pongHub = connection.CreateProxy("MvcWebRole1.Hubs.EchoHub");
                    pongHub.Invoke("DoUpdateMessage", message, myId).Wait();

                    //Process the message and then delete the message
                    DeleteMessageRequest deleteRequest = new DeleteMessageRequest()
                        .WithQueueUrl("https://queue.amazonaws.com/******/codeCampDemo")
                        .WithReceiptHandle(retrievedMessage.ReceiptHandle);
                    sqsClient.DeleteMessage(deleteRequest);
                }

                Thread.Sleep(1000);
            }
        }
    }
}
