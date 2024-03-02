/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbCore.Attributes;
using StardewModdingAPI;

namespace BirbCore;
internal class ModEntry : Mod
{
    // bootstrapping issue, cannot use SMod.Instance here...
    internal static ModEntry Instance;

    public override void Entry(IModHelper helper)
    {
        Instance = this;
        Parser.InitEvents();
        Parser.ParseAll(this);
    }
}
