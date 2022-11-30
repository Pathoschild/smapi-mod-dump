/// Lovingly stolen from the folks at Ridgeside. Thank you for letting me learn off this!
/// https://github.com/Rafseazz/Ridgeside-Village-Mod/blob/main/Ridgeside%20SMAPI%20Component%202.0/RidgesideVillage/Patches/UntimedSO.cs

using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using NeverEndingAdventure.Utils;

namespace NeverEndingAdventure.HarmonyPatches
{
    internal static class UntimedSO
    {
        private static IMonitor Monitor { get; set; } = null!;
        private static IModHelper Helper { get; set; } = null!;

        private readonly static List<string> NPCNames = new() { "Mateo" };

        private static Texture2D? CAGemojis;

        public static void ApplyPatch(Harmony harmony, IModHelper helper)
        {
            Helper = helper;

            Helper.Events.GameLoop.DayEnding += OnDayEnd;

            Log.Trace($"Applying Harmony Patch \"{nameof(UntimedSO)}\" prefixing SDV method.");

            harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.IsTimedQuest)),
                postfix: new HarmonyMethod(typeof(UntimedSO), nameof(SpecialOrders_IsTimed_postfix))
            );


            //causes issues on MAC apparently??
            if (Constants.TargetPlatform == GamePlatform.Windows)
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(SpecialOrdersBoard), nameof(SpecialOrdersBoard.GetPortraitForRequester)),
                   postfix: new HarmonyMethod(typeof(UntimedSO), nameof(SpecialOrdersBoard_GetPortrait_postfix))
               );
            }
            else
            {
                Log.Trace($"Not patching GetProtraitForRequester because platform is {Constants.TargetPlatform}");
            }

        }

        private static void SpecialOrders_IsTimed_postfix(SpecialOrder __instance, ref bool __result)
        {
            if (__instance.questKey.Value.StartsWith("CAGQuest.UntimedSpecialOrder") || __instance.questKey.Value == "Mateo.SpecialOrders.BuildGuild")
            {
                __result = false;
            }

        }

        private static void SpecialOrdersBoard_GetPortrait_postfix(SpecialOrdersBoard __instance, string requester_name, ref KeyValuePair<Texture2D, Rectangle>? __result)
        {
            try
            {
                if (CAGemojis == null)
                {
                    CAGemojis = Helper.GameContent.Load<Texture2D>(PathUtilities.NormalizeAssetName("LooseSprites\\CAGemojis"));
                    if (CAGemojis == null)
                    {
                        Log.Error($"Loading error: Couldn't load {PathUtilities.NormalizeAssetName("LooseSprites\\CAGemojis")}");
                        return;
                    }
                }


                if (__result == null)
                {
                    int index = NPCNames.FindIndex(name => name.Equals(requester_name, StringComparison.OrdinalIgnoreCase));
                    if (index != -1)
                    {
                        __result = new KeyValuePair<Texture2D, Rectangle>(CAGemojis, new Rectangle(index % 14 * 9, index / 14 * 9, 9, 9));
                        return;
                    }

                }
                return;

            }
            catch (Exception e)
            {
                Log.Error($"Error in SpecialOrdersBoard_GetPortrait_postfix:\n\n{e}");
                return;
            }


        }

        public static void OnDayEnd(object? sender, DayEndingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            foreach (SpecialOrder o in Game1.player.team.specialOrders)
            {
                if (o.questKey.Value.StartsWith("CAGQuest.UntimedSpecialOrder") || o.questKey.Value.StartsWith("Mateo.SpecialOrders.BuildGuild"))
                {
                    o.dueDate.Value = Game1.Date.TotalDays + 100;
                }

            }
        }
    }
}