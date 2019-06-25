using System.Threading;
using System;
using SyncService;
using System.Text;
using System.IO;

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
                sb.AppendLine("Last known change times of other services:");
                foreach (var changeKvp in Service.LastKnownChangeTime)
                {
                    sb.AppendLine($"\t- {changeKvp.Key}: {changeKvp.Value}");
                }

                sb.AppendLine("Known Files on the Service:");
                foreach (var fileKvp in Service.GetAllFiles())
                {
                    string output = $"\t{Path.GetFileName(fileKvp.Item1)}: " +
                        fileKvp.Item2
                            .Replace('\n', ' ')
                            .Replace("\t", "  ");
                    if (output.Length > 80)
                    {
                        output = output.Substring(0, 76) + "...";
                    }
                    sb.AppendLine(output);
                }
            }
            return sb.ToString();
        }
    }
}