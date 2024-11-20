using System.Net;

namespace FastRide.Server.Sdk.Refit;

public class ApiResponseMessage
{
    public ApiResponseMessage()
    {
    }

    public ApiResponseMessage(
        bool success,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        string responseMessage = null,
        string reasonPhrase = null)
    {
        this.Success = success;
        this.ResponseMessage = responseMessage;
        this.StatusCode = (int)statusCode;
        this.ReasonPhrase = reasonPhrase;
    }

    public ApiResponseMessage(
        bool success,
        int statusCode,
        string responseMessage = null,
        string reasonPhrase = null)
    {
        this.Success = success;
        this.ResponseMessage = responseMessage;
        this.StatusCode = statusCode;
        this.ReasonPhrase = reasonPhrase;
    }

    public bool Success { get; set; }

    public string ResponseMessage { get; set; }

    public string ReasonPhrase { get; set; }

    public int StatusCode { get; set; }

    public string ClientError { get; set; }
}

public class ApiResponseMessage<T>
{
    public ApiResponseMessage(T response)
    {
        this.Response = response;
    }


    public ApiResponseMessage(bool success, T response, HttpStatusCode statusCode = HttpStatusCode.OK,
        string responseMessage = null, string reasonPhrase = null)
    {
        this.Success = success;
        this.ResponseMessage = responseMessage;
        this.StatusCode = (int)statusCode;
        this.ReasonPhrase = reasonPhrase;
        this.Response = response;
    }

    public ApiResponseMessage(bool success, T response, int statusCode, string responseMessage = null,
        string reasonPhrase = null)
    {
        this.Success = success;
        this.ResponseMessage = responseMessage;
        this.StatusCode = statusCode;
        this.ReasonPhrase = reasonPhrase;
        this.Response = response;
    }


    public bool Success { get; set; }

    public string ResponseMessage { get; set; }

    public string ReasonPhrase { get; set; }

    public int StatusCode { get; set; }

    public T Response { get; set; }

    public string ClientError { get; set; }
}