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
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            
            //Lets activate the asset editor
            //helper.Content.AssetEditors.Add(new AssetEditor(helper, Monitor, helper.Translation, _config));
            helper.Events.Content.AssetRequested += ContentEvent_AssetRequested;

            //Events that happen in the game
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            
            //Apply Harmony Patches
            var harmony = new Harmony(this.ModManifest.UniqueID);
            Monitor.Log("Patching Farm.addCrows with AddCrowsPatch");
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.addCrows), new Type[] {}),
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

        private void ContentEvent_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            HaxorSprinkler.TranslatedName = Helper.Translation.Get("haxorsprinkler.name");
            HaxorSprinkler.TranslatedDescription = Helper.Translation.Get("haxorsprinkler.description");
            HaxorScarecrow.TranslatedName = Helper.Translation.Get("haxorscarecrow.name");
            HaxorScarecrow.TranslatedDescription = Helper.Translation.Get("haxorscarecrow.description");
            
            //Lets start editing the content files.
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/springobjects"))
            {
                e.Edit(asset =>
                {
                    Texture2D sprinkler =
                        Helper.ModContent.Load<Texture2D>("Assets/HaxorSprinkler.png");
                    Texture2D oldImage = asset.AsImage().Data;

                    asset.ReplaceWith(new Texture2D(Game1.graphics.GraphicsDevice,
                        oldImage.Width,
                        System.Math.Max(oldImage.Height, 1200 / 24 * 16)));
                    asset.AsImage().PatchImage(oldImage);
                    asset.AsImage().PatchImage(sprinkler, targetArea: this.GetRectangle(HaxorSprinkler.ParentSheetIndex));
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("TileSheets/Craftables"))
            {
                e.Edit(asset =>
                {
                    Texture2D scarecrow = Helper.ModContent.Load<Texture2D>("Assets/HaxorScareCrow.png");
                    Texture2D oldImage = asset.AsImage().Data;

                    asset.ReplaceWith(new Texture2D(Game1.graphics.GraphicsDevice,
                        oldImage.Width,
                        System.Math.Max(oldImage.Height, 1200 / 8 * 32)));
                    asset.AsImage().PatchImage(oldImage);
                    asset.AsImage().PatchImage(scarecrow, targetArea: this.GetRectangleCraftables(HaxorScarecrow.ParentSheetIndex));
                });
                
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset =>
                {
                    string ass = $"{HaxorSprinkler.Name}/{HaxorSprinkler.Price}/{HaxorSprinkler.Edibility}/{HaxorSprinkler.Type} {HaxorSprinkler.Category}/{HaxorSprinkler.TranslatedName}/{HaxorSprinkler.TranslatedDescription}";
                    asset.AsDictionary<int, string>().Data.Add(HaxorSprinkler.ParentSheetIndex, ass);
                    Monitor.Log($"Added Name: {HaxorSprinkler.Name}({HaxorSprinkler.TranslatedName}) Id: {HaxorSprinkler.ParentSheetIndex}.\r\n {ass}");
                });
                

                
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/BigCraftablesInformation"))
            {
                e.Edit(asset =>
                {
                    asset.AsDictionary<int, string>().Data.Add(HaxorScarecrow.ParentSheetIndex, $"{HaxorScarecrow.Name}/{HaxorScarecrow.Price}/{HaxorScarecrow.Edibility}/{HaxorScarecrow.Type} {HaxorScarecrow.Category}/{HaxorScarecrow.TranslatedDescription}/true/false/0/{HaxorScarecrow.TranslatedName}");
                    Monitor.Log($"Added Name: {HaxorScarecrow.Name}({HaxorScarecrow.TranslatedName}). Id: {HaxorScarecrow.ParentSheetIndex}");
                });
                
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {

                /*
                    
                    Sprinkler
                    {390, 100}, //Easy Mode = 100 Stone
                    {386, 10} // Hard Mode = 10 Iridium Ore
        
                    Scarecrow
                    {388, 100}, //Easy Mode = 100 Wood
                    {337, 10} // Hard Mode = 10 Iridium Bars
                 
                 */
                e.Edit(asset =>
                {
                    var curData = asset.AsDictionary<string, string>();
                    //bool isEn = asset.Locale == "en";
                    string isEnSprik = asset.Locale != "en" ? $"/{HaxorSprinkler.TranslatedName}" : "";
                    string isEnScare = asset.Locale != "en" ? $"/{HaxorScarecrow.TranslatedName}" : "";
                    Monitor.Log("Made it to the else");
                    string sprinklerIngredientsOut = !_config.ActivateHarderIngredients ? $"390 100/Home/{HaxorSprinkler.ParentSheetIndex}/false/null{isEnSprik}" : $"386 10/Home/{HaxorSprinkler.ParentSheetIndex}/false/null{isEnSprik}";
                    string scarecrowIngredientsOut = !_config.ActivateHarderIngredients ? $"388 100/Home/{HaxorScarecrow.ParentSheetIndex}/true/null{isEnScare}" : $"337 10/Home/{HaxorScarecrow.ParentSheetIndex}/true/null{isEnScare}";

                    if (curData.Data.ContainsKey("Haxor Sprinkler"))
                        curData.Data["Haxor Sprinkler"] = sprinklerIngredientsOut;
                    if (curData.Data.ContainsKey("Haxor Scarecrpw"))
                        curData.Data["Haxor Scarecrow"] = scarecrowIngredientsOut;
                    if (!curData.Data.ContainsKey("Haxor Sprinkler") && !curData.Data.ContainsKey("Haxor Scarecrow"))
                    {
                        //Didn't find the recipes, now we add them
                        try
                        {
                            curData.Data.Add("Haxor Sprinkler", sprinklerIngredientsOut);
                            curData.Data.Add("Haxor Scarecrow", scarecrowIngredientsOut);
                            Monitor.Log($"Added Haxor Sprinkler Recipe: {sprinklerIngredientsOut}");
                            Monitor.Log($"Added Haxor Scarecrow: {scarecrowIngredientsOut}");
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"There was an error editing crafting recipes. {ex.ToString()}");
                        }

                    }
                });
                
            }
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

        //Custom Methods
        public Rectangle GetRectangle(int id)
        {
            return new Rectangle(id % 24 * 16, id / 24 * 16, 16, 16);
        }

        public Rectangle GetRectangleCraftables(int id)
        {
            return new Rectangle(id % 8 * 16, id / 8 * 32, 16, 32);
        }
    }
}
