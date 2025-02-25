using Domain.Models;

namespace ServicesApplication.Validators
{
    public class ValidationException : Exception
    {
        public ResponseModel Error { get; set; }
        public ValidationException(string message, string code) : base(message)
        {
            Error = new ResponseModel { Code = code, Message = message };
        }

        public ValidationException(ResponseModel error) : base(error.Message)
        {
            Error = error;
        }
    }
}
