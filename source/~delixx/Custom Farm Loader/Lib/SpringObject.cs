/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Custom_Farm_Loader.Lib.Enums;
using StardewModdingAPI;
using StardewValley;

namespace Custom_Farm_Loader.Lib
{
    public class SpringObject
    {
        //A variety of 2x2 resources
        public static List<SpringObjectID> LargeResources = new List<SpringObjectID> { SpringObjectID.Large_Stump, SpringObjectID.Large_Log, SpringObjectID.Boulder, SpringObjectID.Boulder_Alternative, SpringObjectID.Blue_Boulder, SpringObjectID.Blue_Boulder_Alternative, SpringObjectID.Dense_Boulder, SpringObjectID.Meteorite };


        //A variety of rocks/nodes and their HP aka. MinutesUntilReady
        public static Dictionary<SpringObjectID, int> SmallResources = new Dictionary<SpringObjectID, int>() {
            { SpringObjectID.Diamond_Node, 5 },         { SpringObjectID.Musel_Node, 8 },           { SpringObjectID.Geode_Node, 3 },           { SpringObjectID.Frozen_Geode_Node, 5 },
            { SpringObjectID.Magma_Geode_Node, 7 },     { SpringObjectID.Radioactive_Node, 25 },    { SpringObjectID.Iron_Node, 4 },            { SpringObjectID.Dark_Gray_Rock, 2 },
            { SpringObjectID.Copper_Node, 3 },          { SpringObjectID.Gold_Node, 8 },            { SpringObjectID.Iridium_Node, 16 },        { SpringObjectID.Bone_Node, 4 },
            { SpringObjectID.Clay_Node, 4 },            { SpringObjectID.Omni_Geode_Node, 8 },      { SpringObjectID.Cinder_Shard_Node, 12 },   { SpringObjectID.Volcano_Rock, 6 },
            { SpringObjectID.Volcano_Copper_Node, 1 },  { SpringObjectID.Volcano_Iron_Node, 1 }
        };

        //A variety of gem nodes, that can't be placed down using the same way as small resources
        public static Dictionary<SpringObjectID, int> GemNodes = new Dictionary<SpringObjectID, int>() {
            { SpringObjectID.Ruby_Node, 5 },    { SpringObjectID.Jade_Node, 5 },        { SpringObjectID.Amethyst_Node, 5 }, { SpringObjectID.Topaz_Node, 5  },
            { SpringObjectID.Emerald_Node, 5 }, { SpringObjectID.Aquamarine_Node, 5 },  { SpringObjectID.Gem_Node, 5 },      { SpringObjectID.Mystic_Stone, 5 }
        };

        public static SpringObjectID randomizeResourceIDs(SpringObjectID item)
        {
            bool alt = Game1.random.Next(0, 2) == 1;
            int alt3 = Game1.random.Next(1, 4);

            return item switch {
                SpringObjectID.Boulder => alt ? SpringObjectID.Boulder : SpringObjectID.Boulder_Alternative,
                SpringObjectID.Blue_Boulder => alt ? SpringObjectID.Blue_Boulder : SpringObjectID.Blue_Boulder_Alternative,
                SpringObjectID.Rock => alt ? SpringObjectID.Rock : SpringObjectID.Rock_Alternative,
                SpringObjectID.Twig => alt ? SpringObjectID.Twig : SpringObjectID.Twig_Alternative,
                SpringObjectID.Crystal => alt3 == 1 ? SpringObjectID.Crystal : alt3 == 2 ? SpringObjectID.Crystal_Alternative1 : SpringObjectID.Crystal_Alternative2,
                SpringObjectID.Dark_Gray_Rock => alt ? SpringObjectID.Dark_Gray_Rock : SpringObjectID.Dark_Gray_Rock_Alternative,
                SpringObjectID.Bone_Node => alt ? SpringObjectID.Bone_Node : SpringObjectID.Bone_Node_Alternative,
                SpringObjectID.Cinder_Shard_Node => alt ? SpringObjectID.Cinder_Shard_Node : SpringObjectID.Cinder_Shard_Node_Alternative,
                SpringObjectID.Volcano_Rock => alt3 == 1 ? SpringObjectID.Volcano_Rock : alt3 == 2 ? SpringObjectID.Volcano_Rock_Alternative1 : SpringObjectID.Volcano_Rock_Alternative2,
                _ => item
            };
        }
        public static int getRandomWeedForSeason(string season)
        {
            double random = Game1.random.NextDouble();

            return season switch {
                "spring" => random switch {
                    < 0.33 => 674,
                    < 0.5 => 675,
                    _ => 784
                },

                "summer" => random switch {
                    < 0.33 => 676,
                    < 0.5 => 677,
                    _ => 785
                },

                "fall" => random switch {
                    < 0.33 => 678,
                    < 0.5 => 679,
                    _ => 786
                },

                _ => 674
            };
        }

        public static int getTransformedWeedForSeason(string season)
        {
            double random = Game1.random.NextDouble();

            return season switch {
                "spring" => (int)SpringObjectID.Transformed_Weed,

                "summer" => (int)SpringObjectID.Transformed_Weed_Summer,

                "fall" => (int)SpringObjectID.Transformed_Weed_Fall,

                //There's no winter variant for transformed weeds, so this will have to do
                _ => (int)SpringObjectID.Transformed_Weed
            };
        }
    }
}
