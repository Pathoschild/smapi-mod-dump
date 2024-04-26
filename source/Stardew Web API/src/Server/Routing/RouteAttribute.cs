/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

namespace StardewWebApi.Server.Routing;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RouteAttribute : Attribute
{
    public RouteAttribute(string path, string method = "GET")
    {
        Path = path;
        Method = method;
    }

    public string Path { get; init; }
    public string Method { get; init; }
}