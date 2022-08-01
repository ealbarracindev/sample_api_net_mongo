using System;

namespace web_api.Core.Exceptions
{
    public class InvalidStatusChangeException : Exception
    {
        public string[] AllowedStatus { get; set; }

        public InvalidStatusChangeException(string[] allowedStatus)
        {
            AllowedStatus = allowedStatus;
        }
    }
}
