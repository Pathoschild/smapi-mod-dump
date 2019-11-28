using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace CustomFishing
{
    public class CustomFishing : Mod
    {
        private ModConfig _config;
        //Config Settings
        private int _defaultBobberVal;
        private bool _enabled;
        private int _difficulty;
        private bool _easyFishing;
        public bool AlwaysMaxCast;
        private int _baseBobberBarHeight;
        private bool _instantFishBite;
        private bool _instantCatch;
        private bool _alwaysPerfect;
        private bool _infiniteTackle;
        private bool _alwaysSpawnTreasure;
        private bool _alwaysGetTreasure;

        //
        private bool _showMessage;
        private string _fishingMsg = "";
        private int _motionType = 0;
        
        //Fish Data
        private string _fishName = "";
        private float _dartingAmount;
        private string _bobberBehavior;
        private int _minFishSize;
        private int _maxFishSize;
        private string[] _spawnTimes;
        private string[] _spawnSeasons;
        private string _dummyString;
        private int _spawnStart1 = 0;
        private int _spawnStart2 = 0;
        private int _spawnStop1 = 0;
        private int _spawnStop2 = 0;
        private int _minWaterDepth;
        private double _spawnMultiplier;
        private double _depthMultiplier;
        private int _minFishingLevel;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += AfterLoad;
            //SaveEvents.AfterLoad += AfterLoad;
            helper.Events.GameLoop.UpdateTicked += UpdateTick;
            //GameEvents.UpdateTick += UpdateTick;
            helper.Events.Display.Rendered += DrawTick;
            //GraphicsEvents.OnPostRenderEvent += DrawTick;
            helper.Events.Input.ButtonPressed += KeyPressed;
            //ControlEvents.KeyPressed += KeyPressed;
        }
        public void DrawTick(object sender, RenderedEventArgs e)
        {
            bool inCave = Game1.currentLocation is MineShaft || Game1.currentLocation is FarmCave;
            if (_showMessage)
            {
                
                //DrawTextBox(5, inCave ? 100 : 5, Game1.smallFont, fishName);
                DrawTextBox(5, (Game1.viewport.Height - 200), Game1.dialogueFont, _fishingMsg);
            }
        }
        private void AfterLoad(object sender, SaveLoadedEventArgs e)
        {
            _config = Helper.ReadConfig<ModConfig>();
            RePopulateConfig();
        }
        private void UpdateTick(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !_enabled)
                return;
            //Everything should be good to proceed
            SFarmer player = Game1.player;
            if (_enabled)
            {
                if (Game1.activeClickableMenu == null && player.CurrentTool is FishingRod rod)
                {
                    if (AlwaysMaxCast)
                        rod.castingPower = 1.01F;

                    if (_instantFishBite)
                    {
                        if (rod.timeUntilFishingBite > 0)
                            rod.timeUntilFishingBite = 0;
                    }

                    if (_infiniteTackle && rod.attachments[1] != null)
                        rod.attachments[1].scale.Y = 1;
                }
                if (Game1.activeClickableMenu is BobberBar bobberMenu)
                {
                    GetFish(Helper.Reflection.GetField<int>(bobberMenu, "whichFish").GetValue());
                    _fishingMsg = "Hooked Fish:\n\r" + _fishName;
                    _showMessage = true;

                    if (_instantCatch)
                        Helper.Reflection.GetField<float>(bobberMenu, "distanceFromCatching").SetValue(1);

                    if (_alwaysPerfect)
                        Helper.Reflection.GetField<bool>(bobberMenu, "perfect").SetValue(true);

                    if (_easyFishing)
                    {
                       Helper.Reflection.GetField<int>(bobberMenu, "bobberBarHeight").SetValue(560);
                    }
                    else
                    {
                        _defaultBobberVal = (Game1.tileSize * 3 / 2 + player.FishingLevel * 8) + _baseBobberBarHeight;
                        _defaultBobberVal = ((Game1.tileSize * 3 / 2 + player.FishingLevel * 8) + _baseBobberBarHeight) >= 500 ? 499 : _defaultBobberVal;
                       Helper.Reflection.GetField<int>(bobberMenu, "bobberBarHeight").SetValue(_defaultBobberVal);
                    }

                    if (_alwaysSpawnTreasure)
                    {
                        Helper.Reflection.GetField<bool>(bobberMenu, "treasure").SetValue(true);
                        Helper.Reflection.GetField<float>(bobberMenu, "treasureScale").SetValue(1);
                    }
                    if (_alwaysGetTreasure && Helper.Reflection.GetField<bool>(bobberMenu, "treasure").GetValue())
                    {                        
                        Helper.Reflection.GetField<bool>(bobberMenu, "treasureCaught").SetValue(true);
                        Helper.Reflection.GetField<float>(bobberMenu, "treasureScale").SetValue(0);
                    }
                    if(Helper.Reflection.GetField<bool>(bobberMenu, "treasure").GetValue())
                    {
                        Helper.Reflection.GetField<float>(bobberMenu, "treasureScale").SetValue(1);
                    }
                    if(Helper.Reflection.GetField<bool>(bobberMenu, "treasureCaught").GetValue())
                    {
                        Helper.Reflection.GetField<float>(bobberMenu, "treasureScale").SetValue(0);
                    }
                    
                }
                else
                {
                    _fishName = "";
                    _showMessage = false;
                }
            }
        }
        private void KeyPressed(object sender, ButtonPressedEventArgs e)
        {
            if(e.Button == SButton.F8)
            {
                _config = Helper.ReadConfig<ModConfig>();
                RePopulateConfig();
            }
            if(e.Button == SButton.F7)
            {
                int n1 = 1;
                if (n1 == 2)
                    return;
                int o = _motionType;
                if(o == 4)
                {
                    _motionType = 0;
                }
                else
                {
                    _motionType += 1;
                }
                Monitor.Log($"Changed MotionType to: {_motionType}", LogLevel.Alert);
            }
        }
        //Custom Voids

        private void RePopulateConfig()
        {
            _enabled = _config.Enabled;
            _difficulty = _config.Difficulty;
            _easyFishing = _config.EasyFishing;
            AlwaysMaxCast = _config.AlwaysMaxCast;
            _baseBobberBarHeight = _config.BaseBobberBarHeight;
            _instantFishBite = _config.InstantFishBite;
            _instantCatch = _config.InstantCatch;
            _alwaysPerfect = _config.AlwaysPerfect;
            _infiniteTackle = _config.InfiniteTackle;
            _alwaysSpawnTreasure = _config.AlwaysSpawnTreasure;
            _alwaysGetTreasure = _config.AlwaysGetTreasure;
        }
        //Draw Text Box Thanks to CJB for the coding in CJB Cheat Menu
        public static void DrawTextBox(int x, int y, SpriteFont font, string message, int align = 0, float colorIntensity = 1F)
        {
            SpriteBatch spriteBatch = Game1.spriteBatch;

            Vector2 bounds = font.MeasureString(message);
            int width = (int)bounds.X + Game1.tileSize / 2;
            int height = (int)font.MeasureString(message).Y + Game1.tileSize / 3;
            switch (align)
            {
                case 0:
                    IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height + Game1.tileSize / 16, Color.White * colorIntensity);
                    Utility.drawTextWithShadow(spriteBatch, message, font, new Vector2(x + Game1.tileSize / 4, y + Game1.tileSize / 4), Game1.textColor);
                    break;
                case 1:
                    IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x - width / 2, y, width, height + Game1.tileSize / 16, Color.White * colorIntensity);
                    Utility.drawTextWithShadow(spriteBatch, message, font, new Vector2(x + Game1.tileSize / 4 - width / 2, y + Game1.tileSize / 4), Game1.textColor);
                    break;
                case 2:
                    IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x - width, y, width, height + Game1.tileSize / 16, Color.White * colorIntensity);
                    Utility.drawTextWithShadow(spriteBatch, message, font, new Vector2(x + Game1.tileSize / 4 - width, y + Game1.tileSize / 4), Game1.textColor);
                    break;
            }
        }
        private void GetFish(int fish)
        {
            Dictionary<int, string> dFish = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            if (dFish.ContainsKey(fish))
            {
                string[] fishArray = dFish[fish].Split('/');
                _fishName = fishArray[0];
                _dartingAmount = (float)Convert.ToInt32(fishArray[1]);
                _bobberBehavior = fishArray[2];
                /*
                switch (this.BobberBehavior)
                {
                    case "dart":
                        this.MotionType = 1;
                        break;
                    case "smooth":
                        this.MotionType = 2;
                        break;
                    case "floater":
                        this.MotionType = 4;
                        break;
                    case "sinker":
                        this.MotionType = 3;
                        break;
                    default:
                        this.MotionType = 0;
                        break;
                }*/
                _minFishSize = Convert.ToInt32(fishArray[3]);
                _maxFishSize = Convert.ToInt32(fishArray[4]);
                string[] fSpawn = fishArray[5].Split(' ');
                _spawnStart1 = Convert.ToInt32(fSpawn[0]);
                _spawnStop1 = Convert.ToInt32(fSpawn[1]);
                if(fSpawn.Length > 2)
                {
                    _spawnStart2 = Convert.ToInt32(fSpawn[2]);
                    _spawnStop2 = Convert.ToInt32(fSpawn[3]);
                }
                string[] fSeasons = fishArray[6].Split(' ');

            }
        }
    }
}
