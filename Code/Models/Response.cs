using Core.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Core.Models
{

    public class Response
    {
        [JsonProperty("success")]
        public bool Success { get; internal set; }
        
        [JsonProperty("rows")]
        public long Rows { get; internal set; }

        [JsonProperty("code")]
        public HttpStatusCode Code { get; internal set;  }

        [JsonProperty("message",NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        public Response()
        {
            Success = true;
            Rows = 0;
            Code = HttpStatusCode.OK;
        }

        public Response(bool success, string message = null, HttpStatusCode code = HttpStatusCode.OK)
        {
            Success = success;
            Rows = 0;
            Message = message;
            Code = code;
        }

        public Response(bool success, long rows = 0, string message = null, HttpStatusCode code = HttpStatusCode.OK)
        {
            Success = success;
            Rows = rows;
            Message = message;
            Code = code;
        }

        public static Response<TEntity> Cast<TEntity>(Response response)
        {
            if (response != null)
                return new Response<TEntity>(response.Success, response.Rows, response.Message, response.Code);
            else
                return new Response<TEntity>(false, 0, "Response is null", HttpStatusCode.InternalServerError);
        }

        public static Response<TEntity> Cast<OEntity, TEntity>(Response<OEntity> response)
        {
            if (response != null)
                return new Response<TEntity>(response.Success, (IEnumerable<TEntity>)response.Data, response.Message, response.Code);
            else
                return new Response<TEntity>(false, 0, "Response is null", HttpStatusCode.InternalServerError);
        }

    }

    public class Response<TEntity> : Response, IResponse<TEntity>
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<TEntity> Data { get; internal set; }
        [JsonConstructor]
        public Response(bool success, IEnumerable<TEntity> data, string message = null, HttpStatusCode code = HttpStatusCode.OK) : base(success, message, code)
        {
            Data = data;
            if (data == null)
                base.Rows = 0;
            else
                base.Rows = data.Count();
        }

        public Response(bool success, long rows = 0, string message = null, HttpStatusCode code = HttpStatusCode.OK) : base(success, message, code)
        {
            Data = null;
        }
    }
}