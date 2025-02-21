namespace Domain.Models.Envelope
{
    public class ResponseOk<T> : IResponse<T>
    {
        public bool IsSuccess { get; } = true;

        public T Data { get; } = default!;

        public ErrorModel? Error { get; }

        public ResponseOk(T data)
        {
            Data = data;
        }
    }
}
