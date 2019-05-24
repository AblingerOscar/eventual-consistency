using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Gateway
{
    public class RPCGatewayClient : IDisposable
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue;
        private readonly IBasicProperties props;

        public RPCGatewayClient()
        {
            respQueue = new BlockingCollection<string>();

            ConnectionFactory connFactory = new ConnectionFactory()
            {
                HostName = RabbitMQConfiguration.HostName
            };
            connection = connFactory.CreateConnection();
            channel = connection.CreateModel();

            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            consumer.Received += (model, args) =>
            {
                var body = args.Body;
                if (args.BasicProperties.CorrelationId == correlationId)
                {
                    string response = null;

                    if(body != null)
                        response = Encoding.UTF8.GetString(body);

                    respQueue.Add(response);
                }
            };
        }

        public void Dispose()
        {
            channel.Close();
            connection.Close();
        }

        public int SendGetView(string serviceUid)
        {
            SendPublish(
                serviceUid,
                new RPCGatewayViewServiceDataObject(RPCGatewayViewServiceDataObject.RequestFunction.GET_VIEWS)
                );
            string respString = respQueue.Take();

            if (int.TryParse(respString, out int views))
                return views;
            else
                return -1;
        }

        public void SendAddViews(string serviceUid, int amount)
        {
            SendPublish(
                serviceUid,
                new RPCGatewayViewServiceDataObject(RPCGatewayViewServiceDataObject.RequestFunction.ADD_VIEWS, amount)
                );
            string respString = respQueue.Take();
            if (respString != null)
                Console.Error.WriteLine($"Got the following answer from an AddView: '{respString}'");
        }

        private void SendPublish(string serviceUid, RPCGatewayViewServiceDataObject dataObject)
        {
            channel.BasicPublish(
                exchange: "",
                routingKey: serviceUid,
                basicProperties: props,
                body: dataObject.ToBytes()
            );

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true
            );
        }
    }
}
