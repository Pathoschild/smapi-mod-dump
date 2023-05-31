/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using Microsoft.Xna.Framework.Graphics;
using SpousesIsland.ModContent;

namespace SpousesIsland
{
    internal static class Changes
    {
        internal static void DayStart(object sender, DayStartedEventArgs e)
        {
            if (!ModEntry.IslandToday) return;
            
            var inviteds = ModEntry.Status.Who;
            if (inviteds?.Count == 0 || inviteds == null)
                return;

            foreach(var spouse in inviteds)
            {
                //if not allowed
                if(Values.IsIntegrated(spouse) && !ModEntry.MarriedAndAllowed.Contains(spouse))
                {
                    continue;
                }

                ChangeSchedule(spouse);
            }

            if (ModEntry.Children?.Count == 0 || ModEntry.Children == null)
                return;

            Devan.CorrectSchedule();

            if(!ModEntry.InstalledMods["C2N"] && !ModEntry.InstalledMods["LNPCs"])
                return;

            if (!BedCode.HasAnyKidBeds() && ModEntry.Config.UseFurnitureBed)
            {
                ModEntry.Mon.Log("There's no child beds in island farmhouse. Farmer's kids won't visit.",LogLevel.Warn);
                return;
            }
            
            foreach(var kid in ModEntry.Children)
            {
                ChangeSchedule(kid.Name);
            }
        }

        /// <summary>
        /// Changes a NPC's schedule to IslandVisit one.
        /// </summary>
        /// <param name="who">npc whose schedule to edit.</param>
        private static void ChangeSchedule(string who)
        {
            var npc = Utility.fuzzyCharacterSearch(who,false);

            //stop any npc action
            npc.Halt();
            npc.followSchedule = false;
            npc.clearSchedule();

            if (npc.hasMasterScheduleEntry("IslandVisit"))
            {
                var raw = npc.getMasterScheduleEntry("IslandVisit");
                var rawFixed = raw.Replace("Custom_Random 0 0 0", Values.RandomOrDefault(who));

                //set data
                var schedule = npc.parseMasterSchedule(rawFixed);
                npc.Schedule = schedule;
                npc.followSchedule = true;
            }
            else
            {
                var home = Information.GetReturnPoint("mod");
                var homepoint = $"{home.X} {home.Y}";

                var schedule = $"620 IslandFarmHouse {homepoint}/920 {Values.RandomFree()}/1300 {Values.RandomFree()}/1630 {Values.RandomFree()}/a2150 IslandFarmHouse {homepoint}";

                //set data
                npc.Schedule = npc.parseMasterSchedule(schedule);
                npc.followSchedule = true;
            }
        }

        //moved to a new file. it's not many lines, but i access them a lot (and scrolling past GameLaunched gets annoying by the 100th time).
        internal static void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            //avoid running unnecessary code
            if (ModEntry.IslandToday == false)
                return;

            switch (e.NewTime)
            {
                case >= 800 and <= 900:
                {
                    if(ModEntry.Config.IslandClothes)
                    {
                        foreach(var name in ModEntry.Status.Who)
                        {
                            var npc = Game1.getCharacterFromName(name);
                            //npc.wearIslandAttire();
                            ToggleIslandClothes(npc,true);
                        }
                    }

                    break;
                }
                //if 10pm or later. code for npcs to sleep
                case >= 2200:
                {
                    var ifh = Game1.getLocationFromName("IslandFarmHouse");

                    //first, correct any NOT in the farmhouse
                    foreach(var spouse in ModEntry.Status.Who)
                    {
                        var s = Utility.fuzzyCharacterSearch(spouse, false);
                        if (s.currentLocation.Equals(ifh)) continue;
                        
                        ModEntry.Mon.Log($"Correcting NPC location ({spouse}) to farmhouse.");
                        Game1.warpCharacter(s, ifh.Name, new Point(14,14));
                    }

                    //then, path them to bed
                    foreach (var c in ifh.characters)
                    {
                        if (c.isMarried())
                        {
                            ModEntry.Mon.Log($"Pathing {c.Name} to bed in {ifh.Name}...");
                            try
                            {
                                BedCode.MakeSpouseGoToBed(c, ifh);
                            }
                            catch (Exception ex)
                            {
                                ModEntry.Mon.Log($"An error ocurred while pathing {c.Name} to bed: {ex}");
                            }
                        }

                        else if (!c.isMarried() && !c.isRoommate() && (ModEntry.InstalledMods["C2N"] || ModEntry.InstalledMods["LNPCs"]))
                        {
                            ModEntry.Mon.Log($"Pathing {c.Name} to kid bed in {ifh.Name}...");
                            try
                            {
                                BedCode.MakeKidGoToBed(c, ifh);
                            }
                            catch (Exception ex)
                            {
                                ModEntry.Mon.Log($"An error ocurred while pathing {c.Name} to bed: {ex}");
                            }
                        }
                    }

                    break;
                }
            }

            if (!ModEntry.DevanExists) return; //devan exists
            switch (e.NewTime)
            {
                //if >910 && <930, walk to baby
                case > 910 and < 930:
                    Devan.WalkTo(ModEntry.Children[0]);
                    break;
                case > 1300 and < 1400:
                {
                    //walk to kid(s). use random to choose
                    var whichkid = ModEntry.Random.Next(ModEntry.Children.Count);
                    Devan.WalkTo(ModEntry.Children[whichkid]);
                    break;
                }
                case > 1500 and < 1700:
                    Devan.Wander(8);
                    break;
                case 2300:
                {
                    //warp out of house (if player present, door sound). once at bus stop pos, walk to saloon and then sleep (requires followschedule=false)
                    var devan = Utility.fuzzyCharacterSearch("Devan", false);

                    Game1.warpCharacter(devan, "BusStop", new Point(1,23));
                    
                    if (Game1.currentLocation.Equals(Utility.getHomeOfFarmer(Game1.player)))
                    {
                        Game1.currentLocation.playSound("doorClose", StardewValley.Network.NetAudio.SoundContext.NPC);
                    }

                    devan.Halt();
                    devan.followSchedule = false;
                    devan.clearSchedule();

                    devan.controller = new PathFindController(devan,Game1.getLocationFromName("Saloon"),new Point(44,5),2);
                    break;
                }
            }
        }

        private static void ToggleIslandClothes(NPC npc, bool islandClothes)
        {
            var spritename = "Characters\\" + npc.Name;
            var frame = npc.Sprite.CurrentFrame;
            var w = npc.Sprite.getWidth();
            var h = npc.Sprite.getHeight();

            switch (islandClothes)
            {
                case true:
                    //turn to beach clothes
                    try
                    {
                        npc.Sprite.LoadTexture(spritename + "_Beach");
                        npc.Sprite = new AnimatedSprite(spritename + "_Beach", frame, w, h);

                        var beach = Game1.content.Load<Texture2D>("Portraits\\" + npc.Name + "_Beach");
                        npc.Portrait = beach;
                    }
                    catch (Exception ex)
                    {
                        npc.Sprite.LoadTexture("Characters\\" + npc.Name);
                        ModEntry.Mon.Log($"An error happened while loading beach sprite: {ex}", LogLevel.Error);
                    }

                    break;
                case false:
                    //turn to normal
                    try
                    {
                        npc.Sprite.LoadTexture(spritename);
                        npc.Sprite = new AnimatedSprite(spritename, frame, w, h);

                        var normal = Game1.content.Load<Texture2D>("Portraits\\" + npc.Name);
                        npc.Portrait = normal;
                    }
                    catch (Exception ex)
                    {
                        npc.Sprite.LoadTexture("Characters\\" + npc.Name);
                        ModEntry.Mon.Log($"An error happened while loading normal sprite: {ex}", LogLevel.Error);
                    }

                    break;
                default:
                    //idk. log error
                    throw new ArgumentException();
            }
        }

        internal static void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (ModEntry.IslandToday == false)
                return;

            /* only run once per TWO seconds.
             * 1h = 7s, code runs for 2 hours (3.5 per game 10m)
             * 7 x 2 x 3.5 = 49 per day.
             * Still, it runs 2/3 less than 'ontimechange' code. So i hope it's fine for now.
             */

            if (e.IsMultipleOf(120) == false)
                return;

            switch (Game1.timeOfDay)
            {
                //dont run after 10pm
                case > 2210:
                    return;
                case >= 2000:
                    try
                    {
                        var islandW = Game1.getLocationFromName("IslandWest");
                        var islandS = Game1.getLocationFromName("IslandSouth");
                        var supposedtilelocation = new Point(77, 41);

                        foreach (var chara in ModEntry.PatchPathfind)
                        {
                            var npc = Utility.fuzzyCharacterSearch(chara, false);

                            //to avoid looping
                            /*if (npc.isMoving())
                            continue;*/

                            var inIsland = npc?.currentLocation?.GetLocationContext() == GameLocation.LocationContext.Island;

                            if (!inIsland)
                            {
                                continue;
                            }

                            if(npc?.getTileLocationPoint() != supposedtilelocation) // && npc?.currentLocation != islandW
                            {
                                continue;
                            }

                            ModEntry.Mon.Log("Attempting to fix null endpoint in schedule...", LogLevel.Debug);

                            //clear everything just in case
                            npc.followSchedule = false;
                            npc.controller = null;
                            npc.Schedule.Clear();
                            npc.queuedSchedulePaths.Clear();

                            //warp to entrance
                            Game1.warpCharacter(npc, "IslandFarmHouse", new Point(14, 15));

                            if (Game1.player.currentLocation.Equals(islandW) || Game1.player.currentLocation.Name == "IslandFarmHouse")
                            {
                                Game1.playSound("doorClose");
                            }
                            if(ModEntry.Config.IslandClothes)
                            {
                                //npc.wearNormalClothes(); //return to normal
                                ToggleIslandClothes(npc, false);
                            }

                            ModEntry.Mon.Log("Warped to islandFarmHouse.", LogLevel.Debug);

                            npc.controller = new PathFindController
                            (npc,
                                Game1.getLocationFromName("IslandFarmHouse"),
                                Information.GetReturnPoint(npc.Name),
                                0);

                            if (ModEntry.Config.Debug)
                            {
                                ModEntry.Mon.Log($"Controller information: endpoint {npc.controller.endPoint}, location {npc.controller.location.Name}", LogLevel.Debug);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        ModEntry.Mon.Log("Error: " + ex, LogLevel.Error);
                    }

                    break;
            }
        }
    }
}