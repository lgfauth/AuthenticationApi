using Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Validation
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
