using System;

namespace Pckgs
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message)
        {

        }
    }
}