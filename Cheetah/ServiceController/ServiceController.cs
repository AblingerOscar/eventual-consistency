using SharedClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncService;

namespace Cheetah.ServiceController
{
    internal class ServiceController : IServiceController
    {
        private List<ServiceInformation> services;

        public event OnServiceLogHandler OnServiceLog;

        public IReadOnlyList<ServiceInformation> RunningServices => services.Where(s => s.IsRunning).ToList().AsReadOnly();
        public IReadOnlyList<ServiceInformation> AllServices => services.AsReadOnly();

        public ServiceController()
        {
            services = new List<ServiceInformation>();
        }

        public void AbortService(ServiceInformation si)
        {
            si.Service.Abort();
        }

        public ServiceInformation CreateNewService()
        {
            var si = new ServiceInformation(
                IDService.GenerateNextID(),
                new SyncService.SyncService()
                );
            services.Add(si);

            si.Service.OnLog += (source, args) => OnServiceLog?.Invoke(this, new OnServiceLogHandlerArgs(si, args));

            return si;
        }

        public bool StartService(ServiceInformation si)
        {
            var serviceId = IDService.GetServiceUIDForId(si.ID);
            si.Service.StartUp(
                serviceId,
                Path.Combine(PersistenceConfiguration.SyncDirectory, serviceId),
                Path.Combine(PersistenceConfiguration.DBDirectory, serviceId)
                );
            return true;
        }

        public void StopService(ServiceInformation si)
        {
            si.Service.ShutDown();
        }
    }
}
