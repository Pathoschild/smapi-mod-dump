/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MailboxMenu
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {

        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;

        public static ModEntry context;
        public static bool isMailServicesActive;
        public static bool isWizardAWitch;

        public static string npcPath = "aedenthorn.MailboxMenu/npcs";
        public static string mailPath = "aedenthorn.MailboxMenu/letters";
        public static Dictionary<string, EnvelopeData> envelopeData = new();
        public static Dictionary<string, EnvelopeData> npcEnvelopeData = new();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            if (!Config.ModEnabled)
                return;

            context = this;

            SMonitor = Monitor;
            SHelper = helper;

            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            Helper.Events.Content.AssetRequested += Content_AssetRequested;
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.mailbox)),
                prefix: new HarmonyMethod(typeof(GameLocation_mailbox_Patch), nameof(GameLocation_mailbox_Patch.Prefix))
                );
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (!Config.ModEnabled) return;
            if (e.Button == Config.MenuKey) OpenMenu();
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            npcEnvelopeData = Helper.GameContent.Load<Dictionary<string, EnvelopeData>>(npcPath);
            foreach (var key in npcEnvelopeData.Keys.ToArray())
            {
                if (!string.IsNullOrEmpty(npcEnvelopeData[key].texturePath)) 
                    npcEnvelopeData[key].texture = Helper.GameContent.Load<Texture2D>(npcEnvelopeData[key].texturePath);
            }
            
            envelopeData = Helper.GameContent.Load<Dictionary<string, EnvelopeData>>(mailPath);
            foreach (var key in envelopeData.Keys.ToArray())
            {
                if (!string.IsNullOrEmpty(envelopeData[key].texturePath)) 
                    envelopeData[key].texture = Helper.GameContent.Load<Texture2D>(envelopeData[key].texturePath);
            }

            if (isWizardAWitch) {
                foreach (var data in envelopeData.Values.Where(data => data.sender == "Wizard")) {
                    data.sender = "Witch";
                    data.texture = Helper.GameContent.Load<Texture2D>("aedenthorn.CPMBMEnvelopes/Wizard");
                }
            }
            
            if (!envelopeData.ContainsKey("default"))
            {
                envelopeData["default"] = new EnvelopeData() { 
                    texture = Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "envelope.png")), 
                    scale = 1, 
                };
            }
            Monitor.Log($"npc envelopes: {npcEnvelopeData.Count}, mail data: {envelopeData.Count}");
        }

        private void Content_AssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {

            if (e.NameWithoutLocale.IsEquivalentTo(npcPath))
            {
                e.LoadFrom(() => new Dictionary<string, EnvelopeData>(), StardewModdingAPI.Events.AssetLoadPriority.Exclusive);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo(mailPath))
            {
                e.LoadFrom(() => defaultMailSenders, StardewModdingAPI.Events.AssetLoadPriority.Exclusive);
            }
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            SetupModCompatibility();
        }

        private void SetupModCompatibility() {
            var phone = Helper.ModRegistry.GetApi<IMobilePhoneApi>("JoXW.MobilePhone");
            phone?.AddApp("aedenthorn.MailboxMenu", "Mailbox", OpenMenu, Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "icon.png")));
            
            isMailServicesActive = Helper.ModRegistry.IsLoaded("Digus.MailServicesMod");
            isWizardAWitch = Helper.ModRegistry.IsLoaded("Nom0ri.RomRas");

            var configApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configApi is not null) SetupGMCM(configApi);
        }
        
        private void SetupGMCM(IGenericModConfigMenuApi configApi) {
            // register mod
            configApi.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configApi.AddBoolOption(
                mod: ModManifest,
                name: () => "Mod Enabled",
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );
            configApi.AddBoolOption(
                mod: ModManifest,
                name: () => "Click Mailbox To Open",
                getValue: () => Config.MenuOnMailbox,
                setValue: value => Config.MenuOnMailbox = value
            );
            configApi.AddKeybind(
                mod: ModManifest,
                name: () => "Menu Key",
                getValue: () => Config.MenuKey,
                setValue: value => Config.MenuKey = value
            );
            configApi.AddKeybind(
                mod: ModManifest,
                name: () => "Mod Key",
                tooltip: () => "Hold this down while interacting with the mailbox",
                getValue: () => Config.ModKey,
                setValue: value => Config.ModKey = value
            );
            configApi.AddTextOption(
                mod: ModManifest,
                name: () => "Inbox Text",
                getValue: () => Config.InboxText,
                setValue: value => Config.InboxText = value
            );
            configApi.AddTextOption(
                mod: ModManifest,
                name: () => "Archive Text",
                getValue: () => Config.ArchiveText,
                setValue: value => Config.ArchiveText = value
            );
            configApi.AddNumberOption(
                mod: ModManifest,
                name: () => "Window Width",
                getValue: () => Config.WindowWidth,
                setValue: value => Config.WindowWidth = value
            );
            configApi.AddNumberOption(
                mod: ModManifest,
                name: () => "Window Height",
                getValue: () => Config.WindowHeight,
                setValue: value => Config.WindowHeight = value
            );
            configApi.AddNumberOption(
                mod: ModManifest,
                name: () => "Grid Columns",
                getValue: () => Config.GridColumns,
                setValue: value => Config.GridColumns = value
            );
            configApi.AddNumberOption(
                mod: ModManifest,
                name: () => "Envelope Width",
                getValue: () => Config.EnvelopeWidth,
                setValue: value => Config.EnvelopeWidth = value
            );
            configApi.AddNumberOption(
                mod: ModManifest,
                name: () => "Envelope Height",
                getValue: () => Config.EnvelopeHeight,
                setValue: value => Config.EnvelopeHeight = value
            );
            configApi.AddNumberOption(
                mod: ModManifest,
                name: () => "Side Width",
                getValue: () => Config.SideWidth,
                setValue: value => Config.SideWidth = value
            );
            configApi.AddNumberOption(
                mod: ModManifest,
                name: () => "Row Spacing",
                getValue: () => Config.RowSpace,
                setValue: value => Config.RowSpace = value
            );
            configApi.AddNumberOption(
                mod: ModManifest,
                name: () => "Grid Spacing",
                getValue: () => Config.GridSpace,
                setValue: value => Config.GridSpace = value
            );
        }

        private void OpenMenu()
        {
            if(Config.ModEnabled && Context.IsPlayerFree)
            {
                Game1.activeClickableMenu = new MailMenu();
            }
        }
    }
}
