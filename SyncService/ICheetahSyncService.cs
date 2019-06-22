using System;
using System.Collections.Generic;
using System.Text;

namespace SyncService
{
    public delegate void OnLogHandler(ICheetahSyncService sender, OnLogHandlerArgs args);

    /// <summary>
    /// Additional interface for ISyncService that should only be accessed by Cheetah
    /// </summary>
    public interface ICheetahSyncService: ISyncService
    {
        /// <summary>
        /// Event when the ViewService wants to log something
        /// </summary>
        event OnLogHandler OnLog;

        /// <summary>
        /// THe folder in which the service saves its data
        /// </summary>
        string SavePath { get; }
    }
}
