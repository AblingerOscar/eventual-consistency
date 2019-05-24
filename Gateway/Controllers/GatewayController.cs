﻿using System;
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
        [HttpGet("{serviceUid}/views")]
        public ActionResult<int> GetTotalViews(string serviceUid)
        {
            return Program.rpcClient.SendGetView(serviceUid);
        }

        [HttpPost("{serviceUid}/add-view")]
        public void AddView(string serviceUid)
        {
            Program.rpcClient.SendAddViews(serviceUid, 1);
        }

        [HttpPost("{serviceUid}/add-views/{number}")]
        public void AddViews(string serviceUid, int number)
        {
            Program.rpcClient.SendAddViews(serviceUid, number);
        }
    }
}
