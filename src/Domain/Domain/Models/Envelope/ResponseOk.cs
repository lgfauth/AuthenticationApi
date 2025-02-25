namespace Domain.Models.Envelope
{
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
