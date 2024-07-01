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
using StardewValley;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>A collection of keys for this mod's <see cref="IHaveModData.modData"/> entries.</summary>
            public static class ModDataKeys
            {
                private static string canBePickedUp = null;
                /// <summary>The unique key used with the <see cref="ConfigItem.CanBePickedUp"/> item setting.</summary>
                public static string CanBePickedUp
                {
                    get
                    {
                        if (canBePickedUp == null)
                            canBePickedUp = Utility.Helper.ModRegistry.ModID + "/CanBePickedUp";
                        return canBePickedUp;
                    }
                }

                private static string extraLoot = null;
                /// <summary>The unique key used with the "ExtraLoot" setting in <see cref="MonsterType.Settings"/>.</summary>
                public static string ExtraLoot
                {
                    get
                    {
                        if (extraLoot == null)
                            extraLoot = Utility.Helper.ModRegistry.ModID + "/ExtraLoot";
                        return extraLoot;
                    }
                }

                private static string instantKillImmunity = null;
                /// <summary>The unique key used with the "InstantKillImmunity" setting in <see cref="MonsterType.Settings"/>.</summary>
                public static string InstantKillImmunity
                {
                    get
                    {
                        if (instantKillImmunity == null)
                            instantKillImmunity = Utility.Helper.ModRegistry.ModID + "/InstantKillImmunity";
                        return instantKillImmunity;
                    }
                }

                private static string stunImmunity = null;
                /// <summary>The unique key used with the "StunImmunity" setting in <see cref="MonsterType.Settings"/>.</summary>
                public static string StunImmunity
                {
                    get
                    {
                        if (stunImmunity == null)
                            stunImmunity = Utility.Helper.ModRegistry.ModID + "/StunImmunity";
                        return stunImmunity;
                    }
                }
            }
        }
    }
}
