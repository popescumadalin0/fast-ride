using System;

namespace FastRide_Server.Services.Models;

public class ServiceResponse
{
    public ServiceResponse(Exception? e = null, string errorMessage = "")
    {
        ErrorMessage = errorMessage;
        Exception = e;
        Success = false;
    }

    public ServiceResponse()
    {
        Success = true;
    }

    public string ErrorMessage { get; set; } = string.Empty;

    public Exception? Exception { get; set; }

    public bool Success { get; set; }
}

public class ServiceResponse<T> where T : new()
{
    public ServiceResponse(T response)
    {
        Response = response;
        Success = true;
    }

    public ServiceResponse(Exception? e = null, string errorMessage = "")
    {
        ErrorMessage = errorMessage;
        Exception = e;
        Success = false;
    }

    public string ErrorMessage { get; set; } = string.Empty;

    public Exception? Exception { get; set; }

    public T? Response { get; set; }

    public bool Success { get; set; }
}