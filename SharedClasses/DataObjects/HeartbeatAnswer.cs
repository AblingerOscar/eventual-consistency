using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses.DataObjects
{
    public class HeartbeatAnswer
    {
        public string SenderUID { get; set; }
        public Dictionary<string, DateTime> NewerChanges { get; set; }

        public byte[] ToBytes()
        {
            var json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }

        public static HeartbeatAnswer FromBytes(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<HeartbeatAnswer>(json);
        }
    }
}
