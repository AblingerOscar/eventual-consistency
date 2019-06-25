using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses
{
    public static class ConnectionConfiguration
    {
        public const string HEARTBEAT_SIGNAL_EXCHANGE_NAME = "heartbeat_signal";
        public const string HEARTBEAT_ANSWER_EXCHANGE_NAME = "heartbeat_answer";
        public const string METADATA_REQUEST_EXCHANGE_NAME = "metadata_request";
        public const string METADATA_ANSWER_EXCHANGE_NAME = "metadata_answer";
    }
}
