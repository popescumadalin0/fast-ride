using System.Net;
using System.Net.Mime;
using FastRide.Server.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FastRide.Server.HttpResponse;

public static class ApiServiceResponse
{
    public static IActionResult ApiServiceResultFromJson<T>(ServiceResponse<T> serviceResponse)
        where T : new()
    {
        if (serviceResponse.Success)
        {
            return new ContentResult
            {
                ContentType = MediaTypeNames.Application.Json,
                Content = serviceResponse.Response.ToString(),
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        if (serviceResponse.Exception == null)
        {
            return new ContentResult
            {
                ContentType = MediaTypeNames.Text.Plain,
                Content = serviceResponse.ErrorMessage,
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        return new ContentResult
        {
            ContentType = MediaTypeNames.Text.Plain,
            Content = serviceResponse.ErrorMessage,
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }

    public static IActionResult ApiServiceResult<T>(ServiceResponse<T> serviceResponse)
        where T : new()
    {
        if (serviceResponse.Success)
        {
            return new ObjectResult(serviceResponse.Response);
        }

        if (serviceResponse.Exception == null)
        {
            return new ContentResult
            {
                ContentType = MediaTypeNames.Text.Plain,
                Content = serviceResponse.ErrorMessage,
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        return new ContentResult
        {
            ContentType = MediaTypeNames.Text.Plain,
            Content = serviceResponse.ErrorMessage,
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }

    public static IActionResult ApiServiceResult(ServiceResponse serviceResponse)
    {
        if (serviceResponse.Success)
        {
            return new OkResult();
        }

        if (serviceResponse.Exception != null)
        {
            return new ContentResult
            {
                ContentType = MediaTypeNames.Text.Plain,
                Content = serviceResponse.Exception.Message,
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        if (!string.IsNullOrEmpty(serviceResponse.ErrorMessage))
        {
            return new ContentResult
            {
                ContentType = MediaTypeNames.Text.Plain,
                Content = serviceResponse.ErrorMessage,
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        return new ContentResult
        {
            ContentType = MediaTypeNames.Text.Plain,
            Content = serviceResponse.ErrorMessage,
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }
}