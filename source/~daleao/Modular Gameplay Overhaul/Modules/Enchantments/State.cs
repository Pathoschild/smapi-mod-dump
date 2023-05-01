/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments;

/// <summary>The runtime state variables for ENCH.</summary>
internal sealed class State
{
    internal bool DidArtfulParry { get; set; }

    internal bool GatlingModeEngaged { get; set; }

    internal int DoublePressTimer { get; set; }
}
