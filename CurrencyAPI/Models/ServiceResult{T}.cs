namespace CurrencyAPI.Models
{
    public class ServiceResult<T> : ServiceResult
    {
        public T? Value { get; }

        private ServiceResult(bool isSuccess, T? value, string? error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static ServiceResult<T> Success(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Success result must have a value.");

            return new ServiceResult<T>(true, value, null);
        }

        public static new ServiceResult<T> Failure(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentException("Error message must not be empty.", nameof(error));

            return new ServiceResult<T>(false, default, error);
        }
    }
}
