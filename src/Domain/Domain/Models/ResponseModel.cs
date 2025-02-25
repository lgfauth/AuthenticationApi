namespace Domain.Models
{
    /// <summary>
    /// Data model for errors.
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// A code of execution can help to solve the problem or identificate a kind of response.
        /// </summary>
        /// <example>DB500</example>
        public string Code { get; set; }

        /// <summary>
        /// A message for response.
        /// </summary>
        /// <example>Somenthing happened here.</example>
        public string Message { get; set; }
    }
}