/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Gets the default durability value used in Stardew Valley's code for a type of ore.</summary>
            /// <remarks>This method is designed for use with ore, and may not contain values for other object types.</remarks>
            /// <param name="objectID">The object's ID, a.k.a. ParentSheetIndex.</param>
            /// <returns>The value Stardew typically uses for this object's durability, a.k.a. <see cref="StardewValley.Object.MinutesUntilReady"/>. Returns null if there is no known default.</returns>
            public static int? GetDefaultDurability(int objectID)
            {
                switch (objectID)
                {
                    case 343: //stone (outdoor)
                    case 450: //
                        return 1;
                    case 32:  //stone (quarry/mine)
                    case 38:  //
                    case 40:  //
                    case 42:  //
                    case 668: //
                    case 670: //
                        return 2; //varies by context in SDV's code
                    case 34: //stone (dark)
                    case 36: //
                        return 1;
                    case 48: //stone (blue)
                    case 50: //
                    case 52: //
                    case 54: //
                        return 3;
                    case 56: //stone (red)
                    case 58: //
                        return 4;
                    case 845: //stone (volcano)
                    case 846: //
                    case 847: //
                        return 6;
                    case 75: //geode
                        return 3;
                    case 76: //frozen geode
                        return 5;
                    case 77: //magma geode
                        return 7;
                    case 819: //omni geode
                        return 8;
                    case 751: //copper
                        return 3;
                    case 849: //copper (volcano/challenge)
                        return 6;
                    case 290: //iron
                        return 4;
                    case 764: //gold
                        return 8;
                    case 765: //iridium
                        return 16;
                    case 46: //mystic
                        return 12;
                    case 95: //radioactive
                        return 25;
                    case 2:  //diamond
                    case 4:  //ruby
                    case 6:  //jade
                    case 8:  //amethyst
                    case 10: //topaz
                    case 12: //emerald
                    case 14: //aquamarine
                    case 44: //gem
                        return 5; //varies by context in SDV's code
                    case 25: //mussel
                        return 8;
                    case 816: //fossil
                    case 817: //
                    case 818: //clay
                        return 4;
                    case 843: //cinder shard
                    case 844: //
                        return 12;
                    case 922: //supply crate (beach farm)
                    case 923: //
                    case 924: //
                        return 3;

                    default: //no known durability
                        return null;
                }
            }
        }
    }
}