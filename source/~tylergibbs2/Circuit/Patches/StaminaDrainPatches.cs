/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

namespace Circuit.Patches
{
    internal class StaminaDrainPatches
    {
        [HarmonyPatch(typeof(Axe), nameof(Axe.DoFunction))]
        public class AxeDoFunction
        {
            public static bool Prefix(Farmer who, out float __state)
            {
                __state = who.Stamina;
                return true;
            }

            public static void Postfix(Farmer who, float __state)
            {
                if (!ModEntry.ShouldPatch(EventType.StaminaDrain) || who != Game1.player)
                    return;

                float staminaUsed = __state - who.Stamina;
                if (staminaUsed <= 0f)
                    return;

                who.Stamina -= staminaUsed;
            }
        }

        [HarmonyPatch(typeof(FishingRod), nameof(FishingRod.DoFunction))]
        public class FishingRodDoFunction
        {
            public static bool Prefix(Farmer who, out float __state)
            {
                __state = who.Stamina;
                return true;
            }

            public static void Postfix(Farmer who, float __state)
            {
                if (!ModEntry.ShouldPatch(EventType.StaminaDrain) || who != Game1.player)
                    return;

                float staminaUsed = __state - who.Stamina;
                if (staminaUsed <= 0f)
                    return;

                who.Stamina -= staminaUsed;
            }
        }

        [HarmonyPatch(typeof(Hoe), nameof(Hoe.DoFunction))]
        public class HoeDoFunction
        {
            public static bool Prefix(Farmer who, out float __state)
            {
                __state = who.Stamina;
                return true;
            }

            public static void Postfix(Farmer who, float __state)
            {
                if (!ModEntry.ShouldPatch(EventType.StaminaDrain) || who != Game1.player)
                    return;

                float staminaUsed = __state - who.Stamina;
                if (staminaUsed <= 0f)
                    return;

                who.Stamina -= staminaUsed;
            }
        }

        [HarmonyPatch(typeof(MilkPail), nameof(MilkPail.DoFunction))]
        public class MilkPailDoFunction
        {
            public static bool Prefix(Farmer who, out float __state)
            {
                __state = who.Stamina;
                return true;
            }

            public static void Postfix(Farmer who, float __state)
            {
                if (!ModEntry.ShouldPatch(EventType.StaminaDrain) || who != Game1.player)
                    return;

                float staminaUsed = __state - who.Stamina;
                if (staminaUsed <= 0f)
                    return;

                who.Stamina -= staminaUsed;
            }
        }

        [HarmonyPatch(typeof(Pickaxe), nameof(Pickaxe.DoFunction))]
        public class PickaxeDoFunction
        {
            public static bool Prefix(Farmer who, out float __state)
            {
                __state = who.Stamina;
                return true;
            }

            public static void Postfix(Farmer who, float __state)
            {
                if (!ModEntry.ShouldPatch(EventType.StaminaDrain) || who != Game1.player)
                    return;

                float staminaUsed = __state - who.Stamina;
                if (staminaUsed <= 0f)
                    return;

                who.Stamina -= staminaUsed;
            }
        }

        [HarmonyPatch(typeof(Seeds), nameof(Seeds.DoFunction))]
        public class SeedsDoFunction
        {
            public static bool Prefix(Farmer who, out float __state)
            {
                __state = who.Stamina;
                return true;
            }

            public static void Postfix(Farmer who, float __state)
            {
                if (!ModEntry.ShouldPatch(EventType.StaminaDrain) || who != Game1.player)
                    return;

                float staminaUsed = __state - who.Stamina;
                if (staminaUsed <= 0f)
                    return;

                who.Stamina -= staminaUsed;
            }
        }

        [HarmonyPatch(typeof(Shears), nameof(Shears.DoFunction))]
        public class ShearsDoFunction
        {
            public static bool Prefix(Farmer who, out float __state)
            {
                __state = who.Stamina;
                return true;
            }

            public static void Postfix(Farmer who, float __state)
            {
                if (!ModEntry.ShouldPatch(EventType.StaminaDrain) || who != Game1.player)
                    return;

                float staminaUsed = __state - who.Stamina;
                if (staminaUsed <= 0f)
                    return;

                who.Stamina -= staminaUsed;
            }
        }

        [HarmonyPatch(typeof(WateringCan), nameof(WateringCan.DoFunction))]
        public class WateringCanDoFunction
        {
            public static bool Prefix(Farmer who, out float __state)
            {
                __state = who.Stamina;
                return true;
            }

            public static void Postfix(Farmer who, float __state)
            {
                if (!ModEntry.ShouldPatch(EventType.StaminaDrain) || who != Game1.player)
                    return;

                float staminaUsed = __state - who.Stamina;
                if (staminaUsed <= 0f)
                    return;

                who.Stamina -= staminaUsed;
            }
        }
    }
}
