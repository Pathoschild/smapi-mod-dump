/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/MultipleSpouseDialogs
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace MultipleSpouseDialog
{
    public class ModEntry : Mod
    {
        public static IMonitor PMonitor;
        public static IModHelper PHelper;
        public static ModConfig config;
        public static Random myRand;
        public static List<ConfigDialog> dialogs;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();

            if (!config.EnableMod) return;

            PMonitor = Monitor;
            PHelper = helper;

            myRand = new Random();

            Helper.Events.GameLoop.GameLaunched += onLaunched;

            helper.Events.GameLoop.SaveLoaded += HelperEvents.GameLoop_SaveLoaded;
            helper.Events.GameLoop.DayStarted += HelperEvents.GameLoop_DayStarted;
            helper.Events.GameLoop.DayEnding += HelperEvents.GameLoop_DayEnding;
            helper.Events.GameLoop.ReturnedToTitle += HelperEvents.GameLoop_ReturnedToTitle;

            HelperEvents.Initialize(Monitor, Helper);
            Misc.Initialize(Monitor, Helper, config);
        }

        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            Dialog.Initialize(Monitor, PHelper, ModManifest);
            Dialog.cp_api = this.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");

            Dialog.Load(Path.Combine(Helper.DirectoryPath, "Assets"));

            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                // this.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}", LogLevel.Debug);
                Dialog.Load(contentPack.DirectoryPath);
            }

            config = Helper.ReadConfig<ModConfig>();
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api is null) return;
            api.RegisterModConfig(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
            api.SetDefaultIngameOptinValue(ModManifest, true);
            api.RegisterSimpleOption(ModManifest, "Enable Mod", "Enable Mod", () => config.EnableMod, (bool val) => config.EnableMod = val);
            api.RegisterSimpleOption(ModManifest, "Allow Spouses To Chat", "Allow Spouses To Chat", () => config.AllowSpousesToChat, (bool val) => config.AllowSpousesToChat = val);
            api.RegisterSimpleOption(ModManifest, "Chat with Player", "Chat with Player", () => config.ChatWithPlayer, (bool val) => config.ChatWithPlayer = val);
            api.RegisterSimpleOption(ModManifest, "Prevent Relatives Chatting", "Prevent Relatives From Chatting", () => config.PreventRelativesFromChatting, (bool val) => config.PreventRelativesFromChatting = val);
            api.RegisterSimpleOption(ModManifest, "Min Hearts For Chat", "Min Hearts For Chat", () => config.MinHeartsForChat, (int val) => config.MinHeartsForChat = val);
            api.RegisterClampedOption(ModManifest, "Spouse Chat Chance", "Spouse Chat Chance", () => config.SpouseChatChance, (float val) => config.SpouseChatChance = val, 0.0f, 1.0f, 0.05f);
            api.RegisterSimpleOption(ModManifest, "Min Distance To Chat", "Min Distance To Chat", () => config.MinDistanceToChat, (float val) => config.MinDistanceToChat = val);
            api.RegisterSimpleOption(ModManifest, "Max Distance To Chat", "Max Distance To Chat", () => config.MaxDistanceToChat, (float val) => config.MaxDistanceToChat = val);
            api.RegisterSimpleOption(ModManifest, "Min Spouse Chat Interval", "(seconds)", () => config.MinSpouseChatInterval, (float val) => config.MinSpouseChatInterval = val);
            api.RegisterSimpleOption(ModManifest, "Extra Debug Output", "Extra Debug Output", () => config.ExtraDebugOutput, (bool val) => config.ExtraDebugOutput = val);
        }
    }
}
