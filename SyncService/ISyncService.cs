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
        void StartUp();

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

        /// <summary>
        /// The last known change times for each other service, not including its own
        /// </summary>
        IDictionary<string, DateTime> LastKnownChangeTime { get; }

        /// <summary>
        /// Returns all files that are currently saved and their contents
        /// </summary>
        /// <returns>A list of tuples, where the first value is the file name and the second the content</returns>
        IList<Tuple<string, string>> GetAllFiles();
    }
}
