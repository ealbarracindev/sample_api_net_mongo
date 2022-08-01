using System.Collections.Generic;

namespace web_api.Infrastructure.Responses
{
    public class ApiResponse<T>
    {
        public string traceId { get; set; }
        public string instance { get; set; }
        public bool isSuccess { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public List<string> errors { get; set; }
        public T data { get; set; }
        //public Metadata meta { get; set; }
        public ApiResponse()
        {

        }
        public ApiResponse(T _data)
        {
            isSuccess = true;
            data = _data;
        }
        //public ApiResponse(T _data, Metadata metadata)
        //{
        //    isSuccess = true;
        //    data = _data;
        //    meta = metadata;
        //}
        public ApiResponse(T _data, string _message = null)
        {
            isSuccess = true;
            message = _message;
            data = _data;
        }
        public ApiResponse(string _message)
        {
            isSuccess = false;
            message = _message;
        }

    }
}
