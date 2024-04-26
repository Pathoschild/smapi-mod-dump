/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using HarmonyLib;
using OrnithologistsGuild.Content;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace OrnithologistsGuild
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        internal static ContentPatcher.IContentPatcherAPI CP;

        public static Mod Instance;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;

            this.Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            this.Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            // this.Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;

            this.Helper.Events.Input.ButtonPressed += Input_ButtonPressed;

            SaveDataManager.Initialize();
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            DebugHandleInput(e);
        }

        // private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        // {
        //     if (e.IsOneSecond)
        //     {
        //         this.Monitor.Log(string.Join(",", Game1.player.mailReceived.Select(m => m.ToString())));
        //     }
        // }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            SaveDataManager.Load();
            Mail.Initialize();

            if (ConfigManager.Config.LogMissingBiomes) {
                foreach (var location in StardewValley.Game1.locations)
                {
                    var biomes = location.GetBiomes();
                    if (location.IsOutdoors && (
                        biomes == null ||
                        biomes.Length == 0 ||
                        (biomes.Length == 1 && biomes[0].Equals("default")
                    )))
                    {
                        Monitor.Log($"No biomes specified for outdoor location \"{location.Name}\"", LogLevel.Warn);
                    }
                }
            }
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Custom tokens
            CP = Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
            CP.RegisterToken(this.ModManifest, "LocationBiome", () =>
            {
                if (!Context.IsWorldReady) return null;

                return StardewValley.Game1.player.currentLocation.GetBiomes();
            });

            // Config
            ConfigManager.Initialize();

            // Translation
            I18n.Init(Helper.Translation);

            // Internal content
            ContentManager.Initialize();

            // Ornithologist's Guild content packs
            ContentPackManager.Initialize();
            if (ConfigManager.Config.LoadVanillaPack) ContentPackManager.LoadVanilla();
            if (ConfigManager.Config.LoadBuiltInPack) ContentPackManager.LoadBuiltIn();
            ContentPackManager.LoadExternal();

            // Harmony patches
            var harmony = new Harmony(this.ModManifest.UniqueID);
            GameLocationPatches.Initialize(this.Monitor, harmony);
            TreePatches.Initialize(this.Monitor, harmony);
            ObjectPatches.Initialize(this.Monitor, harmony);

            RegisterDebugCommands();
        }
    }
}