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
            return $"service id: {ID}\n" +
                $"is active: {IsRunning}\n" +
                $"Estimated view count: {Service.GetViewCount()}";
        }
    }
}