/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewWebApi.Game;
using System.Net;
using System.Text;
using System.Text.Json;

namespace StardewWebApi.Server;

internal partial class WebServer
{
    private WebServer()
    {
        // IPv6 binding on Linux doesn't work like this, so only bind this on Windows
        if (Environment.OSVersion.Platform == PlatformID.Win32NT
            && System.Net.Sockets.Socket.OSSupportsIPv6)
        {
            _listener.Prefixes.Add("http://[::1]:7882/");
        }

        CreateRouteTable();
    }

    private static WebServer? _instance;
    public static WebServer Instance => _instance ??= new WebServer();

    private volatile bool _runListenLoop;

    private readonly HttpListener _listener = new()
    {
        Prefixes = { "http://localhost:7882/", "http://127.0.0.1:7882/" }
    };

    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public void StartWebServer()
    {
        _runListenLoop = true;
        Task.Run(MainLoop);
    }

    public void StopWebServer()
    {
        _runListenLoop = false;
    }

    private void MainLoop()
    {
        try
        {
            _listener.Start();
        }
        catch (Exception ex)
        {
            SMAPIWrapper.LogError($"Error starting web server: {ex.Message}");
        }

        var sem = new SemaphoreSlim(20, 20);

        while (_runListenLoop)
        {
            sem.Wait();

            try
            {
                _listener.GetContextAsync().ContinueWith(async (t) =>
                {
                    try
                    {
                        sem.Release();

                        var ctx = await t;
                        await ProcessRequest(ctx);
                    }
                    catch (Exception ex)
                    {
                        SMAPIWrapper.LogError($"Error processing request: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                SMAPIWrapper.LogError($"Error getting web server context: {ex.Message}");
            }
        }

        _listener.Stop();
    }

    private async Task ProcessRequest(HttpListenerContext context)
    {
        using var response = context.Response;

        try
        {
            SMAPIWrapper.LogDebug($"Received request for {context.Request.Url}");

            var path = (context.Request.Url?.AbsolutePath ?? "").ToLower();

            if (String.IsNullOrEmpty(path))
            {
                response.ServerError();
                return;
            }

            if (path == "/events")
            {
                if (context.Request.IsWebSocketRequest)
                {
                    await ProcessWebSocketRequest(context);
                }
                else
                {
                    response.BadRequest("This endpoint requires a WebSocket connection");
                }
            }

            var route = FindRoute(context.Request);

            if (route is not null)
            {
                SMAPIWrapper.LogDebug($"Found matching route for {context.Request.Url!.AbsolutePath}");
                ProcessEndpointRequest(route, context);
            }
            else
            {
                SMAPIWrapper.LogDebug($"No matching route found for {context.Request.Url!.AbsolutePath}");
                response.NotFound();
            }
        }
        catch (Exception ex)
        {
            SMAPIWrapper.LogError($"Error processing request: {ex.Message}");
            response.ServerError(ex);
        }
    }
}

internal static class HttpListenerExtensions
{
    public static void WriteResponseBody(this HttpListenerResponse response, object responseBody)
    {
        response.ContentType = "application/json";
        var buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(responseBody, WebServer.SerializerOptions));
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
    }

    public static void CreateResponse(this HttpListenerResponse response, int statusCode, object? responseBody = null)
    {
        response.StatusCode = statusCode;
        if (responseBody is not null)
        {
            response.WriteResponseBody(responseBody);
        }
    }

    public static void Ok(this HttpListenerResponse response, object? responseBody)
    {
        response.CreateResponse(200, responseBody);
    }

    public static void NoContent(this HttpListenerResponse response)
    {
        response.CreateResponse(204);
    }

    public static void BadRequest(this HttpListenerResponse response, string errorMessage)
    {
        response.CreateResponse(400, new ErrorResponse(errorMessage));
    }

    public static void NotFound(this HttpListenerResponse response, string? errorMessage = null)
    {
        response.CreateResponse(404, errorMessage is not null ? new ErrorResponse(errorMessage) : null);
    }

    public static void ServerError(this HttpListenerResponse response, string errorMessage)
    {
        response.ServerError(new ErrorResponse(errorMessage));
    }

    public static void ServerError(this HttpListenerResponse response, object? responseBody = null)
    {
        response.CreateResponse(500, responseBody);
    }
}