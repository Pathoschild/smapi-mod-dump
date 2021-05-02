/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using xTile.Dimensions;
using xTile.Tiles;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Swim
{
    public class SwimHelperEvents
    {

        private static IMonitor Monitor;
        private static ModConfig Config;
        private static IModHelper Helper;

        public static bool isJumping = false;
        public static Vector2 startJumpLoc;
        public static Vector2 endJumpLoc;
        public static ulong lastJump = 0;
        public static ulong lastProjectile = 0;
        public static int abigailTicks;
        public static SoundEffect breatheEffect = null;
        public static int ticksUnderwater = 0;
        public static int ticksWearingScubaGear = 0;
        public static int bubbleOffset = 0;
        private static int lastBreatheSound; 
        public static SButton[] abigailShootButtons = new SButton[] {
            SButton.Left,
            SButton.Right,
            SButton.Up,
            SButton.Down
        };
        internal static Texture2D bubbleTexture;
        private static bool surfacing;

        public static void Initialize(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
            Helper = helper;
        }

        public static void Player_Warped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation.Name == "ScubaAbigailCave")
            {
                abigailTicks = 0;
                e.NewLocation.characters.Clear();


                Game1.player.changeOutOfSwimSuit();
                if(Game1.player.hat.Value != null && Game1.player.hat.Value.parentSheetIndex != 0)
                    Game1.player.addItemToInventory(Game1.player.hat.Value);
                Game1.player.hat.Value = new Hat(0);
                Game1.player.doEmote(9);
            }
            if (Game1.player.swimming)
            {
                //SwimMaps.SwitchToWaterTiles(e.NewLocation);
            }
        }

        public static void Player_InventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (e.Player != Game1.player)
                return;

            if (!Game1.player.mailReceived.Contains("ScubaTank") && ModEntry.scubaTankID != -1 && e.Added != null && e.Added.Count() > 0 && e.Added.FirstOrDefault() != null && e.Added.FirstOrDefault().parentSheetIndex != null && e.Added.FirstOrDefault().GetType() == typeof(Clothing) && e.Added.FirstOrDefault().parentSheetIndex == ModEntry.scubaTankID)
            {
                Monitor.Log("Player found scuba tank");
                Game1.player.mailReceived.Add("ScubaTank");
            }
            if (!Game1.player.mailReceived.Contains("ScubaMask") && ModEntry.scubaMaskID != -1 && e.Added != null && e.Added.Count() > 0 && e.Added.FirstOrDefault() != null && e.Added.FirstOrDefault().parentSheetIndex != null && e.Added.FirstOrDefault().GetType() == typeof(Hat) && (e.Added.FirstOrDefault() as Hat).which == ModEntry.scubaMaskID)
            {
                Monitor.Log("Player found scuba mask");
                Game1.player.mailReceived.Add("ScubaMask");
            }
            if (!Game1.player.mailReceived.Contains("ScubaFins") && ModEntry.scubaFinsID != -1 && e.Added != null && e.Added.Count() > 0 && e.Added.FirstOrDefault() != null && e.Added.FirstOrDefault().parentSheetIndex != null && e.Added.FirstOrDefault().GetType() == typeof(Boots) && e.Added.FirstOrDefault().parentSheetIndex == ModEntry.scubaFinsID)
            {
                Monitor.Log("Player found scuba fins");
                Game1.player.mailReceived.Add("ScubaFins");
            }
        }
        public static void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // load scuba gear ids

            if (ModEntry.JsonAssets != null)
            {
                ModEntry.scubaMaskID = ModEntry.JsonAssets.GetHatId("Scuba Mask");
                ModEntry.scubaTankID = ModEntry.JsonAssets.GetClothingId("Scuba Tank");

                if (ModEntry.scubaMaskID == -1)
                {
                    Monitor.Log("Can't get ID for Swim mod item #1. Some functionality will be lost.");
                }
                else
                {
                    Monitor.Log(string.Format("Swim mod item #1 ID is {0}.", ModEntry.scubaMaskID));
                }

                if (ModEntry.scubaTankID == -1)
                {
                    Monitor.Log("Can't get ID for Swim mod item #2. Some functionality will be lost.");
                }
                else
                {
                    Monitor.Log(string.Format("Swim mod item #2 ID is {0}.", ModEntry.scubaTankID));
                }

                try
                {
                    ModEntry.scubaFinsID = Helper.Content.Load<Dictionary<int, string>>(@"Data/Boots", ContentSource.GameContent).First(x => x.Value.StartsWith("Scuba Fins")).Key;
                }
                catch
                {
                    Monitor.Log("Can't get ID for Swim mod item #3. Some functionality will be lost.");
                }
                if (ModEntry.scubaFinsID != -1)
                {
                    Monitor.Log(string.Format("Swim mod item #3 ID is {0}.", ModEntry.scubaFinsID));
                    if(Game1.player.boots != null && Game1.player.boots.Value != null && Game1.player.boots.Value.Name == "Scuba Fins" && Game1.player.boots.Value.parentSheetIndex != ModEntry.scubaFinsID)
                    {
                        Game1.player.boots.Value = new Boots(ModEntry.scubaFinsID);
                    }
                }
            }

            // load dive maps

            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                try
                {
                    Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    DiveMapData data = contentPack.ReadJsonFile<DiveMapData>("content.json");
                    SwimUtils.ReadDiveMapData(data);
                }
                catch
                {
                    Monitor.Log($"couldn't read content.json in content pack {contentPack.Manifest.Name}", LogLevel.Warn);
                }
            }

            Monitor.Log($"Reading content pack from assets/swim-map-content.json");

            try
            {
                DiveMapData myData = Helper.Data.ReadJsonFile<DiveMapData>("assets/swim-map-content.json");
                SwimUtils.ReadDiveMapData(myData);
            }
            catch (Exception ex)
            {
                Monitor.Log($"assets/swim-map-content.json file read error. Exception: {ex}", LogLevel.Warn);
            }

            if (!SwimUtils.IsWearingScubaGear() && Config.SwimSuitAlways)
                Game1.player.changeIntoSwimsuit();

            bubbleTexture = Helper.Content.Load<Texture2D>("LooseSprites/temporary_sprites_1", ContentSource.GameContent);
        }





        public static void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (ModEntry.isUnderwater && SwimUtils.IsMapUnderwater(Game1.player.currentLocation.Name))
            {
                if ((ticksUnderwater % 100 / Math.Min(100, Config.BubbleMult)) - bubbleOffset == 0)
                {
                    Game1.playSound("tinyWhip");
                    ModEntry.bubbles.Add(new Vector2(Game1.player.position.X + Game1.random.Next(-24,25), Game1.player.position.Y - 96));
                    if (ModEntry.bubbles.Count > 100)
                    {
                        ModEntry.bubbles = ModEntry.bubbles.Skip(1).ToList();
                    }
                    bubbleOffset = Game1.random.Next(30/ Math.Min(100, Config.BubbleMult));
                }

                for (int k = 0; k < ModEntry.bubbles.Count; k++) 
                {
                    ModEntry.bubbles[k] = new Vector2(ModEntry.bubbles[k].X, ModEntry.bubbles[k].Y - 2);
                }

                foreach (Vector2 v in ModEntry.bubbles)
                {
                    e.SpriteBatch.Draw(bubbleTexture, v + new Vector2((float)Math.Sin(ticksUnderwater / 20f) * 10f - Game1.viewport.X, -Game1.viewport.Y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(132, 20, 8, 8)), new Color(1,1,1,0.5f), 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
                }
                ticksUnderwater++;
            }
            else
            {
                ticksUnderwater = 0;
            }
        }

        public static void Display_RenderedHud(object sender, RenderedHudEventArgs e)
        {
            if(Game1.player.currentLocation.Name == "ScubaAbigailCave")
            {
                if(abigailTicks > 0 && abigailTicks < 30 * 5)
                {
                    e.SpriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.viewport.Width, Game1.viewport.Height) / 2 - new Vector2(78, 31) / 2, new Rectangle?(new Rectangle(353, 1649, 78, 31)), new Color(255,255,255,abigailTicks > 30 * 3 ? (int)Math.Round(255 * (abigailTicks - 90) / 60f) : 255), 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.99f);
                }
                if (abigailTicks > 0 && abigailTicks < 80000 / 16)
                    SwimUtils.MakeOxygenBar((80000 / 16) - abigailTicks, 80000 / 16);
                e.SpriteBatch.Draw(ModEntry.OxygenBarTexture, new Vector2((int)Math.Round(Game1.viewport.Width * 0.13f), 100), Color.White);
                return;
            }
            int maxOx = SwimUtils.MaxOxygen();
            if (ModEntry.oxygen < maxOx)
            {
                SwimUtils.MakeOxygenBar(ModEntry.oxygen, maxOx);
                e.SpriteBatch.Draw(ModEntry.OxygenBarTexture, new Vector2((int)Math.Round(Game1.viewport.Width * 0.13f), 100), Color.White);
            }
        }

        public static void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // load TMX pack

            IContentPack TMXcontentPack = Helper.ContentPacks.CreateFake(Path.Combine(Helper.DirectoryPath, "assets/tmx-pack"));

            object api = Helper.ModRegistry.GetApi("Platonymous.TMXLoader");
            if (api != null)
            {
                Helper.Reflection.GetMethod(api, "AddContentPack").Invoke(TMXcontentPack);
            }

            // load scuba gear

            ModEntry.JsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            bool flag = ModEntry.JsonAssets == null;
            if (flag)
            {
                Monitor.Log("Can't load Json Assets API for scuba gear");
            }
            else
            {
                ModEntry.JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets/json-assets"));
            }

            // fix dive maps

            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                try
                {
                    Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    DiveMapData data = contentPack.ReadJsonFile<DiveMapData>("content.json");
                    foreach(DiveMap map in data.Maps)
                    {
                        if (map.Features.Contains("FixWaterTiles") && !ModEntry.changeLocations.ContainsKey(map.Name))
                        {
                            ModEntry.changeLocations.Add(map.Name, false);
                        }
                    }
                }
                catch
                {
                    Monitor.Log($"couldn't read content.json in content pack {contentPack.Manifest.Name}", LogLevel.Warn);
                }
            }

            // load breath audio

            if (Config.BreatheSound)
            {
                LoadBreatheSound();
            }
        }

        private static void LoadBreatheSound()
        {
            string filePath = Path.Combine(Helper.DirectoryPath, "assets", "breathe.wav");
            if (File.Exists(filePath))
            {
                breatheEffect = SoundEffect.FromStream(new FileStream(filePath, FileMode.Open));
                Monitor.Log("Loaded breathing sound.");
            }
        }

        public static void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            foreach(KeyValuePair<string, DiveMap> kvp in ModEntry.diveMaps)
            {
                GameLocation location = Game1.getLocationFromName(kvp.Key);
                if (location == null)
                {
                    Monitor.Log($"GameLocation {kvp.Key} not found in day started loop");
                    continue;
                }
                if (kvp.Value.Features.Contains("OceanTreasure") || kvp.Value.Features.Contains("OceanResources") || kvp.Value.Features.Contains("Minerals"))
                {
                    Monitor.Log($"Clearing overlay objects from GameLocation {location.Name} ");
                    location.overlayObjects.Clear();
                }
                if (kvp.Value.Features.Contains("OceanTreasure"))
                {
                    Monitor.Log($"Adding ocean treasure to GameLocation {location.Name} ");
                    SwimMaps.AddOceanTreasure(location);
                }
                if (kvp.Value.Features.Contains("OceanResources"))
                {
                    Monitor.Log($"Adding ocean forage to GameLocation {location.Name} ");
                    SwimMaps.AddOceanForage(location);
                }
                if (kvp.Value.Features.Contains("Minerals"))
                {
                    Monitor.Log($"Adding minerals to GameLocation {location.Name} ");
                    SwimMaps.AddMinerals(location);
                }
                if (kvp.Value.Features.Contains("SmolFishies") || kvp.Value.Features.Contains("BigFishies") || kvp.Value.Features.Contains("Crabs"))
                {
                    Monitor.Log($"Clearing characters from GameLocation {location.Name} ");
                    location.characters.Clear();
                }
                if (kvp.Value.Features.Contains("SmolFishies"))
                {
                    Monitor.Log($"Adding smol fishies to GameLocation {location.Name} ");
                    SwimMaps.AddFishies(location);
                }
                if (kvp.Value.Features.Contains("BigFishies"))
                {
                    Monitor.Log($"Adding big fishies to GameLocation {location.Name} ");
                    SwimMaps.AddFishies(location, false);
                }
                if (kvp.Value.Features.Contains("Crabs"))
                {
                    Monitor.Log($"Adding crabs to GameLocation {location.Name} ");
                    SwimMaps.AddCrabs(location);
                }
                if (kvp.Value.Features.Contains("WaterTiles"))
                {
                    Monitor.Log($"Adding water tiles to GameLocation {location.Name} ");
                    SwimMaps.AddWaterTiles(location);
                }
                if (kvp.Value.Features.Contains("Underwater"))
                {
                    Monitor.Log($"Removing water tiles from GameLocation {location.Name} ");
                    SwimMaps.RemoveWaterTiles(location);
                }
            }
            if (Game1.getLocationFromName("ScubaCave") != null && !Game1.player.mailReceived.Contains("ScubaMask"))
            {
                SwimMaps.AddScubaChest(Game1.getLocationFromName("ScubaCave"), new Vector2(10,14), "ScubaMask");
            }
            ModEntry.marinerQuestionsWrongToday = false;
            ModEntry.oxygen = SwimUtils.MaxOxygen();
        }

        public static void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if(Game1.player == null)
            {
                ModEntry.myButtonDown = false;
                return;
            }
            SwimUtils.CheckIfMyButtonDown();
        }


        public static void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.player == null || Game1.player.currentLocation == null)
            {
                ModEntry.myButtonDown = false;
                return;
            }

            if(false && e.Button == SButton.Q)
            {
                SwimUtils.SeaMonsterSay("The quick brown fox jumped over the slow lazy dog.");
            }

            if (Game1.activeClickableMenu != null && Game1.player.currentLocation.Name == "ScubaCrystalCave" && Game1.player.currentLocation.lastQuestionKey.StartsWith("SwimMod_Mariner_"))
            {
                IClickableMenu menu = Game1.activeClickableMenu;
                if (menu == null || menu.GetType() != typeof(DialogueBox))
                    return;

                DialogueBox db = menu as DialogueBox;
                int resp = db.selectedResponse;
                List<Response> resps = db.responses;

                if (resp < 0 || resps == null || resp >= resps.Count || resps[resp] == null)
                    return;
                Game1.player.currentLocation.lastQuestionKey = ""; 
                
                SwimDialog.OldMarinerDialogue(resps[resp].responseKey);
                return;
            }

            if(false && e.Button == SButton.Q)
            {
                var v1 = Game1.game1;
                return;
                //Game1.player.currentLocation.overlayObjects[Game1.player.getTileLocation() + new Vector2(0, 1)] = new Chest(0, new List<Item>() { Helper.Input.IsDown(SButton.LeftShift) ? (Item)(new StardewValley.Object(434, 1)) : (new Hat(ModEntry.scubaMaskID)) }, Game1.player.getTileLocation() + new Vector2(0, 1), false, 0);
            }

            if (e.Button == Config.DiveKey && Game1.activeClickableMenu == null && !Game1.player.UsingTool && ModEntry.diveMaps.ContainsKey(Game1.player.currentLocation.Name) && ModEntry.diveMaps[Game1.player.currentLocation.Name].DiveLocations.Count > 0)
            {
                Point pos = Game1.player.getTileLocationPoint();
                Location loc = new Location(pos.X, pos.Y);

                if (!SwimUtils.IsInWater())
                {
                    return;
                }

                DiveMap dm = ModEntry.diveMaps[Game1.player.currentLocation.Name];
                DiveLocation diveLocation = null;
                foreach(DiveLocation dl in dm.DiveLocations)
                {
                    if (dl.GetRectangle().X == -1 || dl.GetRectangle().Contains(loc))
                    {
                        diveLocation = dl;
                        break;
                    }
                }

                if (diveLocation == null)
                {
                    Monitor.Log($"No dive destination for this point on this map", LogLevel.Debug);
                    return;
                }

                if (Game1.getLocationFromName(diveLocation.OtherMapName) == null)
                {
                    Monitor.Log($"Can't find destination map named {diveLocation.OtherMapName}", LogLevel.Warn);
                    return;
                }

                Monitor.Log($"warping to {diveLocation.OtherMapName}", LogLevel.Debug);
                SwimUtils.DiveTo(diveLocation);
                return; 
            }
            
            if (e.Button == Config.SwimKey && Game1.activeClickableMenu == null && (!Game1.player.swimming || !Config.ReadyToSwim) && !isJumping)
            {
                Config.ReadyToSwim = !Config.ReadyToSwim;
                Helper.WriteConfig<ModConfig>(Config);
                Monitor.Log($"Ready to swim: {Config.ReadyToSwim}");
                return;
            }

            if (e.Button == Config.SwimSuitKey && Game1.activeClickableMenu == null)
            {
                Config.SwimSuitAlways = !Config.SwimSuitAlways;
                Helper.WriteConfig<ModConfig>(Config);
                if (!Game1.player.swimming)
                {
                    if(!Config.SwimSuitAlways)
                        Game1.player.changeOutOfSwimSuit();
                    else
                        Game1.player.changeIntoSwimsuit();
                }
                return;
            }

        }

        public static void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.player.currentLocation == null || Game1.player == null || !Game1.displayFarmer || Game1.player.position == null)
                return;

            ModEntry.isUnderwater = SwimUtils.IsMapUnderwater(Game1.player.currentLocation.Name);

            if (Game1.player.currentLocation.Name == "ScubaAbigailCave")
            {
                AbigailCaveTick();
            }

            if (Game1.activeClickableMenu == null)
            {
                if (ModEntry.isUnderwater)
                {
                    if (ModEntry.oxygen >= 0)
                    {
                        if (!SwimUtils.IsWearingScubaGear())
                            ModEntry.oxygen--;
                        else {
                            if (ModEntry.oxygen < SwimUtils.MaxOxygen())
                                ModEntry.oxygen++;
                            if (ModEntry.oxygen < SwimUtils.MaxOxygen())
                                ModEntry.oxygen++;
                        }
                    }
                    if(ModEntry.oxygen < 0 && !surfacing)
                    {
                        surfacing = true;
                        Game1.playSound("pullItemFromWater");
                        DiveLocation diveLocation = ModEntry.diveMaps[Game1.player.currentLocation.Name].DiveLocations.Last();
                        SwimUtils.DiveTo(diveLocation);
                    }
                }
                else
                {
                    surfacing = false;
                    if (ModEntry.oxygen < SwimUtils.MaxOxygen())
                        ModEntry.oxygen++;
                    if (ModEntry.oxygen < SwimUtils.MaxOxygen())
                        ModEntry.oxygen++;
                }
            }

            if (SwimUtils.IsWearingScubaGear())
            {
                ticksWearingScubaGear++;
                if (Config.BreatheSound && breatheEffect != null && (lastBreatheSound == 0 || ticksWearingScubaGear - lastBreatheSound > 6000 / 16))
                {
                    Monitor.Log("Playing breathe sound");
                    lastBreatheSound = ticksWearingScubaGear;
                    breatheEffect.Play(0.5f * Game1.options.soundVolumeLevel, 0f, 0f);
                }
            }
            else
            {
                if (breatheEffect != null && lastBreatheSound != 0)
                {
                    breatheEffect.Dispose();
                    LoadBreatheSound();
                }
                lastBreatheSound = 0;
                ticksWearingScubaGear = 0;
            }

            if (isJumping)
            {
                float difx = endJumpLoc.X - startJumpLoc.X;
                float dify = endJumpLoc.Y - startJumpLoc.Y;
                float completed = Game1.player.freezePause / (float)Config.JumpTimeInMilliseconds;
                if (Game1.player.freezePause <= 0)
                {
                    Game1.player.position.Value = endJumpLoc;
                    isJumping = false;
                    if (ModEntry.willSwim)
                    {
                        Game1.player.currentLocation.playSound("waterSlosh", NetAudio.SoundContext.Default);
                        Game1.player.swimming.Value = true;
                    }
                    else
                    {
                        if (!Config.SwimSuitAlways)
                            Game1.player.changeOutOfSwimSuit();
                    }
                    return;
                }
                Game1.player.position.Value = new Vector2(endJumpLoc.X - (difx * completed), endJumpLoc.Y - (dify * completed) - (float)Math.Sin(completed * Math.PI) * 64);
                return;
            }

            // only if ready to swim from here on!

            if (!Config.ReadyToSwim || !Context.IsPlayerFree || Game1.player.currentLocation is BeachNightMarket)
            {
                return;
            }

            if (Game1.player.swimming) {
                if (!SwimUtils.IsInWater() && !isJumping)
                {
                    Monitor.Log("Swimming out of water");
                    ModEntry.willSwim = false;
                    Game1.player.freezePause = Config.JumpTimeInMilliseconds;
                    Game1.player.currentLocation.playSound("dwop", NetAudio.SoundContext.Default);
                    Game1.player.currentLocation.playSound("waterSlosh", NetAudio.SoundContext.Default);
                    isJumping = true;
                    startJumpLoc = Game1.player.position.Value;
                    endJumpLoc = Game1.player.position.Value;

                    Game1.player.swimming.Value = false;
                    if (Game1.player.bathingClothes && !Config.SwimSuitAlways)
                        Game1.player.changeOutOfSwimSuit();
                }

                DiveMap dm = null;
                Point edgePos = Game1.player.getTileLocationPoint();

                if (ModEntry.diveMaps.ContainsKey(Game1.player.currentLocation.Name))
                {
                    dm = ModEntry.diveMaps[Game1.player.currentLocation.Name];
                }

                if (Game1.player.position.Y > Game1.viewport.Y + Game1.viewport.Height - 16)
                {
                    Game1.player.position.Value = new Vector2(Game1.player.position.X, Game1.viewport.Y + Game1.viewport.Height - 17);
                    if (dm != null)
                    {
                        EdgeWarp edge = dm.EdgeWarps.Find((x) => x.ThisMapEdge == "Bottom" && x.FirstTile <= edgePos.X && x.LastTile >= edgePos.X);
                        if (edge != null)
                        {
                            Point pos = SwimUtils.GetEdgeWarpDestination(edgePos.X, edge);
                            if (pos != Point.Zero)
                            {
                                Monitor.Log("warping south");
                                Game1.warpFarmer(edge.OtherMapName, pos.X, pos.Y, false);
                            }
                        }
                    }
                }
                else if (Game1.player.position.Y < Game1.viewport.Y - 16)
                {
                    Game1.player.position.Value = new Vector2(Game1.player.position.X, Game1.viewport.Y - 15);

                    if (dm != null)
                    {
                        EdgeWarp edge = dm.EdgeWarps.Find((x) => x.ThisMapEdge == "Top" && x.FirstTile <= edgePos.X && x.LastTile >= edgePos.X);
                        if (edge != null)
                        {
                            Point pos = SwimUtils.GetEdgeWarpDestination(edgePos.X, edge);
                            if (pos != Point.Zero)
                            {
                                Monitor.Log("warping north");
                                Game1.warpFarmer(edge.OtherMapName, pos.X, pos.Y, false);
                            }
                        }
                    }
                }
                else if (Game1.player.position.X > Game1.viewport.X + Game1.viewport.Width - 32)
                {
                    Game1.player.position.Value = new Vector2(Game1.viewport.X + Game1.viewport.Width - 33, Game1.player.position.Y);

                    if (dm != null)
                    {
                        EdgeWarp edge = dm.EdgeWarps.Find((x) => x.ThisMapEdge == "Right" && x.FirstTile <= edgePos.Y && x.LastTile >= edgePos.Y);
                        if (edge != null)
                        {
                            Point pos = SwimUtils.GetEdgeWarpDestination(edgePos.Y, edge);
                            if (pos != Point.Zero)
                            {
                                Monitor.Log("warping east");
                                Game1.warpFarmer(edge.OtherMapName, pos.X, pos.Y, false);
                            }
                        }
                    }

                    if (Game1.player.currentLocation.Name == "Forest")
                    {
                        if (Game1.player.position.Y / 64 > 74)
                            Game1.warpFarmer("Beach", 0, 13, false);
                        else
                            Game1.warpFarmer("Town", 0, 100, false);
                    }
                }
                else if (Game1.player.position.X < Game1.viewport.X - 32)
                {
                    Game1.player.position.Value = new Vector2(Game1.viewport.X - 31, Game1.player.position.Y);

                    if (dm != null)
                    {
                        EdgeWarp edge = dm.EdgeWarps.Find((x) => x.ThisMapEdge == "Left" && x.FirstTile <= edgePos.Y && x.LastTile >= edgePos.Y);
                        if (edge != null)
                        {
                            Point pos = SwimUtils.GetEdgeWarpDestination(edgePos.Y, edge);
                            if (pos != Point.Zero)
                            {
                                Monitor.Log("warping west");
                                Game1.warpFarmer(edge.OtherMapName, pos.X, pos.Y, false);
                            }
                        }
                    }

                    if (Game1.player.currentLocation.Name == "Town")
                    {
                        Game1.warpFarmer("Forest", 119, 43, false);
                    }
                    else if (Game1.player.currentLocation.Name == "Beach")
                    {
                        Game1.warpFarmer("Forest", 119, 111, false);
                    }
                }

                if (Game1.player.bathingClothes && SwimUtils.IsWearingScubaGear() && !Config.SwimSuitAlways)
                    Game1.player.changeOutOfSwimSuit();
                else if (!Game1.player.bathingClothes && (!SwimUtils.IsWearingScubaGear() || Config.SwimSuitAlways))
                    Game1.player.changeIntoSwimsuit();

                if (Game1.player.boots.Value != null && ModEntry.scubaFinsID != -1 && Game1.player.boots.Value.indexInTileSheet == ModEntry.scubaFinsID)
                {
                    int buffId = 42883167;
                    Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault((Buff p) => p.which == buffId);
                    if (buff == null)
                    {
                        BuffsDisplay buffsDisplay = Game1.buffsDisplay;
                        Buff buff2 = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 1, "Scuba Fins", Helper.Translation.Get("scuba-fins"));
                        buff2.which = buffId;
                        buff = buff2;
                        buffsDisplay.addOtherBuff(buff2);
                    }
                    buff.millisecondsDuration = 50;
                }
            }
            else
            {
                if (SwimUtils.IsInWater() && !isJumping)
                {
                    Monitor.Log("In water not swimming");

                    ModEntry.willSwim = true;
                    Game1.player.freezePause = Config.JumpTimeInMilliseconds;
                    Game1.player.currentLocation.playSound("dwop", NetAudio.SoundContext.Default);
                    isJumping = true;
                    startJumpLoc = Game1.player.position.Value;
                    endJumpLoc = Game1.player.position.Value;


                    Game1.player.swimming.Value = true;
                    if (!Game1.player.bathingClothes && !SwimUtils.IsWearingScubaGear())
                        Game1.player.changeIntoSwimsuit();
                }

            }

            SwimUtils.CheckIfMyButtonDown();

            if (!ModEntry.myButtonDown || Game1.player.millisecondsPlayed - lastJump < 250 || SwimUtils.IsMapUnderwater(Game1.player.currentLocation.Name))
                return;

            if (Helper.Input.IsDown(SButton.MouseLeft) && !Game1.player.swimming && (Game1.player.CurrentTool is WateringCan || Game1.player.CurrentTool is FishingRod))
                return;


            List<Vector2> tiles = SwimUtils.GetTilesInDirection(5);
            Vector2 jumpLocation = Vector2.Zero;
            
            double distance = -1;
            int maxDistance = 0;
            switch (Game1.player.FacingDirection)
            {
                case 0:
                    distance = Math.Abs(Game1.player.position.Y - tiles.Last().Y * Game1.tileSize);
                    maxDistance = 72;
                    break;
                case 2:
                    distance = Math.Abs(Game1.player.position.Y - tiles.Last().Y * Game1.tileSize);
                    maxDistance = 48;
                    break;
                case 1:
                case 3:
                    distance = Math.Abs(Game1.player.position.X - tiles.Last().X * Game1.tileSize);
                    maxDistance = 65;
                    break;
            }
            if (Helper.Input.IsDown(SButton.MouseLeft))
            {
                try
                {
                    int xTile = (Game1.viewport.X + Game1.getOldMouseX()) / 64;
                    int yTile = (Game1.viewport.Y + Game1.getOldMouseY()) / 64;
                    bool water = Game1.player.currentLocation.waterTiles[xTile, yTile];
                    if (Game1.player.swimming != water)
                    {
                        distance = -1;
                    }
                }
                catch
                {

                }
            }
            //Monitor.Log("Distance: " + distance);

            bool nextToLand = Game1.player.swimming && !Game1.player.currentLocation.isTilePassable(new Location((int)tiles.Last().X, (int)tiles.Last().Y), Game1.viewport) && !SwimUtils.IsWaterTile(tiles[tiles.Count - 2]) && distance < maxDistance;
            
            bool nextToWater = false;
            try
            {
                nextToWater = !Game1.player.swimming &&
                    !SwimUtils.IsTilePassable(Game1.player.currentLocation, new Location((int)tiles.Last().X, (int)tiles.Last().Y), Game1.viewport) &&
                    (Game1.player.currentLocation.waterTiles[(int)tiles.Last().X, (int)tiles.Last().Y]
                        || SwimUtils.IsWaterTile(tiles[tiles.Count - 2])) 
                    && distance < maxDistance;
            }
            catch
            {
                //Monitor.Log($"exception trying to get next to water: {ex}");
            }

            //Monitor.Log($"next passable {Game1.player.currentLocation.isTilePassable(new Location((int)tiles.Last().X, (int)tiles.Last().Y), Game1.viewport)} next to land: {nextToLand}, next to water: {nextToWater}");


            if (Helper.Input.IsDown(Config.SwimKey) || nextToLand || nextToWater)
            {
                //Monitor.Log("okay to jump");
                for(int i = 0; i < tiles.Count; i++)
                {
                    Vector2 tileV = tiles[i];
                    bool isWater = false;
                    bool isPassable = false;
                    try
                    {
                        Tile tile = Game1.player.currentLocation.map.GetLayer("Buildings").PickTile(new Location((int)tileV.X * Game1.tileSize, (int)tileV.Y * Game1.tileSize), Game1.viewport.Size);
                        isWater = SwimUtils.IsWaterTile(tileV);
                        isPassable = (nextToLand && !isWater && SwimUtils.IsTilePassable(Game1.player.currentLocation, new Location((int)tileV.X, (int)tileV.Y), Game1.viewport)) || (nextToWater && isWater && (tile == null || tile.TileIndex == 76));
                        //Monitor.Log($"Trying {tileV} is passable {isPassable} isWater {isWater}");
                        if (!SwimUtils.IsTilePassable(Game1.player.currentLocation, new Location((int)tileV.X, (int)tileV.Y), Game1.viewport) && !isWater && nextToLand)
                        {
                            //Monitor.Log($"Nixing {tileV}");
                            jumpLocation = Vector2.Zero;
                        }
                    }
                    catch(Exception ex)
                    {
                        Monitor.Log(""+ex);
                    }
                    if (nextToLand && !isWater && isPassable)
                    {
                        Monitor.Log($"Jumping to {tileV}");
                        jumpLocation = tileV;

                    }

                    if (nextToWater && isWater && isPassable)
                    {
                        Monitor.Log($"Jumping to {tileV}");
                        jumpLocation = tileV;

                    }


                }
            }

            if (jumpLocation != Vector2.Zero)
            {
                lastJump = Game1.player.millisecondsPlayed;
                //Monitor.Log("got swim location");
                if (Game1.player.swimming)
                {
                    ModEntry.willSwim = false;
                    Game1.player.swimming.Value = false;
                    Game1.player.freezePause = Config.JumpTimeInMilliseconds;
                    Game1.player.currentLocation.playSound("dwop", NetAudio.SoundContext.Default);
                    Game1.player.currentLocation.playSound("waterSlosh", NetAudio.SoundContext.Default);
                }
                else
                {
                    ModEntry.willSwim = true;
                    if(!SwimUtils.IsWearingScubaGear())
                        Game1.player.changeIntoSwimsuit();
                    
                    Game1.player.freezePause = Config.JumpTimeInMilliseconds;
                    Game1.player.currentLocation.playSound("dwop", NetAudio.SoundContext.Default);
                }
                isJumping = true;
                startJumpLoc = Game1.player.position.Value;
                endJumpLoc = new Vector2(jumpLocation.X * Game1.tileSize, jumpLocation.Y * Game1.tileSize);
            }

        }

        public static void AbigailCaveTick()
        {

            Game1.player.CurrentToolIndex = Game1.player.items.Count;

            List<NPC> list = Game1.player.currentLocation.characters.ToList().FindAll((n) => (n is Monster) && (n as Monster).Health <= 0);
            foreach(NPC n in list)
            {
                Game1.player.currentLocation.characters.Remove(n);
            }

            if (abigailTicks < 0)
            {
                return;
            }
            Game1.exitActiveMenu();

            if (abigailTicks == 0)
            {
                FieldInfo f1 = Game1.player.currentLocation.characters.GetType().GetField("OnValueRemoved", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                f1.SetValue(Game1.player.currentLocation.characters, null);
            }

            Vector2 v = Vector2.Zero;
            float yrt = (float)(1/Math.Sqrt(2));
            if (Helper.Input.IsDown(SButton.Up))
            {
                if(Helper.Input.IsDown(SButton.Right))
                    v = new Vector2(yrt, -yrt);
                else if(Helper.Input.IsDown(SButton.Left))
                    v = new Vector2(-yrt, -yrt);
                else 
                    v = new Vector2(0, -1);
            }
            else if (Helper.Input.IsDown(SButton.Down))
            {
                if(Helper.Input.IsDown(SButton.Right))
                    v = new Vector2(yrt, yrt);
                else if(Helper.Input.IsDown(SButton.Left))
                    v = new Vector2(-yrt, yrt);
                else 
                    v = new Vector2(0, 1);
            }
            else if (Helper.Input.IsDown(SButton.Right))
                v = new Vector2(1, 0);
            else if (Helper.Input.IsDown(SButton.Left))
                v = new Vector2(-1, 0);
            else if (Helper.Input.IsDown(SButton.MouseLeft))
            {
                float x = Game1.viewport.X + Game1.getOldMouseX() - Game1.player.position.X;
                float y = Game1.viewport.Y + Game1.getOldMouseY() - Game1.player.position.Y;
                float dx = Math.Abs(x);
                float dy = Math.Abs(y);
                if(y < 0)
                {
                    if(x > 0)
                    {
                        if(dy > dx)
                        {
                            if (dy-dx > dy / 2)
                                v = new Vector2(0, -1);
                            else
                                v = new Vector2(yrt, -yrt);

                        }
                        else
                        {
                            if (dx - dy > x / 2)
                                v = new Vector2(1, 0);
                            else
                                v = new Vector2(yrt, -yrt);
                        }
                    }
                    else
                    {
                        if (dy > dx)
                        {
                            if (dy - dx > dy / 2)
                                v = new Vector2(0, -1);
                            else
                                v = new Vector2(-yrt, -yrt);

                        }
                        else
                        {
                            if (dx - dy > x / 2)
                                v = new Vector2(-1, 0);
                            else
                                v = new Vector2(-yrt, -yrt);
                        }
                    }
                }
                else
                {
                    if (x > 0)
                    {
                        if (dy > dx)
                        {
                            if (dy - dx > dy / 2)
                                v = new Vector2(0, 1);
                            else
                                v = new Vector2(yrt, yrt);

                        }
                        else
                        {
                            if (dx - dy > x / 2)
                                v = new Vector2(1, 0);
                            else
                                v = new Vector2(yrt, yrt);
                        }
                    }
                    else
                    {
                        if (dy > dx)
                        {
                            if (dy - dx > dy / 2)
                                v = new Vector2(0, -1);
                            else
                                v = new Vector2(-yrt, yrt);

                        }
                        else
                        {
                            if (dx - dy > x / 2)
                                v = new Vector2(-1, 0);
                            else
                                v = new Vector2(-yrt, yrt);
                        }
                    }
                }
            }

            if (v != Vector2.Zero && Game1.player.millisecondsPlayed - lastProjectile > 350)
            {
                Game1.player.currentLocation.projectiles.Add(new AbigailProjectile(1, 383, 0, 0, 0, v.X * 6, v.Y * 6, new Vector2(Game1.player.getStandingX() - 24, Game1.player.getStandingY() - 48), "Cowboy_monsterDie", "Cowboy_gunshot", false, true, Game1.player.currentLocation, Game1.player, true)); 
                lastProjectile = Game1.player.millisecondsPlayed;
            }

            foreach (SButton button in abigailShootButtons)
            {
                if (Helper.Input.IsDown(button))
                {
                    switch (button)
                    {
                        case SButton.Up:
                            break;
                        case SButton.Right:
                            v = new Vector2(1, 0);
                            break;
                        case SButton.Down:
                            v = new Vector2(0, 1);
                            break;
                        default:
                            v = new Vector2(-1, 0);
                            break;
                    }
                }
            }


            abigailTicks++;
            if(abigailTicks > 80000 / 16f)
            {
                if (Game1.player.currentLocation.characters.ToList().FindAll((n) => (n is Monster)).Count > 0)
                    return;

                abigailTicks = -1;
                Game1.player.hat.Value = null;
                Game1.stopMusicTrack(Game1.MusicContext.Default);

                if(!Game1.player.mailReceived.Contains("ScubaFins"))
                {
                    Game1.playSound("Cowboy_Secret");
                    SwimMaps.AddScubaChest(Game1.player.currentLocation, new Vector2(8, 8), "ScubaFins");
                }

                Game1.player.currentLocation.setMapTile(8, 16, 91, "Buildings", null);
                Game1.player.currentLocation.setMapTile(9, 16, 92, "Buildings", null);
                Game1.player.currentLocation.setTileProperty(9, 16, "Back", "Water", "T");
                Game1.player.currentLocation.setMapTile(10, 16, 93, "Buildings", null);
                Game1.player.currentLocation.setMapTile(8, 17, 107, "Buildings", null);
                Game1.player.currentLocation.setMapTile(9, 17, 108, "Back", null);
                Game1.player.currentLocation.setTileProperty(9, 17, "Back", "Water", "T");
                Game1.player.currentLocation.removeTile(9, 17, "Buildings");
                Game1.player.currentLocation.setMapTile(10, 17, 109, "Buildings", null); 
                Game1.player.currentLocation.setMapTile(8, 18, 139, "Buildings", null);
                Game1.player.currentLocation.setMapTile(9, 18, 140, "Buildings", null);
                Game1.player.currentLocation.setMapTile(10, 18, 141, "Buildings", null);
                SwimMaps.AddWaterTiles(Game1.player.currentLocation);
            }
            else
            {
                if (Game1.random.NextDouble() < 0.03)
                {
                    int which = Game1.random.Next(3);
                    Point p = new Point();
                    switch (Game1.random.Next(4))
                    {
                        case 0:
                            p = new Point(8 + which, 1);
                            break;
                        case 1:
                            p = new Point(1, 8 + which);
                            break;
                        case 2:
                            p = new Point(8 + which, 16);
                            break;
                        case 3:
                            p = new Point(16, 8 + which);
                            break;
                    }
                    Game1.player.currentLocation.characters.Add(new AbigailMetalHead(new Vector2(p.X * Game1.tileSize, p.Y * Game1.tileSize), 0));
                }

            }
        }

    }
}
