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

namespace Custom_Farm_Loader.Lib.Enums
{
    public enum SpringObjectID
    {
        Weed = 0,
        Rock = 343,
        Rock_Alternative = 450,
        Twig = 294,
        Twig_Alternative = 295,

        Large_Stump = 600,
        Large_Log = 602,
        Meteorite = 622,
        Dense_Boulder = 672,
        Boulder = 752,
        Boulder_Alternative = 754,
        Blue_Boulder = 756,
        Blue_Boulder_Alternative = 758,

        Diamond_Node = 2, //5
        Ruby_Node = 4, //5
        Jade_Node = 6, //5
        Amethyst_Node = 8, //5
        Topaz_Node = 10, //5
        Emerald_Node = 12, //5
        Aquamarine_Node = 14, //5
        Musel_Node = Mussel_Node, //Typo fix
        Mussel_Node = 25, //8
        Gem_Node = 44, //5
        Mystic_Stone = 46, //5
        Geode_Node = 75, //3
        Frozen_Geode_Node = 76, //5
        Magma_Geode_Node = 77, //7
        Radioactive_Node = 95, //25
        Iron_Node = 290, //4
        Crystal = 319,
        Crystal_Alternative1 = 320,
        Crystal_Alternative2 = 321,
        Artifact_Spot = 590, //initialStack = 1
        Dark_Gray_Rock = 668, //2
        Dark_Gray_Rock_Alternative = 670,
        Copper_Node = 751, //3
        Gold_Node = 764, //8
        Iridium_Node = 765, //16
        Transformed_Weed = 792,
        Transformed_Weed_Summer = 793,
        Transformed_Weed_Fall = 794,
        Bone_Node = 816, //4
        Bone_Node_Alternative = 817,
        Clay_Node = 818, //4
        Omni_Geode_Node = 819, //8
        Cinder_Shard_Node = 843, //12
        Cinder_Shard_Node_Alternative = 844,
        Volcano_Rock = 845, //6
        Volcano_Rock_Alternative1 = 846,
        Volcano_Rock_Alternative2 = 847,
        Volcano_Copper_Node = 849, //1
        Volcano_Iron_Node = 850, //1
    }
}
