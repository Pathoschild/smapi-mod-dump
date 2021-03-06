/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using OneSprinklerOneScarecrow.Framework;
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
        private Config _config;
        private AddCrowsPatch addcrows;
        public override void Entry(IModHelper helper)
        {
            
            //helper.Events.Player.InventoryChanged += InventoryChanged;
            _config = helper.ReadConfig<Config>();
            addcrows = new AddCrowsPatch(Monitor);

            /*
             *
             * haxorSprinklerName = translate.Get("haxorsprinkler.name");
            haxorSprinklerDescription = _translate.Get("haxorsprinkler.description");
            haxorScarecrowName = translate.Get("haxorscarecrow.name");
            haxorScarecrowDescription = translate.Get("haxorscarecrow.description");
             */
            //Lets get the translations. See if this fixes the bloody issues
            HaxorSprinkler.TranslatedName = helper.Translation.Get("haxorsprinkler.name");
            HaxorSprinkler.TranslatedDescription = helper.Translation.Get("haxorsprinkler.description");
            HaxorScarecrow.TranslatedName = helper.Translation.Get("haxorscarecrow.name");
            HaxorScarecrow.TranslatedDescription = helper.Translation.Get("haxorscarecrow.description");
            //Lets activate the asset editor
            helper.Content.AssetEditors.Add(new AssetEditor(helper, Monitor, helper.Translation, _config));


            //Events that happen in the game
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;



            //Apply Harmony Patches
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            Monitor.Log("Patching Farm.addCrows with AddCrowsPatch");
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.addCrows), new Type[] {  }),
                prefix: new HarmonyMethod(typeof(AddCrowsPatch), nameof(AddCrowsPatch.Prefix))
            );

            Monitor.Log("Patching Object.IsSprinkler with IsSprinklerPatch");
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.IsSprinkler), new Type[] {  }),
                prefix: new HarmonyMethod(typeof(IsSprinklerPatch), nameof(IsSprinklerPatch.Prefix))
            );

            Monitor.Log("Patching Object.GetBaseRadiusForSprinkler with GetBaseRadiusForSprinklerPatch");
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.GetBaseRadiusForSprinkler), new Type[] { }),
                prefix: new HarmonyMethod(typeof(GetBaseRadiusForSprinklerPatch), nameof(GetBaseRadiusForSprinklerPatch.Prefix))
            );

        }

        /// <summary>
        /// Event that gets ran when the game is launched
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event Args</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            IJsonAssetsApi jsonApi = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (jsonApi != null)
            {
                IDictionary<string, int> objectIds = jsonApi.GetAllObjectIds();
                IDictionary<string, int> bigCraftIds = jsonApi.GetAllBigCraftableIds();
                Monitor.Log($"Checked GetAlObjectIds Found: {objectIds.Count}, GetAllBigCraftables Found: {bigCraftIds.Count}");
                foreach(var v in bigCraftIds)
                    Monitor.Log($"Id: {v.Value}");
                /*
                Monitor.Log($"Last ObJect Id: {objectIds.Last().Value}. Last BigCraftable id: {bigCraftIds.Last().Value}.");
                HaxorSprinkler.ParentSheetIndex = objectIds.Last().Value + 1;
                HaxorScarecrow.ParentSheetIndex = bigCraftIds.Last().Value + 1;*/
            }
            else
                Monitor.Log("JsonAssetsApi will null.");
                
        }

        /// <summary>
        /// Event gets ran when a save game is loaded.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The Event Arguments</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //Lets run the recipe fixer
            AddRecipes();

            //Run FixLegacyItem.
            FixLegacyItems();
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

        /// <summary>
        /// Method that will search the farm and make sure it gets rid of old haxor items.
        /// </summary>
        private void FixLegacyItems()
        {
            Dictionary<Vector2, SObject> obj = new Dictionary<Vector2, SObject>();
            foreach (var o in Game1.getFarm().objects.Pairs)
            {
                if (o.Value.Name.Contains("Haxor Sprinkler") && o.Value.ParentSheetIndex != HaxorSprinkler.ParentSheetIndex)
                    obj.Add(o.Key, o.Value);
                else if (o.Value.ParentSheetIndex == 273)
                    o.Value.ParentSheetIndex = HaxorScarecrow.ParentSheetIndex;
                else if (o.Value.Name.Contains("Haxarecrow") && o.Value.ParentSheetIndex != HaxorScarecrow.ParentSheetIndex)
                    obj.Add(o.Key, o.Value);
            }
            foreach (var i in obj)
            {
                Game1.getFarm().objects.Remove(i.Key);
                Monitor.Log($"Removed Legacy Item: Name: {i.Value.Name}");
            }

        }

        /// <summary>
        /// Method that adds/removes the recipes for the haxor items.
        /// </summary>
        private void AddRecipes()
        {
            Dictionary<string, int> curRecipes = new Dictionary<string, int>();

            foreach (var r in Game1.player.craftingRecipes.Pairs)
            {
                curRecipes.Add(r.Key, r.Value);
            }

            foreach (var c in curRecipes)
            {
                if (c.Key.Contains("Haxor"))
                {
                    Game1.player.craftingRecipes.Remove(c.Key);
                    Monitor.Log($"Removed: {c.Key} recipe");
                }
            }
            //Now that they have been removed, lets add the new ones.
            Game1.player.craftingRecipes.Add("Haxor Sprinkler", 0);
            Game1.player.craftingRecipes.Add("Haxor Scarecrow", 0);
            Monitor.Log("Added the Haxor item recipes.");
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
