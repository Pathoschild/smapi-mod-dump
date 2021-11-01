/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TwinBuilderOne/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace MoreBundles
{
    public class ModEntry : Mod
    {
        internal static IModHelper ModHelper;

        public bool bundlesEnabled = true;

        private bool optionsMenuActive;

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;

            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            helper.Events.GameLoop.SaveCreated += GameLoop_SaveCreated;

            helper.Content.AssetLoaders.Add(new BundleDataLoader());

#if DEBUG
            helper.ConsoleCommands.Add("junimoliteracy",
                "Lets the player read bundles. Debug only.",
                (string command, string[] args) =>
                {
                    Game1.addMailForTomorrow("canReadJunimoText");
                });
#endif
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (TitleMenu.subMenu is AdvancedGameOptions gameOptions)
            {
                if (!optionsMenuActive)
                {
                    optionsMenuActive = true;

                    Monitor.Log("Replacing drop down...", LogLevel.Debug);

                    OptionsDropDown dropDown = gameOptions.options[2] as OptionsDropDown;

                    dropDown.dropDownDisplayOptions.Add("Expert");
                    dropDown.dropDownOptions.Add("Expert");

                    if (bundlesEnabled)
                    {
                        dropDown.selectedOption = 2;
                    }

                    dropDown.RecalculateBounds();

                    gameOptions.applySettingCallbacks.Add(() =>
                    {
                        try
                        {
                            bundlesEnabled = dropDown.dropDownOptions[dropDown.selectedOption] == "Expert";
                        }
                        catch { }
                    });

                    Action oldCallback = gameOptions.applySettingCallbacks[0];

                    gameOptions.applySettingCallbacks[0] = () =>
                    {
                        if (dropDown.dropDownOptions[dropDown.selectedOption] == "Expert")
                        {
                            bundlesEnabled = true;
                        }
                        else
                        {
                            oldCallback();
                        }
                    };

                    gameOptions.options[2] = dropDown;

                    Monitor.Log("Drop down replaced", LogLevel.Debug);
                }
            }
            else
            {
                optionsMenuActive = false;
            }
        }

        private void GameLoop_SaveCreated(object sender, SaveCreatedEventArgs e)
        {
            if (bundlesEnabled)
            {
                Dictionary<string, string> data = new BundleGenerator().Generate(
                    "Data\\ExpertBundles", new Random((int)Game1.uniqueIDForThisGame * 9));

                data = IridiumQualityItemsFix.FixIridiumQualityItems(data);

                Game1.netWorldState.Value.SetBundleData(data);
            }
        }
    }
}
