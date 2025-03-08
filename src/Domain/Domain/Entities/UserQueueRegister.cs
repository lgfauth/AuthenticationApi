using Domain.Models;

namespace Domain.Entities
{
    /// <summary>
    /// A payload for User queue
    /// </summary>
    public class UserQueueRegister
    {
        private const string TypeRegister = "REGISTER";
        private const string TypeDelete = "DELETE";
        /// <summary>
        /// Constructor for registration request.
        /// </summary>
        /// <param name="request">SubscriptionRequest object</param>
        public UserQueueRegister(SubscriptionRequest request)
        {
            Username = request.Username;
            Password = request.Password;
            Email = request.Email;
            LastName = request.LastName;
            Name = request.Name;

            Type = TypeRegister;
        }

        /// <summary>
        /// Constructor for delete request.
        /// </summary>
        /// <param name="request">UnsubscribeRequest object</param>
        public UserQueueRegister(UnsubscribeRequest request)
        {
            Username = request.Username;
            Password = request.Password;
            Email = request.Email;

            Type = TypeDelete;
        }

        /// <summary>
        /// User name for login.
        /// </summary>
        /// <example>blevers</example>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password for login.
        /// </summary>
        /// <example>test123</example>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// An email for receive confirmations and ads.
        /// </summary>
        /// <example>test@blevers.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The person real name.
        /// </summary>
        /// <example>Jhon</example>
        public string? Name { get; set; }

        /// <summary>
        /// The person real last name.
        /// </summary>
        /// <example>Cena</example>
        public string? LastName { get; set; }

        /// <summary>
        /// Type of action need to take when process the message.
        ///  - register
        ///  - delete
        /// </summary>
        /// <example>register</example>
        public string? Type { get; set; }
    }
}
