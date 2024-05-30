/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shivion/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

//To-Do
//Custom Item
//Custom Filter Interface

namespace FilteredChestHopper
{

    internal class Mod : StardewModdingAPI.Mod
    {
        public ModConfig Config = new ModConfig();
        public int AutomateCountdown;

        //Active Pipelines
        public static List<Pipeline> Pipelines;

        //Applying this flag gets automate to ignore the hopper, so I hijack it
        public const string ModDataFlag = "spacechase0.SuperHopper";

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.World.ObjectListChanged += this.ObjectListChanged;

            this.Config = this.Helper.ReadConfig<ModConfig>();
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            if (Pipelines == null)
            {
                Pipelines = new List<Pipeline>();
            }

            //Config
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Compare Quality",
                tooltip: () => "If true the filters will check the qualities of items as well as the item id",
                getValue: () => this.Config.CompareQuality,
                setValue: value => this.Config.CompareQuality = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Compare Quantity",
                tooltip: () => "If true the filters will check the stack size of the item, and only place that many items in the target chest. Only having 1 item in the filter stack ignores the quantity filter for that item instead of only moving one (for ease of use)",
                getValue: () => this.Config.CompareQuantity,
                setValue: value => this.Config.CompareQuantity = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Tranfer Interval",
                tooltip: () => "How often the item transfer logic runs in frames, ie. 1 is every frame, 60 every 60 frames which should be about every second",
                getValue: () => this.Config.TransferInterval,
                setValue: value => this.Config.TransferInterval = value,
                min: 1,
                max: 600
            );
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            RegeneratePipelines();
        }

        public void RegeneratePipelines()
        {
            Utility.ForEachLocation((GameLocation location) =>
            {
                foreach (var stardewObject in location.objects.Pairs)
                {
                    if (TryGetHopper(stardewObject.Value, out var hopper))
                    {
                        Pipeline pipeline = new Pipeline(hopper);
                        Pipelines.Add(pipeline);
                    }
                }
                return true;
            });
        }
            

        private void ObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (e.Removed != null)
            {
                foreach (var RemovedObject in e.Removed)
                {
                    if (TryGetHopper(RemovedObject.Value, out Chest hopper))
                    {
                        Pipelines.RemoveAll(pipeline => pipeline.Hoppers.Contains(hopper));

                        Chest chestLeft = GetChestAt(e.Location, RemovedObject.Key - new Vector2(1, 0));
                        if (chestLeft != null && TryGetHopper(chestLeft, out var hopperLeft))
                        {
                            Pipeline pipeline = new Pipeline(hopperLeft);
                            Pipelines.Add(pipeline);
                        }

                        Chest chestRight = GetChestAt(e.Location, RemovedObject.Key + new Vector2(1, 0));
                        if (chestRight != null && TryGetHopper(chestRight, out var hopperRight))
                        {
                            Pipeline pipeline = new Pipeline(hopperRight);
                            Pipelines.Add(pipeline);
                        }
                    }
                }
            }

            if (e.Added != null)
            {
                foreach (var AddedObject in e.Added)
                {
                    if (TryGetHopper(AddedObject.Value, out Chest hopper))
                    {
                        Pipelines.RemoveAll(pipeline => pipeline.Hoppers.Count < 1 || AddedObject.Key == pipeline.Hoppers[0].TileLocation - new Vector2(1,0) || AddedObject.Key == pipeline.Hoppers[pipeline.Hoppers.Count - 1].TileLocation + new Vector2(1, 0));
                        Pipeline pipeline = new Pipeline(hopper);
                        Pipelines.Add(pipeline);
                    }
                }
            }
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            this.AutomateCountdown--;
            if (this.AutomateCountdown > 0 || Config == null)
                return;

            this.AutomateCountdown = Config.TransferInterval;

            if (Pipelines != null)
            { 
                foreach (var pipeline in Pipelines)
                {
                    pipeline.AttemptTransfer(this);
                }
            }
        }

        /// <summary>Get the hopper instance if the object is a hopper.</summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="hopper">The hopper instance.</param>
        /// <returns>Returns whether the object is a hopper.</returns>
        public static bool TryGetHopper(StardewValley.Object obj, out Chest hopper)
        {
            if (obj is Chest { SpecialChestType: Chest.SpecialChestTypes.AutoLoader } chest)
            {
                hopper = chest;
                return true;
            }

            hopper = null;
            return false;
        }

        public static Chest GetChestAt(GameLocation location, Vector2 position)
        {
            if (location.objects.TryGetValue(position, out StardewValley.Object obj) && obj != null && obj is Chest chest)
            {
                return chest;
            }
            return null;
        }
    }
}
