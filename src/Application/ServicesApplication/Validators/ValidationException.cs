using Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace ServicesApplication.Validators
{
    [ExcludeFromCodeCoverage]
    public class ValidationException : Exception
    {
        public ResponseModel Error { get; set; }

        public ValidationException(ResponseModel error) : base(error.Message)
        {
            Error = error;
        }
    }
}
