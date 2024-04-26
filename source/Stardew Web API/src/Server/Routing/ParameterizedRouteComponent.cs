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

internal class ParameterizedRouteComponent : RouteComponent
{
    public ParameterizedRouteComponent(string name) : base(RouteComponentType.Parameter)
    {
        Name = name;
    }

    public readonly string Name;

    public bool Equals(ParameterizedRouteComponent? other) => other is not null && other.Name == Name;

    public override bool Equals(object? obj) => Equals(obj as ParameterizedRouteComponent);

    public static bool operator ==(ParameterizedRouteComponent a, ParameterizedRouteComponent b) => a.Equals(b);

    public static bool operator !=(ParameterizedRouteComponent a, ParameterizedRouteComponent b) => !(a == b);

    public override int GetHashCode() => base.GetHashCode();

    public override string ToString() => $"{{{Name}}}";
}