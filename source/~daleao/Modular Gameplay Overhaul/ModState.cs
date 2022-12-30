/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul;

/// <summary>The core mod user-defined settings.</summary>
internal sealed class ModState
{
    internal Modules.Arsenal.State Arsenal { get; set; } = new();

    internal Modules.Professions.State Professions { get; set; } = new();

    internal Modules.Rings.State Rings { get; set; } = new();

    internal Modules.Taxes.State Taxes { get; set; } = new();

    internal Modules.Tools.State Tools { get; set; } = new();

    internal int SecondsOutOfCombat { get; set; }
}
