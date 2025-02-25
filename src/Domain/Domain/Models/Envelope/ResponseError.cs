using System.Diagnostics.CodeAnalysis;

namespace Domain.Models.Envelope
{
    [ExcludeFromCodeCoverage]
    public class ResponseError<T> : IResponse<T>
    {
        public bool IsSuccess { get; } = false;

        public T Data { get; } = default!;

        public ResponseModel Error { get; } = default!;

        public ResponseError(T data)
        {
            Data = data;
        }

        public ResponseError(ResponseModel error)
        {
            Error = error;
        }
    }
}