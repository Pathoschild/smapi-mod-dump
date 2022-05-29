/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MarketDay.Utility
{
    public static class Wizard
    {
        private enum Challenge { Yes, No, Rude, NotAnswered }

        private enum Shops { S3, S6, S9, S12, S15, NotAnswered }

        private enum Day { Friday, Saturday, Sunday, NotAnswered }

        private enum Weather { Fine, Any, NotAnswered }

        private enum ShowGMCM { Yes, No, NotAnswered }

        internal static void ConfigureFromWizard(object o, OneSecondUpdateTickedEventArgs e) {
            // check if host
            if (!Context.IsWorldReady) return;
            if (!Context.IsMainPlayer) return;

            // see if event has run
            if (! Game1.player.eventsSeen.Contains(117780001))
            {
                Game1.getFarm().modData.Remove($"{MarketDay.SMod.ModManifest.UniqueID}/ConfigurationWizardDone");
                return;
            }

            // see if already configured
            if (Game1.getFarm().modData.ContainsKey($"{MarketDay.SMod.ModManifest.UniqueID}/ConfigurationWizardDone")) {
                return;
            }
            
            // set marker that config is done
            Game1.getFarm().modData[$"{MarketDay.SMod.ModManifest.UniqueID}/ConfigurationWizardDone"] = "true";
            
            // collect answers from event
            Challenge p = Challenge.NotAnswered;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789010)) p = Challenge.Yes;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789011)) p = Challenge.No;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789012)) p = Challenge.Rude;

            Shops s = Shops.NotAnswered;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789020)) s = Shops.S3;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789021)) s = Shops.S6;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789022)) s = Shops.S9;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789023)) s = Shops.S12;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789024)) s = Shops.S15;
            
            Day d = Day.NotAnswered;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789030)) d = Day.Friday;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789031)) d = Day.Saturday;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789032)) d = Day.Sunday;

            Weather w = Weather.NotAnswered;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789040)) w = Weather.Fine;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789041)) w = Weather.Any;

            ShowGMCM g = ShowGMCM.NotAnswered;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789050)) g = ShowGMCM.Yes;
            if (Game1.player.dialogueQuestionsAnswered.Contains(117789051)) g = ShowGMCM.No;

            // set config accordingly
            if (p is Challenge.Rude or Challenge.NotAnswered) { return; }
            MarketDay.Config.Progression = p != Challenge.No;

            switch (d)
            {
                case Day.Friday: MarketDay.Config.DayOfWeek = 5; break;
                case Day.Saturday: MarketDay.Config.DayOfWeek = 6; break;
                case Day.Sunday: MarketDay.Config.DayOfWeek = 0; break;
                case Day.NotAnswered: break;
                default: throw new Exception($"Unhandled value {d} for Day");
            }

            switch (w)
            {
                case Weather.Fine: 
                    MarketDay.Config.OpenInRain = false;
                    MarketDay.Config.OpenInSnow = false;
                    break;
                case Weather.Any: 
                    MarketDay.Config.OpenInRain = true;
                    MarketDay.Config.OpenInSnow = true;
                    break;
                case Weather.NotAnswered: break;
                default: throw new Exception($"Unhandled value {w} for Weather");
            }

            switch (s)
            {
                case Shops.S3: MarketDay.Config.NumberOfShops = 3; break;
                case Shops.S6: MarketDay.Config.NumberOfShops = 6; break;
                case Shops.S9: MarketDay.Config.NumberOfShops = 9; break;
                case Shops.S12: MarketDay.Config.NumberOfShops = 12; break;
                case Shops.S15: MarketDay.Config.NumberOfShops = 15; break;
                case Shops.NotAnswered: break;
                default: throw new Exception($"Unhandled value {s} for Shops");
            }

            switch (s)
            {
                case Shops.S3: MarketDay.Config.NumberOfShops = 3; break;
                case Shops.S6: MarketDay.Config.NumberOfShops = 6; break;
                case Shops.S9: MarketDay.Config.NumberOfShops = 9; break;
                case Shops.S12: MarketDay.Config.NumberOfShops = 12; break;
                case Shops.S15: MarketDay.Config.NumberOfShops = 15; break;
                case Shops.NotAnswered: break;
                default: throw new Exception($"Unhandled value {s} for Shops");
            }

            // save config
            MarketDay.SaveConfig();
            
            // pop up GMCM if requested
            if (g == ShowGMCM.Yes) {
                var configMenu = MarketDay.helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                if (configMenu is null) {
                    MessageUtility.SendMessage(MarketDay.Get("wizard.edit-options"));
                    MessageUtility.SendMessage(MarketDay.Get("wizard.install-gmcm"));
                    return;
                }

                ShowConfigurationMenu();
            }
        }

        private static void ShowConfigurationMenu() {
            Game1.playSound("shwip");
            var configMenu = MarketDay.helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            configMenu.OpenModMenu(MarketDay.SMod.ModManifest);
        }
    }
}