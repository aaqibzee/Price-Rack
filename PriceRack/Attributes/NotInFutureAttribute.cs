using PriceRack.Common.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace PriceRack.Attributes
{
    public class NotInFutureAttribute : ValidationAttribute
    {
        private string _attributeName { get; set; }
        public NotInFutureAttribute(string attributeName)
        {
            _attributeName=attributeName;
        }
        public override bool IsValid(object value)
        {
            var date = (DateTime)value;
            if (date > DateTime.Now)
            {
                throw new CustomInvalidException(HttpStatusCode.BadRequest, $"{_attributeName} date cannot be in the future");
            }
            return true;
        }
    }
}