using System;
using System.Collections.Generic;
using System.Text;

namespace ViewService
{
    public delegate void OnViewUpdateHandler(ICheetahViewService sender, OnViewUpdateHandlerArgs args);
    public delegate void OnLogHandler(ICheetahViewService sender, OnLogHandlerArgs args);

    /// <summary>
    /// Additional interface for IViewService that should only be accessed by Cheetah
    /// </summary>
    public interface ICheetahViewService: IViewService
    {
        /// <summary>
        /// Event when the view gets updated.
        /// </summary>
        event OnViewUpdateHandler OnViewUpdate;

        /// <summary>
        /// Event when the ViewService wants to log something
        /// </summary>
        event OnLogHandler OnLog;
    }
}
