using System.Diagnostics.CodeAnalysis;

namespace Domain.Models.Envelope
{
    /// <summary>
    /// Model for success responses.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ExcludeFromCodeCoverage]
    public class ResponseOk<T> : IResponse<T>
    {
        public bool IsSuccess { get; } = true;

        public T Data { get; } = default!;

        public ResponseModel? Error { get; }

        public ResponseOk(T data)
        {
            Data = data;
        }
    }
}
