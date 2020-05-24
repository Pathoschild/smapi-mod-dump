using StardewModdingAPI;
using System.Collections.Generic;
using System;
using StardewValley;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Linq;
using StardewValley.Locations;
using System.CodeDom;

namespace Lockpicks
{
    public class Mod : StardewModdingAPI.Mod
    {
        internal static JsonAssets JA;
        internal static Random RNG = new Random(Guid.NewGuid().GetHashCode());
        internal static int LockpickItemId = -1;
        internal static HashSet<string> LockCache = new HashSet<string>();
        private string GenerateCacheKey(string objectType, GameLocation location, float x, float y) { return objectType + "^" + location.Name + "^" + ((int)x).ToString() + "^" + ((int)y).ToString(); }

        public override void Entry(IModHelper helper)
        {
            JA = new JsonAssets(this);
            if (!JA.IsHappy) return;
            JA.RegisterObject(Helper.Translation.Get("tool"), Helper.Translation.Get("tooltip"), "lockpick.png", "Pierre", StardewValley.Object.junkCategory, 15, 500);
            Helper.Events.GameLoop.SaveLoaded += (s,e) => { LockpickItemId = JA.GetObjectId("Lockpick").Value; };
            Helper.Events.GameLoop.DayStarted += (s,e) => { LockCache.Clear(); };
            Helper.Events.Input.ButtonPressed += (s,e) => { OnInput(e); };
            Helper.Events.Multiplayer.ModMessageReceived += OnMultiplayerPacket;
        }

        private void OnInput(ButtonPressedEventArgs e)
        {
            if (e.IsSuppressed() || (!e.Button.IsActionButton() && !e.Button.IsUseToolButton()) || Game1.eventUp || !Context.IsPlayerFree) return;
            Vector2 vector = Utils.GetTargetedTile();
            string v = Utils.GetAction(Game1.currentLocation, vector);
            if(v != null && OnTileAction(vector, v)) { Helper.Input.Suppress(e.Button); }
            else
            {
                vector = Utility.getTranslatedVector2(vector, Game1.player.FacingDirection, 0f);
                vector.Y += 1;
                v = Utils.GetAction(Game1.currentLocation, vector);
                if(v != null && OnTileAction(vector, v)) { Helper.Input.Suppress(e.Button); }
                else
                {
                    vector = Game1.player.getTileLocation();
                    v = Utils.GetAction(Game1.currentLocation, vector);
                    if (v != null && OnTileAction(vector, v)) { Helper.Input.Suppress(e.Button); }
                }
            }
        }

        private bool OnTileAction(Vector2 vector, string action)
        {
            //Monitor.Log(action, LogLevel.Alert);
            var parameters = action.Split(' ');
            if (parameters.Length < 1) return false;
            bool lockFound = false;
            switch (parameters[0])
            {
                case "LockedDoorWarp":
                    if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason) && Utility.getStartTimeOfFestival() < 1900)
                    {
                        lockFound = true;
                        break;
                    }
                    if (parameters[3] == "WizardHouse" && !Utils.IsWizardHouseUnlocked()) {
                        lockFound = true;
                        break;
                    }
                    if(parameters[3] == "SeedShop" && Game1.Date.DayOfWeek == DayOfWeek.Wednesday && !Game1.MasterPlayer.eventsSeen.Contains(191393))
                    {
                        lockFound = true;
                        break;
                    }
                    if (Game1.timeOfDay < int.Parse(parameters[4]) || Game1.timeOfDay >= int.Parse(parameters[5])) lockFound = true;
                    else if(parameters.Length > 6 && Game1.player.getFriendshipLevelForNPC(parameters[6]) < int.Parse(parameters[7])) lockFound = true;
                    break;
                case "Door":
                    if (parameters.Length > 1 && Game1.player.getFriendshipLevelForNPC(parameters[1]) < 500) lockFound = true;
                    break;
                case "SkullDoor":
                    if (!Game1.MasterPlayer.hasUnlockedSkullDoor && !Game1.MasterPlayer.hasSkullKey) lockFound = true;
                    break;
                case "WarpCommunityCenter":
                    if (!(Game1.MasterPlayer.mailReceived.Contains("ccDoorUnlock") || Game1.MasterPlayer.mailReceived.Contains("JojaMember"))) lockFound = true;
                    break;
                case "WarpWomensLocker":
                    if (Game1.player.IsMale) lockFound = true;
                    break;
                case "WarpMensLocker":
                    if (!Game1.player.IsMale) lockFound = true;
                    break;
                case "WizardHatch":
                    if ((!Game1.player.friendshipData.ContainsKey("Wizard") || Game1.player.friendshipData["Wizard"].Points < 1000)) lockFound = true;
                    break;
                case "SewerGrate":
                case "EnterSewer":
                    if (!Game1.MasterPlayer.hasRustyKey && !Game1.MasterPlayer.mailReceived.Contains("OpenedSewer")) lockFound = true;
                    break;
                case "Warp_Sunroom_Door":
                    if (Game1.player.getFriendshipHeartLevelForNPC("Caroline") < 2) lockFound = true;
                    break;
                case "Theater_Entrance":
                    if (!Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater")) break;
                    if (Game1.player.team.movieMutex.IsLocked() || Game1.isFestival() || Game1.timeOfDay > 2100 || Game1.timeOfDay < 900) lockFound = true;
                    else if (!Game1.player.hasItemInInventory(809, 1, 0)) lockFound = true;
                    else if (Game1.player.lastSeenMovieWeek.Value >= Game1.Date.TotalWeeks) lockFound = true;
                    break;
                case "Message":
                    if (parameters.Length < 2) break;
                    if (parameters[1] == "\"HaleyHouse.1\"") lockFound = true;
                    else if (parameters[1] == "\"AnimalShop.17\"") lockFound = true;
                    break;
            }

            bool cached = LockCache.Contains(GenerateCacheKey(parameters[0], Game1.currentLocation, vector.X, vector.Y));
            if (!cached && !Game1.player.hasItemInInventory(LockpickItemId, 1)) return false; //we're done here

            if (lockFound)
            {
                if (cached)
                {
                    OpenLock(action.Split(' '), (int)vector.X, (int)vector.Y);
                }
                else
                {
                    string key = string.Join("^", new[] { "l", ((int)vector.X).ToString(), ((int)vector.Y).ToString(), action });
                    Game1.currentLocation.lastQuestionKey = "lockpick";
                    Response[] responses = new[] { new Response(key, Helper.Translation.Get("yes")), new Response("No", Helper.Translation.Get("no")) };
                    Game1.currentLocation.createQuestionDialogue(Helper.Translation.Get("use"), responses, (f, a) => {
                        if (a == "No" || !a.StartsWith("l^")) return;
                        var p = a.Substring(2).Split('^');
                        if (p.Length != 3) return;
                        if (RNG.Next(25) == 0) //chance of breaking
                        {
                            Game1.player.removeFirstOfThisItemFromInventory(LockpickItemId);
                            Game1.showRedMessage(Helper.Translation.Get("broke"));
                            Game1.playSound("clank");
                        }
                        else OpenLock(p[2].Split(' '), int.Parse(p[0]), int.Parse(p[1]), true);
                    });
                }
                return true;
            }
            return false;
        }

        private void OpenLock(string[] Lock, int tileX, int tileY, bool picked = false)
        {
            if (picked)
            {
                Game1.playSound("stoneCrack"); Game1.playSound("axchop");
                string key = GenerateCacheKey(Lock[0], Game1.currentLocation, tileX, tileY);
                LockCache.Add(key);
                if (Game1.IsMultiplayer) Helper.Multiplayer.SendMessage<string>(key, "lockpickEvent");
            }
            switch (Lock[0])
            {
                case "LockedDoorWarp":
                    Warp(picked, "doorClose", Lock[3], int.Parse(Lock[1]), int.Parse(Lock[2]));
                    break;
                case "Door":
                    Game1.currentLocation.openDoor(new xTile.Dimensions.Location(tileX, tileY), playSound: !picked);
                    Game1.currentLocation.map.GetLayer("Back").Tiles[tileX, tileY].Properties.Remove("TouchAction");
                    break;
                case "SkullDoor":
                    Game1.showRedMessage(Helper.Translation.Get("complex"));
                    break;
                case "WarpCommunityCenter":
                    Warp(picked, "doorClose", "CommunityCenter", 32, 23);
                    break;
                case "WarpWomensLocker":
                case "WarpMensLocker":
                    Warp(picked || Lock.Length >= 5, "doorClose", Lock[3], Convert.ToInt32(Lock[1]), Convert.ToInt32(Lock[2]));
                    break;
                case "WizardHatch":
                    Warp(picked, "doorClose", "WizardHouseBasement", 4, 4);
                    break;
                case "SewerGrate":
                    Warp(picked, "openChest", "Sewer", 3, 48);
                    break;
                case "EnterSewer":
                    Warp(picked, "stairsdown", "Sewer", 16, 11);
                    break;
                case "Warp_Sunroom_Door":
                    Warp(picked, "doorClose", "Sunroom", 5, 13);
                    break;
                case "Theater_Entrance":
                    Warp(picked, "doorClose", "MovieTheater", 13, 15);
                    break;
                case "Message":
                    if (Lock[1] == "\"HaleyHouse.1\"")
                    {
                        if (!picked) Game1.playSound("doorClose");
                        if(Game1.getLocationFromName("Darkroom") == null)
                        {
                            //add the darkroom
                            Game1.content.Load<xTile.Map>("Maps\\Darkroom");
                            Game1.locations.Add(new GameLocation("Maps\\Darkroom", "Darkroom"));
                            var darkroom = Game1.getLocationFromName("Darkroom");
                            darkroom.resetForPlayerEntry();
                            darkroom.warps.Add(new Warp(3, 8, "HaleyHouse", 4, 4, false));
                        }
                        Warp(picked, "doorClose", "Darkroom", 192 / Game1.tileSize, 384 / Game1.tileSize);
                    }
                    else if (Lock[1] == "\"AnimalShop.17\"")
                    {
                        if (!picked) Game1.playSound("doorClose");
                        if (Game1.getLocationFromName("MarnieBarn") == null)
                        {
                            //add the darkroom
                            Game1.content.Load<xTile.Map>("Maps\\MarnieBarn");
                            Game1.locations.Add(new GameLocation("Maps\\MarnieBarn", "MarnieBarn"));
                            var marniebarn = Game1.getLocationFromName("MarnieBarn");
                            marniebarn.resetForPlayerEntry();
                            //this map has a bugged warp in it that needs to be replaced
                            var buggedwarp = marniebarn.warps.First();
                            buggedwarp.TargetName = "Forest";
                            buggedwarp.TargetX = 97;
                            buggedwarp.TargetY = 16;
                            //and lets add a warp from the yard back inside
                            var forest = Game1.getLocationFromName("Forest") as Forest;
                            forest.warps.Add(new StardewValley.Warp(96, 15, "MarnieBarn", 11, 13, false));
                            forest.warps.Add(new StardewValley.Warp(97, 15, "MarnieBarn", 11, 13, false));
                            forest.warps.Add(new StardewValley.Warp(98, 15, "MarnieBarn", 11, 13, false));
                            forest.warps.Add(new StardewValley.Warp(99, 15, "MarnieBarn", 11, 13, false));
                            marniebarn.warps.Add(new Warp(3, 9, "AnimalShop", 30, 14, false));
                        }
                        Warp(picked, "doorClose", "MarnieBarn", 192 / Game1.tileSize, 448 / Game1.tileSize);
                    }
                    break;
            }
        }

        private void OnMultiplayerPacket(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != Helper.Multiplayer.ModID) return;
            if (!Context.IsWorldReady) return; //don't pick locks on the title screen
            string key = e.ReadAs<string>();
            if (!LockCache.Contains(key))
            {
                string[] split = key.Split('^');
                string lockType = split[0];
                string location = split[1];
                int x = int.Parse(split[2]);
                int y = int.Parse(split[3]);
                if(location == Game1.currentLocation.NameOrUniqueName) Game1.playSound("stoneCrack"); Game1.playSound("axchop");
                if (lockType != "SkullDoor") LockCache.Add(key);
                if (lockType == "Door")
                {
                    Game1.getLocationFromName(location).map.GetLayer("Back").Tiles[x, y].Properties.Remove("TouchAction");
                }
            }
        }

        private void Warp(bool picked, string sound, string destination, int x, int y)
        {
            if (!picked) Game1.playSound(sound);
            Utils.WarpFarmer(destination, x, y);
        }
    }
}