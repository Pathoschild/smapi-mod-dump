/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cl4r3/Halloween-Mod-Jam-2023
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using xTile.ObjectModel;
using static TricksAndTreats.Globals;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TricksAndTreats
{
    public class Tricks
    {
        static IModHelper Helper;
        static IMonitor Monitor;
        static Random r;
        static int mystery_id = -1;

        internal static void Initialize(IMod ModInstance)
        {
            r = new();

            Helper.Events.GameLoop.SaveLoaded += CheckTricksters;
            Helper.Events.Content.AssetRequested += HouseTrick;
            Helper.Events.Multiplayer.PeerConnected += (object sender, PeerConnectedEventArgs e) => { CheckHouseTrick(); };
            Helper.Events.Input.ButtonPressed += OpenHalloweenChest;
            
        }

        internal static void OpenHalloweenChest(object sender, ButtonPressedEventArgs e)
        {
            if (!(Game1.currentSeason == "fall" && Game1.dayOfMonth == 27 && Game1.currentLocation.NameOrUniqueName == "Temp" && Game1.CurrentEvent.isFestival))
                return;
            if (e.Button.IsActionButton() && e.Cursor.GrabTile.X == 33 && e.Cursor.GrabTile.Y == 13)
            {
                if (Game1.currentLocation.Objects.ContainsKey(new Vector2(33, 13)))
                {
                    if (!Game1.player.modData.TryGetValue(StolenKey, out string val))
                    {
                        Log.Trace("TaT: Player hasn't had any items stolen");
                        return;
                    }
                    Game1.currentLocation.localSound("openChest");
                    List<Item> items = new();
                    if (!Game1.player.modData.ContainsKey(ChestKey) || Game1.player.modData[ChestKey] != "true")
                    {
                        items.Add(new StardewValley.Object(373, 1));
                        var logged = val.Split('\\');
                        foreach (string item in logged)
                        {
                            var idnstack = item.Split(' ');
                            StardewValley.Object obj = new(int.Parse(idnstack[0]), int.Parse(idnstack[1]));
                            items.Add(obj);
                        }
                        Game1.activeClickableMenu = new ItemGrabMenu(items).setEssential(essential: true);
                        (Game1.activeClickableMenu as ItemGrabMenu).source = 3;
                        Game1.player.completelyStopAnimatingOrDoingAction();

                        // after menu closed
                        Game1.player.modData[ChestKey] = "true";
                        Game1.player.modData.Remove(StolenKey);
                        if (!Game1.IsMultiplayer)
                            Game1.currentLocation.Objects.Remove(new Vector2(33, 13));
                        else
                            Game1.currentLocation.localSound("doorCreakReverse");
                    }
                }
            }
        }

        [EventPriority(EventPriority.Low)]
        private static void CheckTricksters(object sender, SaveLoadedEventArgs e)
        {
            foreach (KeyValuePair<string, Celebrant> entry in NPCData)
            {
                if (entry.Value.Roles.Contains("trickster"))
                {
                    NPC npc = Game1.getCharacterFromName(entry.Key);

                    if (!npc.Dialogue.ContainsKey("hated_treat"))
                    {
                        npc.Dialogue.Add("hated_treat", Helper.Translation.Get("generic.hated_treat"));
                    }

                    foreach (string trick in entry.Value.PreferredTricks)
                    {
                        if (trick == "all")
                            continue;
                        if (!npc.Dialogue.ContainsKey("before_" + trick))
                        {
                            npc.Dialogue.Add("before_" + trick, Helper.Translation.Get("generic.before_" + trick));
                        }
                        npc.Dialogue["before_" + trick] = npc.Dialogue["hated_treat"] + "#$b#" + npc.Dialogue["before_" + trick];
                    }
                }
            }
        }

        [EventPriority(EventPriority.Low)]
        private static void HouseTrick(object sender, AssetRequestedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.currentSeason == "fall" && Game1.dayOfMonth == 28)
            {
                if (!Config.AllowEgging && !Config.AllowTPing)
                {
                    Log.Trace("TaT: Neither TPing nor egging allowed, so no prank will be pulled.");
                    return;
                }

                if (Game1.MasterPlayer.mailReceived.Contains(HouseFlag)) 
                {
                    if (e.Name.IsEquivalentTo("Buildings/houses"))
                    {
                        Log.Trace("TaT: Pranking main farmhouse...");
                        if (Config.AllowEgging)
                        {
                            e.Edit(asset =>
                            {
                                var editor = asset.AsImage();
                                var sourceImage = Helper.ModContent.Load<IRawTextureData>("assets/egged-houses.png");
                                editor.PatchImage(sourceImage, sourceArea: new Rectangle(0, 0, 272, 432), targetArea: new Rectangle(0, 0, 272, 432), patchMode: PatchMode.Overlay);
                            });
                        }
                        if (Config.AllowTPing)
                        {
                            e.Edit(asset =>
                            {
                                var editor = asset.AsImage();
                                var sourceImage = Helper.ModContent.Load<IRawTextureData>("assets/tp-houses.png");
                                editor.PatchImage(sourceImage, sourceArea: new Rectangle(0, 0, 272, 432), targetArea: new Rectangle(0, 0, 272, 432), patchMode: PatchMode.Overlay);
                            });
                        }
                    }
                }

                foreach(Building place in Game1.getFarm().buildings)
                {
                    //Log.Debug($"TaT: Building is {place.nameOfIndoors}, texture is {place.textureName()}");
                    if (e.Name.IsEquivalentTo(place.textureName()) && place.indoors.Value is Cabin)
                    {
                        FarmHouse home = (FarmHouse)place.indoors.Value;
                        if (home.owner is not null && home.owner.mailReceived.Contains(HouseFlag)) 
                        {
                            Log.Trace($"TaT: Pranking {home.owner.Name}'s cabin...");
                            if (Config.AllowEgging)
                            {
                                e.Edit(asset =>
                                {
                                    var editor = asset.AsImage();
                                    IRawTextureData sourceImage = Helper.ModContent.Load<IRawTextureData>("assets/egged-cabin.png");
                                    editor.PatchImage(sourceImage, sourceArea: new Rectangle(0, 0, 240, 112), targetArea: new Rectangle(0, 0, 240, 112), patchMode: PatchMode.Overlay);
                                });
                            }
                            if (Config.AllowTPing)
                            {
                                Log.Trace($"TaT: TPing {Game1.player.Name}'s cabin...");
                                string img = place.textureName().Split('\\')[1].Replace(' ', '-').ToLower() + ".png";
                                e.Edit(asset =>
                                {
                                    var editor = asset.AsImage();
                                    IRawTextureData sourceImage = Helper.ModContent.Load<IRawTextureData>("assets/tp-" + img);
                                    editor.PatchImage(sourceImage, sourceArea: new Rectangle(0, 0, 240, 112), targetArea: new Rectangle(0, 0, 240, 112), patchMode: PatchMode.Overlay);
                                });
                            }
                        }
                    }
                }
            }
        }

        internal static void CheckHouseTrick()
        {
            if ((Game1.currentSeason == "winter" && Game1.dayOfMonth == 1) || (Game1.currentSeason == "fall" && Game1.dayOfMonth == 28))
            {
                Helper.GameContent.InvalidateCache("Buildings/houses");
                Helper.GameContent.InvalidateCache("Buildings/Log Cabin");
                Helper.GameContent.InvalidateCache("Buildings/Plank Cabin");
                Helper.GameContent.InvalidateCache("Buildings/Stone Cabin");
            }
        }

        internal static void SmallTrick(NPC npc)
        {
            Farmer farmer = Game1.player;
            // Get enabled tricks only
            var tricks = NPCData[npc.Name].PreferredTricks.Where(x => Config.SmallTricks[x]).ToArray();
            if (tricks.Length == 0)
            {
                Log.Trace($"TaT: NPC {npc.Name} has no valid tricks to choose from, therefore will not play a trick.");
                return;
            }

            string trick = tricks[r.Next(tricks.Length)];
            Utils.Speak(npc, "before_" + trick);

            Game1.afterDialogues = delegate
            { 
                string after_trick = "after_" + trick;
                switch (trick)
                {
                    case "steal":
                        if (!StealTrick(farmer))
                            after_trick = "fail_steal";
                        break;
                    case "paint":
                        PaintTrick(farmer);
                        break;
                    case "maze":
                        MazeTrick(farmer, npc);
                        break;
                    case "nickname":
                        NicknameTrick(farmer);
                        break;
                    case "mystery":
                        if (!MysteryTrick(farmer))
                            after_trick = "fail_mystery";
                        break;
                    //case "cobweb": break;
                    default:
                        Log.Error($"TaT: Unhandled small trick option {trick} for {npc.Name}");
                        break;
                }
                DelayedAction.functionAfterDelay(
                    () =>
                    {
                        farmer.freezePause = 2000;
                        farmer.Halt();
                        farmer.CanMove = false;
                        Utils.Speak(npc, after_trick);
                        farmer.CanMove = true;
                    },
                    2000 // delay in milliseconds
                );
            };
        }
        
        /*
        // Disabled till I can figure out graphics
        internal static void CobwebTrick(Farmer farmer)
        {
            if (!farmer.modData.ContainsKey(CobwebKey) || int.Parse(farmer.modData[CobwebKey]) > farmer.Speed*-1)
            {
                farmer.addedSpeed -= Math.Max(farmer.Speed * -1, (farmer.Speed + farmer.addedSpeed) / 2);
                farmer.modData.Add(CobwebKey, farmer.addedSpeed.ToString());
            }
        }
        */

        internal static void NicknameTrick(Farmer farmer)
        {
            var nicknames = Helper.Translation.Get("tricks.dumb_names").ToString().Split(',');
            farmer.Name = nicknames[r.Next(nicknames.Length)].Trim();
        }

        internal static bool MysteryTrick(Farmer farmer)
        {
            if (mystery_id == -1)
                mystery_id = JA.GetObjectId("TaT.mystery-treat");
            int wrapped = 0;
            for (int i = 0; i < farmer.MaxItems; i++)
            {
                Item item = farmer.Items[i];
                if (item is not null && TreatData.ContainsKey(item.Name))
                {
                    var tmp = new StardewValley.Object(mystery_id, farmer.Items[i].Stack);
                    tmp.modData.Add(MysteryKey, item.Name);
                    farmer.Items[i] = tmp;
                    wrapped++;
                }
            }
            return wrapped > 0;
        }

        internal static void MazeTrick(Farmer farmer, NPC warper)
        {
            GameLocation where = null;
            string[] start = null;
            string[] end = null;
            bool use_default = false;
            try
            {
                where = Game1.getLocationFromName("Custom_TaT_Maze-" + warper.Name);
                start = where.Map.Properties["MazeStart"].ToString().Split(" ");
                end = where.Map.Properties["MazeEnd"].ToString().Split(" ");
            }
            catch (Exception e)
            {
                use_default = true;
                Log.Error("TaT: Error while setting up maze. Will use default maze instead.");
                Log.Error(e.Message);
            }

            if (use_default || where is null || start is null || start.Length != 3 || end is null || end.Length != 2)
            {
                where = Game1.getLocationFromName("Custom_TaT_Maze");
                start = where.Map.Properties["MazeStart"].ToString().Split(" ");
                end = where.Map.Properties["MazeEnd"].ToString().Split(" ");
            }

            if (where.Map.Properties.ContainsKey("Warp"))
                where.Map.Properties.Remove("Warp");
            where.Map.Properties.Add("Warp", $"{end[0]} {end[1]} {farmer.currentLocation.Name} {farmer.getTileX()} {farmer.getTileY()}");
            where.updateWarps();

            Game1.warpFarmer(where.Name, int.Parse(start[0]), int.Parse(start[1]), int.Parse(start[2]));
        }          

        internal static void PaintTrick(Farmer farmer)
        {
            if (!farmer.modData.ContainsKey(PaintKey))
                farmer.modData.Add(PaintKey, farmer.skinColor.ToString());
            farmer.changeSkinColor(r.Next(17, 23), true);
            farmer.currentLocation.localSound("slimedead");
        }

        internal static bool StealTrick(Farmer farmer)
        {
            int dud_item = JA.GetObjectId("TaT.rotten-egg");
            List<int> stealables = new();
            for (int i = 0; i < farmer.MaxItems; i++)
            {
                Item item = farmer.Items[i];
                //Log.Debug("TaT: Now checking stealability of item at location " + i);
                if (item is not null && item is not Tool && !TreatData.ContainsKey(item.Name) &&
                    item.ParentSheetIndex != dud_item && Utility.IsNormalObjectAtParentSheetIndex(item, item.ParentSheetIndex))
                {
                    //Log.Debug($"TaT: Adding {item.Name} at index {i} to stealables");
                    stealables.Add(i);
                }
            }
            if (stealables.Count == 0)
                return false;

            int idx = stealables[r.Next(stealables.Count)];
            string egg_val = farmer.Items[idx].ParentSheetIndex.ToString() + " " + farmer.Items[idx].Stack;

            if (!farmer.modData.ContainsKey(StolenKey))
                farmer.modData.Add(StolenKey, egg_val);
            else
                farmer.modData[StolenKey] = farmer.modData[StolenKey] + "\\" + egg_val;

            farmer.Items[idx] = new StardewValley.Object(dud_item, 1);
            farmer.currentLocation.localSound("shwip");
            stealables.Clear();
            return true;
        }
    }
}