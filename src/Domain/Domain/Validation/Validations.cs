using Domain.Models;
using Domain.Models.Envelope;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Domain.Validation
{
    /// <summary>
    /// Form validations class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Validations
    {
        /// <summary>
        /// Validation for SubscriptionRequest form.
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="ValidationException"></exception>
        public static IResponse<ResponseModel> Validate(SubscriptionRequest request)
        {
            if (request == null)
                 return new ResponseError<ResponseModel>(new ResponseModel { Message = "Object SubscriptionRequest is null", Code = "VL005" });

            string patternEmail = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            if (string.IsNullOrWhiteSpace(request.Email) || !Regex.IsMatch(request.Email, patternEmail))
                return new ResponseError<ResponseModel>(new ResponseModel { Message = "Email field is not in a valid format.", Code = "VL006" });

            if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 5)
                return new ResponseError<ResponseModel>(new ResponseModel { Message = "Username field need 5 or more characters.", Code = "VL007" });

            string patternPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$";
            if (string.IsNullOrWhiteSpace(request.Password) || !Regex.IsMatch(request.Password, patternPassword))
                return new ResponseError<ResponseModel>(new ResponseModel { Message = "Password field need match: [a-z], [A-Z], one number and one special character and 8 or more characters.", Code = "VL008" });

            if (string.IsNullOrWhiteSpace(request.Name))
                return new ResponseError<ResponseModel>(new ResponseModel { Message = "Name field is mandatory.", Code = "VL009" });

            if (string.IsNullOrWhiteSpace(request.LastName))
                return new ResponseError<ResponseModel>(new ResponseModel { Message = "LastName field is mandatory.", Code = "VL004" });

            return new ResponseOk<ResponseModel>(new ResponseModel { Message = "All fields are valid.", Code = "SC200" });
        }

        /// <summary>
        /// Validation for UnsubscribeRequest form.
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="ValidationException"></exception>
        public static IResponse<ResponseModel> Validate(UnsubscribeRequest request)
        {
            if (request == null)
                return new ResponseError<ResponseModel>(new ResponseModel { Message = "Object SubscriptionRequest is null", Code = "VL005" });

            string patternEmail = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            if (string.IsNullOrWhiteSpace(request.Email) || !Regex.IsMatch(request.Email, patternEmail))
                return new ResponseError<ResponseModel>(new ResponseModel { Message = "Email field is not in a valid format.", Code = "VL006" });

            if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 5)
                return new ResponseError<ResponseModel>(new ResponseModel { Message = "Username field need 5 or more characters.", Code = "VL007" });

            string patternPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$";
            if (string.IsNullOrWhiteSpace(request.Password) || !Regex.IsMatch(request.Password, patternPassword))
                return new ResponseError<ResponseModel>(new ResponseModel { Message = "Password field need match: [a-z], [A-Z], one number and one special character and 8 or more characters.", Code = "VL008" });

            return new ResponseOk<ResponseModel>(new ResponseModel { Message = "All fields are valid.", Code = "SC200" });
        }

        /// <summary>
        /// Validation for LoginRequest form.
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="ValidationException"></exception>
        public static IResponse<ResponseModel> Validate(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return new ResponseError<ResponseModel>(new ResponseModel { Message = "Username field need be filed.", Code = "VL002" });

            if (string.IsNullOrWhiteSpace(request.Password))
                return new ResponseError<ResponseModel>(new ResponseModel { Message = "Password field need be filed.", Code = "VL001" });

            return new ResponseOk<ResponseModel>(new ResponseModel { Message = "All fields are valid.", Code = "SC200" });
        }
    }
}
