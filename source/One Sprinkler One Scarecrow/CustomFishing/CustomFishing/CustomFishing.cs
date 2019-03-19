using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace CustomFishing
{
    public class CustomFishing : Mod
    {
        private ModConfig Config;
        //Config Settings
        private int DefaultBobberVal;
        private bool Enabled;
        private int Difficulty;
        private bool EasyFishing;
        public bool AlwaysMaxCast;
        private int BaseBobberBarHeight;
        private bool InstantFishBite;
        private bool InstantCatch;
        private bool AlwaysPerfect;
        private bool InfiniteTackle;
        private bool AlwaysSpawnTreasure;
        private bool AlwaysGetTreasure;

        //
        private bool ShowMessage;
        private string FishingMsg = "";
        private int MotionType = 0;
        
        //Fish Data
        private string fishName = "";
        private float DartingAmount;
        private string BobberBehavior;
        private int MinFishSize;
        private int MaxFishSize;
        private string[] SpawnTimes;
        private string[] SpawnSeasons;
        private string DummyString;
        private int SpawnStart1 = 0;
        private int SpawnStart2 = 0;
        private int SpawnStop1 = 0;
        private int SpawnStop2 = 0;
        private int MinWaterDepth;
        private double SpawnMultiplier;
        private double DepthMultiplier;
        private int MinFishingLevel;

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
            if (ShowMessage)
            {
                
                //DrawTextBox(5, inCave ? 100 : 5, Game1.smallFont, fishName);
                DrawTextBox(5, (Game1.viewport.Height - 200), Game1.dialogueFont, FishingMsg);
            }
        }
        private void AfterLoad(object sender, SaveLoadedEventArgs e)
        {
            Config = Helper.ReadConfig<ModConfig>();
            rePopulateConfig();
        }
        private void UpdateTick(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Enabled)
                return;
            //Everything should be good to proceed
            SFarmer Player = Game1.player;
            if (Enabled)
            {
                if (Game1.activeClickableMenu == null && Player.CurrentTool is FishingRod rod)
                {
                    if (AlwaysMaxCast)
                        rod.castingPower = 1.01F;

                    if (InstantFishBite)
                    {
                        if (rod.timeUntilFishingBite > 0)
                            rod.timeUntilFishingBite = 0;
                    }

                    if (InfiniteTackle && rod.attachments[1] != null)
                        rod.attachments[1].scale.Y = 1;
                }
                if (Game1.activeClickableMenu is BobberBar bobberMenu)
                {
                    getFish(Helper.Reflection.GetField<int>(bobberMenu, "whichFish").GetValue());
                    FishingMsg = "Hooked Fish:\n\r" + fishName;
                    ShowMessage = true;

                    if (InstantCatch)
                        Helper.Reflection.GetField<float>(bobberMenu, "distanceFromCatching").SetValue(1);

                    if (AlwaysPerfect)
                        Helper.Reflection.GetField<bool>(bobberMenu, "perfect").SetValue(true);

                    if (EasyFishing)
                    {
                       Helper.Reflection.GetField<int>(bobberMenu, "bobberBarHeight").SetValue(560);
                    }
                    else
                    {
                        DefaultBobberVal = (Game1.tileSize * 3 / 2 + Player.FishingLevel * 8) + BaseBobberBarHeight;
                        DefaultBobberVal = ((Game1.tileSize * 3 / 2 + Player.FishingLevel * 8) + BaseBobberBarHeight) >= 500 ? 499 : DefaultBobberVal;
                       Helper.Reflection.GetField<int>(bobberMenu, "bobberBarHeight").SetValue(DefaultBobberVal);
                    }

                    if (AlwaysSpawnTreasure)
                    {
                        Helper.Reflection.GetField<bool>(bobberMenu, "treasure").SetValue(true);
                        Helper.Reflection.GetField<float>(bobberMenu, "treasureScale").SetValue(1);
                    }
                    if (AlwaysGetTreasure && Helper.Reflection.GetField<bool>(bobberMenu, "treasure").GetValue())
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
                    fishName = "";
                    ShowMessage = false;
                }
            }
        }
        private void KeyPressed(object sender, ButtonPressedEventArgs e)
        {
            if(e.Button == SButton.F8)
            {
                Config = Helper.ReadConfig<ModConfig>();
                rePopulateConfig();
            }
            if(e.Button == SButton.F7)
            {
                int n1 = 1;
                if (n1 == 2)
                    return;
                int o = MotionType;
                if(o == 4)
                {
                    MotionType = 0;
                }
                else
                {
                    MotionType += 1;
                }
                Monitor.Log($"Changed MotionType to: {MotionType}", LogLevel.Alert);
            }
        }
        //Custom Voids

        private void rePopulateConfig()
        {
            Enabled = Config.Enabled;
            Difficulty = Config.Difficulty;
            EasyFishing = Config.EasyFishing;
            AlwaysMaxCast = Config.AlwaysMaxCast;
            BaseBobberBarHeight = Config.BaseBobberBarHeight;
            InstantFishBite = Config.InstantFishBite;
            InstantCatch = Config.InstantCatch;
            AlwaysPerfect = Config.AlwaysPerfect;
            InfiniteTackle = Config.InfiniteTackle;
            AlwaysSpawnTreasure = Config.AlwaysSpawnTreasure;
            AlwaysGetTreasure = Config.AlwaysGetTreasure;
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
        private void getFish(int fish)
        {
            Dictionary<int, string> dFish = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            if (dFish.ContainsKey(fish))
            {
                string[] fishArray = dFish[fish].Split('/');
                fishName = fishArray[0];
                DartingAmount = (float)Convert.ToInt32(fishArray[1]);
                BobberBehavior = fishArray[2];
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
                MinFishSize = Convert.ToInt32(fishArray[3]);
                MaxFishSize = Convert.ToInt32(fishArray[4]);
                string[] fSpawn = fishArray[5].Split(' ');
                SpawnStart1 = Convert.ToInt32(fSpawn[0]);
                SpawnStop1 = Convert.ToInt32(fSpawn[1]);
                if(fSpawn.Length > 2)
                {
                    SpawnStart2 = Convert.ToInt32(fSpawn[2]);
                    SpawnStop2 = Convert.ToInt32(fSpawn[3]);
                }
                string[] fSeasons = fishArray[6].Split(' ');

            }
        }
    }
}
