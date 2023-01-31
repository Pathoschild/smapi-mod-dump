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
        public string ID;
        public string Type;

        public int ZOffset;

        public int Range;
        public int MaxFlocks;

        public static FeederDef FromFeeder(CustomBigCraftable feeder)
        {
            return ContentManager.Feeders.FirstOrDefault(feederDef => feederDef.ID == feeder.Id);
        }
    }
}
