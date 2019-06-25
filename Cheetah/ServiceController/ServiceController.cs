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
            if (si.Service.IsRunning)
                si.Service.Abort();
        }

        public ServiceInformation CreateNewService()
        {
            CheckPathsExist();

            var id = IDService.GenerateNextID();
            var serviceId = IDService.GetServiceUIDForId(id);
            var si = new ServiceInformation(
                id,
                new SyncService.SyncService(
                    serviceId,
                    Path.Combine(PersistenceConfiguration.SyncDirectory, serviceId),
                    Path.Combine(PersistenceConfiguration.DBDirectory, serviceId + ".dat")
                    )
                );
            services.Add(si);

            si.Service.OnLog += (source, args) => OnServiceLog?.Invoke(this, new OnServiceLogHandlerArgs(si, args));

            return si;
        }

        public bool StartService(ServiceInformation si)
        {
            if (si.Service.IsRunning)
                return true;

            si.Service.StartUp();
            return false;
        }

        private void CheckPathsExist()
        {
            Directory.CreateDirectory(PersistenceConfiguration.SyncDirectory);
            Directory.CreateDirectory(PersistenceConfiguration.DBDirectory);
        }

        public void StopService(ServiceInformation si)
        {
            if (si.Service.IsRunning)
                si.Service.ShutDown();
        }

        public bool UploadFile(ServiceInformation si, string fileName, string localFilePath)
        {
            string content = null;
            try
            {
                content = File.ReadAllText(Path.Combine(PersistenceConfiguration.ExampleFilesDirectory, localFilePath));
            } catch(Exception)
            {
                
                return false;
            }

            si.Service.AddFile(fileName, content);
            return true;
        }

        public bool UpdateFile(ServiceInformation si, string fileName, string localFilePath)
        {
            string content = null;
            try
            {
                content = File.ReadAllText(Path.Combine(PersistenceConfiguration.ExampleFilesDirectory, localFilePath));
            } catch(Exception)
            {
                
                return false;
            }

            si.Service.UpdateFile(fileName, content);
            return true;
        }

        public void DeleteFile(ServiceInformation si, string fileName)
        {
            si.Service.DeleteFile(fileName);
        }
    }
}
