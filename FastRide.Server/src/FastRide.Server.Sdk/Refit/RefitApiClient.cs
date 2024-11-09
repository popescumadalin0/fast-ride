using System;
using System.Net;
using System.Threading.Tasks;
using Refit;

namespace FastRide.Server.Sdk.Refit;

public abstract class RefitApiClient<T> where T : class
{
    public event OnApiCallExecuted OnApiCallExecuted = _param1 => { };

    public async Task<ApiResponseMessage<T>> Execute<T>(Task<T> task)
    {
        try
        {
            T response = await task;
            OnApiCallExecuted(new ApiResponseMessage(true));
            return new ApiResponseMessage<T>(true, response);
        }
        catch (ApiException ex)
        {
            OnApiCallExecuted(new ApiResponseMessage(false, ex.StatusCode, ex.ReasonPhrase + " ; " + ex.Content));
            return new ApiResponseMessage<T>(false, Activator.CreateInstance<T>(), ex.StatusCode,
                ex.ReasonPhrase + " ; " + ex.Content);
        }
        catch (Exception ex)
        {
            OnApiCallExecuted(new ApiResponseMessage(false, HttpStatusCode.InternalServerError,
                "SDK Common : " + ex.Message));
            return new ApiResponseMessage<T>(false, Activator.CreateInstance<T>(), HttpStatusCode.InternalServerError,
                "SDK Common : " + ex.Message);
        }
    }

    public async Task<ApiResponseMessage> Execute(Task task)
    {
        try
        {
            OnApiCallExecuted(new ApiResponseMessage(true));
            return new ApiResponseMessage(true);
        }
        catch (ApiException ex)
        {
            OnApiCallExecuted(new ApiResponseMessage(false, ex.StatusCode, ex.ReasonPhrase + " ; " + ex.Content));
            return new ApiResponseMessage(false, ex.StatusCode, ex.ReasonPhrase + " ; " + ex.Content);
        }
        catch (Exception ex)
        {
            OnApiCallExecuted(new ApiResponseMessage(false, HttpStatusCode.InternalServerError,
                "SDK Common : " + ex.Message));
            return new ApiResponseMessage(false, HttpStatusCode.InternalServerError, "SDK Common : " + ex.Message);
        }
    }


    public async Task<ApiResponseMessage> ExecuteWithNoContentResponse(
        Task task)
    {
        try
        {
            await task;
            OnApiCallExecuted(new ApiResponseMessage(true));
            return new ApiResponseMessage(true);
        }
        catch (ApiException ex)
        {
            OnApiCallExecuted(new ApiResponseMessage(false, ex.StatusCode, ex.ReasonPhrase + " ; " + ex.Content));
            return new ApiResponseMessage(false, ex.StatusCode, ex.ReasonPhrase + " ; " + ex.Content);
        }
        catch (Exception ex)
        {
            OnApiCallExecuted(new ApiResponseMessage(false, HttpStatusCode.InternalServerError,
                "SDK Common : " + ex.Message));
            return new ApiResponseMessage(false, HttpStatusCode.InternalServerError, "SDK Common : " + ex.Message);
        }
    }
}