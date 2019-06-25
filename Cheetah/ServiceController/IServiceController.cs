using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SyncService;

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
        /// <summary>
        /// Uploads the text content of a local file to a service
        /// </summary>
        /// <param name="si">The service information for the service</param>
        /// <param name="fileName">The name the new file on the service should have</param>
        /// <param name="localFilePath">The path to a file in the local 'exampleFiles' folder</param>
        /// <returns>False if the local file could not be found</returns>
        bool UploadFile(ServiceInformation si, string fileName, string localFilePath);
        /// <summary>
        /// Updates a file that's on a service with new content
        /// </summary>
        /// <param name="si">The service information for the service</param>
        /// <param name="fileName">The name of the file on the service</param>
        /// <param name="localFilePath">The path to a file in the local 'exampleFiles' folder</param>
        /// <returns>False if the local file could not be found</returns>
        bool UpdateFile(ServiceInformation si, string fileName, string localFilePath);
        /// <summary>
        /// Deletes a file on the service
        /// </summary>
        /// <param name="si">The service information for the service</param>
        /// <param name="fileName">The name of the file on the service</param>
        void DeleteFile(ServiceInformation si, string fileName);
    }
}
