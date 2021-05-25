/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Indicates whether SDV normally allows a placed object with the given ID to be picked up.</summary>
            /// <param name="objectID">The object's ID, a.k.a. parent sheet index.</param>
            /// <returns>True if SDV normally allows this object to be picked up.</returns>
            private static bool CanBePickedUp(int objectID)
            {
                //if this object ID match any known "cannot be picked up" ID, return false; otherwise, return true
                switch (objectID)
                {
                    case 0:   //weeds
                    case 2:   //ruby ore
                    case 4:   //diamond ore
                    case 6:   //jade ore
                    case 8:   //amethyst ore
                    case 10:  //topaz ore
                    case 12:  //emerald ore
                    case 14:  //aquamarine ore
                    case 25:  //mussel ore
                    case 44:  //gem ore
                    case 46:  //mystic ore
                    case 75:  //geode ore
                    case 76:  //frozen geode ore
                    case 77:  //magma geode ore
                    case 95:  //radioactive ore
                    case 290: //iron ore
                    case 294: //twig
                    case 295: //
                    case 313: //weeds
                    case 314: //
                    case 315: //
                    case 316: //
                    case 317: //
                    case 318: //
                    case 319: //ice crystal (called "weeds" in the object data)
                    case 320: //
                    case 321: //
                    case 343: //stone
                    case 450: //
                    case 452: //weeds
                    case 590: //buried artifact spot
                    case 668: //stone
                    case 670: //
                    case 674: //weeds
                    case 675: //
                    case 676: //
                    case 677: //
                    case 678: //
                    case 679: //
                    case 751: //copper ore
                    case 760: //stone
                    case 762: //
                    case 764: //gold ore
                    case 765: //iridium ore
                    case 784: //weeds
                    case 785: //
                    case 786: //
                    case 792: //forest farm weed (spring)
                    case 793: //forest farm weed (summer)
                    case 794: //forest farm weed (fall)
                    case 816: //fossil ore
                    case 817: //
                    case 818: //clay ore
                    case 819: //omni geode ore
                    case 843: //cinder shard ore
                    case 844: //
                    case 845: //stone
                    case 846: //
                    case 847: //
                    case 849: //copper ore (volcano/challenge)
                    case 850: //iron ore (volcano/challenge)
                    case 882: //weeds
                    case 883: //
                    case 884: //
                    case 922: //supply crate (beach farm)
                    case 923: //
                    case 924: //
                        return false; //this ID cannot be picked up
                    default:
                        return true; //this ID can be picked up
                }
            }
        }
    }
}