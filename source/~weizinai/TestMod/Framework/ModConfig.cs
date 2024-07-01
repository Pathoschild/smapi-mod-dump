/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.TestMod.Framework;

public class ModConfig
{
    public KeybindList Key { get; set; } = new(SButton.L);
    
    public int MineShaftMap { get; set; } = 40;
    public int VolcanoDungeonMap { get; set; } = 46;
    
    public int RandomInt { get; set; } = 0;
    public bool RandomBool { get; set; } = false;
}