/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MercuryVN/AutoCatch
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace AutoCatch;

public sealed class ModConfig
{
    public Keybind ToggleModButton { get; set; } = new Keybind(SButton.OemTilde);
    public bool EnableMod { get; set; } = true;
}