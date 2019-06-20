using System;
using System.Collections.Generic;

namespace SharedClasses.DataObjects
{
    [Serializable]
    public class MetaDataRequest
    {
        public DateTime DomesticChangesSince;
        public IDictionary<string, DateTime> AlienChangesSince;

        public MetaDataRequest() { }

        public MetaDataRequest(DateTime domesticChangesSince, IDictionary<string, DateTime> alienChangesSince)
        {
            DomesticChangesSince = domesticChangesSince;
            AlienChangesSince = alienChangesSince;
        }
    }
}
