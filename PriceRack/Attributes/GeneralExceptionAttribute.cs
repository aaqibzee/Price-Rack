
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PriceRack.Common.Exceptions;
using System.Net;

namespace PriceRack.Attributes
{
    public class GeneralExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is CustomInvalidException customException)
            {
                context.Result = new ObjectResult(customException.Message)
                {
                    StatusCode = (int)customException.Status
                };
                context.ExceptionHandled = true;
            }
            else
            {
                var errorMessage = "An unexpected error occurred";
                context.Result = new ObjectResult(errorMessage)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                context.ExceptionHandled = true;
            }
        }
    }
}