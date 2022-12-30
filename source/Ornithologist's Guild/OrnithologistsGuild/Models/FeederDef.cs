/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System.Linq;
using DynamicGameAssets.Game;
using OrnithologistsGuild.Content;

namespace OrnithologistsGuild.Models
{
    public class FeederDef
    {
        // TODO capitalize like BirdieDef
        public string id;
        public string type;

        public int zOffset;

        public int range;
        public int maxFlocks;

        public static FeederDef FromFeeder(CustomBigCraftable feeder)
        {
            return ContentManager.Feeders.FirstOrDefault(feederDef => feederDef.id == feeder.Id);
        }
    }
}

