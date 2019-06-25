using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncService.Modules
{
    public abstract class RabbitMQNode
    {
        private IConnection receiveConnection;
        private IModel receiveChannel;
        private string consumerTag;

        private string exchangeName;
        private string exchangeType;

        protected RabbitMQNode(string exchangeName, string exchangeType)
        {
            this.exchangeName = exchangeName;
            this.exchangeType = exchangeType;
        }

        protected void SendBasicPublishFanout(byte[] body)
        {
            SendBasicPublish("", body);
        }

        protected void SendBasicPublish(string routingKey, byte[] body)
        {
            var factory = new ConnectionFactory();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body
                    );
            }
        }

        #region StartListening
        protected void StartListeningFanout()
        {
            StartListening(new string[] { "" });
        }

        protected void StartListening(string[] routingKeys)
        {
            DeclareConnectionAndChannel();
            DeclareExchange();
            var queueName = DeclareQueue(routingKeys);
            RegisterConsumer(queueName);
        }

        protected abstract void OnReceiveConsumer(object sender, BasicDeliverEventArgs ea);

        private void DeclareConnectionAndChannel()
        {
            var factory = new ConnectionFactory();
            receiveConnection = factory.CreateConnection();
            receiveChannel = receiveConnection.CreateModel();
        }

        private void DeclareExchange()
        {
            receiveChannel.ExchangeDeclare(exchangeName, exchangeType);
        }

        private string DeclareQueue(string[] routingKeys)
        {
            var queueName = receiveChannel.QueueDeclare().QueueName;

            foreach (var routingKey in routingKeys)
            {
                receiveChannel.QueueBind(
                    queue: queueName,
                    exchange: exchangeName,
                    routingKey: routingKey
                    );
            }

            return queueName;
        }

        private void RegisterConsumer(string queueName)
        {
            var consumer = new EventingBasicConsumer(receiveChannel);
            consumer.Received += OnReceiveConsumer;
            ;
            consumerTag = receiveChannel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
        #endregion

        #region StopListening
        protected void StopListening()
        {
            CancelConsumer();
            DisposeConnectionAndChannel();
        }

        private void CancelConsumer()
        {
            receiveChannel.BasicCancel(consumerTag);
        }

        private void DisposeConnectionAndChannel()
        {
            receiveChannel.Dispose();
            receiveConnection.Dispose();
        }
        #endregion
    }
}
