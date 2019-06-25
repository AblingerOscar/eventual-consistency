using SyncService.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncService
{
    public delegate void OnLogHandler(ICheetahSyncService sender, OnLogHandlerArgs args);

    /// <summary>
    /// Additional interface for ISyncService that should only be accessed by Cheetah
    /// </summary>
    public interface ICheetahSyncService : ISyncService
    {
        /// <summary>
        /// Event when the ViewService wants to log something
        /// </summary>
        event OnLogHandler OnLog;

        /// <summary>
        /// THe folder in which the service saves its data
        /// </summary>
        string SavePath { get; }

        /// <summary>
        /// Adds a File to the domestic file system
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="content">Text content of the file</param>
        void AddFile(string fileName, string content);

        /// <summary>
        /// Deletes a File of the domestic file system
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        void DeleteFile(string fileName);

        /// <summary>
        /// Updates a File on the domestic file system
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="content">New text content of the file</param>
        void UpdateFile(string fileName, string content);
    }
}
