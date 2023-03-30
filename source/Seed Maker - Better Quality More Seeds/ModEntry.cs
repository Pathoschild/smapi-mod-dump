/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/vabrell/sdw-seed-maker-mod
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using GenericModConfigMenu;

namespace SM_bqms
{
    public class ModEntry : Mod 
    {
        /*
        * Constants
        */
        private readonly List<SeedIds> SeedsToSkip = new List<SeedIds>
        {
            SeedIds.MixedSeed,
            SeedIds.AncientSeed
        };
        /*
        * State
        */
        private bool IsModInitialized;
        private Farmer Player;
        public static List<SeedMaker> SeedMakers;
        private StardewValley.Object LastHeldCropOrFruit;
        public static ModConfig Config;
        /*
        * Mod Entry
        */
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.PreInitMod;
            helper.Events.GameLoop.ReturnedToTitle += this.ResetMod;
            helper.Events.GameLoop.SaveLoaded += this.InitMod;

            helper.Events.World.ObjectListChanged += this.UpdateSeedMakers;
            helper.Events.GameLoop.UpdateTicking += this.WatchSeedMakers;

            if (helper.ModRegistry.IsLoaded("Pathoschild.Automate")) {
                this.Monitor.Log("This mod patches Automate. If you notice issues with Automate, make sure it happens without this mod before reporting it to the Automate page.\n", LogLevel.Debug);
                AutomateSeedMakerMachinePatcher.Initialize(helper, this.Monitor, this.ModManifest.UniqueID);
            }
        }
        /*
        * Mod state handling
        */
        private void ResetMod(object sender, ReturnedToTitleEventArgs e)
        {
            SeedMakers = null;
            this.LastHeldCropOrFruit = null;
            this.Player = null;
            this.IsModInitialized = false;
        }
        private void PreInitMod(object sender, GameLaunchedEventArgs e)
        {
            Config = this.Helper.ReadConfig<ModConfig>();
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is not null)
            {
                configMenu.Register(
                    mod: this.ModManifest,
                    reset: () => Config = new ModConfig(),
                    save: () => Helper.WriteConfig(Config)
                );

                configMenu.AddSectionTitle(
                    mod: this.ModManifest,
                    text: () => "Modifiers (1-3 + modifier)"
                );
                configMenu.AddNumberOption(
                    mod: this.ModManifest,
                    name: () => "Normal Modifier",
                    tooltip: () => "The modifier that will be used for normal crops",
                    getValue: () => Config.NormalModifier,
                    setValue: (value) => Config.NormalModifier = value
                );
                configMenu.AddNumberOption(
                    mod: this.ModManifest,
                    name: () => "Silver Modifier",
                    tooltip: () => "The modifier that will be used for silver crops",
                    getValue: () => Config.SilverModifier,
                    setValue: (value) => Config.SilverModifier = value
                );
                configMenu.AddNumberOption(
                    mod: this.ModManifest,
                    name: () => "Gold Modifier",
                    tooltip: () => "The modifier that will be used for gold crops",
                    getValue: () => Config.GoldModifier,
                    setValue: (value) => Config.GoldModifier = value
                );
                configMenu.AddNumberOption(
                    mod: this.ModManifest,
                    name: () => "Iridium Modifier",
                    tooltip: () => "The modifier that will be used for iridium crops",
                    getValue: () => Config.IridiumModifier,
                    setValue: (value) => Config.IridiumModifier = value
                );

                configMenu.AddSectionTitle(
                    mod: this.ModManifest,
                    text: () => "Debug"
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Enable debug mode",
                    tooltip: () => "When enabled the mod will print out debug values to the console",
                    getValue: () => Config.EnableDebug,
                    setValue: (value) => Config.EnableDebug = value
                );
            }
        }
        private void InitMod(object sender, SaveLoadedEventArgs e)
        {
            List<SeedMaker> seedMakers = new List<SeedMaker>();
            foreach(GameLocation loc in Game1.locations)
            {
                foreach (SerializableDictionary<Vector2, StardewValley.Object> gameObjects in loc.Objects) {
                    foreach (var o in gameObjects)
                    {
                        if (o.Value.Name == "Seed Maker")
                        {
                            seedMakers.Add(new SeedMaker {
                                GameObject = o.Value,
                                isHandled = false,
                            });
                        }
                    }
                }
            }
            SeedMakers = seedMakers;
            this.Player = Game1.player;
            this.IsModInitialized = true;
            if (Config.EnableDebug) {
                this.Monitor.LogOnce($"Mod initialized - {SeedMakers.Count} Seed Makers found\n", LogLevel.Debug);
            }
        }
        /*
        * Seed Maker handlers
        */
        private void UpdateSeedMakers(object sender, ObjectListChangedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            foreach (KeyValuePair<Vector2, StardewValley.Object> gameObject in e.Added)
            {
                if (gameObject.Value.Name == "Seed Maker") {
                    SeedMaker seedMaker = new SeedMaker()
                    {
                        GameObject = gameObject.Value,
                        isHandled = false,
                    };
                    SeedMakers.Add(seedMaker);
                    if (Config.EnableDebug) {
                        this.Monitor.Log($"SeedMaker at {seedMaker.GameObject.TileLocation.ToString()} added\n", LogLevel.Debug);
                    }
                }
            }
            foreach (KeyValuePair<Vector2, StardewValley.Object> gameObject in e.Removed)
            {
                if (gameObject.Value.Name == "Seed Maker") {
                    SeedMaker seedMaker = new SeedMaker()
                    {
                        GameObject = gameObject.Value,
                    };
                    SeedMakers.Remove(seedMaker);
                    if (Config.EnableDebug) {
                        this.Monitor.Log($"SeedMaker at {seedMaker.GameObject.TileLocation.ToString()} removed\n", LogLevel.Debug);
                    }
                }
            }
        }
        private void WatchSeedMakers(object sender, UpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (!this.IsModInitialized) return;
            foreach (SeedMaker seedMaker in SeedMakers)
            {
                if (!seedMaker.isHandled
                    && seedMaker.GameObject.heldObject.Value != null
                    && this.LastHeldCropOrFruit != null)
                {
                    int gameObjectId = seedMaker.GameObject.heldObject.Value.ParentSheetIndex;
                    if ((FruitIds)this.LastHeldCropOrFruit.ParentSheetIndex != FruitIds.AncientFruit) {
                        if (this.SeedsToSkip.Contains((SeedIds)gameObjectId)) return;
                    }
                    seedMaker.GameObject.heldObject.Value = new StardewValley.Object(
                        gameObjectId,
                        SM_Helper.generateSeedAmountBasedOnQuality(
                            seedMaker.GameObject.TileLocation,
                            this.LastHeldCropOrFruit.Quality,
                            Config.EnableDebug,
                            this.Monitor
                        )
                    );
                    seedMaker.isHandled = true;
                    this.LastHeldCropOrFruit = null;
                }
                if (seedMaker.isHandled
                    && seedMaker.GameObject.heldObject.Value == null)
                {
                    seedMaker.isHandled = false;
                }
            }
            StardewValley.Object activeObject = this.Player.ActiveObject;
            if (activeObject == null)
            {
                return;
            }
            SM_Helper.UpdateSeedLookupCache();
            if (SM_Helper.SeedLookupCache.ContainsKey(activeObject.ParentSheetIndex))
            {
                this.LastHeldCropOrFruit = activeObject;
                return;
            }
            this.LastHeldCropOrFruit = null;
        }
    }
}
