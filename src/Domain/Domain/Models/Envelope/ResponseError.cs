using System.Diagnostics.CodeAnalysis;

namespace Domain.Models.Envelope
{
    /// <summary>
    /// Model for error responses.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ExcludeFromCodeCoverage]
    public class ResponseError<T> : IResponse<T>
    {
        public bool IsSuccess { get; } = false;

        public T Data { get; } = default!;

        public ResponseModel Error { get; } = default!;

        public ResponseError(ResponseModel error)
        {
            Error = error;
        }
    }
}