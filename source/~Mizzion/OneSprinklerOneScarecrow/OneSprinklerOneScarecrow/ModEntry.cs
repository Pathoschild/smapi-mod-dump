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
using System.Security;
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
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.GameData;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Objects;
using SObject = StardewValley.Object;

namespace OneSprinklerOneScarecrow
{
    public class ModEntry : Mod
    {
        private Config _config;
        
        private bool isDebugging = false;
        
        public override void Entry(IModHelper helper)
        {
            
            //helper.Events.Player.InventoryChanged += InventoryChanged;
            _config = helper.ReadConfig<Config>();
            


            //Lets activate the asset editor
            helper.Events.Content.AssetRequested += ContentEvent_AssetRequested;

            //Events that happen in the game
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            helper.Events.Input.ButtonPressed += OnButtonPressed;

            
            //Apply Harmony Patches
            
            var harmony = new Harmony(this.ModManifest.UniqueID);
            

            Monitor.Log("Patching Object.IsSprinkler with IsSprinklerPatch");
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.IsSprinkler)),
                prefix: new HarmonyMethod(typeof(IsSprinklerPatch), nameof(IsSprinklerPatch.Prefix))
            );

            //Patch GetBaseRadius
            Monitor.Log("Patching Object.GetBaseRadiusForSprinkler");
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.GetBaseRadiusForSprinkler)),
                prefix: new HarmonyMethod(typeof(GetBaseRadiusForSprinklerPatch), nameof(GetBaseRadiusForSprinklerPatch.Prefix))
                );


        }

        private void ContentEvent_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            //Sprinkler
            HaxorSprinkler.TranslatedName = Helper.Translation.Get("haxorsprinkler.name");
            HaxorSprinkler.TranslatedDescription = Helper.Translation.Get("haxorsprinkler.description");
            HaxorSprinkler.Texture = Helper.ModContent.GetInternalAssetName("assets/HaxorSprinkler.png").ToString()?.Replace("/", "\\");

            //Scarecrow
            HaxorScarecrow.TranslatedName = Helper.Translation.Get("haxorscarecrow.name");
            HaxorScarecrow.TranslatedDescription = Helper.Translation.Get("haxorscarecrow.description");
            HaxorScarecrow.Texture = Helper.ModContent.GetInternalAssetName("assets/HaxorScarecrow.png").ToString()?.Replace("/", "\\");
            

            //Lets start editing the content files.

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ObjectData>();

                    var haxorSprinkler = new ObjectData()
                    {
                        Name = HaxorSprinkler.ItemID,
                        DisplayName = HaxorSprinkler.TranslatedName,
                        Price = HaxorSprinkler.Price,
                        Description = HaxorSprinkler.TranslatedDescription,
                        SpriteIndex = HaxorSprinkler.ParentSheetIndex,
                        Texture = HaxorSprinkler.Texture,
                        Type = HaxorSprinkler.Type,
                        Category = HaxorSprinkler.Category
                    };

                    var newItem = new Dictionary<string, ObjectData>()
                    {
                        { HaxorSprinkler.ItemID, haxorSprinkler }
                    };

                    foreach (var a in newItem.Where(a => !data.Data.Contains(a)))
                    {
                        data.Data.Add(a);
                    }

                });
                

                
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/BigCraftables"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, BigCraftableData>();

                    var haxorCrow = new BigCraftableData()
                    {
                        Name = HaxorScarecrow.ItemID,
                        DisplayName = HaxorScarecrow.TranslatedName,
                        Price = HaxorScarecrow.Price,
                        Description = HaxorScarecrow.TranslatedDescription,
                        SpriteIndex = HaxorScarecrow.ParentSheetIndex,
                        Texture = HaxorScarecrow.Texture,
                        CanBePlacedOutdoors = HaxorScarecrow.CanBePlacedOutside,
                        CanBePlacedIndoors = HaxorScarecrow.CanBePlacedInside,
                        ContextTags = new List<string>(){"crow_scare", "crow_scare_radius_300"}
                        
                    };

                    var newItem = new Dictionary<string, BigCraftableData>() {{HaxorScarecrow.ItemID, haxorCrow }};
                    
                    foreach (var a in newItem.Where(a => !data.Data.Contains(a)))
                    {
                        data.Data.Add(a);
                    }
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
                    var isEnSprik = asset.Locale != "en" ? $"/{HaxorSprinkler.TranslatedName}" : "";
                    var isEnScare = asset.Locale != "en" ? $"/{HaxorScarecrow.TranslatedName}" : "";
                    Monitor.Log("Made it to the else");
                    var sprinklerIngredientsOut = !_config.ActivateHarderIngredients ? $"390 100/Home/{HaxorSprinkler.ItemID}/false/null{isEnSprik}" : $"386 10/Home/{HaxorSprinkler.ItemID}/false/null{isEnSprik}";
                    var scarecrowIngredientsOut = !_config.ActivateHarderIngredients ? $"388 100/Home/{HaxorScarecrow.ItemID}/true/null{isEnScare}" : $"337 10/Home/{HaxorScarecrow.ItemID}/true/null{isEnScare}";

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
                            Monitor.Log($"There was an error editing crafting recipes. {ex}");
                        }

                    }
                });
                
            }/*
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectContextTags"))
            {
                e.Edit(asset =>
                {
                    asset.AsDictionary<string, string>().Data.Add($"{HaxorScarecrow.Name}", "crow_scare, crow_scare_radius_300");
                    Monitor.Log($"Added context tags for HaxorScarecrow.");
                });
            }*/
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree)
                return;

            if (e.IsDown(SButton.NumPad7) && isDebugging)
            {
                var data = Game1.bigCraftableData; //asset.AsDictionary<string, BigCraftableData>().Data;

                if (data is null)
                    return;

                foreach (var d in data)
                {
                    Monitor.Log($"String: {d.Key}, Data: {d.Value.DisplayName}");
                }
            }
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
        /// Method that will search the farm and make sure it gets rid of old haxor items.
        /// </summary>
        private void FixLegacyItems()
        {
            var obj = new Dictionary<Vector2, SObject>();
            foreach (var o in Game1.getFarm().objects.Pairs)
            {
                if (o.Value.Name.Contains("Haxor Sprinkler") && o.Value.ParentSheetIndex != HaxorSprinkler.ParentSheetIndex)
                    obj.Add(o.Key, o.Value);
                else if (o.Value.ParentSheetIndex == 273)
                    o.Value.ParentSheetIndex = HaxorScarecrow.ParentSheetIndex;
                else if (o.Value.Name.Contains("Haxor Scarecrow") && o.Value.ParentSheetIndex != HaxorScarecrow.ParentSheetIndex)
                    obj.Add(o.Key, o.Value);
            }
            foreach (var i in obj)
            {
                Game1.getFarm().objects.Remove(i.Key);
                var newSprinkler = new SObject(HaxorSprinkler.ItemID, 1);
                var newScare = new SObject(i.Key, HaxorScarecrow.ItemID);
                var replacedWith = i.Value.Name.Contains("Sprinkler") ? $"Replaced with {HaxorSprinkler.ItemID}" : $"Replaced with {HaxorScarecrow.ItemID}";
                if (i.Value.Name.Contains("Sprinkler"))
                    Game1.getFarm().objects.Add(i.Key, newSprinkler);
                if (i.Value.Name.Contains("Scare"))
                {
                    //newScare.bigCraftable.Value = true;
                    Game1.getFarm().objects.Add(i.Key, newScare);
                }
                    

                Monitor.Log($"Removed Legacy Item: Name: {i.Value.Name}, {replacedWith}");
            }

        }

        /// <summary>
        /// Method that adds/removes the recipes for the haxor items.
        /// </summary>
        private void AddRecipes()
        {
            var curRecipes = new Dictionary<string, int>();

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
            Game1.player.craftingRecipes.Add(HaxorSprinkler.Name, 0);
            Game1.player.craftingRecipes.Add(HaxorScarecrow.Name, 0);
            Monitor.Log("Added the Haxor item recipes.");
        }

        /// <summary>Get all in-game locations.</summary>
        private IEnumerable<GameLocation> GetLocations()
        {
            var mainLocations = (Context.IsMainPlayer ? Game1.locations : this.Helper.Multiplayer.GetActiveLocations()).ToArray();

            foreach (var location in mainLocations.Concat(MineShaft.activeMines).Concat(VolcanoDungeon.activeLevels))
            {
                yield return location;

                foreach (var building in location.buildings)
                {
                    if (building.indoors.Value != null)
                        yield return building.indoors.Value;
                }
            }
        }
       
    }
}
