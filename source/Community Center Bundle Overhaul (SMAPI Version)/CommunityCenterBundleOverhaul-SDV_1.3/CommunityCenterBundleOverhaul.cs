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
using System.IO;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using CommunityCenterBundleOverhaul_SDV_13.Framework;
using System.Linq;

namespace CommunityCenterBundleOverhaul_SDV_13
{
    /// <summary>The mod entry point.</summary>
    public class CommunityCenterBundleOverhaul : Mod
    {
        /*********
        ** Properties
        *********/

        private ModConfig config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.ConsoleCommands.Add("ccbo", helper.Translation.Get("ccbo.desc"), this.ccbo);
            this.Monitor.Log("Initialisation finished. Bundels are now out of control :D", LogLevel.Info);

            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
        }

        /// <summary>Command for administrating CCBO Mod. Usage '/ccbo' for more information</summary>
        /// <param name="ccbo">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void ccbo(string command, string[] args)
        {
            Bundle[] data = this.Helper.ReadJsonFile<Bundle[]>(@"bundles\bundles.json");
            if (!Context.IsMultiplayer)
                config = this.Helper.ReadJsonFile<ModConfig>($"data/{Constants.SaveFolderName}.json") ?? new ModConfig();

            if (Context.IsMultiplayer)
                config = this.Helper.ReadJsonFile<ModConfig>($"data/{Game1.player.UniqueMultiplayerID}.json") ?? new ModConfig();

            string name;
            int id;

            try
            {
                if (String.IsNullOrEmpty(args[0]))
                    this.Monitor.Log(this.Helper.Translation.Get("ccbo.desc"));

                if (args[0].Equals("get"))
                    foreach (Bundle bundle in data)
                    {
                        if (bundle.ID == config.SelectionID)
                        {
                            this.Monitor.Log(this.Helper.Translation.Get("cmd.get.desc") + bundle.Name + " ("+config.SelectionID+")");
                        }
                    }

                if (args[0].Equals("listAll"))
                {
                    this.Monitor.Log(this.Helper.Translation.Get("cmd.listAll.desc"));

                    foreach (Bundle bundle in data)
                    {
                        name = bundle.Name;
                        id = bundle.ID;
                        this.Monitor.Log($"- {name} ({id})\n");
                    }
                }

                if (args[0].Equals("set"))
                {
                    switch (args[1])
                    {
                        case "0":
                            config.SelectionID = int.Parse(args[1]);
                            if (!Context.IsMultiplayer)
                                this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", config);

                            if (Context.IsMultiplayer)
                                this.Helper.WriteJsonFile($"data/{Game1.player.UniqueMultiplayerID}.json", config);

                            dummyMethod(data);
                            break;
                        case "1":
                            config.SelectionID = int.Parse(args[1]);
                            if (!Context.IsMultiplayer)
                                this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", config);

                            if (Context.IsMultiplayer)
                                this.Helper.WriteJsonFile($"data/{Game1.player.UniqueMultiplayerID}.json", config);
                            dummyMethod(data);
                            break;
                        case "2":
                            config.SelectionID = int.Parse(args[1]);
                            if (!Context.IsMultiplayer)
                                this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", config);

                            if (Context.IsMultiplayer)
                                this.Helper.WriteJsonFile($"data/{Game1.player.UniqueMultiplayerID}.json", config);
                            dummyMethod(data);
                            break;
                        case "3":
                            config.SelectionID = int.Parse(args[1]);
                            if (!Context.IsMultiplayer)
                                this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", config);

                            if (Context.IsMultiplayer)
                                this.Helper.WriteJsonFile($"data/{Game1.player.UniqueMultiplayerID}.json", config);
                            dummyMethod(data);
                            break;
                        case "4":
                            config.SelectionID = int.Parse(args[1]);
                            if (!Context.IsMultiplayer)
                                this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", config);

                            if (Context.IsMultiplayer)
                                this.Helper.WriteJsonFile($"data/{Game1.player.UniqueMultiplayerID}.json", config);
                            dummyMethod(data);
                            break;
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                this.Monitor.Log(this.Helper.Translation.Get("ccbo.desc"));
            }
        }

        /*********
        ** Private methods
        *********/
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {

            if (!Context.IsWorldReady)
                return;

            if (Context.IsMultiplayer)
                useMP(config);

            if (!Context.IsMultiplayer)
                useSP(config);

        }

        private void useMP(ModConfig config)
        {
            config = this.Helper.ReadJsonFile<ModConfig>($"data/{Game1.player.UniqueMultiplayerID}.json") ?? new ModConfig();
            triggerEditor(config);
        }

        private void useSP(ModConfig config)
        {
            config = this.Helper.ReadJsonFile<ModConfig>($"data/{Constants.SaveFolderName}.json") ?? new ModConfig();
            triggerEditor(config);
        }

        private void triggerEditor(ModConfig config)
        {
            this.Helper.Content.AssetEditors.Add(new ImageEditor(this.Helper, this.Monitor, config));
            this.Helper.Content.AssetEditors.Add(new BundleEditor(this.Helper, this.Monitor, config));
        }

        private void InvalidateCache(IModHelper helper)
        {
            string bundleXnb = "Data\\Bundles.xnb";
            string JunimoNoteXnb = "LooseSprites\\JunimoNote.xnb";

            //this.Monitor.Log($"{bundleXnb} {JunimoNoteXnb}");

            helper.Content.InvalidateCache(bundleXnb);
            helper.Content.InvalidateCache(JunimoNoteXnb);
        }

        private void dummyMethod(Bundle[] data)
        {
            
            this.Monitor.Log("[CCBO] Changing Bundle ...");
            this.Monitor.Log(this.Helper.Translation.Locale);
            InvalidateCache(this.Helper);

            foreach (Bundle bundle in data)
            {
                if (bundle.ID == config.SelectionID)
                {
                    Game1.addHUDMessage(new HUDMessage("Changed Community Center Bundle to: " + bundle.Name, 3) { noIcon = true, timeLeft = HUDMessage.defaultTime });
                    this.Helper.Content.AssetEditors.Add(new ImageEditor(this.Helper, this.Monitor, config));
                    this.Helper.Content.AssetEditors.Add(new BundleEditor(this.Helper, this.Monitor, config));
                }
            }
            this.Monitor.Log("[CCBO] Bundle change saved successfully. Please open a Bundle so that chances will take effect. If smth. is missing, you must restart your game.");
        }
    }
}
