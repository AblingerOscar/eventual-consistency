using Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewService;

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
            si.Service.Abort();
            si.IsRunning = false;
        }

        public ServiceInformation CreateNewService()
        {
            ICheetahViewService service = null; //new ViewService(); TODO
            IClient client = null; //new Client();
            return new ServiceInformation(IDService.GenerateNextID(), service, client);
        }

        public Task<ServiceInformation> SendViews(ServiceInformation si, int amount = 1)
        {
            si.Client.AddViews(amount);
        }

        public void StartSendingRepeatedViewsToService(ServiceInformation si, int interval, int amount = 1)
        {
            si.Client.StartPeriodicRequests(amount, interval);
        }

        public bool StartService(ServiceInformation si)
        {
            if (si.IsRunning)
                return true;

            si.Service.StartUp(IDService.GetServiceUIDForId(si.ID), "TODO");
            si.IsRunning = true;
            return false;
        }

        public bool StopSendingRepeatedViewsToService(ServiceInformation si)
        {
            si.Client.StopPeriodicRequests();
        }

        public bool StopService(ServiceInformation si)
        {
            si.Service.ShutDown();
            si.IsRunning = false;
        }
    }
}
