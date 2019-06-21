using SharedClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewService;

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
            si.IsRunning = false;
        }

        public ServiceInformation CreateNewService()
        {
            throw new NotImplementedException();
        }

        public bool StartService(ServiceInformation si)
        {
            throw new NotImplementedException();
        }

        public void StopService(ServiceInformation si)
        {
            si.Service.ShutDown();
            si.IsRunning = false;
        }
    }
}
