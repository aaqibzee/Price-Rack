using System.Net;

namespace PriceRack.Common.Exceptions
{
    /// <summary>
    /// CustomInvalidException class
    /// </summary>
    public class CustomInvalidException : Exception
    {
        /// <summary>
        /// Message
        /// </summary>
        public override string Message { get; }
        /// <summary>
        /// Status
        /// </summary>
        public HttpStatusCode Status { get; }
        /// <summary>
        /// CustomInvalidException
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public CustomInvalidException(HttpStatusCode status, string message)
        {
            Message = message;
            Status = status;
        }
    }
}