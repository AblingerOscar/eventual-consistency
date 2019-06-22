namespace SyncService
{
    public enum OutputLevel
    {
        INFO, ERROR
    }

    public enum LogReason
    {
        DEBUG, VIEWINCREASE, ERROR
    }

    public class OnLogHandlerArgs
    {
        public string Text;

        /// <summary>
        /// The reason why it wants to log.
        /// Cheetah will later allow (de-)activating some logs based on this
        /// </summary>
        public LogReason Reason = LogReason.DEBUG;

        /// <summary>
        /// Wether the Log should be an error or not
        /// </summary>
        public OutputLevel OutputLevel = OutputLevel.INFO;

        public OnLogHandlerArgs(string text, LogReason reason, OutputLevel outputLevel = OutputLevel.INFO)
        {
            Text = text;
            Reason = reason;
            OutputLevel = outputLevel;
        }
    }
}