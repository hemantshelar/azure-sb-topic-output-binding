using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace azure_sb_topic_output_binding
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly ServiceBusClient _client;

        public Function1(ILogger<Function1> logger, ServiceBusClient client)
        {
            _logger = logger;
            _client = client;
        }

        [Function(nameof(Function1))]
        [ServiceBusOutput("test-topic", Connection = "CONN")]
        public async Task Run(
            [ServiceBusTrigger("test-queue", Connection = "CONN")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions
            )
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            #region This is working but not able to set custom properties
            OutputData outputData = new OutputData
            {
                ID = 123,
                Name = "Test"
            };
            /*
            //Following return 
            //Is there any way to set custom properties of this message?
            //Along with custom property, I would also like to set messageProperty contentType to application/json

            return outputData;
            */
            #endregion

            #region BrokeredMessage not working
            /*
            //As per sloution mentioned in below link, I tried to set custom properties but it is not working.
            //https://stackoverflow.com/questions/50457428/custom-message-properties-on-azure-queue-topic-message-from-azure-function

            BrokeredMessage brokeredMessage = new();
            brokeredMessage.ContentType = "application/json";
            brokeredMessage.Properties.Add("ID", 123);
            brokeredMessage.Properties.Add("Name", "Test");
            //Injecting  ICollector<BrokeredMessage>  notworking as its always null.
            //collector.Add(brokeredMessage);
            */
            #endregion


            #region ServiceBusMessage not working
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(outputData))
            {
                ContentType = "application/json"
            };

            serviceBusMessage.ContentType = "application/json";
            serviceBusMessage.ApplicationProperties.Add("ID", 123);
            serviceBusMessage.ApplicationProperties.Add("Name", "Test");
            var sender = _client.CreateSender("test-topic").SendMessageAsync(serviceBusMessage);
            #endregion
            // Complete the message
            await messageActions.CompleteMessageAsync(message);


        }
    }
}
