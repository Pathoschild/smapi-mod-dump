using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomNPCFramework.Framework.Enums;
using CustomNPCFramework.Framework.Graphics;
using CustomNPCFramework.Framework.ModularNpcs.ColorCollections;
using CustomNPCFramework.Framework.NPCS;
using CustomNPCFramework.Framework.Utilities;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace CustomNPCFramework
{
    /// <summary>
    /// BETA VERSION 0.1.0: Lots of ways this can be improved upon.
    /// TODO:
    /// 
    ///
    /// List all asset managers in use.
    /// Have all asset managers list what assets they are using.
    /// 
    /// Have asset info have a var called age.
    ///     ...where 
    ///         0=adult
    ///         1=child
    /// 
    ///    /// Have asset info have a var called bodyType.
    ///     ...where 
    ///         0=thin
    ///         1=normal
    ///         2=muscular
    ///         3=big
    /// 
    /// Load in the assets and go go go.
    ///     -Collect a bunch of assets together to test this thing.
    ///     
    /// Find way to make sideways shirts render correctly.
    /// 
    ///Get suggestions from modding community on requests and ways to improve the mod. 
    /// </summary>
    public class Class1 : Mod
    {
        /// <summary>The mod helper for the mod.</summary>
        public static IModHelper ModHelper;

        /// <summary>The mod monitor for the mod.</summary>
        public static IMonitor ModMonitor;

        /// <summary>The npc tracker for the mod. Keeps track of all npcs added by the custom framework and cleans them up during saving.</summary>
        public static NpcTracker npcTracker;

        /// <summary>Keeps track of all of the asets/textures added in by the framework. Also manages all of the asset managers that are the ones actually managing the textures.</summary>
        public static AssetPool assetPool;

        public static IManifest Manifest;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = this.Helper;
            ModMonitor = this.Monitor;
            Manifest = this.ModManifest;

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            npcTracker = new NpcTracker();
            assetPool = new AssetPool();
            var assetManager = new AssetManager();
            assetPool.addAssetManager(new KeyValuePair<string, AssetManager>("testNPC", assetManager));
            this.initializeExamples();
            this.initializeAssetPool();
            assetPool.loadAllAssets();
        }

        /// <summary>Initialize the asset pool with some test variables.</summary>
        public void initializeAssetPool()
        {
            string relativePath = Path.Combine("Content", "Graphics", "NPCS");
            assetPool.getAssetManager("testNPC").addPathCreateDirectory(new KeyValuePair<string, string>("characters", relativePath));
        }

        /// <summary>Raised after the game finishes writing data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaved(object sender, SavedEventArgs e)
        {
            npcTracker.afterSave();
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            // clean up all the npcs from the game world to prevent it from crashing
            npcTracker.cleanUpBeforeSave();
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // TODO For testing purposes only. Will remove in future release.

            /*
            if (Game1.player.currentLocation == null) return;
            if (Game1.activeClickableMenu != null) return;
            foreach (var v in Game1.player.currentLocation.characters)
            {
                v.speed = 1;
                if(v is ExtendedNPC)
                {
                    (v as ExtendedNPC).SetMovingAndMove(Game1.currentGameTime, Game1.viewport, Game1.player.currentLocation, Direction.right, true);
                }
                //v.MovePosition(Game1.currentGameTime, Game1.viewport, Game1.player.currentLocation);
                //ModMonitor.Log(v.sprite.spriteHeight.ToString());
            }
            */
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // TODO Used to spawn a custom npc just as an example. Don't keep this code. GENERATE NPC AND CALL THE CODE

            ExtendedNpc myNpc3 = assetPool.generateNPC(Genders.female, 0, 1, new StandardColorCollection(null, null, Color.Blue, null, Color.Yellow, null));
            MerchantNpc merch = new MerchantNpc(new List<Item>()
            {
                new StardewValley.Object(475,999)
            }, myNpc3);
            npcTracker.addNewNpcToLocation(Game1.getLocationFromName("BusStop", false), merch, new Vector2(2, 23));
        }

        /// <summary>Used to initialize examples for other modders to look at as reference.</summary>
        public void initializeExamples()
        {
            return;
            string relativeDirPath = Path.Combine("Content", "Templates");
            var aManager = assetPool.getAssetManager("testNPC");
            aManager.addPathCreateDirectory(new KeyValuePair<string, string>("templates", relativeDirPath));

            // write example
            {
                string relativeFilePath = Path.Combine(relativeDirPath, "Example.json");
                if (!File.Exists(Path.Combine(this.Helper.DirectoryPath, relativeFilePath)))
                {
                    ModMonitor.Log("THIS IS THE PATH::: " + relativeFilePath);
                    AssetInfo info = new AssetInfo("MyExample", new NamePairings("StandingExampleL", "StandingExampleR", "StandingExampleU", "StandingExampleD"), new NamePairings("MovingExampleL", "MovingExampleR", "MovingExampleU", "MovingExampleD"), new NamePairings("SwimmingExampleL", "SwimmingExampleR", "SwimmingExampleU", "SwimmingExampleD"), new NamePairings("SittingExampleL", "SittingExampleR", "SittingExampleU", "SittingExampleD"), new Vector2(16, 16), false);
                    info.writeToJson(relativeFilePath);

                }
            }

            // write advanced example
            {
                string relativeFilePath = Path.Combine(relativeDirPath, "AdvancedExample.json");
                if (!File.Exists(Path.Combine(this.Helper.DirectoryPath, relativeFilePath)))
                {
                    ExtendedAssetInfo info2 = new ExtendedAssetInfo("AdvancedExample", new NamePairings("AdvancedStandingExampleL", "AdvancedStandingExampleR", "AdvancedStandingExampleU", "AdvancedStandingExampleD"), new NamePairings("AdvancedMovingExampleL", "AdvancedMovingExampleR", "AdvancedMovingExampleU", "AdvancedMovingExampleD"), new NamePairings("AdvancedSwimmingExampleL", "AdvancedSwimmingExampleR", "AdvancedSwimmingExampleU", "AdvancedSwimmingExampleD"), new NamePairings("AdvancedSittingExampleL", "AdvancedSittingExampleR", "AdvancedSittingExampleU", "AdvancedSittingExampleD"), new Vector2(16, 16), false, Genders.female, new List<Seasons>() { Seasons.spring, Seasons.summer }, PartType.hair);
                    info2.writeToJson(relativeFilePath);
                }
            }
        }

        /// <summary>Used to finish cleaning up absolute asset paths into a shortened relative path.</summary>
        public static string getRelativeDirectory(string path)
        {
            return path
                .Split(new[] { ModHelper.DirectoryPath }, 2, StringSplitOptions.None)
                .Last()
                .TrimStart(Path.DirectorySeparatorChar);
        }
    }
}
