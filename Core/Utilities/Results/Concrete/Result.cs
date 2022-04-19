using Core.Utilities.Results.Abstract;

namespace Core.Utilities.Results.Concrete
{
    public class Result : IResult
    {
        protected Result(bool success, string message):this(success)
        {
            Success = success;
            Message = message;
        }

        protected Result(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
        public string Message { get; set; }
    }
}