
using System.Text.Json.Serialization;

namespace SharedLibrary.Dtos
{
    public class Response<T> where T : class
    {
        public T? Data { get; private set; }
        public int StatusCode { get; private set; }
        public ErrorDto? Error { get; private set; }
        [JsonIgnore]
        public bool isSuccess { get; private set; }

        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode, isSuccess = true };
        }
        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default, StatusCode = statusCode, isSuccess = true };
        }
        public static Response<T> Fail(int statusCode, ErrorDto error)
        {
            return new Response<T> { StatusCode = statusCode, Error = error, isSuccess = false };
        }
        public static Response<T> Fail(int statusCode, string errorMessage, bool isShow)
        {
            var errorDto = new ErrorDto(errorMessage, isShow);
            return new Response<T> { StatusCode = statusCode, Error = errorDto, isSuccess = false };
        }

    }
}
