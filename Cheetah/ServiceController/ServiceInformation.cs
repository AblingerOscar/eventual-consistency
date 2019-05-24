using ViewService;
using Client;
using System.Threading;
using System;

namespace Cheetah.ServiceController
{
    internal class ServiceInformation
    {
        public int ID { get; }
        internal ICheetahViewService Service;
        internal IClient Client;
        public bool IsRunning { get; internal set; }

        public ServiceInformation(int ID, ICheetahViewService service, IClient client)
        {
            this.ID = ID;
            Service = service;
            Client = client;
            IsRunning = false;
        }

        public override string ToString()
        {
            string msg = $"service id: {ID}\n" + $"is active: {IsRunning}";
            if (IsRunning)
            {
                msg += $"\nEstimated view count: {Service.GetViewCount()}";
            }
            return msg;
        }
    }
}