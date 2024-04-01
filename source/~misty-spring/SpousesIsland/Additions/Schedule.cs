/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Pathfinding;

namespace SpousesIsland.Additions;

internal static class Schedule
{
    internal static void Babysitter()
    {
        /*if (Beds.HasCrib(Game1.player))
            return;

        ModEntry.Mon.Log("No cribs found. reloading schedule...", LogLevel.Debug);*/

        var npc = Game1.getCharacterFromName("DevanSpring");
        var house = Utility.getHomeOfFarmer(Game1.player);
        var entry = house.getEntryLocation().ToVector2();
        entry.Y--;
        Game1.warpCharacter(npc, house, entry);
        var broom = Maps.RandomPoint(house);

        string scheduleData;

        var kitchen = house.getKitchenStandingSpot();
        if (Beds.HasCrib(Game1.player))
        {
            var crib = house.GetCribBounds() ?? new Rectangle(15, 2, 3, 4);
            var cribPoint = new Point(crib.X + crib.Width - 1, crib.Y + crib.Height + 1);

            scheduleData =
                $"610 FarmHouse {entry.X} {entry.Y - 2}/630 FarmHouse {cribPoint.X} {cribPoint.Y} 0/730 FarmHouse {kitchen.X + 2} {kitchen.Y} 0/800 FarmHouse {kitchen.X - 2} {kitchen.Y} 0 devan_spoon/830 FarmHouse {kitchen.X - 2} {kitchen.Y} 2 devan_bottle/900 FarmHouse {kitchen.X + 1} {kitchen.Y} 0 devan_washing/1150 FarmHouse {kitchen.X} {kitchen.Y} 0 devan_cook/1230 FarmHouse {kitchen.X} {kitchen.Y} 2 devan_plate/1420 FarmHouse 8 14 0 devan_washing/1510 FarmHouse {broom.X} {broom.Y} 2 devan_broom/1600 FarmHouse {broom.X} {broom.Y + 1} 2/2130 FarmHouse {cribPoint.X} {cribPoint.Y} 0";
        }
        else
        {
            scheduleData =
                $"610 FarmHouse {entry.X} {entry.Y - 2}/630 FarmHouse {kitchen.X} {kitchen.Y} 2/730 FarmHouse {kitchen.X + 2} {kitchen.Y} 0/800 FarmHouse {kitchen.X - 2} {kitchen.Y} 0 devan_spoon/830 FarmHouse {kitchen.X - 2} {kitchen.Y} 2 devan_bottle/900 FarmHouse {kitchen.X + 1} {kitchen.Y} 0 devan_washing/1150 FarmHouse {kitchen.X} {kitchen.Y} 0 devan_cook/1230 FarmHouse {kitchen.X} {kitchen.Y} 2 devan_plate/1420 FarmHouse 8 14 0 devan_washing/1510 FarmHouse {broom.X} {broom.Y} 0 devan_broom/1600 FarmHouse {broom.X} {broom.Y + 1} 2";
        }

        if (Game1.player.friendshipData.TryGetValue("DevanSpring", out var friendship) == false || friendship is null)
        {
            //get spouse. if no spouse (shouldn't happen), default to penny
            var companion = Game1.player.getSpouse() ?? Game1.getCharacterFromName("Penny");
            //get baby. if none (shouldn't happen), default to pet
            Character baby;
            try
            {
                baby = Game1.random.ChooseFrom(Game1.player.getChildren());
            }
            catch (Exception)
            {
                baby =  Game1.player.getPet();
            }
            
            var text = string.Format(npc.Dialogue["intro"], companion.displayName, baby.displayName);
            var dialogue = new Dialogue(npc, null, text);
            npc.CurrentDialogue.Clear();
            npc.CurrentDialogue.Push(dialogue);
        }
        Change(npc, scheduleData);
    }
    
    /// <summary>
    /// Changes a NPC's schedule to IslandVisit one.
    /// </summary>
    /// <param name="npc">npc whose schedule to edit.</param>
    internal static void Island(NPC npc)
    {
        //Dictionary<int, SchedulePathDescription> schedule;
        string raw;

        //if has schedule AND (no randomized OR nextbool)
        if (npc.hasMasterScheduleEntry("IslandVisit") && (ModEntry.Config.ScheduleRandom == false || Game1.random.NextBool()))
        {
            var rawUnparsed = npc.getMasterScheduleEntry("IslandVisit");
            raw = rawUnparsed.Replace("$random_island", Maps.RandomOrDefault(npc.Name));
            //schedule = npc.parseMasterSchedule("IslandVisit", raw);
        }
        else
        {
            var home = Maps.RandomPoint("IslandFarmHouse");
            var homepoint = $"{home.X} {home.Y}";
            raw = $"610 IslandFarmHouse {homepoint}/920 {Maps.RandomFree()}/1300 {Maps.RandomFree()}/1630 {Maps.RandomFree()}/a2150 IslandFarmHouse {homepoint}";
            //schedule = npc.parseMasterSchedule("IslandVisit", raw);
        }

        Change(npc, raw, true);
    }

    private static void Change(NPC npc, string path, bool isSpouse = false)
    {
        //stop any npc action
        npc.Halt();
        npc.followSchedule = false;

        npc.Schedule?.Clear();
        npc.TryLoadSchedule("IslandVisit", path);
        npc.followSchedule = true;

        if (!isSpouse) 
            return;
        try
        {
            /*
            var dialogues = npc.currentMarriageDialogue;*/
            npc.CurrentDialogue?.Clear();
            npc.currentMarriageDialogue?.Clear();
            npc.addMarriageDialogue("MarriageDialogue", "funLeave_" + npc.Name);
            /*
            if (dialogues is not null && dialogues.Any())
            {
                npc.currentMarriageDialogue?.AddRange(dialogues);
            }*/
        }
        catch (Exception e)
        {
            ModEntry.Mon.Log($"Couldn't set funLeave dialogue: {e}", LogLevel.Error);
        }
    }

    private static void Change(NPC npc, Dictionary<int, SchedulePathDescription> schedule)
    {
        //stop any npc action
        npc.Halt();
        npc.followSchedule = false;

        npc.Schedule.Clear();
        foreach (var (key, path) in schedule)
        {
            npc.Schedule.Add(key,path);
        }
        npc.followSchedule = true;
    }
    
    private static void IslandClothes(NPC npc, bool islandClothes)
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
        }
    }

    public static Point RandomSpot(Character who, int maxDistance = 3)
    {
        var result = Point.Zero;

        //can try at max. 10 times to fix position
        for (var i = 0; i < 30; i++)
        {
            var randomTile = who.currentLocation.getRandomTile();
            
            if (who.currentLocation.IsTileOccupiedBy(randomTile))
                continue;

            var difference = new Point(
                (int)(randomTile.X - who.Position.X),
                (int)(randomTile.Y - who.Position.Y));

            if (Math.Abs(difference.X) > maxDistance || Math.Abs(difference.Y) > maxDistance) 
                continue;
            
            result = randomTile.ToPoint();
            break;
        }

        return result;
    }

    public static void WalkTo(NPC actor, Character target)
    {
        var position = new Point();
        for (var i = 1; i < 30; i++)
        {
            var toLeft = new Vector2(target.TilePoint.X - i, target.TilePoint.Y);
            if (!target.currentLocation.IsTileOccupiedBy(toLeft))
            {
                position = toLeft.ToPoint();
                break;
            }
            
            var toRight = new Vector2(target.TilePoint.X + i, target.TilePoint.Y);
            if (!target.currentLocation.IsTileOccupiedBy(toRight))
            {
                position = toRight.ToPoint();
                break;
            }
            
            var toUp = new Vector2(target.TilePoint.X, target.TilePoint.Y - i);
            if (!target.currentLocation.IsTileOccupiedBy(toUp))
            {
                position = toUp.ToPoint();
                break;
            }
            
            var toDown = new Vector2(target.TilePoint.X, target.TilePoint.Y + i);
            if (!target.currentLocation.IsTileOccupiedBy(toDown))
            {
                position = toDown.ToPoint();
                break;
            }

            var upperLeft= new Vector2(target.TilePoint.X - i, target.TilePoint.Y - 1);
            if (!target.currentLocation.IsTileOccupiedBy(upperLeft))
            {
                position = upperLeft.ToPoint();
                break;
            }
            
            var lowerLeft= new Vector2(target.TilePoint.X - i, target.TilePoint.Y + 1);
            if (!target.currentLocation.IsTileOccupiedBy(lowerLeft))
            {
                position = lowerLeft.ToPoint();
                break;
            }
            
            var upperRight= new Vector2(target.TilePoint.X + i, target.TilePoint.Y - 1);
            if (!target.currentLocation.IsTileOccupiedBy(upperRight))
            {
                position = upperRight.ToPoint();
                break;
            }
            
            var lowerRight= new Vector2(target.TilePoint.X + i, target.TilePoint.Y + 1);
            if (!target.currentLocation.IsTileOccupiedBy(lowerRight))
            {
                position = lowerRight.ToPoint();
                break;
            }
        }
        
        actor.controller = new PathFindController(actor, actor.currentLocation, position, 0, FaceTowardsTarget);
        return;

        void FaceTowardsTarget(Character c, GameLocation location) => actor.faceGeneralDirection(target.Tile);
    }

    internal static void Wander(NPC who, int maxDistance)
    {
        var randomlocation = RandomSpot(who, 8);
        who.controller = new PathFindController(who, who.currentLocation, randomlocation, Game1.random.Next(0, 4));
    }
}