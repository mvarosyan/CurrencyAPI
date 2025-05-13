namespace CurrencyAPI.Models
{
    public class ServiceResult
    {
        public bool IsSuccess { get; }
        public string? Error { get; }

        protected ServiceResult(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static ServiceResult Success() => new ServiceResult(true, null);
        public static ServiceResult Failure(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentException("Error message must not be empty.", nameof(error));

            return new ServiceResult(false, error);
        }
    }
}
