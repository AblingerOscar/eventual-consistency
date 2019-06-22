using SyncService;
using System.Threading;
using System;

namespace Cheetah.ServiceController
{
    internal class ServiceInformation
    {
        public int ID { get; }
        internal ICheetahSyncService Service;



        public bool IsRunning {
            get {
                return Service.IsRunning;
            }
        }

        public ServiceInformation(int ID, ICheetahSyncService service)
        {
            this.ID = ID;
            Service = service;
        }

        public override string ToString()
        {
            string msg = $"service id: {ID}\n" + $"is active: {IsRunning}";
            if (IsRunning)
            {
            }
            throw new NotImplementedException();
            return msg;
        }
    }
}