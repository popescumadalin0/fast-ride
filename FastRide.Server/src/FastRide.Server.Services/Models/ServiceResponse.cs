using System;

namespace FastRide.Server.Services.Models;

public class ServiceResponse<T>
{
    public ServiceResponse(T response)
    {
        Success = true;
        Response = response;
    }

    public ServiceResponse(T response, string errorMessage = "")
    {
        Success = false;
        Response = response;
        ErrorMessage = errorMessage;
    }

    public ServiceResponse(Exception e = null, string errorMessage = "")
    {
        Exception = e;
        ErrorMessage = errorMessage;
        Success = false;
    }

    public T Response { get; set; }
    public bool Success { get; set; }
    public Exception Exception { get; set; }
    public string ErrorMessage { get; set; }
}

public class ServiceResponse
{
    public ServiceResponse(Exception e = null, string errorMessage = "")
    {
        Exception = e;
        ErrorMessage = errorMessage;
        Success = false;
    }

    public ServiceResponse()
    {
        Success = true;
    }

    public bool Success { get; set; }
    public Exception Exception { get; set; }
    public string ErrorMessage { get; set; }
}