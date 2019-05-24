using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ViewService
{
    public class RPCViewServiceServer: IDisposable
    {
        IConnection connection;
        IModel channel;

        public RPCViewServiceServer(
            string viewServiceUid,
            Action<string> onError,
            Func<int> getViews,
            Action<int?> addViews
            )
        {
            var factory = new ConnectionFactory()
            {
                HostName = RabbitMQConfiguration.HostName
            };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: viewServiceUid,
                durable: false,
                exclusive: true,
                autoDelete: false,
                arguments: null
                );
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(
                queue: viewServiceUid,
                autoAck: false,
                consumer: consumer
                );

            consumer.Received += (model, args) =>
            {
                byte[] responseBytes = null;

                var body = args.Body;
                var props = args.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var dataObject = RPCGatewayViewServiceDataObject.FromBytes(body);
                    switch(dataObject.RequestedFunction)
                    {
                        case RPCGatewayViewServiceDataObject.RequestFunction.GET_VIEWS:
                            int viewAmount = getViews.Invoke();
                            responseBytes = Encoding.UTF8.GetBytes(viewAmount.ToString());
                            break;
                        case RPCGatewayViewServiceDataObject.RequestFunction.ADD_VIEWS:
                            addViews.Invoke(dataObject.views);
                            break;
                    }
                    // do stuff
                }
                catch (Exception e)
                {
                    onError?.Invoke(e.Message);
                }
                finally
                {
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: props.ReplyTo,
                        basicProperties: replyProps,
                        body: responseBytes
                        );
                    channel.BasicAck(
                        deliveryTag: args.DeliveryTag,
                        multiple: false
                        );
                }
            };
        }

        public void Dispose()
        {
            channel.Close();
            connection.Close();
        }
    }
}
