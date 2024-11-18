using System.Net;
using System.Net.Mime;
using FastRide.Server.Sdk.Refit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FastRide.Proxy.HttpResponse;

public static class ApiProxyResult
{
    public static IActionResult ApiProxyResultFromJson<T>(ApiResponseMessage<T> response)
        where T : new()
    {
        if (response.Success)
        {
            return new ContentResult
            {
                ContentType = MediaTypeNames.Application.Json,
                Content = JsonConvert.SerializeObject(response.Response),
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        return new ContentResult
        {
            ContentType = MediaTypeNames.Text.Plain,
            Content = JsonConvert.SerializeObject(response.ResponseMessage),
            StatusCode = response.StatusCode
        };
    }

    public static IActionResult CreateApiProxyResult<T>(ApiResponseMessage<T> response)
        where T : new()
    {
        if (response.Success)
        {
            return new ObjectResult(response.Response);
        }

        return new ContentResult
        {
            ContentType = MediaTypeNames.Text.Plain,
            Content = JsonConvert.SerializeObject(response.ResponseMessage),
            StatusCode = response.StatusCode
        };
    }

    public static IActionResult CreateApiProxyResult(ApiResponseMessage response)
    {
        if (response.Success)
        {
            return new OkResult();
        }
        
        return new ContentResult
        {
            ContentType = MediaTypeNames.Text.Plain,
            Content = JsonConvert.SerializeObject(response.ResponseMessage),
            StatusCode = response.StatusCode
        };
    }
}