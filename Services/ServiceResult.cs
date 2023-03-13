namespace Forum.Services
{
    public class ServiceResult
    {
        public Result Result { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ServiceResult<T>
    {
        public T? Value { get; set; }
        public Result Result { get; set; } = Result.Success;
        public string Message { get; set; } = string.Empty;
    }

    public enum Result
    {
        Success,
        Error,
    }
}
