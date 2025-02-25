namespace Domain.Models
{
    /// <summary>
    /// Request model for unsubscribe.
    /// </summary>
    public class UnsubscribeRequest
    {
        /// <summary>
        /// User username.
        /// </summary>
        /// <example>blevers</example>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// A user password.
        /// </summary>
        /// <example>test123</example>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// A user email used on subscription.
        /// </summary>
        /// <example>test@blevers.com</example>
        public string Email { get; set; } = string.Empty;
    }
}
