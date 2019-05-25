using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;

namespace ViewService
{
    public class ViewService : ICheetahViewService
    {   
        private string ServiceId { get; set; }
        private bool IsRunning { get; set; } = false;
        private string FilePath;
        private ViewDataObject viewDO;

        private bool viewsChanged = true;

        private RPCViewServiceServer rpcServer = null;
        private IConnection Connection { get; set; } = null;
        private IModel Channel { get; set; } = null;

        private Timer viewUpdateTimer = null;

        private readonly static object addViewsLock = new object();

        public event OnViewUpdateHandler OnViewUpdate;
        public event OnLogHandler OnLog;

        public void Abort()
        {
            if (IsRunning)
            {
                IsRunning = false;
                viewUpdateTimer.Dispose();
                viewUpdateTimer = null;
                Connection.Close();

                rpcServer.Dispose();
                rpcServer = null;
            }
        }

        public void ShutDown()
        {
            if (IsRunning)
            {
                IsRunning = false;
                viewUpdateTimer.Dispose();
                viewUpdateTimer = null;
                Connection.Close();

                rpcServer.Dispose();
                rpcServer = null;

                PersistData();
            }
        }

        public int GetViewCount()
        {
            if (IsRunning)
            {
                lock (addViewsLock)
                {
                    return viewDO.TotalViews;
                }
            } else
            {
                throw new NotSetupException(
                    "Tried to get view count on stopped service");
            }
        }

        private void PersistData()
        {
            string json = viewDO.ToJson();
            File.WriteAllText(FilePath, json);
        }

        public void StartUp(string uid, string contextPath)
        {
            ServiceId = uid;
            IsRunning = true;
            viewsChanged = true;

            FilePath = contextPath;

            if (!File.Exists(FilePath))
                viewDO = new ViewDataObject();
            else
            {
                string json = File.ReadAllText(FilePath);
                viewDO = ViewDataObject.FromJson(json);
            }

            // Communication between Gateway and ViewService
            InitializeRPCServer();

            // Communication and synchronization between ViewServices
            InitializeServiceCommunication();

            // Syncronization between ViewServices are triggered by timer interval
            InitializeUpdateTimer();
        }

        private void InitializeRPCServer()
        {
            rpcServer = new RPCViewServiceServer(
                ServiceId,
                (msg) => {
                    OnLog?.Invoke(this, new OnLogHandlerArgs(msg, LogReason.ERROR, OutputLevel.ERROR));
                },
                GetViewCount,
                number => AddViews(number ?? 1)
            );
        }

        private void InitializeServiceCommunication()
        {
            var factory = new ConnectionFactory() { HostName = RabbitMQConfiguration.HostName };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            Channel.ExchangeDeclare(exchange: RabbitMQConfiguration.ChannelExchangeName, type: "fanout");

            BindServiceConnsumer();
        }

        private void BindServiceConnsumer()
        {
            var queueName = Channel.QueueDeclare().QueueName;
            Channel.QueueBind(
                queue: queueName,
                exchange: RabbitMQConfiguration.ChannelExchangeName,
                routingKey: ""
            );

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += HandleSyncViews;

            Channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer
            );
        }

        private void HandleSyncViews(object model, BasicDeliverEventArgs eventArgs)
        {
            var body = ViewCountSyncDataObject.FromBytes(eventArgs.Body);

            if (body.SenderServiceId == ServiceId) { return; }
            
            lock (addViewsLock) {
                if (viewDO.Views.ContainsKey(body.SenderServiceId))
                {
                    viewDO.Views[body.SenderServiceId] = body.Views;
                } else
                {
                    viewDO.Views.Add(body.SenderServiceId, body.Views);
                }
            }

            OnLog?.Invoke(this, new OnLogHandlerArgs($"Received {body.SenderServiceId}'s views ({body.Views})", LogReason.DEBUG));
        }

        private void InitializeUpdateTimer()
        {
            if (viewUpdateTimer != null)
            {
                viewUpdateTimer.Dispose();
                viewUpdateTimer = null;
            }
            viewUpdateTimer = new Timer(SaveAndBroadcastViews, null, 0, RabbitMQConfiguration.SyncUpdateInterval);
        }

        private void SaveAndBroadcastViews(object args)
        {
            PersistData();
            if (viewsChanged)
                // no locking here, because race condition is not problematic (only update delay)
            {
                lock (addViewsLock)
                {
                    viewsChanged = false;
                }
                BroadcastViewCount();
            }
        }

        private void BroadcastViewCount()
        {
            var body = new ViewCountSyncDataObject(ServiceId, viewDO.OwnViews).ToBytes();

            Channel.BasicPublish(
                exchange: "view-count-syncs",
                routingKey: "",
                basicProperties: null,
                body: body
            );

            OnViewUpdate?.Invoke(this, new OnViewUpdateHandlerArgs(viewDO.OwnViews));
            OnLog?.Invoke(this, new OnLogHandlerArgs($"Sent {viewDO.OwnViews}", LogReason.DEBUG));
        }

        public void AddViews(int number = 1)
        {
            lock (addViewsLock)
            {
                viewsChanged = true;
                viewDO.OwnViews += number;
            }
        }

        private void AddViewsFromOther(string serviceId, int number = 1)
        {
            lock (addViewsLock)
            {
                if (ServiceId  == serviceId)
                {
                    viewsChanged = true;
                    viewDO.OwnViews += number;
                } else if (viewDO.Views.ContainsKey(serviceId))
                {
                    viewDO.Views[serviceId] += number;
                } else
                {
                    viewDO.Views.Add(serviceId, number);
                }
            }
        }

        bool IViewService.IsRunning()
        {
            return IsRunning;
        }
    }
}
