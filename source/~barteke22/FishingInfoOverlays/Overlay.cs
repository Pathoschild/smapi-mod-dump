/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace StardewMods
{
    /// <summary>The mod entry point.</summary>
    public class Overlay
    {
        ITranslationHelper translate;
        private IMonitor Monitor;
        private IModHelper Helper;
        private IManifest ModManifest;

        private bool hideText = false;    //world fish preview data 
        private Farmer who;
        private int screen;
        private int totalPlayersOnThisPC;


        private List<int> fishHere;
        private Dictionary<int, int> fishChances;
        private Dictionary<int, int> fishChancesSlow;
        private int fishChancesModulo;
        private List<int> oldGeneric;
        private Dictionary<int, int> fishFailed;
        private bool isMinigameOther = false;

        private bool isMinigame = false;    //minigame fish preview data, Reflection
        private int miniFish;
        private float miniFishPos;
        private int miniXPositionOnScreen;
        private int miniYPositionOnScreen;
        private Vector2 miniFishShake;
        private Vector2 miniEverythingShake;
        private Vector2 miniBarShake;
        private Vector2 miniTreasureShake;
        private float miniScale;
        private bool miniBobberInBar;
        private float miniBobberBarPos;
        private float miniBobberBarHeight;
        private float miniTreasurePosition;
        private float miniTreasureScale;
        private float miniTreasureCatchLevel;
        private bool miniTreasureCaught;


        public static Dictionary<string, string> locationData;
        public static Dictionary<int, string> fishData;
        public static Texture2D[] background = new Texture2D[2];
        public static Color colorBg;
        public static Color colorText;


        public static int[] miniMode = new int[4];   //config values
        public static bool[] barCrabEnabled = new bool[4];
        public static Vector2[] barPosition = new Vector2[4];
        public static int[] iconMode = new int[4];
        public static float[] barScale = new float[4];
        public static int[] maxIcons = new int[4];
        public static int[] maxIconsPerRow = new int[4];
        public static int[] backgroundMode = new int[4];
        public static int extraCheckFrequency;
        public static int[] scanRadius = new int[4];
        public static bool[] showTackles = new bool[4];
        public static bool[] showPercentages = new bool[4];
        public static int[] sortMode = new int[4];
        public static bool[] uncaughtDark = new bool[4];
        public static bool[] onlyFish = new bool[4];


        public Overlay(ModEntry entry)
        {
            this.Helper = entry.Helper;
            this.Monitor = entry.Monitor;
            this.ModManifest = entry.ModManifest;
            this.translate = entry.Helper.Translation;
        }



        public void Rendered(object sender, RenderedEventArgs e)
        {
            screen = Context.ScreenId;
            who = Game1.player;
            if (Game1.eventUp || who.CurrentItem == null ||
                !((who.CurrentItem is FishingRod) || (who.CurrentItem.Name.Equals("Crab Pot", StringComparison.Ordinal) && barCrabEnabled[screen]))) return;//code stop conditions

            totalPlayersOnThisPC = 1;
            foreach (IMultiplayerPeer peer in Helper.Multiplayer.GetConnectedPlayers())
            {
                if (peer.IsSplitScreen) totalPlayersOnThisPC++;
            }

            if (Game1.player.CurrentItem is FishingRod)  //dummy workaround for preventing player from getting special items
            {
                who = new Farmer();
                who.mailReceived.CopyFrom(Game1.player.mailReceived);
                who.mailReceived.Add("CalderaPainting");
                who.currentLocation = Game1.player.currentLocation;
                who.setTileLocation(Game1.player.getTileLocation());
                who.FishingLevel = Game1.player.FishingLevel;
                //if there's ever any downside of referencing player rod directly, use below + add bait/tackle to it
                //FishingRod rod = (FishingRod)(Game1.player.CurrentTool as FishingRod).getOne();
                //who.CurrentTool = rod;
                who.CurrentTool = Game1.player.CurrentTool;
                who.LuckLevel = Game1.player.LuckLevel;
                foreach (var item in Game1.player.fishCaught) who.fishCaught.Add(item);
                who.secretNotesSeen.CopyFrom(Game1.player.secretNotesSeen);
            }

            SpriteFont font = Game1.smallFont;                                                          //UI INIT
            Rectangle source = GameLocation.getSourceRectForObject(who.CurrentItem.ParentSheetIndex);      //for average icon size
            SpriteBatch batch = Game1.spriteBatch;

            batch.End();    //stop current UI drawing and start mode where where layers work from 0f-1f
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            //MINIGAME PREVIEW
            if (isMinigame && miniMode[screen] < 2 && miniScale == 1f)//scale == 1f when moving elements appear
            {
                if (miniMode[screen] == 0) //Full minigame
                {
                    //rod+bar textture cut to only cover the minigame bar
                    batch.Draw(Game1.mouseCursors, Utility.ModifyCoordinatesForUIScale(new Vector2(miniXPositionOnScreen + 126, miniYPositionOnScreen + 292) + miniEverythingShake),
                        new Rectangle(658, 1998, 15, 149), Color.White * miniScale, 0f, new Vector2(18.5f, 74f) * miniScale, Utility.ModifyCoordinateForUIScale(4f * miniScale), SpriteEffects.None, 0.01f);

                    //green moving bar player controls
                    batch.Draw(Game1.mouseCursors, Utility.ModifyCoordinatesForUIScale(new Vector2(miniXPositionOnScreen + 64, miniYPositionOnScreen + 12 + (int)miniBobberBarPos) + miniBarShake + miniEverythingShake),
                        new Rectangle(682, 2078, 9, 2), miniBobberInBar ? Color.White : (Color.White * 0.25f * ((float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0), 2) + 2f)), 0f, Vector2.Zero, Utility.ModifyCoordinateForUIScale(4f), SpriteEffects.None, 0.89f);
                    batch.Draw(Game1.mouseCursors, Utility.ModifyCoordinatesForUIScale(new Vector2(miniXPositionOnScreen + 64, miniYPositionOnScreen + 12 + (int)miniBobberBarPos + 8) + miniBarShake + miniEverythingShake),
                        new Rectangle(682, 2081, 9, 1), miniBobberInBar ? Color.White : (Color.White * 0.25f * ((float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0), 2) + 2f)), 0f, Vector2.Zero, Utility.ModifyCoordinatesForUIScale(new Vector2(4f, miniBobberBarHeight - 16)), SpriteEffects.None, 0.89f);
                    batch.Draw(Game1.mouseCursors, Utility.ModifyCoordinatesForUIScale(new Vector2(miniXPositionOnScreen + 64, miniYPositionOnScreen + 12 + (int)miniBobberBarPos + miniBobberBarHeight - 8) + miniBarShake + miniEverythingShake),
                        new Rectangle(682, 2085, 9, 2), miniBobberInBar ? Color.White : (Color.White * 0.25f * ((float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0), 2) + 2f)), 0f, Vector2.Zero, Utility.ModifyCoordinateForUIScale(4f), SpriteEffects.None, 0.89f);

                    //treasure
                    batch.Draw(Game1.mouseCursors, Utility.ModifyCoordinatesForUIScale(new Vector2(miniXPositionOnScreen + 64 + 18, (float)(miniYPositionOnScreen + 12 + 24) + miniTreasurePosition) + miniTreasureShake + miniEverythingShake),
                        new Rectangle(638, 1865, 20, 24), Color.White, 0f, new Vector2(10f, 10f), Utility.ModifyCoordinateForUIScale(2f * miniTreasureScale), SpriteEffects.None, 0.9f);
                    if (miniTreasureCatchLevel > 0f && !miniTreasureCaught)//treasure progress
                    {
                        batch.Draw(Game1.staminaRect, new Rectangle((int)Utility.ModifyCoordinateForUIScale(miniXPositionOnScreen + 64), (int)Utility.ModifyCoordinateForUIScale(miniYPositionOnScreen + 12 + (int)miniTreasurePosition), (int)Utility.ModifyCoordinateForUIScale(40), (int)Utility.ModifyCoordinateForUIScale(8)), null, Color.DimGray * 0.5f, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                        batch.Draw(Game1.staminaRect, new Rectangle((int)Utility.ModifyCoordinateForUIScale(miniXPositionOnScreen + 64), (int)Utility.ModifyCoordinateForUIScale(miniYPositionOnScreen + 12 + (int)miniTreasurePosition), (int)Utility.ModifyCoordinateForUIScale((miniTreasureCatchLevel * 40f)), (int)Utility.ModifyCoordinateForUIScale(8)), null, Color.Orange, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                    }
                }
                else batch.Draw(Game1.mouseCursors, Utility.ModifyCoordinatesForUIScale(new Vector2(miniXPositionOnScreen + 82, (miniYPositionOnScreen + 36) + miniFishPos) + miniFishShake + miniEverythingShake),
                    new Rectangle(614 + (FishingRod.isFishBossFish(miniFish) ? 20 : 0), 1840, 20, 20), Color.Black, 0f, new Vector2(10f, 10f),
                    Utility.ModifyCoordinateForUIScale(2.05f), SpriteEffects.None, 0.9f);//Simple minigame shadow fish

                //fish
                source = GameLocation.getSourceRectForObject(miniFish);
                batch.Draw(Game1.objectSpriteSheet, Utility.ModifyCoordinatesForUIScale(new Vector2(miniXPositionOnScreen + 82, (miniYPositionOnScreen + 36) + miniFishPos) + miniFishShake + miniEverythingShake),
                    source, (!uncaughtDark[screen] || who.fishCaught.ContainsKey(miniFish)) ? Color.White : Color.DarkSlateGray, 0f, new Vector2(9.5f, 9f),
                    Utility.ModifyCoordinateForUIScale(3f), SpriteEffects.FlipHorizontally, 1f);
            }



            if (iconMode[screen] != 3)
            {
                float iconScale = Game1.pixelZoom / 2f * barScale[screen];
                int iconCount = 0;
                float boxWidth = 0;
                float boxHeight = 0;
                Vector2 boxTopLeft = barPosition[screen];
                Vector2 boxBottomLeft = barPosition[screen];


                //this.Monitor.Log("\n", LogLevel.Debug);
                if (who.currentLocation is MineShaft && who.CurrentItem.Name.Equals("Crab Pot", StringComparison.Ordinal))//crab pot
                {
                    string warning = translate.Get("Bar.CrabMineWarning");
                    DrawStringWithBorder(batch, font, warning, boxBottomLeft + new Vector2(source.Width * iconScale, 0), Color.Red, 0f, Vector2.Zero, 1f * barScale[screen], SpriteEffects.None, 1f, colorBg); //text
                    batch.End();
                    batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
                    return;
                }


                if (showTackles[screen] && who.CurrentItem is FishingRod)    //BAIT AND TACKLE (BOBBERS) PREVIEW
                {
                    int bait = (who.CurrentItem as FishingRod).getBaitAttachmentIndex();
                    int tackle = (who.CurrentItem as FishingRod).getBobberAttachmentIndex();
                    if (bait > -1)
                    {
                        source = GameLocation.getSourceRectForObject(bait);
                        if (backgroundMode[screen] == 0) AddBackground(batch, boxTopLeft, boxBottomLeft, iconCount, source, iconScale, boxWidth, boxHeight);

                        int baitCount = (who.CurrentItem as FishingRod).attachments[0].Stack;
                        batch.Draw(Game1.objectSpriteSheet, boxBottomLeft, source, Color.White, 0f, Vector2.Zero, 1.9f * barScale[screen], SpriteEffects.None, 0.9f);

                        if ((who.CurrentItem as FishingRod).attachments[0].Quality == 4) batch.Draw(Game1.mouseCursors, boxBottomLeft + (new Vector2(13f, (showPercentages[screen] ? 24 : 16)) * barScale[screen]),
                            new Rectangle(346, 392, 8, 8), Color.White, 0f, Vector2.Zero, 1.9f * barScale[screen], SpriteEffects.None, 1f);
                        else Utility.drawTinyDigits(baitCount, batch, boxBottomLeft + new Vector2((source.Width * iconScale) - Utility.getWidthOfTinyDigitString(baitCount, 2f * barScale[screen]),
                            (showPercentages[screen] ? 26 : 19) * barScale[screen]), 2f * barScale[screen], 1f, colorText);

                        if (iconMode[screen] == 1) boxBottomLeft += new Vector2(0, (source.Width * iconScale) + (showPercentages[screen] ? 10 * barScale[screen] : 0));
                        else boxBottomLeft += new Vector2(source.Width * iconScale, 0);
                        iconCount++;
                    }
                    if (tackle > -1)
                    {
                        source = GameLocation.getSourceRectForObject(tackle);
                        if (backgroundMode[screen] == 0) AddBackground(batch, boxTopLeft, boxBottomLeft, iconCount, source, iconScale, boxWidth, boxHeight);

                        int tackleCount = FishingRod.maxTackleUses - (who.CurrentItem as FishingRod).attachments[1].uses.Value;
                        batch.Draw(Game1.objectSpriteSheet, boxBottomLeft, source, Color.White, 0f, Vector2.Zero, 1.9f * barScale[screen], SpriteEffects.None, 0.9f);

                        if ((who.CurrentItem as FishingRod).attachments[1].Quality == 4) batch.Draw(Game1.mouseCursors, boxBottomLeft + (new Vector2(13f, (showPercentages[screen] ? 24 : 16)) * barScale[screen]),
                            new Rectangle(346, 392, 8, 8), Color.White, 0f, Vector2.Zero, 1.9f * barScale[screen], SpriteEffects.None, 1f);
                        else Utility.drawTinyDigits(tackleCount, batch, boxBottomLeft + new Vector2((source.Width * iconScale) - Utility.getWidthOfTinyDigitString(tackleCount, 2f * barScale[screen]),
                            (showPercentages[screen] ? 26 : 19) * barScale[screen]), 2f * barScale[screen], 1f, colorText);

                        if (iconMode[screen] == 1) boxBottomLeft += new Vector2(0, (source.Width * iconScale) + (showPercentages[screen] ? 10 * barScale[screen] : 0));
                        else boxBottomLeft += new Vector2(source.Width * iconScale, 0);
                        iconCount++;
                    }
                    if (iconMode[screen] == 2 && (bait + tackle) > -1)
                    {
                        boxBottomLeft = boxTopLeft + new Vector2(0, (source.Width * iconScale) + (showPercentages[screen] ? 10 * barScale[screen] : 0));
                        boxWidth = (iconCount * source.Width * iconScale) + boxTopLeft.X;
                        boxHeight += (source.Width * iconScale) + (showPercentages[screen] ? 10 * barScale[screen] : 0);
                        if (bait > 0 && tackle > 0) iconCount--;
                    }
                }



                bool foundWater = false;
                Vector2 nearestWaterTile = new Vector2(99999f, 99999f);      //any water nearby + nearest water tile check
                if (who.currentLocation.canFishHere())
                {
                    Vector2 scanTopLeft = who.getTileLocation() - new Vector2(scanRadius[screen] + 1);
                    Vector2 scanBottomRight = who.getTileLocation() + new Vector2(scanRadius[screen] + 2);
                    for (int x = (int)scanTopLeft.X; x < (int)scanBottomRight.X; x++)
                    {
                        for (int y = (int)scanTopLeft.Y; y < (int)scanBottomRight.Y; y++)
                        {
                            if (who.currentLocation.isTileFishable(x, y) && !who.currentLocation.isTileBuildingFishable(x, y))
                            {
                                Vector2 tile = new Vector2(x, y);
                                float distance = Vector2.DistanceSquared(who.getTileLocation(), tile);
                                float distanceNearest = Vector2.DistanceSquared(who.getTileLocation(), nearestWaterTile);
                                if (distance < distanceNearest || (distance == distanceNearest && Game1.player.GetGrabTile() == tile)) nearestWaterTile = tile;
                                foundWater = true;
                            }
                        }
                    }
                }

                if (foundWater)
                {
                    if (who.CurrentItem is FishingRod) who.setTileLocation(nearestWaterTile);
                    string locationName = who.currentLocation.Name;    //LOCATION FISH PREVIEW
                    if (who.CurrentItem is FishingRod)
                    {
                        if (!isMinigame)
                        {
                            if (oldGeneric == null)
                            {
                                oldGeneric = new List<int>();
                                fishFailed = new Dictionary<int, int>();
                                fishHere = new List<int> { 168 };
                                fishChances = new Dictionary<int, int> { { -1, 0 }, { 168, 0 } };
                                fishChancesSlow = new Dictionary<int, int>();
                                fishChancesModulo = 1;
                            }
                            AddGenericFishToList(locationName);
                        }
                    }
                    else AddCrabPotFish();
                    //for (int i = 0; i < 20; i++)    //TEST ITEM INSERT
                    //{
                    //    fishHere.Add(100 + i);
                    //    fishChances.Add(100 + i, 1000);
                    //}

                    foreach (var fish in fishHere)
                    {
                        if (onlyFish[screen] && fish != 168 && !fishData.ContainsKey(fish)) continue;//skip if not fish, except trash

                        int percent = fishChancesSlow.ContainsKey(fish) ? (int)Math.Round((float)fishChancesSlow[fish] / fishChancesSlow[-1] * 100f) : 0; //chance of this fish

                        if (iconCount < maxIcons[screen] && percent > 0)
                        {
                            bool caught = (!uncaughtDark[screen] || who.fishCaught.ContainsKey(fish));
                            if (fish == 168) caught = true;

                            iconCount++;
                            string fishNameLocalized = "???";

                            if (fish > 900000)//Hat (workaround)
                            {
                                if (caught) fishNameLocalized = new Hat(fish - 900000).DisplayName;

                                batch.Draw(FarmerRenderer.hatsTexture, boxBottomLeft, new Rectangle((int)(fish - 900000) * 20 % FarmerRenderer.hatsTexture.Width, (int)(fish - 900000) * 20 / FarmerRenderer.hatsTexture.Width * 20 * 4, 20, 20),
                                    Color.White, 0f, Vector2.Zero, 1.5f * barScale[screen], SpriteEffects.None, 0.98f);//icon
                            }
                            else if (new Object(fish, 1).Name.Equals("Error Item", StringComparison.Ordinal))  //Furniture
                            {
                                if (caught) fishNameLocalized = new Furniture(fish, Vector2.Zero).DisplayName;

                                batch.Draw(Furniture.furnitureTexture, boxBottomLeft, new Furniture(fish, Vector2.Zero).defaultSourceRect.Value,
                                    (caught) ? Color.White : Color.DarkSlateGray, 0f, Vector2.Zero, 0.95f * barScale[screen], SpriteEffects.None, 0.98f);//icon
                            }
                            else                                                                                        //Item
                            {
                                if (caught) fishNameLocalized = new Object(fish, 1).DisplayName;

                                source = GameLocation.getSourceRectForObject(fish);
                                if (fish == 168) batch.Draw(Game1.objectSpriteSheet, boxBottomLeft + new Vector2(2 * barScale[screen], -5 * barScale[screen]), source, (caught) ? Color.White : Color.DarkSlateGray,
                                    0f, Vector2.Zero, 1.9f * barScale[screen], SpriteEffects.None, 0.98f);//icon trash
                                else batch.Draw(Game1.objectSpriteSheet, boxBottomLeft, source, (caught) ? Color.White : Color.DarkSlateGray,
                                    0f, Vector2.Zero, 1.9f * barScale[screen], SpriteEffects.None, 0.98f);//icon
                            }

                            if (showPercentages[screen])
                            {
                                DrawStringWithBorder(batch, font, percent + "%", boxBottomLeft + new Vector2((source.Width * iconScale / 2f), 27f * barScale[screen]),
                                    (caught) ? colorText : colorText * 0.8f, 0f, new Vector2(font.MeasureString(percent + "%").X / 2f, 0f), 0.58f * barScale[screen], SpriteEffects.None, 1f, colorBg);//%
                            }

                            if (fish == miniFish && miniMode[screen] < 3) batch.Draw(background[0], new Rectangle((int)boxBottomLeft.X - 1, (int)boxBottomLeft.Y - 1, (int)(source.Width * iconScale) + 1, (int)((source.Width * iconScale) + (showPercentages[screen] ? 10 * barScale[screen] : 0) + 1)),
                                null, Color.GreenYellow, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);//minigame outline

                            if (backgroundMode[screen] == 0) AddBackground(batch, boxTopLeft, boxBottomLeft, iconCount, source, iconScale, boxWidth, boxHeight);


                            if (iconMode[screen] == 0)      //Horizontal Preview
                            {
                                if (iconCount % maxIconsPerRow[screen] == 0) boxBottomLeft = new Vector2(boxTopLeft.X, boxBottomLeft.Y + (source.Width * iconScale) + (showPercentages[screen] ? 10 * barScale[screen] : 0)); //row switch
                                else boxBottomLeft += new Vector2(source.Width * iconScale, 0);
                            }
                            else                    //Vertical Preview
                            {
                                if (iconMode[screen] == 2 && !hideText)  // + text
                                {
                                    DrawStringWithBorder(batch, font, fishNameLocalized, boxBottomLeft + new Vector2(source.Width * iconScale, 0), (caught) ? colorText : colorText * 0.8f, 0f, new Vector2(0, -3), 1f * barScale[screen], SpriteEffects.None, 0.98f, colorBg); //text
                                    boxWidth = Math.Max(boxWidth, boxBottomLeft.X + (font.MeasureString(fishNameLocalized).X * barScale[screen]) + (source.Width * iconScale));
                                }

                                if (iconCount % maxIconsPerRow[screen] == 0) //row switch
                                {
                                    if (iconMode[screen] == 2) boxBottomLeft = new Vector2(boxWidth + (20 * barScale[screen]), boxTopLeft.Y);
                                    else boxBottomLeft = new Vector2(boxBottomLeft.X + (source.Width * iconScale), boxTopLeft.Y);
                                }
                                else boxBottomLeft += new Vector2(0, (source.Width * iconScale) + (showPercentages[screen] ? 10 * barScale[screen] : 0));
                                if (iconMode[screen] == 2 && iconCount <= maxIconsPerRow[screen]) boxHeight += (source.Width * iconScale) + (showPercentages[screen] ? 10 * barScale[screen] : 0);
                            }
                        }
                    }
                    if (backgroundMode[screen] == 1) AddBackground(batch, boxTopLeft, boxBottomLeft, iconCount, source, iconScale, boxWidth, boxHeight);
                }
                else if (backgroundMode[screen] == 1) AddBackground(batch, boxTopLeft, boxBottomLeft, iconCount, source, iconScale, boxWidth, boxHeight);
            }

            batch.End();
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
        }


        public void OnMenuChanged(object sender, MenuChangedEventArgs e)   //Minigame data
        {
            if (e.NewMenu is BobberBar) isMinigame = true;
            else
            {
                isMinigame = false;
                if (e.OldMenu is BobberBar) miniFish = -1;
            }
        }
        public void OnRenderMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (Game1.activeClickableMenu is BobberBar bar  && isMinigame)
            {
                miniFish = Helper.Reflection.GetField<int>(bar, "whichFish").GetValue();
                if (miniMode[screen] < 2)
                {
                    miniScale = Helper.Reflection.GetField<float>(bar, "scale").GetValue();
                    miniFishPos = Helper.Reflection.GetField<Single>(bar, "bobberPosition").GetValue();
                    miniXPositionOnScreen = Helper.Reflection.GetField<int>(bar, "xPositionOnScreen").GetValue();
                    miniYPositionOnScreen = Helper.Reflection.GetField<int>(bar, "yPositionOnScreen").GetValue();
                    miniFishShake = Helper.Reflection.GetField<Vector2>(bar, "fishShake").GetValue();
                    miniEverythingShake = Helper.Reflection.GetField<Vector2>(bar, "everythingShake").GetValue();
                }
                if (miniMode[screen] == 0)
                {
                    miniBarShake = Helper.Reflection.GetField<Vector2>(bar, "barShake").GetValue();
                    miniTreasureShake = Helper.Reflection.GetField<Vector2>(bar, "treasureShake").GetValue();
                    miniBobberInBar = Helper.Reflection.GetField<bool>(bar, "bobberInBar").GetValue();
                    miniBobberBarPos = Helper.Reflection.GetField<float>(bar, "bobberBarPos").GetValue();
                    miniBobberBarHeight = Helper.Reflection.GetField<int>(bar, "bobberBarHeight").GetValue();
                    miniTreasurePosition = Helper.Reflection.GetField<float>(bar, "treasurePosition").GetValue();
                    miniTreasureScale = Helper.Reflection.GetField<float>(bar, "treasureScale").GetValue();
                    miniTreasureCatchLevel = Helper.Reflection.GetField<float>(bar, "treasureCatchLevel").GetValue();
                    miniTreasureCaught = Helper.Reflection.GetField<bool>(bar, "treasureCaught").GetValue();
                }
            }
        }
        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == "barteke22.FishingMinigames")
            {
                if (e.Type == "whichFish")
                {
                    miniFish = e.ReadAs<int>();
                    if (miniFish == -1) isMinigameOther = false;
                    else isMinigameOther = true;
                }
                if (e.Type == "hideText") hideText = e.ReadAs<bool>();
            }
        }

        private void AddGenericFishToList(string locationName)         //From GameLocation.cs getFish()
        {
            List<int> tempFish = new List<int>();
            bool magicBait = who.currentLocation.IsUsingMagicBait(who);
            if (locationName.Equals("BeachNightMarket", StringComparison.Ordinal)) locationName = "Beach";

            if (locationData.ContainsKey(locationName))
            {
                string[] rawFishData;
                if (!magicBait) rawFishData = locationData[locationName].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' '); //fish by season
                else
                {
                    List<string> all_season_fish = new List<string>(); //magic bait = all fish
                    for (int k = 0; k < 4; k++)
                    {
                        if (locationData[locationName].Split('/')[4 + k].Split(' ').Length > 1) all_season_fish.AddRange(locationData[locationName].Split('/')[4 + k].Split(' '));
                    }
                    rawFishData = all_season_fish.ToArray();
                }

                Dictionary<string, string> rawFishDataWithLocation = new Dictionary<string, string>();

                if (rawFishData.Length > 1) for (int j = 0; j < rawFishData.Length; j += 2) rawFishDataWithLocation[rawFishData[j]] = rawFishData[j + 1];

                string[] keys = rawFishDataWithLocation.Keys.ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    bool fail = true;
                    int key = Convert.ToInt32(keys[i]);
                    string[] specificFishData = fishData[key].Split('/');
                    string[] timeSpans = specificFishData[5].Split(' ');
                    int location = Convert.ToInt32(rawFishDataWithLocation[keys[i]]);
                    if (location == -1 || who.currentLocation.getFishingLocation(who.getTileLocation()) == location)
                    {
                        for (int l = 0; l < timeSpans.Length; l += 2)
                        {
                            if (Game1.timeOfDay >= Convert.ToInt32(timeSpans[l]) && Game1.timeOfDay < Convert.ToInt32(timeSpans[l + 1]))
                            {
                                fail = false;
                                break;
                            }
                        }
                    }
                    if (!specificFishData[7].Equals("both", StringComparison.Ordinal))
                    {
                        if (specificFishData[7].Equals("rainy", StringComparison.Ordinal) && !Game1.IsRainingHere(who.currentLocation)) fail = true;
                        else if (specificFishData[7].Equals("sunny", StringComparison.Ordinal) && Game1.IsRainingHere(who.currentLocation)) fail = true;
                    }
                    if (magicBait) fail = false; //I guess magic bait check comes at this exact point because it overrides all conditions except rod and level?

                    bool beginnersRod = who != null && who.CurrentItem != null && who.CurrentItem is FishingRod && (int)who.CurrentTool.UpgradeLevel == 1;

                    if (Convert.ToInt32(specificFishData[1]) >= 50 && beginnersRod) fail = true;
                    if (who.FishingLevel < Convert.ToInt32(specificFishData[12])) fail = true;
                    if (!fail && !tempFish.Contains(key))
                    {
                        tempFish.Add(key);
                    }
                }
                if ((tempFish.Count == 0 && oldGeneric.Count != 0) || tempFish.Count > 0 && (!(new HashSet<int>(oldGeneric).SetEquals(tempFish))))//reset lists if generic list changed
                {
                    oldGeneric = tempFish.ToList();
                    fishFailed = new Dictionary<int, int>();
                    fishHere = new List<int> { 168 };
                    fishChances = new Dictionary<int, int> { { -1, 0 }, { 168, 0 } };
                    fishChancesSlow = new Dictionary<int, int>();
                    fishChancesModulo = 1;

                    foreach (var key in oldGeneric)
                    {
                        if (sortMode[screen] == 0) SortItemIntoListByDisplayName(key);
                        else fishHere.Add(key);

                        if (!fishChances.ContainsKey(key)) fishChances.Add(key, 0);
                    }
                }
            }
            AddFishToListDynamic();
        }
        private void AddFishToListDynamic()                            //very performance intensive check for fish fish available in this area - simulates fishing
        {
            int freq = (isMinigame || isMinigameOther) ? 6 / totalPlayersOnThisPC : extraCheckFrequency / totalPlayersOnThisPC; //minigame lowers frequency
            for (int i = 0; i < freq; i++)
            {
                Game1.stats.TimesFished++;
                int fish = AddHardcoded();
                Game1.stats.TimesFished--;
                if (fish != -2)//not fully hardcoded
                {
                    if (fish == -1)//dynamic
                    {
                        int nuts = 5;                                                                           //"fix" for preventing player from not getting specials       ----start1
                        bool mail1 = false;
                        bool mail2 = false;
                        bool caughtIridiumKrobus = Game1.player.mailReceived.Contains("caughtIridiumKrobus");
                        if (who.currentLocation is IslandLocation)
                        {
                            nuts = (Game1.player.team.limitedNutDrops.ContainsKey("IslandFishing")) ? Game1.player.team.limitedNutDrops["IslandFishing"] : 0;
                            if (nuts < 5) Game1.player.team.limitedNutDrops["IslandFishing"] = 5;
                            mail1 = Game1.player.mailReceived.Contains("islandNorthCaveOpened");
                            mail2 = Game1.player.mailForTomorrow.Contains("islandNorthCaveOpened");
                            if (mail1) Game1.player.mailReceived.Remove("islandNorthCaveOpened");
                            if (mail2) Game1.player.mailForTomorrow.Remove("islandNorthCaveOpened");                                                                         //-----end1
                        }

                        Game1.stats.TimesFished++;
                        item = who.currentLocation.getFish(0, 1, 5, who, 100, who.getTileLocation(), who.currentLocation.Name);
                        Game1.stats.TimesFished--;
                        try
                        {
                            if (item.DisplayName.Equals("Error Item")) 
                            {
                                Monitor.LogOnce("Skipped Object of type" + item.GetType() + ", ID: " + item.ParentSheetIndex + ", CodeName: " + item.Name + ", Catefory: " + item.Category + ". DisplayName is \"Error Item\".", LogLevel.Error);
                                continue;
                            }
                            fish = item.ParentSheetIndex;
                        }
                        catch (Exception)
                        {
                            Monitor.LogOnce("Skipped Object of type" + item.GetType() + ", ID: " + item.ParentSheetIndex + ", CodeName: " + item.Name + ", Catefory: " + item.Category + ". Missing DisplayName.", LogLevel.Error);
                            continue;
                        }

                        if (who.currentLocation is IslandLocation)
                        {
                            if (!caughtIridiumKrobus && Game1.player.mailReceived.Contains("caughtIridiumKrobus")) Game1.player.mailReceived.Remove("caughtIridiumKrobus");//"fix"----start2
                            if (nuts < 5) Game1.player.team.limitedNutDrops["IslandFishing"] = nuts;
                            if (mail1) Game1.player.mailReceived.Add("islandNorthCaveOpened");
                            if (mail2) Game1.player.mailForTomorrow.Add("islandNorthCaveOpened");                                                                                //-----end2
                        }
                    }
                    int val;
                    if (fishChances[-1] < int.MaxValue) //percentages, slow version (the one shown) is updated less over time
                    {
                        if (fish >= 167 && fish <= 172)
                        {
                            fishChances.TryGetValue(168, out val);
                            fishChances[168] = val + 1;
                        }
                        else if (!fishHere.Contains(fish))
                        {
                            fishChances = new Dictionary<int, int> { { -1, 0 } };//reset % on new fish added
                            foreach (var f in fishHere) fishChances.Add(f, 1);
                            fishChancesSlow = new Dictionary<int, int>();
                            fishChancesModulo = 1;

                            if (sortMode[screen] == 0) SortItemIntoListByDisplayName(fish); //sort by name
                            else fishHere.Add(fish);
                            fishChances.Add(fish, 1);
                        }
                        else
                        {
                            fishChances.TryGetValue(fish, out val);
                            fishChances[fish] = val + 1;
                        }
                    }
                    fishChances.TryGetValue(-1, out val);
                    fishChances[-1] = val + 1;
                    if (fishChances[-1] % fishChancesModulo == 0)
                    {
                        if (fishChancesModulo < 10000) fishChancesModulo *= 10;
                        fishChancesSlow = fishChances.ToDictionary(entry => entry.Key, entry => entry.Value);
                    }
                    if (sortMode[screen] == 1) SortListByPercentages(); //sort by %



                    //if fish not in last X attempts, redo lists
                    if ((fish < 167 || fish > 172))
                    {
                        fishChances.TryGetValue(fish, out val);
                        float chance = (float)val / fishChances[-1] * 100f;
                        if (chance < 0.5f) fishFailed[fish] = 5000;
                        else if (chance < 1f) fishFailed[fish] = 3500;
                        else if (chance < 2f) fishFailed[fish] = 3000;
                        else if (chance < 3f) fishFailed[fish] = 2500;
                        else if (chance < 4f) fishFailed[fish] = 1500;
                        else fishFailed[fish] = 1000;
                    }
                }
                foreach (var key in fishFailed.Keys.ToList())
                {
                    fishFailed[key]--;
                    if (fishFailed[key] < 1) oldGeneric = null;
                }
            }
        }

        private int AddHardcoded()//-2 skip dynamic, -1 dynamic, above -1 = item to add to dynamic
        {
            if (who.currentLocation is Caldera)
            {
                if (Game1.random.NextDouble() < 0.05 && !Game1.player.mailReceived.Contains("CalderaPainting")) return 2732;//physics 101
                return -1;
            }
            if (who.currentLocation is Forest)
            {
                if (who.getTileY() > 108f && !Game1.player.mailReceived.Contains("caughtIridiumKrobus")) return 2396;//iridium krobus
                return -1;
            }
            if (who.currentLocation is IslandLocation)
            {
                if (new Random((int)(Game1.stats.DaysPlayed + Game1.stats.TimesFished + Game1.uniqueIDForThisGame)).NextDouble() < 0.15 && (!Game1.player.team.limitedNutDrops.ContainsKey("IslandFishing") || Game1.player.team.limitedNutDrops["IslandFishing"] < 5)) return 73;

                if (who.currentLocation is IslandFarmCave)
                {
                    if (Game1.random.NextDouble() < 0.1) return 900078;//frog hat + 900000
                    else if (who.currentLocation.HasUnlockedAreaSecretNotes(Game1.player) && Game1.random.NextDouble() < (0.08) && who.currentLocation.tryToCreateUnseenSecretNote(Game1.player) != null) return 842;//journal
                    else return 168;
                }

                if (who.currentLocation is IslandNorth)
                {
                    if ((bool)(Game1.getLocationFromName("IslandNorth") as IslandNorth).bridgeFixed.Value &&
                        (new Random(who.getTileX() * 2000 + who.getTileY() * 777 + (int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)Game1.stats.TimesFished)).NextDouble() < 0.1) return 821;
                    return -1;
                }

                if (who.currentLocation is IslandSouthEast && who.getTileLocation().X >= 17 && who.getTileLocation().X <= 21 && who.getTileLocation().Y >= 19 && who.getTileLocation().Y <= 23)
                {
                    if (!(Game1.player.currentLocation as IslandSouthEast).fishedWalnut.Value)
                    {
                        fishHere = new List<int>() { 73 };
                        fishChancesSlow = new Dictionary<int, int>() { { -1, 1 }, { 73, 1 }, { 168, 0 } };
                    }
                    else
                    {
                        fishHere = new List<int>() { 168 };
                        fishChancesSlow = new Dictionary<int, int>() { { -1, 1 }, { 168, 1 } };
                    }
                    oldGeneric = null;
                    return -2;
                }

                if (who.currentLocation is IslandWest)
                {
                    if (Game1.player.hasOrWillReceiveMail("islandNorthCaveOpened") &&
                        (new Random(who.getTileX() * 2000 + who.getTileY() * 777 + (int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)Game1.stats.TimesFished)).NextDouble() < 0.1) return 825;
                    return -1;
                }
            }
            if (who.currentLocation is Railroad)
            {
                if (Game1.currentSeason.Equals("winter")) return -2;
                else if (Game1.player.secretNotesSeen.Contains(GameLocation.NECKLACE_SECRET_NOTE_INDEX) && !Game1.player.hasOrWillReceiveMail(GameLocation.CAROLINES_NECKLACE_MAIL)) return GameLocation.CAROLINES_NECKLACE_ITEM;
                else if (!who.mailReceived.Contains("gotSpaFishing")) return 2423;
                else if (Game1.random.NextDouble() < 0.08) return 2423;
                else return 168;
            }
            return -1;
        }

        private void AddCrabPotFish()
        {
            fishHere = new List<int>();
            bool isMariner = who.professions.Contains(10);
            if (!isMariner) fishHere.Add(168);//trash
            fishChancesSlow = new Dictionary<int, int>();

            bool ocean = who.currentLocation is Beach || who.currentLocation.catchOceanCrabPotFishFromThisSpot((int)who.getTileLocation().X, (int)who.getTileLocation().Y);
            float failChance = (isMariner ? 1f : 0.8f - (float)who.currentLocation.getExtraTrashChanceForCrabPot((int)who.getTileLocation().X, (int)who.getTileLocation().Y));

            foreach (var fish in fishData)
            {
                if (!fish.Value.Contains("trap")) continue;

                string[] rawSplit = fish.Value.Split('/');
                if ((rawSplit[4].Equals("ocean", StringComparison.Ordinal) && ocean) || (rawSplit[4].Equals("freshwater", StringComparison.Ordinal) && !ocean))
                {
                    if (!fishHere.Contains(fish.Key))
                    {
                        if (sortMode[screen] == 0) SortItemIntoListByDisplayName(fish.Key);
                        else fishHere.Add(fish.Key);

                        if (showPercentages[screen] || sortMode[screen] == 1)
                        {
                            float rawChance = float.Parse(rawSplit[2]);
                            fishChancesSlow.Add(fish.Key, (int)Math.Round(rawChance * failChance * 100f));
                            failChance *= (1f - rawChance);
                        }
                    }
                }
            }
            if (isMariner) fishChancesSlow.Add(-1, fishChancesSlow.Sum(x => x.Value));
            else
            {
                fishChancesSlow.Add(168, 100 - fishChancesSlow.Sum(x => x.Value));
                fishChancesSlow.Add(-1, 100);
            }
            if (sortMode[screen] == 1) SortListByPercentages();
        }

        private Object item;
        private void SortItemIntoListByDisplayName(int itemId)
        {
            string name = (itemId > 900000) ? new Hat(itemId - 900000).DisplayName : (new Object(itemId, 1).Name.Equals("Error Item", StringComparison.Ordinal)) ? new Furniture(itemId, Vector2.Zero).DisplayName : new Object(itemId, 1).DisplayName;
            for (int j = 0; j < fishHere.Count; j++)
            {
                string name2 = (fishHere[j] > 900000) ? new Hat(fishHere[j] - 900000).DisplayName : (new Object(fishHere[j], 1).Name.Equals("Error Item", StringComparison.Ordinal)) ? new Furniture(fishHere[j], Vector2.Zero).DisplayName : new Object(fishHere[j], 1).DisplayName;

                if (string.Compare(name, name2, StringComparison.CurrentCulture) <= 0)
                {
                    fishHere.Insert(j, itemId);
                    return;
                }
            }
            fishHere.Add(itemId);
        }

        private void SortListByPercentages()
        {
            int index = 0;
            foreach (var item in fishChancesSlow.OrderByDescending(d => d.Value).ToList())
            {
                if (fishHere.Contains(item.Key))
                {
                    fishHere.Remove(item.Key);
                    fishHere.Insert(index, item.Key);
                    index++;
                }
            }
        }


        /// <summary>Makes text a tiny bit bolder and adds a border behind it. The border uses text colour's alpha for its aplha value. 6 DrawString operations, so 6x less efficient.</summary>
        private void DrawStringWithBorder(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, Color? borderColor = null)
        {
            Color border = borderColor.HasValue ? borderColor.Value : Color.Black;
            border.A = color.A;
            batch.DrawString(font, text, position + new Vector2(-1.2f * scale, -1.2f * scale), border, rotation, origin, scale, effects, layerDepth - 0.00001f);
            batch.DrawString(font, text, position + new Vector2(1.2f * scale, -1.2f * scale), border, rotation, origin, scale, effects, layerDepth - 0.00001f);
            batch.DrawString(font, text, position + new Vector2(-1.2f * scale, 1.2f * scale), border, rotation, origin, scale, effects, layerDepth - 0.00001f);
            batch.DrawString(font, text, position + new Vector2(1.2f * scale, 1.2f * scale), border, rotation, origin, scale, effects, layerDepth - 0.00001f);

            batch.DrawString(font, text, position + new Vector2(-0.2f * scale, -0.2f * scale), color, rotation, origin, scale, effects, layerDepth);
            batch.DrawString(font, text, position + new Vector2(0.2f * scale, 0.2f * scale), color, rotation, origin, scale, effects, layerDepth);
        }
        private void AddBackground(SpriteBatch batch, Vector2 boxTopLeft, Vector2 boxBottomLeft, int iconCount, Rectangle source, float iconScale, float boxWidth, float boxHeight)
        {
            if (backgroundMode[screen] == 0)
            {
                batch.Draw(background[backgroundMode[screen]], new Rectangle((int)boxBottomLeft.X - 1, (int)boxBottomLeft.Y - 1, (int)(source.Width * iconScale) + 1, (int)((source.Width * iconScale) + 1 + (showPercentages[screen] ? 10 * barScale[screen] : 0))),
                    null, colorBg, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            }
            else if (backgroundMode[screen] == 1)
            {
                if (iconMode[screen] == 0) batch.Draw(background[backgroundMode[screen]], new Rectangle((int)boxTopLeft.X - 2, (int)boxTopLeft.Y - 2, (int)(source.Width * iconScale * Math.Min(iconCount, maxIconsPerRow[screen])) + 5,
               (int)(((source.Width * iconScale) + (showPercentages[screen] ? 10 * barScale[screen] : 0)) * Math.Ceiling(iconCount / (maxIconsPerRow[screen] * 1.0))) + 5), null, colorBg, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                else if (iconMode[screen] == 1) batch.Draw(background[backgroundMode[screen]], new Rectangle((int)boxTopLeft.X - 2, (int)boxTopLeft.Y - 2, (int)(source.Width * iconScale * Math.Ceiling(iconCount / (maxIconsPerRow[screen] * 1.0))) + 5,
                    (int)(((source.Width * iconScale) + (showPercentages[screen] ? 10 * barScale[screen] : 0)) * Math.Min(iconCount, maxIconsPerRow[screen])) + 5), null, colorBg, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                else if (iconMode[screen] == 2) batch.Draw(background[backgroundMode[screen]], new Rectangle((int)boxTopLeft.X - 2, (int)boxTopLeft.Y - 2, (int)(boxWidth - boxTopLeft.X + 6), (int)boxHeight + 4),
                    null, colorBg, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            }
        }
    }
}
