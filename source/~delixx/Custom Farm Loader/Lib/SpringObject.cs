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

        //TODO: I seemingly don't need to remember the node HP anymore in 1.6

        //A variety of rocks/nodes and their HP aka. MinutesUntilReady
        public static Dictionary<SpringObjectID, int> SmallResources = new Dictionary<SpringObjectID, int>() {
            { SpringObjectID.Diamond_Node, 5 },         { SpringObjectID.Mussel_Node, 8 },           { SpringObjectID.Geode_Node, 3 },           { SpringObjectID.Frozen_Geode_Node, 5 },
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
            var variants = getVariants(item);
            return variants.OrderBy(x => Game1.random.Next()).First();
        }
        
        public static List<SpringObjectID> getVariants(SpringObjectID item)
        {
            var ret = new List<SpringObjectID> { item };

            ret.AddRange(item switch {
                SpringObjectID.Boulder => new SpringObjectID[] { SpringObjectID.Boulder_Alternative },
                SpringObjectID.Blue_Boulder => new SpringObjectID[] { SpringObjectID.Blue_Boulder_Alternative },
                SpringObjectID.Rock => new SpringObjectID[] { SpringObjectID.Rock_Alternative },
                SpringObjectID.Twig => new SpringObjectID[] { SpringObjectID.Twig_Alternative },
                SpringObjectID.Crystal => new SpringObjectID[] { SpringObjectID.Crystal_Alternative1, SpringObjectID.Crystal_Alternative2 },
                SpringObjectID.Dark_Gray_Rock => new SpringObjectID[] { SpringObjectID.Dark_Gray_Rock_Alternative },
                SpringObjectID.Bone_Node => new SpringObjectID[] { SpringObjectID.Bone_Node_Alternative },
                SpringObjectID.Cinder_Shard_Node => new SpringObjectID[] { SpringObjectID.Cinder_Shard_Node_Alternative },
                SpringObjectID.Volcano_Rock => new SpringObjectID[] { SpringObjectID.Volcano_Rock_Alternative1, SpringObjectID.Volcano_Rock_Alternative2 },
                _ => new SpringObjectID[] { }
            });

            return ret;
        }

        public static string getRandomWeedForSeason(Season season)
        {
            double random = Game1.random.NextDouble();

            return (season switch {
                Season.Spring => random switch {
                    < 0.33 => 674,
                    < 0.5 => 675,
                    _ => 784
                },

                Season.Summer => random switch {
                    < 0.33 => 676,
                    < 0.5 => 677,
                    _ => 785
                },

                Season.Fall => random switch {
                    < 0.33 => 678,
                    < 0.5 => 679,
                    _ => 786
                },

                _ => 674
            }).ToString();
        }

        public static SpringObjectID getTransformedWeedForSeason(Season season)
        {
            double random = Game1.random.NextDouble();

            return season switch {
                Season.Spring => SpringObjectID.Transformed_Weed,

                Season.Summer => SpringObjectID.Transformed_Weed_Summer,

                Season.Fall => SpringObjectID.Transformed_Weed_Fall,

                //There's no winter variant for transformed weeds, so this will have to do
                _ => SpringObjectID.Transformed_Weed
            };
        }
    }
}
