namespace Domain.Models
{
    /// <summary>
    /// Subscription object request.
    /// </summary>
    public class SubscriptionRequest
    {
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
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The person real last name.
        /// </summary>
        /// <example>Cena</example>
        public string LastName { get; set; } = string.Empty;
    }
}
