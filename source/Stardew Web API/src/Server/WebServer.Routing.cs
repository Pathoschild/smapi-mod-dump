/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using System.Net;
using System.Reflection;
using StardewWebApi.Server.Routing;

namespace StardewWebApi.Server;

internal partial class WebServer
{
    private readonly SortedList<Route, MethodInfo> _routes = new();

    private void CreateRouteTable()
    {
        var apiControllers =
        Assembly.GetCallingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ApiControllerBase)));

        foreach (var controller in apiControllers)
        {
            var classRouteAttribute = controller.GetCustomAttribute(typeof(RouteAttribute), false) as RouteAttribute;
            var methods = controller.GetMethods()
                .Where(m => m.GetCustomAttribute(typeof(RouteAttribute), false) is not null);

            foreach (var m in methods)
            {
                var routeAttribute = m.GetCustomAttribute(typeof(RouteAttribute), false) as RouteAttribute;

                if (classRouteAttribute is not null)
                {
                    var fullPath = classRouteAttribute.Path.AppendAsUri(routeAttribute!.Path);
                    _routes.Add(new Route(fullPath), m);
                }
                else
                {
                    _routes.Add(new Route(routeAttribute!.Path), m);
                }
            }
        }
    }

    private MatchedRoute? FindRoute(HttpListenerRequest request)
    {
        var path = request.Url?.AbsolutePath ?? "";
        var route = _routes.Keys.FirstOrDefault(r => r.MatchesStaticRoute(path));

        return route is not null
            ? new MatchedRoute(route, _routes[route], path)
            : null;
    }
}

public static class UriStringExtensions
{
    public static string AppendAsUri(this string s, string other)
    {
        return $"{s.TrimEnd('/')}/{other.TrimStart('/')}";
    }
}