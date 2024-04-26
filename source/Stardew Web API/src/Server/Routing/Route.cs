/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using System.Text.RegularExpressions;

namespace StardewWebApi.Server.Routing;

internal class Route : IComparable<Route>, IEquatable<Route>
{
    public Route(string rawRoute)
    {
        Components = ParseComponents(rawRoute);
    }

    public List<RouteComponent> Components { get; }

    private static List<RouteComponent> ParseComponents(string rawRoute)
    {
        var components = new List<RouteComponent>();
        var rawRouteComponents = rawRoute.Trim('/').Split('/');

        foreach (var rawComponent in rawRouteComponents)
        {
            if (String.IsNullOrWhiteSpace(rawComponent))
            {
                throw new ArgumentException("Route component cannot contain empty values");
            }

            if (IsValidParamaterizedRoute(rawComponent))
            {
                components.Add(new ParameterizedRouteComponent(GetRouteParameterName(rawComponent)));
            }
            else if (IsValidStaticRoute(rawComponent))
            {
                components.Add(new StaticRouteComponent(rawComponent));
            }
            else
            {
                throw new ArgumentException($"Invalid name specified for parameterized route component: {rawComponent}");
            }
        }

        return components;
    }

    public bool MatchesStaticRoute(string staticRoute)
    {
        var staticRouteParts = staticRoute.Trim('/').Split('/');

        if (staticRouteParts.Length != Components.Count)
        {
            return false;
        }

        for (int x = 0; x < Components.Count; x++)
        {
            if (Components[x] is StaticRouteComponent staticRouteComponent)
            {
                if (staticRouteParts[x].ToLower() == staticRouteComponent.Value.ToLower())
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
        }

        return true;
    }

    public Dictionary<string, string> GetRouteParameters(string staticRoute)
    {
        var parameters = new Dictionary<string, string>();

        if (MatchesStaticRoute(staticRoute))
        {
            var staticRouteParts = staticRoute.Trim('/').Split('/');

            for (int x = 0; x < Components.Count; x++)
            {
                if (Components[x] is ParameterizedRouteComponent parameterizedRouteComponent)
                {
                    parameters.Add(parameterizedRouteComponent.Name, staticRouteParts[x]);
                }
            }
        }

        return parameters;
    }

    public static bool IsValidStaticRoute(string s)
    {
        return Regex.IsMatch(s, @"^[a-zA-Z_][\w]*$");
    }

    public static bool IsValidParamaterizedRoute(string s)
    {
        return Regex.IsMatch(s, @"^{[a-zA-Z_][\w]*}$");
    }

    public static string GetRouteParameterName(string s)
    {
        return s.TrimStart('{').TrimEnd('}');
    }


    public bool Equals(Route? otherRoute)
    {
        if (otherRoute is null || otherRoute.Components.Count != Components.Count) return false;

        for (int x = 0; x < Components.Count; x++)
        {
            if (Components[x].Type != otherRoute.Components[x].Type)
            {
                return false;
            }

            if (Components[x] is StaticRouteComponent staticRouteComponent)
            {
                if (staticRouteComponent.Value != (otherRoute.Components[x] as StaticRouteComponent)!.Value)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public override bool Equals(object? obj) => Equals(obj as Route);

    public static bool operator ==(Route a, Route b) => a.Equals(b);

    public static bool operator !=(Route a, Route b) => !(a == b);

    public int CompareTo(Route? otherRoute)
    {
        if (otherRoute is null) throw new ArgumentNullException(nameof(otherRoute));

        if (Components.Count != otherRoute.Components.Count)
        {
            return Components.Count.CompareTo(otherRoute.Components.Count);
        }

        for (int i = 0; i < Components.Count; i++)
        {
            if (Components[i] == otherRoute.Components[i])
            {
                continue;
            }

            if (Components[i].Type == RouteComponentType.Static
                && otherRoute.Components[i].Type == RouteComponentType.Static)
            {
                return (Components[i] as StaticRouteComponent)!.Value.CompareTo((otherRoute.Components[i] as StaticRouteComponent)!.Value);
            }
            else if (Components[i].Type == RouteComponentType.Static
                && otherRoute.Components[i].Type == RouteComponentType.Parameter)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        return 0;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"/{String.Join('/', Components)}";
    }
}