/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/siweipancc/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace AutoMeleeAttack;

public sealed class ModConfig
{
    public bool SkipRockCrab { get; set; }

    public int DetectTiles { get; set; } = 3;

    public SButton Toggle { get; set; } = SButton.Q;

    public SortedDictionary<string, bool>? SkipAlso { get; set; } = new();
}