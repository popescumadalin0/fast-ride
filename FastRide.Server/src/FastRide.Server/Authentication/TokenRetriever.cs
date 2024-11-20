using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;

namespace FastRide.Server.Authentication;

public static class TokenRetriever
{
    public static bool TryGetIdToken(FunctionContext context, out string token)
    {
        return TryGetToken(context, "authentication", out token);
    }

    public static bool TryGetAccessToken(FunctionContext context, out string token)
    {
        return TryGetToken(context, "authorization", out token);
    }
    
    public static List<T> GetCustomAttributesOnClassAndMethod<T>(MethodInfo targetMethod)
        where T : Attribute
    {
        var methodAttributes = targetMethod.GetCustomAttributes<T>();
        var classAttributes = targetMethod.DeclaringType!.GetCustomAttributes<T>();
        return methodAttributes.Concat(classAttributes).ToList();
    }

    private static bool TryGetToken(FunctionContext context, string header, out string token)
    {
        token = null;
        if (!context.BindingContext.BindingData.TryGetValue("Headers", out var headers))
        {
            return false;
        }

        if (headers is not string cookieString)
        {
            return false;
        }

        var headersDict = JsonSerializer.Deserialize<Dictionary<string, string>>(cookieString);
        var normalizedKeyHeaders = headersDict.ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value);
        if (!normalizedKeyHeaders.TryGetValue(header, out var authHeaderValue))
        {
            return false;
        }

        if (!authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        token = authHeaderValue["Bearer ".Length..].Trim();
        return true;
    }
}