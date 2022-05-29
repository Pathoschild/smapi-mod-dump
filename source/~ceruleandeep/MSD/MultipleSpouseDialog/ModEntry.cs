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
using System.IO;
using ContentPatcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MultipleSpouseDialog
{
    public class ModEntry : Mod
    {
        internal static IMonitor PMonitor;
        internal static ModConfig config;
        internal static Random myRand;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();

            if (!config.EnableMod) return;

            PMonitor = Monitor;

            myRand = new Random();

            Helper.Events.GameLoop.GameLaunched += onLaunched;

            helper.Events.GameLoop.SaveLoaded += HelperEvents.GameLoop_SaveLoaded;
            helper.Events.GameLoop.DayStarted += HelperEvents.GameLoop_DayStarted;
            helper.Events.GameLoop.DayEnding += HelperEvents.GameLoop_DayEnding;
            helper.Events.GameLoop.ReturnedToTitle += HelperEvents.GameLoop_ReturnedToTitle;

            HelperEvents.Initialize(Helper);
            Misc.Initialize(Helper);
        }

        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            Dialog.Initialize(Monitor, ModManifest);
            Dialog.cp_api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

            Dialog.Load(Path.Combine(Helper.DirectoryPath, "assets"));

            foreach (var contentPack in Helper.ContentPacks.GetOwned())
                // this.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}", LogLevel.Debug);
                Dialog.Load(contentPack.DirectoryPath);

            config = Helper.ReadConfig<ModConfig>();
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api is null) return;
            api.RegisterModConfig(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
            api.SetDefaultIngameOptinValue(ModManifest, true);
            api.RegisterSimpleOption(ModManifest, "Enable Mod", "Enable Mod", () => config.EnableMod,
                val => config.EnableMod = val);
            api.RegisterSimpleOption(ModManifest, "Allow Spouses To Chat", "Allow Spouses To Chat",
                () => config.AllowSpousesToChat, val => config.AllowSpousesToChat = val);
            api.RegisterSimpleOption(ModManifest, "Chat with Player", "Chat with Player", () => config.ChatWithPlayer,
                val => config.ChatWithPlayer = val);
            api.RegisterSimpleOption(ModManifest, "Prevent Relatives Chatting", "Prevent Relatives From Chatting",
                () => config.PreventRelativesFromChatting, val => config.PreventRelativesFromChatting = val);
            api.RegisterSimpleOption(ModManifest, "Min Hearts For Chat", "Min Hearts For Chat",
                () => config.MinHeartsForChat, val => config.MinHeartsForChat = val);
            api.RegisterClampedOption(ModManifest, "Spouse Chat Chance", "Spouse Chat Chance",
                () => config.SpouseChatChance, val => config.SpouseChatChance = val, 0.0f, 1.0f, 0.05f);
            api.RegisterSimpleOption(ModManifest, "Min Distance To Chat", "Min Distance To Chat",
                () => config.MinDistanceToChat, val => config.MinDistanceToChat = val);
            api.RegisterSimpleOption(ModManifest, "Max Distance To Chat", "Max Distance To Chat",
                () => config.MaxDistanceToChat, val => config.MaxDistanceToChat = val);
            api.RegisterSimpleOption(ModManifest, "Min Spouse Chat Interval", "(seconds)",
                () => config.MinSpouseChatInterval, val => config.MinSpouseChatInterval = val);
            api.RegisterSimpleOption(ModManifest, "Extra Debug Output", "Extra Debug Output",
                () => config.ExtraDebugOutput, val => config.ExtraDebugOutput = val);
        }
    }
}