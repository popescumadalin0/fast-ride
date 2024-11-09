using System.Net;

namespace FastRide.Server.Sdk.Refit;

public class ApiResponseMessage
{
    public ApiResponseMessage(bool success, HttpStatusCode statusCode = HttpStatusCode.OK,
        string responseMessage = null)
    {
        Success = success;
        ResponseMessage = responseMessage;
        StatusCode = (int)statusCode;
    }

    public ApiResponseMessage(bool success, int statusCode, string responseMessage = null)
    {
        Success = success;
        ResponseMessage = responseMessage;
        StatusCode = statusCode;
    }

    public bool Success { get; set; }

    public string ResponseMessage { get; set; }

    public int StatusCode { get; set; }
}

public class ApiResponseMessage<T>
{
    public ApiResponseMessage(
        bool success,
        T response,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        string responseMessage = null)
    {
        Success = success;
        ResponseMessage = responseMessage;
        StatusCode = (int)statusCode;
        Response = response;
    }

    public ApiResponseMessage(bool success, T response, int statusCode, string responseMessage = null)
    {
        Success = success;
        ResponseMessage = responseMessage;
        StatusCode = statusCode;
        Response = response;
    }

    public bool Success { get; set; }

    public string ResponseMessage { get; set; }

    public int StatusCode { get; set; }

    public T Response { get; set; }
}