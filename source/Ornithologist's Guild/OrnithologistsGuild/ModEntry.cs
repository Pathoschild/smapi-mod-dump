/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System.IO;
using System.Linq;
using DynamicGameAssets.PackData;
using HarmonyLib;
using OrnithologistsGuild.Content;
using OrnithologistsGuild.Game.Items;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace OrnithologistsGuild
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        internal static ContentPatcher.IContentPatcherAPI CP;

        internal static DynamicGameAssets.IDynamicGameAssetsApi DGA;
        internal static ContentPack DGAContentPack;

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

            // Verify that all outdoor maps have biomes specified
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

            // Dynamic Game Assets content pack
            DGA = Helper.ModRegistry.GetApi<DynamicGameAssets.IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");
            DGA.AddEmbeddedPack(this.ModManifest, Path.Combine(Helper.DirectoryPath, "assets", "dga"));
            DGAContentPack = DynamicGameAssets.Mod.GetPacks().First(cp => cp.GetManifest().UniqueID == ModManifest.UniqueID);

            // Save serializer
            var sc = Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
            sc.RegisterSerializerType(typeof(JojaBinoculars));
            sc.RegisterSerializerType(typeof(AntiqueBinoculars));
            sc.RegisterSerializerType(typeof(ProBinoculars));
            sc.RegisterSerializerType(typeof(LifeList));

            // Harmony patches
            var harmony = new Harmony(this.ModManifest.UniqueID);

            GameLocationPatches.Initialize(this.Monitor);
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.addBirdies)),
               prefix: new HarmonyMethod(typeof(GameLocationPatches), nameof(GameLocationPatches.addBirdies_Prefix))
            );

            TreePatches.Initialize(this.Monitor);
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Tree), nameof(StardewValley.TerrainFeatures.Tree.performUseAction)),
               prefix: new HarmonyMethod(typeof(TreePatches), nameof(TreePatches.performUseAction_Prefix))
            );

            RegisterDebugCommands();
        }
    }
}