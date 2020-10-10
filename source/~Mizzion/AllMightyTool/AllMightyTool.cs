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
using AllMightyTool.Framework;
using AllMightyTool.Framework.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyStardewMods.Common;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace AllMightyTool
{
    public class AllMightyTool : Mod
    {
        private static bool _usePailonOtherAnimals;
        private static int _powerLevel = -1;
        private static int _upgradeLevel = 5;
        private static string _hoeDirtTool = "";
        private static ModConfig _toolConfig;
        private static Axe _ghostAxe;
        private static Pickaxe _ghostPickaxe;
        private static Hoe _ghostHoe;
        private static WateringCan _ghostWaterCan;
        private static CustomScythe _ghostScythe;
        private static CustomShearPail _ghostShearPail;
        private static SButton _actKey;
        private static SButton _cropKey;
        private static Texture2D _buildingPlacementTiles;
        private Pan _ghostPan;

        public override void Entry(IModHelper helper)
        {
            _toolConfig = Helper.ReadConfig<ModConfig>();
            _upgradeLevel = _toolConfig.ToolLevel;


            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.RenderingHud += OnRenderingHud;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        /*
         * Private Methods
         */
        //Events
        
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //Make sure that the activate key is valid
            if (!Enum.TryParse(_toolConfig.KeyBindClear, true, out _actKey))
            {
                _actKey = SButton.Z;
                Monitor.Log("Keybind was invalid. setting it to Z");
            }
            //Make sure the crop key is valid
            if (!Enum.TryParse(_toolConfig.KeyBindCrop, true, out _cropKey))
            {
                _cropKey = SButton.X;
                Monitor.Log("Keybind was invalid. setting it to X");
            }

            //Set up Phantom Tools
            _hoeDirtTool = "PickAxe";
            _ghostScythe = new CustomScythe(47) { UpgradeLevel = _toolConfig.ToolLevel };
            _ghostAxe = new Axe { UpgradeLevel = LevelCheck(_toolConfig.ToolLevel) };
            _ghostPickaxe = new Pickaxe { UpgradeLevel = LevelCheck(_toolConfig.ToolLevel) };
            _ghostHoe = new Hoe { UpgradeLevel = LevelCheck(_toolConfig.ToolLevel) };
            _ghostWaterCan = new WateringCan { UpgradeLevel = LevelCheck(_toolConfig.ToolLevel) };
            _ghostPan = new Pan();
            _usePailonOtherAnimals = _toolConfig.UsePailonOtherAnimals;
            _ghostShearPail = new CustomShearPail(_usePailonOtherAnimals);


        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!(e.IsDown(_actKey) || e.IsDown(_cropKey)) || !Context.IsWorldReady)
                return;
            if (Game1.currentLocation == null || Game1.player == null || (Game1.player.UsingTool || !Game1.player.CanMove || (Game1.activeClickableMenu != null || Game1.CurrentEvent != null)) || Game1.gameMode != 3)
                return;
            GameLocation currentLocation = Game1.currentLocation;
            ICursorPosition cur = Helper.Input.GetCursorPosition();
            var c = Game1.getMousePosition();
            Vector2[] grid = GetTileGrid(cur.Tile * 64f, _upgradeLevel).ToArray();



            if (_powerLevel <= -1)
                _powerLevel = 1;
            Game1.player.toolPower = _powerLevel;

            //May need to rewrite this crap

            //Action key pressed.
            if (e.IsDown(_actKey))
            {
                foreach (var i in grid)
                {
                    var g = i;
                    currentLocation.Objects.TryGetValue(g, out SObject @object);
                    currentLocation.terrainFeatures.TryGetValue(g, out TerrainFeature @terain);

                    //Not sure if this will work lets test
                    g = (g * Game1.tileSize) + new Vector2(Game1.tileSize / 2f);
                    
                    /*
                    if (currentLocation.doesTileHaveProperty((int)(i.X), (int)(i.Y), "Water", "Back") != null ||
                    currentLocation.doesTileHaveProperty((int)(i.X), (int)(i.Y), "WaterSource", "Back") != null ||
                    (currentLocation as BuildableGameLocation)?.getBuildingAt(g) != null &&
                    (currentLocation as BuildableGameLocation).getBuildingAt(g).buildingType.Value.Equals("Well"))
                    {
                        // ISSUE: explicit non-virtual call
                        if (currentLocation.orePanPoint.Value != Point.Zero)
                            _ghostPan.DoFunction(currentLocation, (int)i.X, (int)i.Y, 1, Game1.player);
                        else
                            UseghostWaterCan(currentLocation, (int)i.X, (int)i.Y);
                    }
                    else*/
                    Vector2 ca = new Vector2(i.X, i.Y) * 64f;

                    if (@object != null)
                    {
                        if (@object.Name.Equals("Twig") || @object.Name.Contains("ood Fence"))
                            _ghostAxe.DoFunction(currentLocation, (int)ca.X, (int)ca.Y, 0, Game1.player);
                        else if (@object.Name.Contains("Weed"))
                            _ghostScythe.DoDamage(currentLocation, (int)ca.X, (int)ca.Y, Game1.player.getFacingDirection(), 0, Game1.player);
                        else if (@object.Name.Contains("Stone"))
                            _ghostPickaxe.DoFunction(currentLocation, (int)ca.X, (int)ca.Y, 0, Game1.player);

                    }
                    else if (currentLocation is AnimalHouse &&
                             _ghostShearPail.TargetAnimal(currentLocation,
                                 (int)ca.X,
                                 (int)ca.Y,
                                 Game1.player) !=
                             null)
                        _ghostShearPail.DoFunction(currentLocation, (int)ca.X, (int)ca.Y, 0, Game1.player);
                    else if (@terain != null)
                    {
                        //Monitor.Log($"terrainFeature wasn't null ....");
                        //Lets do som checks

                        if (@terain is Tree tree)
                            _ghostAxe.DoFunction(currentLocation, (int)ca.X, (int)ca.Y, 0, Game1.player);
                        else if (@terain is HoeDirt dirt)
                        {
                            if (currentLocation.IsFarm || currentLocation.Name.Contains("Greenhouse"))
                            {
                                if (dirt.crop != null || dirt.fertilizer.Value != 0)
                                {
                                    if (dirt.crop != null && (dirt.crop.harvestMethod.Value == 1 && dirt.readyForHarvest() || !dirt.crop.dead.Value))
                                        _ghostScythe.DoDamage(currentLocation, (int)ca.X, (int)ca.Y, Game1.player.getFacingDirection(), 0, Game1.player);
                                    else
                                        UseghostWaterCan(currentLocation, (int)ca.X, (int)ca.Y);
                                }
                                else if (_hoeDirtTool.Equals("Pickaxe", StringComparison.InvariantCultureIgnoreCase))
                                    _ghostPickaxe.DoFunction(currentLocation, (int)ca.X, (int)ca.Y, 1, Game1.player);
                                else if (_hoeDirtTool.Equals("Hoe", StringComparison.InvariantCultureIgnoreCase))
                                    _ghostHoe.DoFunction(currentLocation, (int)ca.X, (int)ca.Y, _powerLevel + 1, Game1.player);
                                else
                                    UseghostWaterCan(currentLocation, (int)ca.X, (int)ca.Y);
                            }
                        }
                        else if (@terain is Grass grass)
                            _ghostScythe.DoDamage(currentLocation, (int)ca.X, (int)ca.Y, Game1.player.getFacingDirection(), 0, Game1.player);
                    }
                    else if (currentLocation is SlimeHutch)
                    {
                        UseghostWaterCan(currentLocation, (int)ca.X, (int)ca.Y);
                    }
                    else
                    {
                        //Monitor.Log("All if's failed... run for the hills.");
                        //if (currentLocation.doesTileHaveProperty((int)(i.X), (int)(i.Y), "Diggable", "Back") != null)
                        // _ghostHoe.DoFunction(currentLocation, (int)i.X, (int)i.Y, 0, Game1.player);
                        /*
                        Farmer player1 = Game1.player;
                        player1.stamina = (float)(player1.stamina + (2.0 - Game1.player.MiningLevel * 0.100000001490116));
                        _ghostAxe.DoFunction(currentLocation, (int)i.X, (int)i.Y, 1, Game1.player);
                        _ghostPickaxe.DoFunction(currentLocation, (int)i.X, (int)i.Y, 1, Game1.player);
                        if (currentLocation.doesTileHaveProperty((int)(i.X), (int)(i.Y), "Diggable", "Back") != null)
                        {
                            Game1.player.toolPower = Game1.player.toolPower >= _ghostHoe.UpgradeLevel ? _ghostHoe.UpgradeLevel : Game1.player.toolPower;
                            Farmer player2 = Game1.player;
                            player2.stamina = (float)(player2.stamina + (2.0 - Game1.player.FarmingLevel * 0.100000001490116));
                            _ghostHoe.DoFunction(currentLocation, (int)i.X, (int)i.Y, _powerLevel + 1, Game1.player);
                        }*/
                    }
                }
            }
            //Crop key pressed
            if (e.IsDown(_cropKey))
            {
                foreach (var i in grid)
                {
                    var g = i;
                    currentLocation.Objects.TryGetValue(g, out SObject @object);
                    currentLocation.terrainFeatures.TryGetValue(g, out TerrainFeature @terrain);

                    //Not sure if this will work lets test
                    g = (g * Game1.tileSize) + new Vector2(Game1.tileSize / 2f);

                    if (@object != null)
                    {
                        if (@object.Name.Equals("Artifact Spot"))
                            _ghostHoe.DoFunction(currentLocation, (int)i.X, (int)i.Y, 0, Game1.player);
                    }
                    else if (@terrain != null)
                    {
                        Farmer player = Game1.player;
                        /*
                            public const int dry = 0;
                            public const int watered = 1;
                            public const int invisible = 2;
                            public const int noFertilizer = 0;
                            public const int fertilizerLowQuality = 368;
                            public const int fertilizerHighQuality = 369;
                            public const int waterRetentionSoil = 370;
                            public const int waterRetentionSoilQUality = 371;
                            public const int speedGro = 465;
                            public const int superSpeedGro = 466;
                            Plant Category = -74
                            Fertilizer = -19
                         */
                        if (@terrain is HoeDirt dirt)
                        {
                            if (dirt.crop == null &&
                                player.ActiveObject != null &&
                                ((player.ActiveObject.Category == SObject.SeedsCategory || player.ActiveObject.Category == -19) &&
                                 dirt.canPlantThisSeedHere(player.ActiveObject.ParentSheetIndex, (int)i.X, (int)i.Y, player.ActiveObject.Category == -19)))
                            {
                                if ((dirt.plant(player.ActiveObject.ParentSheetIndex, (int)i.X, (int)i.Y, player, player.ActiveObject.Category == -19, currentLocation) && player.IsLocalPlayer))
                                    player.reduceActiveItemByOne();
                                Game1.haltAfterCheck = false;
                            }
                        }
                    }
                    else
                    {
                        //@object and terrainFeature was null, must be dirt.
                        _ghostHoe.DoFunction(currentLocation, (int)i.X, (int)i.Y, 0, Game1.player);
                    }
                }
            }
            _powerLevel = -1;
            Game1.player.Stamina = Game1.player.MaxStamina;
            //End May need to rewrite
        }


        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (_buildingPlacementTiles == null)
                _buildingPlacementTiles = Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles");

            if (!(Helper.Input.IsDown(_actKey) || Helper.Input.IsDown(_cropKey)) || Game1.currentLocation == null || (Game1.player == null || !Game1.hasLoadedGame) || (Game1.player.UsingTool || !Game1.player.CanMove || (Game1.activeClickableMenu != null || Game1.CurrentEvent != null)) || Game1.gameMode != 3)
                return;
            DrawRadius(Game1.spriteBatch, _upgradeLevel);
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !e.IsMultipleOf(30))
                return;
            if (!Helper.Input.IsDown(_actKey) || Game1.currentLocation == null || (Game1.player == null || !Game1.hasLoadedGame) || (Game1.player.UsingTool || !Game1.player.CanMove || (Game1.activeClickableMenu != null || Game1.CurrentEvent != null)) || (Game1.gameMode != 3 || _powerLevel >= _upgradeLevel))
                return;
            /*
            _powerLevel++;
            if (Game1.soundBank == null || _powerLevel < 1)
                return;
            Cue cue = Game1.soundBank.GetCue("toolCharge");
            cue.SetVariable("Pitch", _powerLevel * 500);
            cue.Play();*/
        }

        //Events Over
        private int LevelCheck(int level)
        {
            return level >= 1 && level <= 4 ? level : 4;
        }


        /// <summary>Get a grid of tiles.</summary>
        /// <param name="origin">The center of the grid.</param>
        /// <param name="distance">The number of tiles in each direction to include.</param>
        private IEnumerable<Vector2> GetTileGrid(Vector2 origin, int distance)
        {
            for (int x = -distance; x <= distance; x++)
            {
                for (int y = -distance; y <= distance; y++)
                    yield return new Vector2(origin.X + x, origin.Y + y);
            }
        }

        /// <summary>Draw a radius around the player.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        /// <param name="radius">The radius to draw</param>
        public void DrawRadius(SpriteBatch spriteBatch, int radius)
        {
            //bool enabled = this.IsEnabled();
            ICursorPosition cur = Helper.Input.GetCursorPosition();
            foreach (Vector2 tile in GetTileGrid(cur.Tile * 64f, radius))
            {
                // get tile area in screen pixels
                Rectangle area = new Rectangle((int)(tile.X * Game1.tileSize - Game1.viewport.X), (int)(tile.Y * Game1.tileSize - Game1.viewport.Y), Game1.tileSize, Game1.tileSize);

                // choose tile color
                Color color = Color.Green;//enabled ? Color.Green : Color.Red;

                // draw background
                spriteBatch.DrawLine(area.X, area.Y, new Vector2(area.Width, area.Height), color * 0.2f);

                // draw border
                int borderSize = 1;
                Color borderColor = color * 0.5f;
                spriteBatch.DrawLine(area.X, area.Y, new Vector2(area.Width, borderSize), borderColor); // top
                spriteBatch.DrawLine(area.X, area.Y, new Vector2(borderSize, area.Height), borderColor); // left
                spriteBatch.DrawLine(area.X + area.Width, area.Y, new Vector2(borderSize, area.Height), borderColor); // right
                spriteBatch.DrawLine(area.X, area.Y + area.Height, new Vector2(area.Width, borderSize), borderColor); // bottom
            }
        }


        private void UseghostWaterCan(GameLocation location, int x, int y)
        {
            Game1.player.toolPower = Game1.player.toolPower >= (_ghostWaterCan).UpgradeLevel ? (_ghostWaterCan).UpgradeLevel : Game1.player.toolPower;
            _ghostWaterCan.DoFunction(location, x, y, _powerLevel, Game1.player);
            if (!_toolConfig.ShowWaterLeftMessage || _ghostWaterCan.WaterLeft <= 0)
                return;
            Game1.showGlobalMessage("Water:" + _ghostWaterCan.WaterLeft);
        }

    }

}
