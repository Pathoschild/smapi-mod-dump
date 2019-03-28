using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using OneSprinklerOneScarecrow.Framework.Overrides;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace OneSprinklerOneScarecrow
{
    public class ModEntry : Mod
    {
        private AddCrowsPatch addcrows;
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += OnDayStarted;

            // Harmony Original Code credit goes to Cat from the SDV Modding Discord, I modified his Harmony code.
            try
            {
                HarmonyInstance Harmony = HarmonyInstance.Create("mizzion.onesprinkleronescarecrow");

                //Now we set up the patches, will use a dictionary, just in case I need to expand later. Idea of using Harmony this way came from Cat#2506's mod  from the SDV discord
                IDictionary<string, Type> replacements = new Dictionary<string, Type>
                {
                    [nameof(Farm.addCrows)] = typeof(AddCrowsPatch)
                };

                IList<Type> typesToPatch = new List<Type>();
                typesToPatch.Add(typeof(Farm));
                

                //Go through and set up the patching
                foreach (Type t in typesToPatch)
                    foreach (KeyValuePair<string, Type> replacement in replacements)
                    {
                        MethodInfo original = t.GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(m => m.Name == replacement.Key);

                        MethodInfo prefix = replacement.Value
                            .GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(item => item.Name == "Prefix");
                        MethodInfo postfix = replacement.Value
                            .GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(item => item.Name == "Postfix");

                        //this.Monitor.Log($"Patching {original} with {prefix} {postfix}", LogLevel.Trace);
                        this.Monitor.Log($"Patching {original} with {prefix} {postfix}", LogLevel.Trace);

                        Harmony.Patch(original, prefix == null ? null : new HarmonyMethod(prefix),
                            postfix == null ? null : new HarmonyMethod(postfix));
                    }

            }
            catch (Exception ex)
            {
                Monitor.Log($"There was an error setting up harmony.\n {ex}", LogLevel.Trace);
            }
            addcrows = new AddCrowsPatch(Monitor);
        }

        /// <summary>
        /// Event that runs when a new day is started
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">The EventArgs</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            //Make sure the game is loaded
            if (!Context.IsWorldReady)
                return;

            //Go through each location and modify the sprinklers.
            foreach (var loc in GetLocations())
            {
                foreach (SObject obj in loc.Objects.Values)
                {
                    if (obj.Name == "Haxor Sprinkler")
                    {
                        //go through and and do the watering
                        foreach (var waterSpots in loc.terrainFeatures.Pairs)
                        {
                            if (waterSpots.Value is HoeDirt dirt)
                                dirt.state.Value = HoeDirt.watered;
                        }
                    }
                }
            }
        }


        /// <summary>Get all in-game locations.</summary>
        private IEnumerable<GameLocation> GetLocations()
        {
            foreach (GameLocation location in Game1.locations)
            {
                yield return location;
                if (location is BuildableGameLocation buildableLocation)
                {
                    foreach (Building building in buildableLocation.buildings)
                    {
                        if (building.indoors.Value != null)
                            yield return building.indoors.Value;
                    }
                }
            }
        }
    }
}
