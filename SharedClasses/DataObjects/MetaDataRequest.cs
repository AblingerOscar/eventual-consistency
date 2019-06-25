using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses.DataObjects
{
    [Serializable]
    public class MetaDataRequest
    {
        public string ServiceId;
        public DateTime DomesticChangesSince;
        public IDictionary<string, DateTime> AlienChangesSince;

        public byte[] ToBytes()
        {
            var json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }

        public static MetaDataRequest FromBytes(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<MetaDataRequest>(json);
        }
    }
}
