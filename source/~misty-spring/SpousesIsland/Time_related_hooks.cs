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

namespace SpousesIsland
{
    internal class Changes
    {
        internal static void DayStart(object sender, DayStartedEventArgs e)
        {
            if(ModEntry.IslandToday)
            {

                var inviteds = ModEntry.Status[ModEntry.Player_MP_ID].Who;
                
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

                if (ModEntry.Children?.Count == 0 || ModEntry.Children == null || ModEntry.HasC2N_Or_LNPCs == false)
                    return;

                foreach(var kid in ModEntry.Children)
                {
                    ChangeSchedule(kid.Name);
                }
            }
        }

        /// <summary>
        /// Changes a NPC's schedule to IslandVisit one.
        /// </summary>
        /// <param name="who">npc whose schedule to edit.</param>
        internal static void ChangeSchedule(string who)
        {
            var npc = Game1.getCharacterFromName(who);

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

            if(e.NewTime >= 800 && e.NewTime <= 900)
            {
                if(ModEntry.IslandAtt)
                {
                    foreach(var name in ModEntry.Status[ModEntry.Player_MP_ID].Who)
                    {
                        var npc = Game1.getCharacterFromName(name);
                        //npc.wearIslandAttire();
                        ToggleIslandClothes(npc,true);
                    }
                }
            }
            //if 10pm or later. code for npcs to sleep
            if (e.NewTime >= 2200)
            {
                var bc = new BedCode();
                var ifh = Game1.getLocationFromName("IslandFarmHouse");

                foreach (NPC c in ifh.characters)
                {
                    if (c.isMarried())
                    {
                        ModEntry.Mon.Log($"Pathing {c.Name} to bed in {ifh.Name}...");
                        try
                        {
                            bc.MakeSpouseGoToBed(c, ifh);
                        }
                        catch (Exception ex)
                        {
                            ModEntry.Mon.Log($"An error ocurred while pathing {c.Name} to bed: {ex}");
                        }
                    }

                    else if (!c.isMarried() && !c.isRoommate() && ModEntry.HasC2N_Or_LNPCs == true)
                    {
                        ModEntry.Mon.Log($"Pathing {c.Name} to kid bed in {ifh.Name}...");
                        try
                        {
                            bc.MakeKidGoToBed(c, ifh);
                        }
                        catch (Exception ex)
                        {
                            ModEntry.Mon.Log($"An error ocurred while pathing {c.Name} to bed: {ex}");
                        }
                    }
                }
            }
        }

        private static void ToggleIslandClothes(NPC npc, bool islandClothes)
        {
            string spritename = "Characters\\" + npc.Name;
            int frame = npc.Sprite.CurrentFrame;
            int w = npc.Sprite.getWidth();
            int h = npc.Sprite.getHeight();

            if (islandClothes)
            {
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
            }
            else if(islandClothes == false)
            {
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
            }
            else
            {
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

            //dont run after 10pm
            if (Game1.timeOfDay > 2210)
                return;

            if (Game1.timeOfDay >= 2000)
            {
                try
                {
                    var islandW = Game1.getLocationFromName("IslandWest");
                    var islandS = Game1.getLocationFromName("IslandSouth");
                    var supposedtilelocation = new Point(77, 41);

                    foreach (var chara in ModEntry.MustPatchPF)
                    {
                        var npc = Game1.getCharacterFromName(chara, false, false);

                        //to avoid looping
                        /*if (npc.isMoving())
                            continue;*/

                        var inIsland = npc?.currentLocation?.GetLocationContext() == GameLocation.LocationContext.Island;

                        if (!inIsland)
                        {
                            continue;
                        }

                        /* this didnt work rip
                        bool positioned = npc?.getTileLocationPoint() == new Point(77, 41);

                        if (!positioned && npc.currentLocation != islandW)
                        {
                            continue;
                        }
                        */

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

                        if (Game1.player.currentLocation == islandW || Game1.player.currentLocation.Name == "IslandFarmHouse")
                        {
                            Game1.playSound("doorClose");
                        }
                        if(ModEntry.IslandAtt)
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

                        if (ModEntry.IsDebug)
                        {
                            ModEntry.Mon.Log($"Controller information: endpoint {npc.controller.endPoint}, location {npc.controller.location.Name}", LogLevel.Debug);
                        }
                    }
                }
                catch(Exception ex)
                {
                    ModEntry.Mon.Log("Error: " + ex, LogLevel.Error);
                }
            }
        }
    }
}