﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Central.Controllers
{
    public class DiffController : ApiController
    {
        protected DiffLib.ICentralServer Central;

        public DiffController()
        {
            var state = System.Web.HttpContext.Current.Cache[WebApiApplication.CentralStateKey] as DiffLib.CentralServerState;
            if (state == null)
                throw new NullReferenceException($"Cache with key: {WebApiApplication.CentralStateKey} is null!");

            Central = new DiffLib.AspNetCentralServer(state);
        }
        [Route("api/v{version:apiVersion}/diff")]
        [HttpPost]
        public DiffLib.Packets.CreateIdResponse Create([FromBody] DiffLib.Packets.CreateIdCentralRequest data)
        {
            if (data == null)
                throw new NullReferenceException("data is null");

            string id = Central.CreateId(data.Data);

            return new DiffLib.Packets.CreateIdResponse() { Id = id };
        }

        [Route("api/v{version:apiVersion}/diff/{id}")]
        [HttpPost]
        public DiffLib.Packets.CompleteIdResponse Complete(string id, [FromBody] DiffLib.Packets.CompleteIdCentralRequest data)
        {
            if (data == null)
                throw new NullReferenceException("data is null");

            if (!Central.CompleteId(id, data.Data))
                throw new ApplicationException("Central complete id failed to return true.");

            return new DiffLib.Packets.CompleteIdResponse() { Id = id };
        }

        [Route("api/v{version:apiVersion}/getdiff/{id}")]
        [HttpPost]
        public DiffLib.Packets.GetDiffResponse GetDiff(string id, [FromBody] DiffLib.Packets.GetDiffRequest data)
        {
            if (data == null)
                throw new NullReferenceException("data is null");

            var result = Central.GetDiff(id);

            return new DiffLib.Packets.GetDiffResponse() { Id = id, Result = result };
        }



        [Route("api/v{version:apiVersion}/test/diff/{id}")]
        [HttpPost]
        public async Task<DiffLib.Packets.CompleteIdResponse> Test(string id, [FromBody] DiffLib.Packets.CompleteIdWorkerRequest data)
        {
            id = id ?? "EMPTY";
            string xdata = "";
            if (data == null)
                xdata = "ISNULL";
            else
            {
                xdata = data.Data;
            }

            return await Task.Run(() => new DiffLib.Packets.CompleteIdResponse() { Id = "CENTRAL|" + id + "|" + xdata });
        }

        [Route("api/v{version:apiVersion}/test/diff/{id}")]
        [HttpGet]
        public async Task<DiffLib.Packets.CompleteIdResponse> TestGet(string id, [FromBody] DiffLib.Packets.CompleteIdWorkerRequest data)
        {
            id = id ?? "EMPTY";
            string xdata = "";
            if (data == null)
                xdata = "ISNULL";
            else
            {
                xdata = data.Data;
            }

            return await Task.Run(() => new DiffLib.Packets.CompleteIdResponse() { Id = "CENTRAL|" + id + "|" + xdata });
        }
    }
}