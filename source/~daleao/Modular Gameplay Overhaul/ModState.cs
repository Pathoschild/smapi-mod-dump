/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
namespace DaLion.Overhaul;

/// <summary>The collection of state for each module.</summary>
internal sealed class ModState
{
    internal Modules.Professions.State Professions { get; set; } = new();

    internal Modules.Weapons.State Weapons { get; set; } = new();

    internal Modules.Slingshots.State Slingshots { get; set; } = new();

    internal Modules.Tools.State Tools { get; set; } = new();

    internal Modules.Rings.State Rings { get; set; } = new();

    internal Modules.Taxes.State Taxes { get; set; } = new();
}
#pragma warning restore CS1591
#pragma warning restore SA1600
