using System;

namespace Pckgs
{
    public class ApiConnectionException : ApiException
    {
        public ApiConnectionException(string message) : base(message)
        {
        }
    }
}