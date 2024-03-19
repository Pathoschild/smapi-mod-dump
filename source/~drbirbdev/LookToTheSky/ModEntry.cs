/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using BirbCore.Attributes;
using StardewModdingAPI;

namespace LookToTheSky
{
    // TODOs:
    // Multiplayer compatibility (shared animations/controls/sound)
    // Text notifications option
    // More items (balloon)
    // Screen resize functionality
    // Make sure sprites are centered correctly
    // Loot tables in JSON
    // Slingshot charge sound/cooldown???
    // Content pack functionality
    [SMod]
    public class ModEntry : Mod
    {
        [SMod.Instance]
        internal static ModEntry Instance;
        internal static Config Config;
        internal static Assets Assets;

        public readonly List<SkyObject> SkyObjects = [];
        public readonly List<SkyProjectile> Projectiles = [];

        public override void Entry(IModHelper helper)
        {
            Parser.ParseAll(this);
        }
    }
}
