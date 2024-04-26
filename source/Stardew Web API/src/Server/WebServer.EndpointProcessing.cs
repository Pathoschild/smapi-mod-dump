/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using Microsoft.AspNetCore.Components.Routing;
using StardewValley;
using StardewWebApi.Game;
using StardewWebApi.Server.Routing;
using System.Net;
using System.Reflection;

namespace StardewWebApi.Server;

internal partial class WebServer
{
    private static void ProcessEndpointRequest(MatchedRoute route, HttpListenerContext context)
    {
        if ((route.Method.GetCustomAttribute<RequireLoadedGameAttribute>() is not null
            || route.Method.DeclaringType?.GetCustomAttribute<RequireLoadedGameAttribute>() is not null)
            && !Game1.hasLoadedGame)
        {
            context.Response.BadRequest("No save loaded. Please load a save and try again.");
            return;
        }

        var controller = Activator.CreateInstance(route.Method.DeclaringType!);
        (controller as ApiControllerBase)!.HttpContext = context;

        if (TryPopulateEndpointParameters(route, context, out var parameters))
        {
            route.Method.Invoke(controller, parameters);
        }
    }

    private static bool TryPopulateEndpointParameters(MatchedRoute route, HttpListenerContext context, out object?[]? outParams)
    {
        try
        {
            var sortedParams = new SortedDictionary<int, ParameterInfo>(
                route.Method.GetParameters().ToDictionary(p => p.Position)
            );

            var parameters = new List<object?>();

            if (sortedParams.Count > 0)
            {
                foreach (var param in sortedParams.Values)
                {
                    if (route.Parameters.ContainsKey(param.Name!))
                    {
                        if (TryParseParameter(param, route.Parameters[param.Name!], out var result))
                        {
                            parameters.Add(result);
                            continue;
                        }
                        else
                        {
                            context.Response.BadRequest($"'{route.Parameters[param.Name!]}' is not a valid value for {param.Name}");
                            outParams = null;
                            return false;
                        }
                    }

                    if (context.Request.QueryString[param.Name!] is null && !param.IsOptional)
                    {
                        context.Response.BadRequest($"Missing required parameter: {param.Name}");
                        outParams = null;
                        return false;
                    }
                    else
                    {
                        if (TryParseParameter(param, context.Request.QueryString[param.Name!], out var result))
                        {
                            parameters.Add(result);
                            continue;
                        }
                        else
                        {
                            context.Response.BadRequest($"'{context.Request.QueryString[param.Name!]}' is not a valid value for {param.Name}");
                            outParams = null;
                            return false;
                        }
                    }
                }
            }

            outParams = parameters.ToArray();
            return true;
        }
        catch (Exception ex)
        {
            SMAPIWrapper.LogError($"Error populating route parameters: {ex.Message}");
            context.Response.ServerError(ex.Message);
        }

        outParams = null;
        return false;
    }

    private static bool TryParseParameter(ParameterInfo param, string? value, out object? result)
    {
        if (String.IsNullOrEmpty(value))
        {
            if (param.IsOptional)
            {
                result = param.DefaultValue;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        if (param.ParameterType.IsEnum || Nullable.GetUnderlyingType(param.ParameterType)?.IsEnum == true)
        {
            var enumType = (param.ParameterType.IsNullable()
                ? Nullable.GetUnderlyingType(param.ParameterType)
                : param.ParameterType)!;

            if (Enum.TryParse(enumType, value, true, out result)
                && ((result is null && param.ParameterType.IsNullable())
                     || (result is not null && Enum.IsDefined(enumType, result))
                )
            )
            {
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        if (UrlValueConstraint.TryGetByTargetType(param.ParameterType, out var constraint))
        {
            if (constraint.TryParse(value, out result))
            {
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
        else
        {
            result = Activator.CreateInstance(param.ParameterType);
            return true;
        }
    }
}

public static class TypeExtensions
{
    public static bool IsNullable(this Type type)
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;
    }
}