using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewService;

namespace Cheetah.ServiceController
{
    internal delegate void OnServiceLogHandler(IServiceController source, OnServiceLogHandlerArgs args);

    internal interface IServiceController
    {
        IReadOnlyList<ServiceInformation> RunningServices { get; }
        IReadOnlyList<ServiceInformation> AllServices { get; }

        /// <summary>
        /// The piped log event from any viewService
        /// </summary>
        event OnServiceLogHandler OnServiceLog;

        /// <summary>
        /// Creates not only the service, but also a client for it
        /// </summary>
        /// <returns>The information object of the new Service</returns>
        ServiceInformation CreateNewService();
        /// <summary>
        /// Starts an existing Service.
        /// Does nothing if the service is already started
        /// </summary>
        /// <param name="si">The service information for the service to start</param>
        /// <returns>Whether the service was running</returns>
        bool StartService(ServiceInformation si);
        /// <summary>
        /// Stops an existing Service
        /// </summary>
        /// <param name="si">The service information for the service to stop</param>
        void StopService(ServiceInformation si);
        /// <summary>
        /// Aborts a service
        /// </summary>
        /// <param name="si">The service information for the service to abort</param>
        void AbortService(ServiceInformation si);
    }
}
