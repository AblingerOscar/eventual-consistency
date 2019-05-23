using ViewService;
using Client;
using System.Threading;
using System;

namespace Cheetah.ServiceController
{
    internal class ServiceInformation
    {
        public int ID { get; }
        internal Thread ExecutionThread;
        internal ICheetahViewService Service;
        internal IClient Client;
        public bool IsRunning { get; internal set; }

        public override string ToString()
        {
            return $"service id: {ID}\n" +
                $"is active: {IsRunning}\n" +
                $"Estimated view count: {Service.GetViewCount()}";
        }
    }
}