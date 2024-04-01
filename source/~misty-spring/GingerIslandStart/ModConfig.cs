/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace GingerIslandStart;

public class ModConfig
{
    public string Difficulty { get; set; } = "normal";
    public string Shops { get; set; } = "normal";
    public string MonsterDifficulty { get; set; } = "normal";
    public bool FasterWeaponAccess { get; set; }
    public bool RodUpgrades { get; set; }
}