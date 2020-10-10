/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chaos234/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using StardewConfigFramework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using CommunityCenterBundleOverhaul.Framework;

namespace CommunityCenterBundleOverhaul
{
    /// <summary>The mod entry point.</summary>
    public class CommunityCenterBundleOverhaul : Mod
    {
        /*********
        ** Properties
        *********/
        private ModOptionSelection DropDown;
        private ModOptions options;
        private ModOptionTrigger saveButton;
        private string[] saves;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            bool isLoaded = this.Helper.ModRegistry.IsLoaded("Juice805.StardewConfigMenu");
            if (!isLoaded)
            {
                this.Monitor.Log("Initialisation failed because StardewConfigMenu seems not to be correctly installed. This Mod will now do nothing.", LogLevel.Error);
                return;
            }
            this.Monitor.Log("Initialisation finished. Bundels are now out of control :D", LogLevel.Info);
            saves = Directory.GetFiles(this.Helper.DirectoryPath, "StardewConfig-*", SearchOption.TopDirectoryOnly);

            if (saves.Length < 1)
            {
                create_menu();
            }
            
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
        }
        
        /*********
        ** Private methods
        *********/
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {

            if (!Context.IsWorldReady)
                return;
            //options = ModOptions.LoadCharacterSettings(this, Constants.SaveFolderName);
            if (saves.Length > 0)
            {
                create_menu();
            }

            this.Helper.Content.AssetEditors.Add(new ImageEditor(this.Helper, this.Monitor, this.DropDown));
            this.Helper.Content.AssetEditors.Add(new BundleEditor(this.Helper, this.Monitor, this.DropDown));
        }

        private void InvalidateCache(IModHelper helper)
        {
            string bundleXnb = "Data\\Bundles.xnb";
            string JunimoNoteXnb = "LooseSprites\\JunimoNote.xnb";

            this.Monitor.Log($"{bundleXnb} {JunimoNoteXnb}");

            helper.Content.InvalidateCache(bundleXnb);
            helper.Content.InvalidateCache(JunimoNoteXnb);
        }

        private void create_menu()
        {
            options = ModOptions.LoadCharacterSettings(this, Constants.SaveFolderName);
            IModSettingsFramework.Instance.AddModOptions(options);

            var list = new ModSelectionOptionChoices
            {
                {"default", this.Helper.Translation.Get("default")},
                {"v02", this.Helper.Translation.Get("v02")},
                {"v02a", this.Helper.Translation.Get("v02a")},
                {"v02b", this.Helper.Translation.Get("v02b")},
                {"v02c", this.Helper.Translation.Get("v02c")}
            };

            this.DropDown = options.GetOptionWithIdentifier<ModOptionSelection>("bundle") ?? new ModOptionSelection("bundle", "Bundels", list);
            options.AddModOption(this.DropDown);

            this.DropDown.hoverTextDictionary = new Dictionary<string, string>
            {
                {"default", this.Helper.Translation.Get("default.desc")},
                {"v02", this.Helper.Translation.Get("v02.desc")},
                {"v02a", this.Helper.Translation.Get("v02a.desc")},
                {"v02b", this.Helper.Translation.Get("v02b.desc")},
                {"v02c", this.Helper.Translation.Get("v02c.desc")}
            };

            saveButton = new ModOptionTrigger("okButton", this.Helper.Translation.Get("okButton"), OptionActionType.OK);
            options.AddModOption(saveButton);

            saveButton.ActionTriggered += id =>
            {
                this.Monitor.Log("[CCBO] Changing Bundle ...");

                options.SaveCharacterSettings(Constants.SaveFolderName);

                this.Monitor.Log(this.Helper.Translation.Locale);

                InvalidateCache(this.Helper);
                Game1.addHUDMessage(new HUDMessage("Changed Community Center Bundle to: " + this.DropDown.Selection, 3) { noIcon = true, timeLeft = HUDMessage.defaultTime });
                this.Monitor.Log("[CCBO] Bundle changed successfully. If smth. is missing, you must restart your game.");
            };
        }
    }
}
