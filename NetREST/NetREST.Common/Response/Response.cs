using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace NetREST.Common.Response
{
    public class Response
    {
        [JsonIgnore]
        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.OK;
        public dynamic ErrorCode { get; set; } = 0;
    }

    public class Response<T> : Response where T : class
    {
        public T ResultData { get; set; }
    }

    public class ErrorResponse : Response
    {
        public string ErrorMessage { get; set; }
        public IEnumerable<Error> Errors { get; set; } = new Error[0];
    }

    public class ErrorResponse<T> : Response<T> where T : class
    {
        public string ErrorMessage { get; set; }
        public IEnumerable<Error> Errors { get; set; } = new Error[0];
    }

    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Field { get; set; }
    }
}