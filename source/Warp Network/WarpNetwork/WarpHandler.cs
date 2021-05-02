/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework;
using SpaceCore.Events;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using System;
using System.Collections.Generic;

namespace WarpNetwork
{
    class WarpHandler
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static Config Config;
        private static GameLocation.afterQuestionBehavior QuestionResponder = new GameLocation.afterQuestionBehavior(answerQuestion);
        internal static bool FromWand = false;
        internal static bool ConsumeOnSelect = false;
        internal static Point? DesertWarp = null;
        public static Dictionary<string, CustomLocationHandler> CustomLocs = new Dictionary<string, CustomLocationHandler>(StringComparer.OrdinalIgnoreCase);
        internal static void Init(IMonitor monitor, IModHelper helper, Config config)
        {
            Monitor = monitor;
            Helper = helper;
            Config = config;
        }
        public static void ShowWarpMenu(string exclude = "")
        {
            if (!Config.MenuEnabled)
            {
                ShowFailureText();
                Monitor.Log("Warp menu is disabled in config, menu not displayed.");
                return;
            }

            //Game1.activeClickableMenu = new WarpSelectMenu();
            List<Response> dests = new List<Response>();
            Dictionary<String, WarpLocation> locs = Utils.GetWarpLocations();

            if (locs.ContainsKey(exclude))
            {
                if (!locs[exclude].Enabled && !Config.AccessFromDisabled)
                {
                    ShowFailureText();
                    Monitor.Log("Access from locked locations is disabled, menu not displayed.");
                    return;
                }
            }
            string normalized = exclude.ToLower();
            foreach (string id in locs.Keys)
            {
                WarpLocation loc = locs[id];
                string normid = id.ToLower();
                if (
                    !loc.AlwaysHide &&
                    !CustomLocs.ContainsKey(id) && (
                    normalized == "_force" || 
                    (loc.Enabled && normid != normalized) || 
                    (normid == "farm" && normalized == "_wand")
                    )
                )
                {
                    if(Game1.getLocationFromName(loc.Location) != null)
                    {
                        dests.Add(new Response(id, loc.Label));
                    } else
                    {
                        Monitor.Log("Invalid Location name '"+loc.Location+"'; skipping entry.", LogLevel.Warn);
                    }
                }
            }
            foreach (string id in CustomLocs.Keys)
            {
                CustomLocationHandler handler = CustomLocs[id];
                if(normalized == "_force" || (handler.GetEnabled(id) && id.ToLowerInvariant() != normalized))
                {
                    dests.Add(new Response(id, handler.GetLabel(id)));
                }
            }
            if(dests.Count == 0)
            {
                Monitor.Log("No valid warp destinations, menu not displayed.");
                ShowFailureText();
                return;
            }
            dests.Add(new Response("_", Game1.parseText(Helper.Translation.Get("ui-cancel"))));

            FromWand = normalized == "_wand";
            Game1.currentLocation.createQuestionDialogue(Game1.parseText(Helper.Translation.Get("ui-label")), dests.ToArray(), QuestionResponder);
            //Game1.drawObjectQuestionDialogue(Game1.parseText(Helper.Translation.Get("warpnet-label")), dests);
        }
        private static void answerQuestion(Farmer who, String answer)
        {
            if(answer == "_" || !who.IsLocalPlayer)
            {
                ConsumeOnSelect = false;
                FromWand = false;
                return;
            }
            if (CustomLocs.ContainsKey(answer))
            {
                if (ConsumeOnSelect)
                {
                    who.reduceActiveItemByOne();
                }
                ConsumeOnSelect = false;
                CustomLocs[answer].Warp(answer);
                FromWand = false;
            }
            else
            {
                Dictionary<String, WarpLocation> locs = Utils.GetWarpLocations();
                WarpLocation loc = locs[answer];
                if (!(loc is null) && !(Game1.getLocationFromName(loc.Location) is null))
                {
                    if (ConsumeOnSelect)
                    {
                        who.reduceActiveItemByOne();
                    }
                    ConsumeOnSelect = false;
                    WarpToLocation(loc);
                }
            }
        }
        private static void ShowFailureText()
        {
            Game1.drawObjectDialogue(Game1.parseText(Helper.Translation.Get("ui-fail")));
        }
        public static void HandleAction(object sender, EventArgsAction action)
        {
            if (action.Action.ToLower() == "warpnetwork")
            {
                Monitor.Log("Warp Network Activated");
                string[] args = action.ActionString.Split(' ');
                ShowWarpMenu((args.Length > 1) ? args[1] : "");
            }
            if (action.Action.ToLower() == "warpnetworkto")
            {
                Monitor.Log("Direct Warp Activated");
                string[] args = action.ActionString.Split(' ');
                DirectWarp(args);
            }
        }
        public static bool DirectWarp(String[] args)
        {
            if (args.Length > 1)
            {
                bool force = (args.Length > 2);
                return DirectWarp(args[1], force);
            }
            else
            {
                Monitor.Log("Warning! Map '" + Game1.currentLocation.Name + "' has invalid WarpNetworkTo property! Location MUST be specified!", LogLevel.Warn);
                return false;
            }
        }
        public static bool DirectWarp(string location, bool force)
        {
            if(location is null)
            {
                Monitor.Log("Destination is null! Cannot warp!", LogLevel.Error);
                ShowFailureText();
                return false;
            }
            if (CustomLocs.ContainsKey(location))
            {
                CustomLocationHandler cloc = CustomLocs[location];
                if(force || cloc.GetEnabled(location))
                {
                    cloc.Warp(location);
                    return true;
                } else
                {
                    ShowFailureText();
                    return false;
                }
            }
            Dictionary<String, WarpLocation> locs = Utils.GetWarpLocations();
            WarpLocation loc = locs[location];
            if (locs.ContainsKey(location))
            {
                if (!(Game1.getLocationFromName(loc.Location) is null))
                {
                    if (!Utils.IsFestivalAtLocation(loc.Location) || Utils.IsFestivalReady())
                    {
                        if (force || loc.Enabled)
                        {
                            WarpToLocation(loc);
                            return true;
                        }
                        else
                        {
                            ShowFailureText();
                            return false;
                        }
                    } else
                    {
                        Monitor.Log("Failed to warp to '" + loc.Location + "': Festival at location not ready.", LogLevel.Debug);
                        ShowFailureText();
                        return false;
                    }
                }
                else
                {
                    Monitor.Log("Failed to warp to '" + loc.Location + "': Location with that name does not exist!", LogLevel.Error);
                    ShowFailureText();
                    return false;
                }
            }
            else
            {
                Monitor.Log("Warp to '" + location + "' failed: warp network location not registered with that name", LogLevel.Warn);
                ShowFailureText();
                return false;
            }
        }
        private static void WarpToLocation(WarpLocation where)
        {
            int x = where.X;
            int y = where.Y;
            if (where.Location == "Farm")
            {
                if(FromWand)
                {
                    Point dest = GetFrontDoor(Game1.player);
                    FromWand = false;
                    DoWarpEffects(() => Game1.warpFarmer("Farm", dest.X, dest.Y, false));
                    return;
                }
                Point farmTotem = Utils.GetActualFarmPoint(x, y);
                x = farmTotem.X;
                y = farmTotem.Y;
            }
            FromWand = false;
            if(where.Location == "Desert")
            {
                //desert has bus scene hardcoded. Must warp to hardcoded spot, then use obelisk patch to move the player afterwards.
                if (!where.OverrideMapProperty)
                {
                    DesertWarp = Game1.getLocationFromName("Desert").GetMapPropertyPosition("WarpNetworkEntry", where.X, where.Y);
                } else
                {
                    DesertWarp = new Point(where.X, where.Y);
                }
                DoWarpEffects(() => Game1.warpFarmer("Desert", 35, 43, false));
            }
            if (!where.OverrideMapProperty)
            {
                Point coords = Game1.getLocationFromName(where.Location).GetMapPropertyPosition("WarpNetworkEntry", x, y);
                DoWarpEffects(() => Game1.warpFarmer(where.Location, coords.X, coords.Y, false));
            } else
            {
                DoWarpEffects(() => Game1.warpFarmer(where.Location, x, y, false));
            }
        }
        private static Point GetFrontDoor(Farmer who)
        {
            FarmHouse home = Utility.getHomeOfFarmer(who);
            if(!(home is null))
            {
                return home.getFrontDoorSpot();
            }
            return Game1.getLocationFromName("Farm").GetMapPropertyPosition("FarmHouseEntry", 64, 15);
        }
        private static void DoWarpEffects(Action action)
        {
            Farmer who = Game1.player;
            // reflection
            Multiplayer mp = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            // --
            for (int index = 0; index < 12; ++index)
                mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(
                    354,
                    Game1.random.Next(25, 75), 6, 1,
                    new Vector2(Game1.random.Next((int)who.Position.X - 256, (int)who.Position.X + 192), Game1.random.Next((int)who.Position.Y - 256, (int)who.Position.Y + 192)),
                    false,
                    Game1.random.NextDouble() < 0.5)
                    );
            who.currentLocation.playSound("wand", NetAudio.SoundContext.Default);
            Game1.displayFarmer = false;
            who.temporarilyInvincible = true;
            who.temporaryInvincibilityTimer = -2000;
            who.freezePause = 1000;
            Game1.flashAlpha = 1f;
            DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(() => {
                action();
                Game1.fadeToBlackAlpha = 0.99f;
                Game1.screenGlow = false;
                Game1.player.temporarilyInvincible = false;
                Game1.player.temporaryInvincibilityTimer = 0;
                Game1.displayFarmer = true;
            }), 1000);
            new Rectangle(who.GetBoundingBox().X, who.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
            int num = 0;
            for (int index = who.getTileX() + 8; index >= who.getTileX() - 8; --index)
            {
                mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(6, new Vector2(index, who.getTileY()) * 64f, Color.White, 8, false, 50f, 0, -1, -1f, -1, 0)
                {
                    layerDepth = 1f,
                    delayBeforeAnimationStart = num * 25,
                    motion = new Vector2(-0.25f, 0.0f)
                });
                ++num;
            }
        }
    }
}
