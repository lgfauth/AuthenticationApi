namespace Domain.Models
{
    /// <summary>
    /// Login request body.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// A username for login.
        /// </summary>
        /// <example>blevers</example>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// A password for login.
        /// </summary>
        /// <example>test123</example>
        public string Password { get; set; } = string.Empty;
    }
}
