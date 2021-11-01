/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/shekurika/DailySpecialOrders
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using HarmonyLib;
using static StardewValley.SpecialOrder;

namespace DailySpecialOrders
{
    public class ModEntry : Mod
    {
        ModConfig Config;
        Harmony harmony;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            harmony = new Harmony(this.ModManifest.UniqueID);
            HarmonyPatches.ApplyPatches(harmony);
        }

        //setup GMCM
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config = this.Helper.ReadConfig<ModConfig>() ?? new ModConfig();
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                var Translation = Helper.Translation;
                api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
                api.SetDefaultIngameOptinValue(ModManifest, true);
                api.RegisterSimpleOption(ModManifest, Translation.Get("Immediate.Refresh"), "", () => Config.RefreshAfterPicking, (bool val) => Config.RefreshAfterPicking = val);
                api.RegisterParagraph(ModManifest, Translation.Get("Immediate.Paragraph"));
                api.RegisterSimpleOption(ModManifest, Translation.Get("Refresh.on.Tuesday"), "", () => Config.RefreshTuesday, (bool val) => Config.RefreshTuesday = val);
                api.RegisterSimpleOption(ModManifest, Translation.Get("Refresh.on.Wednesday"), "", () => Config.RefreshWednesday, (bool val) => Config.RefreshWednesday = val);
                api.RegisterSimpleOption(ModManifest, Translation.Get("Refresh.on.Thursday"), "", () => Config.RefreshThursday, (bool val) => Config.RefreshThursday = val);
                api.RegisterSimpleOption(ModManifest, Translation.Get("Refresh.on.Friday"), "", () => Config.RefreshFriday, (bool val) => Config.RefreshFriday = val);
                api.RegisterSimpleOption(ModManifest, Translation.Get("Refresh.on.Saturday"), "", () => Config.RefreshSaturday, (bool val) => Config.RefreshSaturday = val);
                api.RegisterSimpleOption(ModManifest, Translation.Get("Refresh.on.Sunday"), "", () => Config.RefreshSunday, (bool val) => Config.RefreshSunday = val);

            }
        }

        /// <summary>
        /// Detect if the Special Order Board was just closed and if yes, refresh Special Orders if needed
        /// </summary>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (Config.RefreshAfterPicking && e.OldMenu is SpecialOrdersBoard && !(e.NewMenu is SpecialOrdersBoard))
            {
                refreshSpecialOrders();
            }
        }

        /// <summary>
        /// Refreshes if appropiate
        /// </summary>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Game1.player.IsMainPlayer)
            {
                return;
            }
            bool shouldRefresh = false;
            switch (Game1.dayOfMonth % 7)
            {
                case 2:
                    shouldRefresh = Config.RefreshTuesday;
                    break;
                case 3:
                    shouldRefresh = Config.RefreshWednesday;
                    break;
                case 4:
                    shouldRefresh = Config.RefreshThursday;
                    break;
                case 5:
                    shouldRefresh = Config.RefreshFriday;
                    break;
                case 6:
                    shouldRefresh = Config.RefreshSaturday;
                    break;
                case 7:
                    shouldRefresh = Config.RefreshSunday;
                    break;
            }
            if (shouldRefresh)
            {
                refreshSpecialOrders();
            }
        }


        private void refreshSpecialOrders()
        {
            SpecialOrder.UpdateAvailableSpecialOrders(force_refresh: true);
        }

        /// <summary>
        /// Postfix SetDuration so the player gets the correct duration to complete the SpecialOrder
        /// (In vanilla they'd get (duration - daysSinceMonday), now its duration)
        /// </summary>
        static class HarmonyPatches{

            public static void ApplyPatches(Harmony harmony)
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.SetDuration)),
                   postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.SpecialOrder_SetDuration_PostFix))
                );
            }
            /// <summary>
            /// Harmony Postifx function to change the remaining duration of the special order to not start on monday.
            /// </summary>
            /// <param name="__instance"></param>
            /// <param name="duration"></param>
            public static void SpecialOrder_SetDuration_PostFix(ref SpecialOrder __instance, QuestDuration duration)
            {
                if(__instance == null)
                {
                    return;
                }
                WorldDate date = new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
                switch (duration)
                {
                    case QuestDuration.Week:
                        date.TotalDays += 7;
                        break;
                    case QuestDuration.TwoWeeks:
                        date.TotalDays += 14;
                        break;
                    case QuestDuration.Month:
                        date.TotalDays += 28;
                        break;
                    case QuestDuration.TwoDays:
                        date.TotalDays += 2;
                        break;
                    case QuestDuration.ThreeDays:
                        date.TotalDays += 3;
                        break;
                }
                __instance.dueDate.Value = date.TotalDays;
            }
        }
}

}