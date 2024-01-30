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
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SObject = StardewValley.Object;

namespace StardewCheater
{
    public class StardewCheater : Mod
    {
        private readonly Dictionary<int, String> _indexOfCropNames = new Dictionary<int, string>();
        private StardewValley.Object _currentTile;
        private TerrainFeature _terrain;
        private Building _currentTileBuilding = null;
        

        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            //helper.Events.GameLoop.DayStarted += OnDayStarted;
            //helper.Events.GameLoop.DayEnding += OnDayEnding;
            //helper.Events.GameLoop.SaveLoaded += OnDayEnding;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.IsDown(SButton.NumPad6))
            {
                GameLocation loc = Game1.currentLocation;
                Game1.player.MagneticRadius = 650;
                int curStam = Convert.ToInt32(Game1.player.Stamina);

                if (loc == null)
                    return;
                //Lets try to get All Objects.
                
                foreach (var obj in loc.objects.Pairs)
                {
                    //Vector2 pos = Game1.GlobalToLocal(Game1.viewport, new Vector2(obj.TileLocation.X, obj.TileLocation.Y));
                    //pos = Utility.ModifyCoordinatesForUIScale(pos);
                    float x = obj.Key.X * 64;
                    float y = obj.Key.Y * 64;
                    Monitor.Log($"Found Object: Name:{obj.Value.Name} Coords X: {x}, Y: {y}.");
                    if (obj.Value.IsSpawnedObject)
                    {
                        Item i = obj.Value;
                        //loc.objects.Remove(obj.Value.TileLocation);
                        //Game1.createItemDebris(i, new Vector2(x, y), -1);
                       obj.Value.performRemoveAction(obj.Value.TileLocation, loc);
                        
                    }
                    
                    
                }

                foreach (var i in loc.resourceClumps)
                {
                    Monitor.Log($"Found Resource Clump: X: {i.tile.X} Y: {i.tile.Y}.");
                }
                /*
                for (int xTile = 0; xTile < loc.Map.Layers[0].LayerWidth; ++xTile)
                {
                    for (int yTile = 0; yTile < loc.Map.Layers[0].LayerHeight; ++yTile)
                    {
                        loc.objects.TryGetValue(new Vector2(xTile, yTile), out var obj);
                       if(obj == null)
                        {
                            Monitor.Log("Couldn't find any objects");
                            return;
                        }
                        //loc.terrainFeatures.TryGetValue(new Vector2(xTile, yTile), out var ter);
                        Monitor.Log($"Found Object: Name:{obj.Name} Coords X: {obj.TileLocation.X}, Y: {obj.TileLocation.Y}.");
                        if (obj.IsSpawnedObject)
                        {
                            Game1.createItemDebris(obj.getOne(), new Vector2(obj.TileLocation.X, obj.TileLocation.X), -1);
                            loc.objects.Remove(new Vector2(obj.TileLocation.X, obj.TileLocation.Y));
                        }
                        //loc.checkAction(new Location((int)obj.TileLocation.X, (int)obj.TileLocation.Y), Game1.viewport, Game1.player);
                    }
                }*/
            }
            if(e.IsDown(SButton.NumPad4) && Game1.isFestival())
            {
                Game1.player.festivalScore += 1000;
            }
        }

        private void OnDayEnding(object sender, SaveLoadedEventArgs e /*DayEndingEventArgs e*/)
        {
            /*
            var loc = Game1.getFarm().buildings;
            if (loc == null)
                return;*/
            foreach (Building b2 in Game1.getFarm().buildings.Where(b => b.buildingType.Value.Contains("Coop")))
            {
                
                if (b2 is Coop coop)
                {
                    Monitor.Log($"Found a {coop.nameOfIndoors}");
                    coop.animalDoorOpen.Value = true;
                    
                    NetInt h = Helper.Reflection.GetField<NetInt>(coop, "yPositionOfAnimalDoor").GetValue();
                    Monitor.Log($"yPositionOfAnimalDoor: {h.ToString()}");
                    NetInt h1 = Helper.Reflection.GetField<NetInt>(coop, "yPositionOfAnimalDoor").GetValue();
                    h1.Value = -52;
                    Monitor.Log($"yPositionOfAnimalDoor: {h1.Value.ToString()}");
                }
                else
                    Monitor.Log("Couldn't find a coop.");
            }
            /*
            IEnumerator<Building> enumerator = Game1.getFarm().buildings.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Building b = enumerator.Current;
                    AnimalHouse bb = b?.indoors.Value as AnimalHouse;
                    if (b is Coop coop)
                    {
                        coop.animalDoorOpen.Value = true;
                        int h = Helper.Reflection.GetField<int>(coop, "yPositionOfAnimalDoor").GetValue();
                        Monitor.Log($"yPositionOfAnimalDoor: {h.ToString()}");
                    }
                }
            }
            finally
            {
                enumerator.Dispose();
            }*/
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            GameLocation loc = Game1.getFarm();
            if (loc == null)
                return;
            IEnumerator<Building> enumerator = Game1.getFarm().buildings.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Building b = enumerator.Current;
                    if (b != null)
                    {
                        AnimalHouse bb = b.indoors.Value as AnimalHouse;
                        if(!Game1.isRaining && !Game1.isSnowing)
                            b.animalDoorOpen.Value = true;
                    }
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!e.IsMultipleOf(4))
                return;

            #region GetTileUnderCursor

            // get tile under cursor
            _currentTileBuilding = Game1.currentLocation is BuildableGameLocation buildableLocation
                ? buildableLocation.getBuildingAt(Game1.currentCursorTile)
                : null;
            if (Game1.currentLocation != null)
            {
                if (Game1.currentLocation.Objects == null ||
                    !Game1.currentLocation.Objects.TryGetValue(Game1.currentCursorTile, out _currentTile))
                {
                    _currentTile = null;
                }

                if (Game1.currentLocation.terrainFeatures == null ||
                    !Game1.currentLocation.terrainFeatures.TryGetValue(Game1.currentCursorTile, out _terrain))
                {
                    if (_currentTile is IndoorPot pot &&
                        pot.hoeDirt.Value != null)
                    {
                        _terrain = pot.hoeDirt.Value;
                    }
                    else
                    {
                        _terrain = null;
                    }
                }
            }
            else
            {
                _currentTile = null;
                _terrain = null;
            }

            #endregion

            if (!e.IsMultipleOf(60))
                return;
            //Restore Energy
            if (Game1.player.currentLocation is FarmHouse house && Game1.player.isSitting.Value &&
                Game1.player.stamina < Game1.player.MaxStamina)
                Game1.player.stamina += 10;
        }
        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            //DoTimes();
            
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
           // DoTrackerSkill();
        }
        //My custom Voids
        private void DoTimes()
        {
            // draw hover tooltip
            if (_currentTileBuilding != null)
            {
                if (_currentTileBuilding is Mill millBuilding)
                {
                    if (millBuilding.input.Value != null)
                    {
                        if (!millBuilding.input.Value.isEmpty())
                        {
                            int wheatCount = 0;
                            int beetCount = 0;

                            foreach (var item in millBuilding.input.Value.items)
                            {
                                if (item != null &&
                                    !String.IsNullOrEmpty(item.Name))
                                {
                                    switch (item.Name)
                                    {
                                        case "Wheat": wheatCount = item.Stack; break;
                                        case "Beet": beetCount = item.Stack; break;
                                    }
                                }
                            }

                            StringBuilder builder = new StringBuilder();

                            if (wheatCount > 0)
                                builder.Append(wheatCount + " wheat");

                            if (beetCount > 0)
                            {
                                if (wheatCount > 0)
                                    builder.Append(Environment.NewLine);
                                builder.Append(beetCount + " beets");
                            }

                            if (builder.Length > 0)
                            {
                                IClickableMenu.drawHoverText(
                                   Game1.spriteBatch,
                                   builder.ToString(),
                                   Game1.smallFont);
                            }
                        }
                    }
                }
            }
            else if (_currentTile != null &&
                (!_currentTile.bigCraftable.Value ||
                _currentTile.MinutesUntilReady > 0))
            {
                if (_currentTile.bigCraftable.Value &&
                    _currentTile.MinutesUntilReady > 0 &&
                    _currentTile.heldObject.Value != null &&
                    _currentTile.Name != "Heater")
                {
                    StringBuilder hoverText = new StringBuilder();
                    hoverText.AppendLine(_currentTile.heldObject.Value.DisplayName);

                    if (_currentTile is Cask currentCask)
                    {
                        hoverText.Append((int)(currentCask.daysToMature.Value / currentCask.agingRate.Value))
                            .Append(" Days");
                    }
                    else
                    {
                        int min = _currentTile.MinutesUntilReady;
                        int days = min / 1440;
                        int hours = (min % 1440) / 60;
                        int minutes = min % 60;
                        if (days > 0)
                            hoverText.Append(days).Append(" Days ");
                        if (hours > 0)
                            hoverText.Append(hours).Append(" Hours ");
                        hoverText.Append(minutes).Append(" Minutes");
                    }
                    IClickableMenu.drawHoverText(
                        Game1.spriteBatch,
                        hoverText.ToString(),
                        Game1.smallFont);
                }
            }
            else if (_terrain != null)
            {
                if (_terrain is HoeDirt)
                {
                    HoeDirt hoeDirt = _terrain as HoeDirt;
                    if (hoeDirt.crop != null &&
                        !hoeDirt.crop.dead.Value)
                    {
                        int num = 0;

                        if (hoeDirt.crop.fullyGrown.Value &&
                            hoeDirt.crop.dayOfCurrentPhase.Value > 0)
                        {
                            num = hoeDirt.crop.dayOfCurrentPhase.Value;
                        }
                        else
                        {
                            for (int i = 0; i < hoeDirt.crop.phaseDays.Count - 1; ++i)
                            {
                                if (hoeDirt.crop.currentPhase.Value == i)
                                    num -= hoeDirt.crop.dayOfCurrentPhase.Value;

                                if (hoeDirt.crop.currentPhase.Value <= i)
                                    num += hoeDirt.crop.phaseDays[i];
                            }
                        }

                        if (hoeDirt.crop.indexOfHarvest.Value > 0)
                        {
                            Item i = new SObject(hoeDirt.crop.indexOfHarvest.Value, 1);
                            String hoverText = i.DisplayName + " ";//_indexOfCropNames[hoeDirt.crop.indexOfHarvest.Value];
                            if (String.IsNullOrEmpty(hoverText))
                            {
                                hoverText = new StardewValley.Object(new Debris(hoeDirt.crop.indexOfHarvest.Value, Vector2.Zero, Vector2.Zero).chunkType.Value, 1).DisplayName;
                                _indexOfCropNames.Add(hoeDirt.crop.indexOfHarvest.Value, hoverText);
                            }

                            StringBuilder finalHoverText = new StringBuilder();
                            finalHoverText.Append(hoverText).Append(": ");
                            if (num > 0)
                            {
                                finalHoverText.Append(num).Append(" Days");
                            }
                            else
                            {
                                finalHoverText.Append(" Ready To Harvest");
                            }
                            IClickableMenu.drawHoverText(
                                Game1.spriteBatch,
                                finalHoverText.ToString(),
                                Game1.smallFont);
                        }
                    }
                }
                else if (_terrain is FruitTree)
                {
                    FruitTree tree = _terrain as FruitTree;
                    var text = new StardewValley.Object(new Debris(tree.indexOfFruit.Value, Vector2.Zero, Vector2.Zero).chunkType.Value, 1).DisplayName;
                    if (tree.daysUntilMature.Value > 0)
                    {
                        text += Environment.NewLine + tree.daysUntilMature.Value + " Days";

                    }
                    IClickableMenu.drawHoverText(
                            Game1.spriteBatch,
                            text,
                            Game1.smallFont);
                }
            }/*
            else if (_currentTile != null)
            {
                if(_currentTile.IsSpawnedObject)
            }*/
        }

        private void DoTrackerSkill()
        {
            #region Testing Tracker Skill
            if (Game1.currentLocation != null)
            {
                foreach (KeyValuePair<Vector2, SObject> v in Game1.currentLocation.objects.Pairs)
                {
                    
                    if (((bool)v.Value.IsSpawnedObject || v.Value.ParentSheetIndex == 590) &&
                        Utility.isOnScreen(v.Key * 64f + new Vector2(32f, 32f), 64))
                    {
                        Monitor.Log($"Found Object at:{v.Value.TileLocation.X}, {v.Value.TileLocation.Y}. It was a {v.Value.DisplayName} ", LogLevel.Info);
                        Microsoft.Xna.Framework.Rectangle vpbounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
                        Vector2 onScreenPosition2 = default(Vector2);
                        float rotation2 = 0f;
                        if (v.Key.X * 64f > (float)(Game1.viewport.MaxCorner.X - 64))
                        {
                            onScreenPosition2.X = vpbounds.Right - 8;
                            rotation2 = (float)Math.PI / 2f;
                        }
                        else if (v.Key.X * 64f < (float)Game1.viewport.X)
                        {
                            onScreenPosition2.X = 8f;
                            rotation2 = -(float)Math.PI / 2f;
                        }
                        else
                        {
                            onScreenPosition2.X = v.Key.X * 64f - (float)Game1.viewport.X;
                        }

                        if (v.Key.Y * 64f > (float)(Game1.viewport.MaxCorner.Y - 64))
                        {
                            onScreenPosition2.Y = vpbounds.Bottom - 8;
                            rotation2 = (float)Math.PI;
                        }
                        else if (v.Key.Y * 64f < (float)Game1.viewport.Y)
                        {
                            onScreenPosition2.Y = 8f;
                        }
                        else
                        {
                            onScreenPosition2.Y = v.Key.Y * 64f - (float)Game1.viewport.Y;
                        }

                        if (onScreenPosition2.X == 8f && onScreenPosition2.Y == 8f)
                        {
                            rotation2 += (float)Math.PI / 4f;
                        }

                        if (onScreenPosition2.X == 8f && onScreenPosition2.Y == (float)(vpbounds.Bottom - 8))
                        {
                            rotation2 += (float)Math.PI / 4f;
                        }

                        if (onScreenPosition2.X == (float)(vpbounds.Right - 8) && onScreenPosition2.Y == 8f)
                        {
                            rotation2 -= (float)Math.PI / 4f;
                        }

                        if (onScreenPosition2.X == (float)(vpbounds.Right - 8) &&
                            onScreenPosition2.Y == (float)(vpbounds.Bottom - 8))
                        {
                            rotation2 -= (float)Math.PI / 4f;
                        }

                        Microsoft.Xna.Framework.Rectangle srcRect = new Microsoft.Xna.Framework.Rectangle(412, 495, 5, 4);
                        float renderScale = 4f;
                        Vector2 safePos =
                            Utility.makeSafe(
                                renderSize: new Vector2((float)srcRect.Width * renderScale,
                                    (float)srcRect.Height * renderScale), renderPos: onScreenPosition2);
                       /* Game1.spriteBatch.Draw(Game1.mouseCursors, safePos, srcRect, Microsoft.Xna.Framework.Color.White,
                            rotation2, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);*/
                        Game1.spriteBatch.Draw(Game1.mouseCursors2, safePos, srcRect, Color.White, rotation2, 
                            new Vector2(2f,2f), renderScale, SpriteEffects.None, 1f);
                    }
                }
            }

            #endregion
        }

        /// <summary>Get a rectangle representing the tile area in absolute pixels from the map origin.</summary>
        /// <param name="tile">The tile position.</param>
        protected Rectangle GetAbsoluteTileArea(Vector2 tile)
        {
            Vector2 pos = tile * Game1.tileSize;
            return new Rectangle((int)pos.X, (int)pos.Y, Game1.tileSize, Game1.tileSize);
        }
    }
    }