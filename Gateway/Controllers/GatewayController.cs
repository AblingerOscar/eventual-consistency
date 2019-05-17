using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [Route("api")]
    [ApiController]
    public class GatewayController : ControllerBase
    {
        [HttpGet("{service-uid}/views")]
        public ActionResult<int> GetTotalViews(int serviceUid)
        {
            Console.WriteLine($"Get cumulated known views from {serviceUid}");
            // TODO: get cumulated known views from service
            return 0;
        }

        [HttpPost("{service-uid}/addView")]
        public void AddView(int serviceUid)
        {
            Console.WriteLine($"Add view to {serviceUid}");
            // TODO: add view to service with uid
        }

        [HttpPost("{service-uid}/addView/{number}")]
        public void AddViews(int serviceUid, int number)
        {
            Console.WriteLine($"Add {number} views to {serviceUid}");
            // TODO: add number of views to service with uid
        }
    }
}
