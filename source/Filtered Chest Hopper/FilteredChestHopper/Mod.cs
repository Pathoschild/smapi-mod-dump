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
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

//To-Do
//Figure out how to read automate config?
//Copy automates connecting with paths?

namespace FilteredChestHopper
{

    internal class Mod : StardewModdingAPI.Mod
    {
        public int AutomationInterval { get; set; } = 60;
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
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            if (Pipelines == null)
            {
                Pipelines = new List<Pipeline>();
            }
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
                        Pipelines.RemoveAll(pipeline => AddedObject.Key == pipeline.Hoppers[0].TileLocation - new Vector2(1,0) || AddedObject.Key == pipeline.Hoppers[pipeline.Hoppers.Count - 1].TileLocation + new Vector2(1, 0));
                        Pipeline pipeline = new Pipeline(hopper);
                        Pipelines.Add(pipeline);
                    }
                }
            }
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            this.AutomateCountdown--;
            if (this.AutomateCountdown > 0)
                return;

            this.AutomateCountdown = AutomationInterval;

            if (Pipelines != null)
            { 
                foreach (var pipeline in Pipelines)
                {
                    pipeline.AttemptTransfer();
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

        //Will be used to limit regenerating unchanged pipelines
        private bool CheckIfInBounds(Vector2 point, Vector2 boundsStart, Vector2 boundsSize)
        {
            return point.X >= boundsStart.X && point.X <= boundsStart.X + boundsSize.X && point.Y >= boundsStart.Y && point.Y <= boundsStart.Y + boundsSize.Y;
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
