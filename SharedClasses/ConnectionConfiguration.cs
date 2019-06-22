using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses
{
    public static class ConnectionConfiguration
    {
        public const string HostName = "http://localhost:5001/";
        public const string HEARTBEAT_SIGNAL_EXCHANGE_NAME = "heartbeat_signal";
        public const string HEARTBEAT_ANSWER_EXCHANGE_NAME = "heartbeat_answer";
    }
}
