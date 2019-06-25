using Newtonsoft.Json;
using SharedClasses.DataObjects.ChangeMetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses.DataObjects
{
    [Serializable]
    public class MetaDataRequestAnswer
    {
        public ChangeSet DomesticChanges;
        public IList<ChangeSet> AlienChanges;

        public byte[] ToByte()
        {
            var json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }

        public static MetaDataRequestAnswer FromBytes(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<MetaDataRequestAnswer>(json);
        }
    }
}
