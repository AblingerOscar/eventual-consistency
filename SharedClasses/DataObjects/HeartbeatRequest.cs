using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses.DataObjects
{
    public class HeartbeatRequest
    {
        public string SenderUID;
        public IDictionary<string, DateTime> KnownChanges
            = new Dictionary<string, DateTime>();

        public byte[] ToBytes()
        {
            var json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }

        public static HeartbeatRequest FromBytes(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<HeartbeatRequest>(json);
        }
    }
}
