using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses
{
    public static class RabbitMQConfiguration
    {
        public const string HostName = "localhost";
        public const string ChannelExchangeName = "view-count-syncs";
        public const int SyncUpdateInterval = 5000;
    }
}
