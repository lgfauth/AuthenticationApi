namespace Domain.Models
{
    public class AuthResponse
    {
        /// <summary>
        /// Encrypted JWT Token for authentication.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c</example>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// A date time at the token expire.
        /// </summary>
        /// <example>2026-02-20T12:12:13</example>
        public DateTime ExpiresAt { get; set; }
    }
}
