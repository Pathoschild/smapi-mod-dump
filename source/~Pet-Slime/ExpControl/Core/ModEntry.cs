/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using StardewModdingAPI;
using BirbCore.Attributes;

namespace ExpControl.Core
{
    [SMod]
    public class ModEntry : Mod
    {
        [SMod.Instance]
        internal static ModEntry Instance;

        internal static Config Config;

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            Parser.ParseAll(this);
        }
    }
}
