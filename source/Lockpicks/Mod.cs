using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using Modworks = bwdyworks.Modworks;

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace Lockpicks
{
    public class Mod : StardewModdingAPI.Mod
    {
        internal static bool Debug = false;
        [System.Diagnostics.Conditional("DEBUG")]
        public void EntryDebug() { Debug = true; }
        internal static string Module;

        internal static HashSet<string> LockCache;

        public override void Entry(IModHelper helper)
        {
            Module = helper.ModRegistry.ModID;
            EntryDebug();
            if (!Modworks.InstallModule(Module, Debug)) return;

            LockCache = new HashSet<string>();

            Modworks.Items.AddItem(Module, new bwdyworks.BasicItemEntry(this, "lockpick", 30, -300, "Basic", StardewValley.Object.junkCategory, "Lockpick", "Used to bypass locked doors."));
            Modworks.Items.AddMonsterLoot(Module, new bwdyworks.MonsterLootEntry(Module, "Green Slime", "lockpick", 0.3));
            Modworks.Items.AddMonsterLoot(Module, new bwdyworks.MonsterLootEntry(Module, "Green Slime", "lockpick", 0.15));
            Modworks.Items.AddTrashLoot(Module, new bwdyworks.Structures.TrashLootEntry(Module, "lockpick"));
            Modworks.Events.TileCheckAction += Events_TileCheckAction;
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            LockCache.Clear();
        }

        private bool DoesPlayerPossessLockpick()
        {
            int? lockpickId = Modworks.Items.GetModItemId(Module, "lockpick");
            if (!lockpickId.HasValue) return false;
            return Modworks.Player.HasItem(lockpickId.Value);
        }

        private void BreakLockpick()
        {
            int? lockpickId = Modworks.Items.GetModItemId(Module, "lockpick");
            if (!lockpickId.HasValue) return; //no idea how you pulled this off
            Modworks.Player.RemoveItem(lockpickId.Value);
            Game1.showRedMessage("The lockpick broke!");
            Game1.playSound("clank");
        }

        private string GenerateCacheKey(GameLocation location, float x, float y)
        {
            return location.Name + "^" + ((int)x).ToString() + "^" + ((int)y).ToString();
        }

        private void Events_TileCheckAction(object sender, bwdyworks.Events.TileCheckActionEventArgs args)
        {
            if (args.Cancelled) return; //already eaten by someone else

            var cacheKey = GenerateCacheKey(args.GameLocation, args.TileLocation.X, args.TileLocation.Y);
            bool cached = LockCache.Contains(cacheKey);
            if (!cached && !DoesPlayerPossessLockpick()) return; //we're done here

            var parameters = args.Action.Split(' ');
            if (parameters.Length < 1) return;
            //Modworks.Log.Alert("ACTION EVENT: " + args.Action);
            bool lockFound = false;
            if(parameters[0] == "LockedDoorWarp")
            {
                int unlockTime = int.Parse(parameters[4]);
                int lockTime = int.Parse(parameters[5]);
                if(Game1.timeOfDay < unlockTime || Game1.timeOfDay > lockTime)
                {
                    if (!cached) Modworks.Log.Trace("Locked warp door! Offering lockpick.");
                    lockFound = true;
                    args.Cancelled = true;
                }
            } else if(parameters[0] == "Door") {
                if (parameters.Length > 1)
                {
                    int friendship = Modworks.Player.GetFriendshipPoints(parameters[1]);
                    if (friendship < 500)
                    {
                        if(!cached) Modworks.Log.Trace("Locked bedroom door! Offering lockpick.");
                        lockFound = true;
                        args.Cancelled = true;
                    }
                }
            } else if (parameters[0] == "SkullDoor")
            {
                if (!args.Farmer.hasUnlockedSkullDoor)
                {
                    if (!cached) Modworks.Log.Trace("Locked Skull door! Offering lockpick.");
                    lockFound = true;
                    args.Cancelled = true;
                }
            }
            else if (parameters[0] == "WarpCommunityCenter")
            {
                if (!(Game1.MasterPlayer.mailReceived.Contains("ccDoorUnlock") || Game1.MasterPlayer.mailReceived.Contains("JojaMember")))
                {
                    if (!cached) Modworks.Log.Trace("Locked CC door! Offering lockpick.");
                    lockFound = true;
                    args.Cancelled = true;
                }
            }
            else if (parameters[0] == "WarpWomensLocker")
            {
                if (Game1.player.IsMale)
                {
                    if (!cached) Modworks.Log.Trace("Locked women's locker room door! Offering lockpick.");
                    lockFound = true;
                    args.Cancelled = true;
                }
            }
            else if (parameters[0] == "WarpMensLocker")
            {
                if (!Game1.player.IsMale)
                {
                    if (!cached) Modworks.Log.Trace("Locked men's locker room door! Offering lockpick.");
                    lockFound = true;
                    args.Cancelled = true;
                }
            }
            else if (parameters[0] == "WizardHatch")
            {
                if (!Game1.player.friendshipData.ContainsKey("Wizard") || Game1.player.friendshipData["Wizard"].Points < 1000)
                {
                    if (!cached) Modworks.Log.Trace("Locked wizard hatch! Offering lockpick.");
                    lockFound = true;
                    args.Cancelled = true;
                }
            }
            else if (parameters[0] == "EnterSewer")
            {
                if (!Game1.player.hasRustyKey && !Game1.player.mailReceived.Contains("OpenedSewer"))
                {
                    if (!cached) Modworks.Log.Trace("Locked sewer! Offering lockpick.");
                    lockFound = true;
                    args.Cancelled = true;
                }
            }
            if (lockFound)
            {
                if (cached)
                {
                    Modworks.Log.Trace("Opening cached lock (already picked today).");
                    OpenLock(args.Action.Split(' '), (int)args.TileLocation.X, (int)args.TileLocation.Y);
                }
                else OfferLockpick(args.Action, args.TileLocation);
            }
        }

        private void OpenLock(string[] Lock, int tileX, int tileY, bool picked = false)
        {
            if (picked)
            {
                Game1.playSound("stoneCrack");
                Game1.playSound("axchop");
                //add to cache
                LockCache.Add(GenerateCacheKey(Game1.currentLocation, tileX, tileY));
            }
            if (Lock[0] == "LockedDoorWarp")
            {
                int tx = int.Parse(Lock[1]);
                int ty = int.Parse(Lock[2]);
                Game1.player.warpFarmer(new Warp(tx, ty, Lock[3], tx, ty, false));
            }
            else if (Lock[0] == "Door")
            {
                Rumble.rumble(0.1f, 100f);
                Game1.currentLocation.openDoor(new xTile.Dimensions.Location(tileX, tileY), playSound: !picked);
                Game1.currentLocation.map.GetLayer("Back").Tiles[tileX, tileY].Properties["TouchAction"] = "";
                Game1.currentLocation.map.GetLayer("Back").Tiles[tileX, tileY].Properties.Remove("TouchAction");
            }
            else if (Lock[0] == "SkullDoor")
            {
                Game1.showRedMessage("The lock is too complex to open with a lockpick.");
            }
            else if (Lock[0] == "WarpCommunityCenter")
            {
                if (!picked) Game1.playSound("doorClose");
                Game1.warpFarmer("CommunityCenter", 32, 23, flip: false);
            }
            else if (Lock[0] == "WarpWomensLocker" || Lock[0] == "WarpMensLocker")
            { 
                if (Lock.Length < 5 && !picked) Game1.playSound("doorClose");
                Game1.warpFarmer(Lock[3], Convert.ToInt32(Lock[1]), Convert.ToInt32(Lock[2]), flip: false);
            } else if(Lock[0] == "WizardHatch")
            {
                if (!picked) Game1.playSound("doorClose");
                Game1.warpFarmer("WizardHouseBasement", 4, 4, flip: true);
            } else if(Lock[0] == "EnterSewer")
            {
                if (!picked) Game1.playSound("stairsdown");
                Game1.warpFarmer("Sewer", 16, 11, 2);
            }
        }

        private void OfferLockpick(string LockAction, Vector2 TileLocation)
        {
            string key = string.Join("^", new[] { "l", ((int)TileLocation.X).ToString(), ((int)TileLocation.Y).ToString(), LockAction });
            Modworks.Menus.AskQuestion("Use lockpick?", new[] { new Response(key, "Yes"), new Response("No", "No") }, AcceptLockpick);
        }

        public void AcceptLockpick(Farmer who, string answerKey)
        {
            if (answerKey == "No") return;
            if (!answerKey.StartsWith("l^")) return; //wtf
            var parameters = answerKey.Substring(2).Split('^');
            if (parameters.Length != 3) return; //also wtf
            int tileX = int.Parse(parameters[0]);
            int tileY = int.Parse(parameters[1]);
            var Lock = parameters[2].Split(' ');
            var loc = Game1.currentLocation;
            if (Modworks.RNG.NextDouble() > 0.4f + Modworks.Player.GetLuckFactorFloat()) BreakLockpick();
            else OpenLock(Lock, tileX, tileY, true);
        }
    }
}