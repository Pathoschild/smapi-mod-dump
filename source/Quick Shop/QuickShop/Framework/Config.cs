/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/QuickShop
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace QuickShop.Framework;

public class Config
{
    public static Config Default { get; } = new();
    public KeybindList OpenQuickShop { get; set; } = new(SButton.M);
    public bool AllowToolUpgradeAgain { get; set; } = false;
    
    public bool AllowBuildingAgain { get; set; } = false;
}