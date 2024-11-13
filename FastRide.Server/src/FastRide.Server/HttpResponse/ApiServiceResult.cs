using System.Net;
using System.Net.Mime;
using FastRide.Server.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FastRide.Server.HttpResponse;

public static class ApiServiceResult
{
    public static IActionResult ApiServiceResultFromJson<T>(ServiceResponse<T> serviceResponse)
        where T : new()
    {
        if (serviceResponse.Success)
        {
            return new ContentResult
            {
                ContentType = MediaTypeNames.Application.Json,
                Content = JsonConvert.SerializeObject(serviceResponse.Response),
                StatusCode = (int)HttpStatusCode.OK
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
            Content = JsonConvert.SerializeObject(serviceResponse.Exception),
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }

    public static IActionResult CreateApiResult<T>(ServiceResponse<T> serviceResponse)
        where T : new()
    {
        if (serviceResponse.Success)
        {
            return new ObjectResult(serviceResponse.Response);
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
            Content = JsonConvert.SerializeObject(serviceResponse.Exception),
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }

    public static IActionResult CreateApiResult(ServiceResponse serviceResponse)
    {
        if (serviceResponse.Success)
        {
            return new OkResult();
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
            Content = JsonConvert.SerializeObject(serviceResponse.Exception),
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }
}