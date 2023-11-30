/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

global using SObject = StardewValley.Object;
using BirbCore.Attributes;
using StardewModdingAPI;

namespace RanchingToolUpgrades;

[SMod]
internal class ModEntry : Mod
{
    [SMod.Instance]
    public static ModEntry Instance;
    public static Config Config;

    public override void Entry(IModHelper helper)
    {
        Parser.ParseAll(this);
    }
}
