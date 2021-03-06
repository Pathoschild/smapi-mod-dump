/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using DeluxeGrabberRedux.Grabbers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeluxeGrabberRedux
{
    public class ModEntry : Mod
    {
        internal ModConfig Config { get; set; }
        internal readonly ModApi Api;

        public ModEntry()
        {
            Api = new ModApi(this);
        }

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += OnLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.TimeChanged += OnTenMinuteUpdate;
            helper.Events.Display.RenderedWorld += OnRenderedWorld; 
            helper.Events.World.ObjectListChanged += OnObjectListChanged;
        }

        public void LogDebug(string message)
        {
            Monitor.Log(message, LogLevel.Trace);
        }

        public override object GetApi()
        {
            return Api;
        }

        private void OnLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            var configApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configApi != null)
            {
                configApi.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
                configApi.RegisterLabel(ModManifest, "Crop Harvesting", "These options are only considered if 'Harvest Crops' is enabled");
                configApi.RegisterSimpleOption(ModManifest, "Harvest Crops", "", () => Config.harvestCrops, v => Config.harvestCrops = v);
                configApi.RegisterSimpleOption(ModManifest, "Harvest Crops Inside Pots", "This is ignored if 'Harvest Crops' is disabled", () => Config.harvestCropsIndoorPots, v => Config.harvestCropsIndoorPots = v);
                configApi.RegisterSimpleOption(ModManifest, "Harvest Flowers", "This is ignored if 'Harvest crops' is disabled", () => Config.flowers, v => Config.flowers = v);
                configApi.RegisterSimpleOption(ModManifest, "Harvest Range", "This is ignored if 'Harvest Crops' is disabled. Set to -1 to use infinite range. This ONLY affects crop harvesting.", () => Config.harvestCropsRange, v => Config.harvestCropsRange = Math.Max(-1, v));
                configApi.RegisterChoiceOption(ModManifest, "Harvest Range Mode", "'Walk': the distance is the walking distance (in four directions) from the grabber, becoming a diamond shape. 'Square': the distance is a square like a sprinkler. Covers more area than 'Walk'.", () => Config.harvestCropsRangeMode, v => Config.harvestCropsRangeMode = v, ModConfig.HarvestCropsRangeMode);
                configApi.RegisterLabel(ModManifest, "Other Harvesting", "");
                configApi.RegisterSimpleOption(ModManifest, "Harvest Fruit Trees", "", () => Config.fruitTrees, v => Config.fruitTrees = v);
                configApi.RegisterSimpleOption(ModManifest, "Harvest Berry Bushes", "", () => Config.bushes, v => Config.bushes = v);
                configApi.RegisterSimpleOption(ModManifest, "Shake Seed Trees", "", () => Config.seedTrees, v => Config.seedTrees = v);
                configApi.RegisterSimpleOption(ModManifest, "Grab Slime Balls", "", () => Config.slimeHutch, v => Config.slimeHutch = v);
                configApi.RegisterSimpleOption(ModManifest, "Grab Farm Cave Mushrooms", "This will also work for mushroom boxes placed outside the farm cave", () => Config.farmCaveMushrooms, v => Config.farmCaveMushrooms = v);
                configApi.RegisterSimpleOption(ModManifest, "Dig Up Artifact Spots", "Note that lost books and secret notes will not be dug up", () => Config.artifactSpots, v => Config.artifactSpots = v);
                configApi.RegisterSimpleOption(ModManifest, "Collect Ore From Panning Sites", "", () => Config.orePan, v => Config.orePan = v);
                configApi.RegisterSimpleOption(ModManifest, "Fell Stumps in Secret Woods", "", () => Config.fellSecretWoodsStumps, v => Config.fellSecretWoodsStumps = v);
                configApi.RegisterSimpleOption(ModManifest, "Search Garbage Cans in Town", "", () => Config.garbageCans, v => Config.garbageCans = v);
                configApi.RegisterLabel(ModManifest, "Miscellaneous", "");
                configApi.RegisterSimpleOption(ModManifest, "Report Yield", "Logs to the SMAPI console the yield of each auto grabber", () => Config.reportYield, v => Config.reportYield = v);
                configApi.RegisterSimpleOption(ModManifest, "Gain Experience", "Gain appropriate experience as if you foraged or harvested yourself", () => Config.gainExperience, v => Config.gainExperience = v);
            }
        }

        private void OnObjectListChanged(object sender, StardewModdingAPI.Events.ObjectListChangedEventArgs e)
        {
            LogDebug($"Object list changed at {e.Location.Name}");
            GrabAtLocation(e.Location);
        }

        private void OnTenMinuteUpdate(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            if (e.NewTime % 100 == 0) 
            {
                LogDebug($"Autograbbing on time change");
                foreach (var location in Game1.locations)
                {
                    var orePanGrabber = new OrePanGrabber(this, location);
                    if (orePanGrabber.CanGrab())
                    {
                        orePanGrabber.GrabItems();
                    }
                }
            }
        }

        private void OnDayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            LogDebug($"Autograbbing on day start");
            var locations = Game1.locations.Concat(Game1.getFarm().buildings.Select(building => building.indoors.Value)).Where(location => location != null);
            foreach (var location in locations) GrabAtLocation(location);
        }

        private void OnRenderedWorld(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            if (!(Context.IsPlayerFree && !Game1.eventUp && Game1.farmEvent == null && Config.harvestCropsRange > 0 && Config.harvestCrops)) return;

            if (Config.harvestCrops 
                && Config.harvestCropsRange > 0 
                && Game1.player.ActiveObject != null 
                && Game1.player.ActiveObject.bigCraftable.Value 
                && Game1.player.ActiveObject.ParentSheetIndex == ItemIds.Autograbber)
            {
                // impl @ StardewValley::Object::drawPlacementBounds
                var placementTile = Game1.GetPlacementGrabTile();
                var X = (int)placementTile.X;
                var Y = (int)placementTile.Y;
                if (Game1.IsPerformingMousePlacement())
                {
                    var range = Config.harvestCropsRange;
                    for (int x = X - range; x <= X + range; x++)
                    {
                        for (int y = Y - range; y <= Y + range; y++)
                        {
                            if (Config.harvestCropsRangeMode == "Walk" && Math.Abs(X - x) + Math.Abs(Y - y) > range) continue;
                            Game1.spriteBatch.Draw(
                                        Game1.mouseCursors,
                                        Game1.GlobalToLocal(new Vector2(x, y) * 64),
                                        new Rectangle(194, 388, 16, 16),
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        4,
                                        SpriteEffects.None,
                                        0.01f
                            );
                        }
                    }
                }
            }
        }

        private bool GrabAtLocation(GameLocation location)
        {
            var grabber = new AggregateDailyGrabber(this, location);
            var previousInventory = Config.reportYield ? grabber.GetInventory() : null;
            var grabbed = grabber.GrabItems();
            if (previousInventory != null)
            {
                var nextInventory = grabber.GetInventory();
                var sb = new StringBuilder($"Yield of autograbber(s) at {location.Name}:\n");
                var shouldPrint = false;
                foreach (var pair in nextInventory)
                {
                    var item = pair.Key;
                    var stack = pair.Value;
                    var diff = stack;
                    if (previousInventory.ContainsKey(pair.Key))
                    {
                        diff -= previousInventory[pair.Key];
                    }
                    if (diff <= 0) continue;
                    sb.AppendLine($"    {item.Name} ({item.QualityName}) x{diff}");
                    shouldPrint = true;
                }
                if (shouldPrint) Monitor.Log(sb.ToString(), LogLevel.Info);
            }
            return grabbed;
        }
    }
}
