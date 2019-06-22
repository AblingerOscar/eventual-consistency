using System;
using System.Collections.Generic;
using System.Text;

namespace SyncService
{
    public interface ISyncService
    {
        /// <summary>
        /// Starts the client in the current Thread
        /// </summary>
        /// <param name="uid">The unique identifier for the Service</param>
        /// <param name="syncPath">The folder with the actual files that should get synchronized</param>
        /// <param name="savePath">The folder in which the service can save its data</param>
        void StartUp(string uid, string syncPath, string savePath);

        /// <summary>
        /// Shuts down the Service gracefully. It persists its data and might send a last update to other Services
        /// </summary>
        void ShutDown();

        /// <summary>
        /// Shuts down the Service simulating an unhandled exception or a physical failure
        /// </summary>
        void Abort();

        /// <summary>
        /// Returns whether the Service is running or stopped (not started, aborted, or shut down).
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Unique identifier
        /// </summary>
        string UID { get; }

        /// <summary>
        /// Folder in which the synchronized files are saved
        /// </summary>
        string SyncPath { get; }
    }
}
