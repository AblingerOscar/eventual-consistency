using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ViewService
{
    [JsonObject]
    class ViewCountSyncDataObject
    {
        public string SenderServiceId { get; set; } = "";
        public int Views { get; set; } = 0;

        public ViewCountSyncDataObject() { }
        public ViewCountSyncDataObject(string id, int views = 0)
        {
            SenderServiceId = id;
            Views = views;
        }

        public byte[] ToBytes()
        {
            var json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }

        public static ViewCountSyncDataObject FromBytes(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<ViewCountSyncDataObject>(json);
        }
    }
}
