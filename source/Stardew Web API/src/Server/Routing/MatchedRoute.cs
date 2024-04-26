/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using System.Reflection;

namespace StardewWebApi.Server.Routing;

internal class MatchedRoute
{
    public MatchedRoute(Route route, MethodInfo method, string staticRoute)
    {
        Route = route;
        Method = method;
        Parameters = route.GetRouteParameters(staticRoute);
    }

    public Route Route { get; }
    public MethodInfo Method { get; }
    public Dictionary<string, string> Parameters { get; }
}