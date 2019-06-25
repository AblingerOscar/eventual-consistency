using System.Threading;
using System;
using SyncService;
using System.Text;

namespace Cheetah.ServiceController
{
    internal class ServiceInformation
    {
        public int ID { get; }
        public ICheetahSyncService Service;

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
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Service id: {ID}\n" + $"is active: {IsRunning}");
            if (IsRunning)
            {
                sb.AppendLine($"\tLast known change times of other services:");
                foreach (var changeKvp in Service.LastKnownChangeTime)
                {
                    sb.AppendLine($"\t- {changeKvp.Key}: {changeKvp.Value}");
                }
            }
            return sb.ToString();
        }
    }
}