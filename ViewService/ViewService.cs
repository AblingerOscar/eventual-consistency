using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using SharedClasses;

namespace ViewService
{
    public class ViewService : ICheetahViewService
    {
        private string ServiceId { get; set; }
        private int ViewCount { get; set; } = 0;
        private bool IsRunning { get; set; } = false;

        private IConnection Connection { get; set; } = null;
        private IModel Channel { get; set; } = null;

        private Timer viewUpdateTimer = null;

        private IDictionary<string, int> views = new Dictionary<string, int>();

        private readonly static object addViewsLock = new object();

        public event OnViewUpdateHandler OnViewUpdate;
        public event OnLogHandler OnLog;

        public void Abort()
        {
            if (IsRunning)
            {
                IsRunning = false;
                Connection.Close();
            }
        }

        public int GetViewCount()
        {
            if (IsRunning)
            {
                return ViewCount;
            } else
            {
                throw new NotSetupException(
                    "Tried to get view count on stopped service");
            }
        }

        public void ShutDown()
        {
            if (IsRunning)
            {
                IsRunning = false;
                // TODO: write to db before exiting
                Connection.Close();
            }            
        }

        public void StartUp(string uid, string contextPath)
        {
            ServiceId = uid;
            IsRunning = true;

            // TODO: context path is DB location uri

            // TODO: read viewcount from local db
            ViewCount = 0;

            // TODO: AddRPCViewServiceClientLogic();

            DeclareChannel();
            InitializeViewCountUpdaterTimer();
        }

        private void DeclareChannel()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            Connection = factory.CreateConnection();

            Channel = Connection.CreateModel();
            Channel.QueueDeclare("service-queue", false, false, false, null);
        }

        private void InitializeViewCountUpdaterTimer()
        {
            if (viewUpdateTimer != null)
            {
                viewUpdateTimer.Dispose();
                viewUpdateTimer = null;
            }
            viewUpdateTimer = new Timer(BroadcastViewCount, null, 0, 5000);
        }

        private void BroadcastViewCount(object args)
        {
            Channel.BasicPublish("", "routing-key", null, Encoding.UTF8.GetBytes("test"));
            OnViewUpdate?.Invoke(this, new OnViewUpdateHandlerArgs(ViewCount));
            OnLog?.Invoke(this, new OnLogHandlerArgs($"Sent {ViewCount}", LogReason.DEBUG));
        }

        public void AddViews(int number = 1)
        {
            lock (addViewsLock)
            {
                if (views.ContainsKey(ServiceId))
                {
                    views[ServiceId]++;
                }
                ViewCount += number;
            }
        }

        bool IViewService.IsRunning()
        {
            return IsRunning;
        }
    }
}
