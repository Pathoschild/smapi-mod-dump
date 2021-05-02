/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/valruno/ChooseRandomFarmEvent
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace ChooseRandomFarmEvent
{

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        internal static EventData eventData;
        internal static List<EventData> giantCrops = new List<EventData>();
        internal static ModConfig Config;

        internal static ModEntry mod;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            mod = this;

            Config = helper.ReadConfig<ModConfig>();
            FarmEventPatch.Initialize(Monitor, helper);
            EventData.Initialize(Monitor, helper);

            helper.ConsoleCommands.Add("set_farmevent", "Sets tonight's farm event.\n\n" +
                "Usage: set_farmevent <event>\n- event: the type of event.\n\n" +
                "Valid events: " + String.Join(", ", EventData.EventTypes), SetFarmEvent);
            helper.ConsoleCommands.Add("spawn_giantcrop", "Sets a giant crop to spawn tonight at the provide tile coordinates. There must already be a crop at those coordinates.\n\n" +
                "Usage: spawn_giantcrop <x> <y> [crop_id] [when]\n" +
                "- x: the x-coordinate of the tile\n" +
                "- y: the y-coordinate of the tile\n" +
                "- crop_id (optional): the item ID of the normal version of the crop. This is only allowed if EnforceEventConditions is false.\n" +
                "- when (optional): Only accepted value is \"now\". This will make the giant crop try to spawn immediately instead of overnight.", SpawnGiantCrop);
            helper.ConsoleCommands.Add("clear_giantcrops", "Clears giant crops added by Choose Random Farm Event.\n\n" +
                "Usage: clear_giantcrops <from_when>\n" +
                "- from_when: \"all\" clears all CRFE-added giant crops, \"yesterday\" clears just those added the previous night," +
                "\"today\" clears ones added today, and \"tonight\" clears the list of giant crops set to be spawned by CRFE tonight", ClearGiantCrops);

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.Saving += OnSaving;

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Utility), nameof(StardewValley.Utility.pickFarmEvent)),
               postfix: new HarmonyMethod(typeof(FarmEventPatch), nameof(FarmEventPatch.PickFarmEvent_PostFix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Utility), nameof(StardewValley.Utility.pickPersonalFarmEvent)),
               postfix: new HarmonyMethod(typeof(FarmEventPatch), nameof(FarmEventPatch.PickPersonalFarmEvent_PostFix))
            );
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            eventData = null;
            giantCrops.Clear();
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            foreach (EventData giantCrop in giantCrops)
            {
                /* remove the watered condition when the giant crop actually spawns, 
                * because unlike the base game's random giant crop generation,
                * this doesn't actually run until after hoe dirt has become unwatered */
                giantCrop.Conditions.Remove(giantCrop.Conditions.Find(c => c.Value.Contains("is not watered")));

                AddGiantCropToLocation(giantCrop);
            }
        }

        private void SetFarmEvent(string command, string[] args)
        {
            if (args.Length != 1)
            {
                Monitor.Log("Command requires exactly one argument. For more info, type: help set_farmevent", LogLevel.Info);
                return;
            }
            string eventType = args[0];
            if (!EventData.EventTypes.Contains(eventType))
            {
                Monitor.Log($"{eventType} is not a valid event type.\n Valid event types: {String.Join(", ", EventData.EventTypes)}", LogLevel.Info);
                return;
            }
            eventData = new EventData(eventType);
            eventData.SetUp();
            
            if (Config.EnforceEventConditions && !eventData.EnforceEventConditions(out string reason))
                Monitor.Log($"Under current game conditions, the event \"{eventData.Name}\" will not be able to run tonight because {reason}.\n" +
                    $"The event will still try to run tonight and check if this condition has changed by the time the players go to bed.", LogLevel.Info);
            else
                Monitor.Log(eventData.SuccessMessage, LogLevel.Info);
        }

        private void SpawnGiantCrop(string command, string[] args)
        {
            int id = -1;
            bool now = false;
            if (args.Length != 2 && args.Length != 3 && args.Length != 4)
            {
                Monitor.Log("Command requires two to four arguments. For more info, type: help spawn_giantcrop", LogLevel.Info);
                return;
            }
            if (args.Length == 3)
            {
                if (int.TryParse(args[2], out int ID))
                    id = ID;
                else if (args[2] == "now")
                    now = true;
                else
                {
                    Monitor.Log("The only accepted 3rd argument is \"now\" or an item ID number. For more info, type: help spawn_giantcrop", LogLevel.Info);
                    return;
                }
            }
            if (args.Length == 4)
            {
                if (int.TryParse(args[2], out int ID))
                    id = ID;
                else
                {
                    Monitor.Log("The only accepted 3rd argument is an item ID number. For more info, type: help spawn_giantcrop", LogLevel.Info);
                    return;
                }
                if (args[3] == "now")
                    now = true;
                else
                {
                    Monitor.Log("The only accepted 4th argument is \"now\". For more info, type: help spawn_giantcrop", LogLevel.Info);
                    return;
                }
            }
            if (!int.TryParse(args[0], out int x) || x <= 0 || x >= Game1.currentLocation.Map.DisplayWidth / 64
                || !int.TryParse(args[1], out int y) || y <= 0 || y >= Game1.currentLocation.Map.DisplayHeight / 64)
            {
                Monitor.Log($"({args[0]}, {args[1]}) are not valid tile coordinates for this location.", LogLevel.Info);
                return;
            }
            if (Config.EnforceEventConditions && id > -1)
            {
                Monitor.Log("You can only choose which type of giant crop to spawn if EnforceEventConditions is false.", LogLevel.Info);
                return;
            }

            EventData giantCrop = new EventData("giant_crop", x, y, id);
            giantCrop.SetUp();
            if (giantCrop.giantCrop == null)
                return;
            
            if (Config.EnforceEventConditions && !giantCrop.EnforceEventConditions(out string reason))
                Monitor.Log($"A giant crop will not be able to spawn at ({x}, {y}) because {reason}.", LogLevel.Info);
            else
            {
                if (now)
                {
                    AddGiantCropToLocation(giantCrop);
                    return;
                }
                Monitor.Log(giantCrop.SuccessMessage, LogLevel.Info);
                giantCrops.Add(giantCrop);
            }
        }

        private void ClearGiantCrops(string command, string[] args)
        {
            if (args.Length != 1)
            {
                Monitor.Log("Command requires exactly one argument. For more info, type: help clear_giantcrops", LogLevel.Info);
                return;
            }
            if (args[0] != "all" && args[0] != "yesterday" && args[0] != "tonight" && args[0] != "today")
            {
                Monitor.Log("Command only accepts \"all\", \"yesterday\", \"today\", and \"tonight\" as arguments. For more info, type: help clear_giantcrops", LogLevel.Info);
                return;
            }
            if (args[0] == "tonight")
            {
                giantCrops.Clear();
                Monitor.Log("The list of giant crops set to spawn tonight has been cleared.", LogLevel.Info);
                return;
            }
            foreach (GameLocation location in Game1.locations)
            {
                ResourceClump[] clumps = new ResourceClump[location.resourceClumps.Count];
                location.resourceClumps.CopyTo(clumps, 0);
                foreach (ResourceClump clump in clumps)
                {
                    if (clump is GiantCrop && clump.modData.ContainsKey(ModManifest.UniqueID)
                        && ((clump.modData[ModManifest.UniqueID] == SDate.Now().AddDays(-1).ToString() && args[0] == "yesterday")
                        || (clump.modData[ModManifest.UniqueID] == SDate.Now().ToString() && args[0] == "today")
                        || args[0] == "all"))
                        location.resourceClumps.Remove(clump);
                }
            }
            string when = "";
            if (args[0] == "yesterday") when = "yesterday ";
            if (args[0] == "today") when = "today ";
            Monitor.Log($"The giant crops spawned by this mod {when}have been removed.", LogLevel.Info);
        }

        private bool AddGiantCropToLocation(EventData giantCrop)
        {
            int x = (int)giantCrop.tile.X;
            int y = (int)giantCrop.tile.Y;

            if (Config.EnforceEventConditions && !giantCrop.EnforceEventConditions(out string reason))
            {
                Monitor.Log($"Giant crop could not spawn at ({x}, {y}) in {giantCrop.location.Name} because {reason}.", LogLevel.Debug);
                return false;
            }

            for (int x1 = x - 1; x1 <= x + 1; x1++)
            {
                for (int y1 = y - 1; y1 <= y + 1; y1++)
                {
                    Vector2 v = new Vector2(x1, y1);
                    if (giantCrop.location.terrainFeatures.ContainsKey(v))
                    {
                        if (giantCrop.location.terrainFeatures[v] is HoeDirt h && h.crop != null)
                            h.crop = null;
                        else
                            giantCrop.location.terrainFeatures.Remove(v);
                    }
                }
            }
            giantCrop.location.resourceClumps.Add(giantCrop.giantCrop);
            Monitor.Log($"A giant crop has spawned at ({x}, {y}) in {giantCrop.location.Name}.", LogLevel.Info);
            return true;
        }
    }

    public class ModConfig
    {
        public bool EnforceEventConditions { get; set; } = true;
    }
}