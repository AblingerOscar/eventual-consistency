using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ViewService
{
    public class ViewDataObject
    {
        public int OwnViews { get; set; } = 0;
        public IDictionary<string, int> Views { get; set; } = new Dictionary<string, int>();

        [JsonIgnore]
        public int TotalViews {
            get {
                int total = OwnViews;
                foreach (var kvp in Views)
                    total += kvp.Value;
                return total;
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static ViewDataObject FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ViewDataObject>(json);
        }
    }
}
