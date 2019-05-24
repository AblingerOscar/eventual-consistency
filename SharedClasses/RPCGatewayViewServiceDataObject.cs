using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RPCGatewayViewServiceDataObject
    {
        public enum RequestFunction
        {
            GET_VIEWS = 0,
            ADD_VIEWS = 1
        }

        public RequestFunction RequestedFunction = RequestFunction.GET_VIEWS;
        public int? views = null;

        public RPCGatewayViewServiceDataObject() { }
        public RPCGatewayViewServiceDataObject(RequestFunction requestedFunction, int? views = null)
        {
            RequestedFunction = requestedFunction;
            this.views = views;
        }

        public byte[] ToBytes()
        {
            var json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }

        public static RPCGatewayViewServiceDataObject FromBytes(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<RPCGatewayViewServiceDataObject>(json);
        }
    }
}
