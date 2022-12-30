/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.IO;
using System.Linq;
using DynamicGameAssets.PackData;
using HarmonyLib;
using OrnithologistsGuild.Content;
using OrnithologistsGuild.Game.Items;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace OrnithologistsGuild
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
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
        }

        // private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        // {
        //     if (e.IsOneSecond)
        //     {
        //         this.Monitor.Log(string.Join(",", Game1.player.mailReceived.Select(m => m.ToString())));
        //     }
        // }

        //private void Player_Warped(object sender, WarpedEventArgs e)
        //{
        //    var kyle = e.NewLocation.characters.FirstOrDefault(c => c.Name == "OrinothlogistsGuild_Kyle");

        //    if (kyle != null)
        //    {
        //        // Increase width of character sprite
        //        kyle.Sprite = new AnimatedSprite(kyle.Sprite.loadedTexture, 0, 32, 32);
        //    }
        //}

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            SaveDataManager.Load();
            Mail.Initialize();
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
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

            LocationPatches.Initialize(this.Monitor);
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.addBirdies)),
               prefix: new HarmonyMethod(typeof(LocationPatches), nameof(LocationPatches.addBirdies_Prefix))
            );

            TreePatches.Initialize(this.Monitor);
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Tree), nameof(StardewValley.TerrainFeatures.Tree.performUseAction)),
               prefix: new HarmonyMethod(typeof(TreePatches), nameof(TreePatches.performUseAction_Prefix))
            );

            // Console commands
            Helper.ConsoleCommands.Add("og_debug", "Adds debug items to inventory", OnDebugCommand);
            Helper.ConsoleCommands.Add("og_spawn", "Consistently spawns specified creature ID", OnDebugCommand);
        }

        private void OnDebugCommand(string cmd, string[] args)
        {
            if (cmd.Equals("og_debug"))
            {
                //Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(270, 32)); // Corn
                Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(770, 32)); // Mixed Seeds
                //Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(431, 32)); // Sunflower Seeds
                Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(832, 32)); // Pineapple

                Game1.player.addItemByMenuIfNecessary((Item)DGAContentPack.Find("WoodenHopper").ToItem());
                //Game1.player.addItemByMenuIfNecessary((Item)DGAContentPack.Find("WoodenPlatform").ToItem());
                //Game1.player.addItemByMenuIfNecessary((Item)DGAContentPack.Find("PlasticTube").ToItem());
                //Game1.player.addItemByMenuIfNecessary((Item)DGAContentPack.Find("SeedHuller").ToItem());
                //Game1.player.addItemByMenuIfNecessary((Item)new LifeList());
                //Game1.player.addItemByMenuIfNecessary((Item)new JojaBinoculars());
                //Game1.player.addItemByMenuIfNecessary((Item)new AntiqueBinoculars());
                //Game1.player.addItemByMenuIfNecessary((Item)new ProBinoculars());
            } else if (cmd.Equals("og_spawn"))
            {
                BirdieDef birdieDef = ContentPackManager.BirdieDefs.Values.FirstOrDefault(birdieDef => birdieDef.ID.Equals(args[0], StringComparison.OrdinalIgnoreCase));

                BetterBirdieSpawner.debugAlwaysSpawn = birdieDef;
            }
        }
    }
}