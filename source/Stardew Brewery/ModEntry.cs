using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Buildings;
using StardewValley.Objects;
using xTile;
using SObject = StardewValley.Object;
using StardewBrewery.Integration;

namespace StardewBrewery
{
    public class ModEntry : Mod, IAssetEditor, IAssetLoader
    {
        // set up vars for later
        private bool suppressCaskMessage = false;
        private Texture2D breweryTexture;
        private bool suppressDrink = false;

        public override void Entry(IModHelper helper)
        {

            // add our event handlers
            InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            MenuEvents.MenuChanged += this.MenuEvents_MenuChanged;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            LocationEvents.CurrentLocationChanged += this.LocationEvents_CurrentLocationChanged;
            GameEvents.FirstUpdateTick += this.GameEvents_FirstUpdateTick;
            // initially set the texture, this gets reset after the game is loaded
            breweryTexture = Helper.Content.Load<Texture2D>(@"assets/Spring_Brewery.png", ContentSource.ModFolder);
        }



        public bool CanLoad<Texture2D>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Buildings\Brewery") || asset.AssetNameEquals(@"Maps\Brewery");
        }

        public T Load<T>(IAssetInfo asset)
        {
            if(asset.AssetNameEquals(@"Buildings\Brewery"))
                return (T)(object)breweryTexture;
            else
                return (T)(object)Helper.Content.Load<Map>("assets/SDBBrewery.tbin", ContentSource.ModFolder);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\Blueprints");
        }

        public void Edit<T>(IAssetData asset)
        {
            asset.AsDictionary<string, string>().Data.Add("Brewery", "388 600 390 800 348 5/11/6/5/5/-1/-1/Brewery/Brewery/A brewery! You can brew and age whatever you like here./Buildings/none/96/96/20/null/Farm/150000/false");
        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (Context.IsWorldReady && e.IsActionButton && Game1.currentLocation.name == "Brewery" && Game1.currentLocation.objects.TryGetValue(e.Cursor.GrabTile, out SObject obj) && obj is Cask cask && cask.heldObject == null)
            { // If the player activates an empty cask in the brewery, suppress input and hijack the cask.
                suppressCaskMessage = true;
                e.SuppressButton();
                Item held = Game1.player.CurrentItem;
                if (held != null)
                {
                    if (held.parentSheetIndex == 348 || held.parentSheetIndex == 346 || held.parentSheetIndex == 303 || held.parentSheetIndex == 459 || held.parentSheetIndex == 426 || held.parentSheetIndex == 424)
                    {
                        switch (held.parentSheetIndex)
                        {
                            case 348:
                                SObject wine = new SObject(348, 1)
                                {
                                    name = $"{held.Name}",
                                    price = ((SObject)Game1.player.CurrentItem).price,
                                    preserve = SObject.PreserveType.Wine,
                                    preservedParentSheetIndex = held.parentSheetIndex,
                                    quality = ((SObject)Game1.player.CurrentItem).quality
                                };
                                cask.heldObject = wine;
                                cask.agingRate = 1;
                                break;
                            case 346:
                                SObject beer = new SObject(346, 1)
                                {
                                    price = ((SObject)Game1.player.CurrentItem).price,
                                    quality = ((SObject)Game1.player.CurrentItem).quality
                                };
                                cask.heldObject = beer;
                                cask.agingRate = 2;
                                break;
                            case 303:
                                SObject paleAle = new SObject(303, 1)
                                {
                                    price = ((SObject)Game1.player.CurrentItem).price,
                                    quality = ((SObject)Game1.player.CurrentItem).quality
                                };
                                cask.heldObject = paleAle;
                                cask.agingRate = 1.66f;
                                break;
                            case 459:
                                SObject mead = new SObject(459, 1)
                                {
                                    price = ((SObject)Game1.player.CurrentItem).price,
                                    quality = ((SObject)Game1.player.CurrentItem).quality
                                };
                                cask.heldObject = mead;
                                cask.agingRate = 2;
                                break;
                            case 426:
                                SObject goatCheese = new SObject(426, 1)
                                {
                                    price = ((SObject)Game1.player.CurrentItem).price,
                                    quality = ((SObject)Game1.player.CurrentItem).quality
                                };
                                cask.heldObject = goatCheese;
                                cask.agingRate = 4;
                                break;
                            case 424:
                                SObject cheese = new SObject(424, 1)
                                {
                                    price = ((SObject)Game1.player.CurrentItem).price,
                                    quality = ((SObject)Game1.player.CurrentItem).quality
                                };
                                cask.heldObject = cheese;
                                cask.agingRate = 4;
                                break;
                        }
                        cask.daysToMature = 56;
                        cask.minutesUntilReady = 999999;
                        switch (cask.heldObject.quality)
                        {
                            case SObject.medQuality:
                                cask.daysToMature = 42;
                                break;
                            case SObject.highQuality:
                                cask.daysToMature = 28;
                                break;
                            case SObject.bestQuality:
                                cask.daysToMature = 0;
                                cask.minutesUntilReady = 1;
                                break;
                        }

                        held.Stack--;
                        if (held.Stack <= 0)
                        {
                            Game1.player.items.Remove(held);
                        }
                        
                        Game1.playSound("Ship");
                        Game1.playSound("bubbles");
                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.animations, new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, cask.tileLocation * (float)Game1.tileSize + new Vector2(0.0f, (float)(-Game1.tileSize * 2)), false, false, (float)(((double)cask.tileLocation.Y + 1.0) * (double)Game1.tileSize / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                        {
                            alphaFade = 0.005f
                        });
                        suppressDrink = true;
                    }
                }

            }
        }

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        { // add the blueprint to CarpenterMenu
            if (Context.IsWorldReady && e.NewMenu is CarpenterMenu && Helper.Reflection.GetField<bool>(e.NewMenu, "magicalConstruction").GetValue() == false)
            {
                Helper.Reflection.GetField<List<BluePrint>>(e.NewMenu, "blueprints").GetValue().Add(new BluePrint("Brewery"));
            }
            if (e.NewMenu is DialogueBox eatMenu)
            {
                if (suppressDrink == true)
                {
                    Response no = Helper.Reflection.GetField<List<Response>>(eatMenu, "responses").GetValue()[1];
                    Game1.currentLocation.answerDialogue(no);
                    eatMenu.closeDialogue();
                    suppressDrink = false;
                }
                else
                    return; // wait until confirmation closed
            }
        }

        // this is for removing the "Only usable in Cellar" message
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (suppressCaskMessage == true)
            {
                string text = Game1.content.LoadString("Strings\\Objects:CaskNoCellar");
                HUDMessage message = Game1.hudMessages.FirstOrDefault(p => p.message == text);
                if (message != null)
                    Game1.hudMessages.Remove(message);
            }
        }
        private void LocationEvents_CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e)
        {
            if (e.NewLocation is Farm farm)
            {
                for (int i = 0; i < farm.buildings.Count; ++i)
                {
                    // This is probably a new building if it hasn't been converted yet.
                    if (farm.buildings[i].buildingType == "Brewery" &&
                         farm.buildings[i].GetType() == typeof(Building))
                    {
                        var b = farm.buildings[i];

                        farm.buildings[i] = new BreweryBuilding();
                        farm.buildings[i].buildingType = b.buildingType;
                        farm.buildings[i].humanDoor = b.humanDoor;
                        farm.buildings[i].indoors = b.indoors;
                        farm.buildings[i].indoors.isOutdoors = false;
                        farm.buildings[i].nameOfIndoors = b.nameOfIndoors;
                        farm.buildings[i].tileX = b.tileX;
                        farm.buildings[i].tileY = b.tileY;
                        farm.buildings[i].tilesWide = b.tilesWide;
                        farm.buildings[i].tilesHigh = b.tilesHigh;
                        farm.buildings[i].load();
                    }
                }
            }
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            if (Game1.dayOfMonth == 1)
            {
                breweryTexture = Helper.Content.Load<Texture2D>($@"assets/{ Game1.currentSeason }_Brewery.png", ContentSource.ModFolder);
                this.Helper.Content.InvalidateCache(@"Buildings\Brewery");
            }
            
        }
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            breweryTexture = Helper.Content.Load<Texture2D>($@"assets/{ Game1.currentSeason }_Brewery.png", ContentSource.ModFolder);
            this.Helper.Content.InvalidateCache(@"Buildings\Brewery");
        }

        private void GameEvents_FirstUpdateTick(object sender, EventArgs e)
        {
            // enable Farm Expansion integration
            FarmExpansionIntegration farmExpansion = new FarmExpansionIntegration(this.Helper.ModRegistry, this.Monitor);
            if (farmExpansion.IsLoaded)
            {
                BluePrint bp = new BluePrint("Brewery");
                farmExpansion.AddFarmBluePrint(bp);
            }
        }
    }
}