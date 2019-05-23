using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheetah.ServiceController
{
    internal class ServiceController : IServiceController
    {
        private List<ServiceInformation> services;

        public IReadOnlyList<ServiceInformation> RunningServices => services.Where(s => s.IsRunning).ToList().AsReadOnly();
        public IReadOnlyList<ServiceInformation> AllServices => services.AsReadOnly();

        public ServiceController()
        {
            services = new List<ServiceInformation>();
        }

        public void AbortService(ServiceInformation si)
        {
            throw new NotImplementedException();
        }

        public ServiceInformation CreateNewService()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceInformation> SendViews(ServiceInformation si, int amount = 1)
        {
            throw new NotImplementedException();
        }

        public void StartSendingRepeatedViewsToService(ServiceInformation si, int interval, int amount = 1)
        {
            throw new NotImplementedException();
        }

        public bool StartService(ServiceInformation si)
        {
            throw new NotImplementedException();
        }

        public bool StopSendingRepeatedViewsToService(ServiceInformation si)
        {
            throw new NotImplementedException();
        }

        public bool StopService(ServiceInformation si)
        {
            throw new NotImplementedException();
        }
    }
}
