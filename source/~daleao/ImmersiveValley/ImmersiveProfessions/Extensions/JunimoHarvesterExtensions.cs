/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Common.Extensions.Reflection;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using System;

#endregion using directives

/// <summary>Extensions for the <see cref="JunimoHarvester"/> class.</summary>
public static class JunimoHarvesterExtensions
{
    private static readonly Func<JunimoHarvester, JunimoHut?> _GetHome = typeof(JunimoHarvester)
        .RequirePropertyGetter("home").CompileUnboundDelegate<Func<JunimoHarvester, JunimoHut?>>();

    /// <summary>The the <see cref="Farmer"/> who built the <see cref="JunimoHut"/> which houses the <see cref="JunimoHarvester"/>.</summary>
    public static Farmer GetOwner(this JunimoHarvester junimo)
    {
        var home = _GetHome(junimo);
        if (home is null) return Game1.MasterPlayer;

        return Game1.getFarmerMaybeOffline(home.owner.Value) ?? Game1.MasterPlayer;
    }
}