/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using FarmVisitors.Datamodels;
using FarmVisitors.Visit;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.Pathfinding;

namespace FarmVisitors;

/* Events *
 * placing these here so modentry.cs isn't so clogged */
internal static class Events
{
    internal static void SaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        ModEntry._firstLoadedDay = true;

        //clear ALL values and temp data on load. this makes sure there's no conflicts with savedata cache (e.g if player had returned to title)
        Content.ClearValues();
        Content.CleanTemp();

        ModEntry.RetiringDialogue = ModEntry.Help.GameContent.Load<Dictionary<string,List<string>>>("mistyspring.farmhousevisits/Dialogue/Retiring");
        ModEntry.InlawDialogue = ModEntry.Help.GameContent.Load<Dictionary<string,List<string>>>("mistyspring.farmhousevisits/Dialogue/InlawOf");

        var isTimeCoherent = ModEntry.Config.StartingHours < ModEntry.Config.EndingHours;
        ModEntry.IsConfigValid = isTimeCoherent;

        //check config
        if (!isTimeCoherent)
        {
            ModEntry.Log(
                "Starting hours can't happen after ending hours! To use the mod, fix this and reload savefile.",
                LogLevel.Warn);
            return;
        }

        ModEntry.Log("User config is valid.");

        //get all possible visitors- which also checks blacklist and married NPCs, etc.
        Content.GetAllVisitors();

        /* if allowed, get all inlaws.
         * this doesn't need daily reloading. NPC dispositions don't vary
         * (WON'T add compat for the very small % of mods with conditional disp., its an expensive action)*/
        if (ModEntry.Config.InLawComments != "VanillaAndMod")
            return;

        ModEntry.Log("Getting all in-laws...");
        var tempdict = ModEntry.Help.GameContent.Load<Dictionary<string, CharacterData>>("Data\\Characters");
        //on characters player has befriended
        foreach (var name in ModEntry.NameAndLevel.Keys)
        {
            var temp = Data.InlawOf_Mod(tempdict, name);
            if (temp is not null)
            {
                ModEntry.InLaws.Add(name, temp);
            }
        }
    }

    internal static void AssetRequest(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("mistyspring.farmhousevisits/Schedules", true))
        {
            e.LoadFrom(
                () => new Dictionary<string, ScheduleData>(),
                AssetLoadPriority.Medium
            );
        }

        if (e.NameWithoutLocale.IsEquivalentTo("mistyspring.farmhousevisits/Dialogue/Retiring", true))
        {
            e.LoadFrom(
                () => Data.LoadRetiringTemplate(),
                AssetLoadPriority.Medium
            );
        }

        if (e.NameWithoutLocale.IsEquivalentTo("mistyspring.farmhousevisits/Dialogue/InlawOf", true))
        {
            e.LoadFrom(
                () => Data.LoadInlawTemplate(),
                AssetLoadPriority.Medium
            );
        }
    }

    internal static void AssetInvalidated(object sender, AssetsInvalidatedEventArgs e)
    {
        //if none are from this mod
        if(!e.NamesWithoutLocale.Any(a => a.Name.StartsWith("mistyspring.farmhousevisits/")))
            return;

        //if any match, reload specific file

        if (e.NamesWithoutLocale.Any(a => a.Name == ("mistyspring.farmhousevisits/Schedules")))
            Content.ReloadCustomschedules();

        if (e.NamesWithoutLocale.Any(a => a.Name == ("mistyspring.farmhousevisits/Dialogue/Retiring")))
            ModEntry.RetiringDialogue = ModEntry.Help.GameContent.Load<Dictionary<string,List<string>>>("mistyspring.farmhousevisits/Dialogue/Retiring");
        
        if(e.NamesWithoutLocale.Any(a => a.Name == ("mistyspring.farmhousevisits/Dialogue/InlawOf")))
            ModEntry.InlawDialogue = ModEntry.Help.GameContent.Load<Dictionary<string,List<string>>>("mistyspring.farmhousevisits/Dialogue/InlawOf");
    }

    internal static void DayEnding(object sender, DayEndingEventArgs e)
    {
        var v = ModEntry.Visitor;
        v?.currentLocation.characters.Remove(v);

        ModEntry._firstLoadedDay = false;
        Content.CleanTemp();

        if (ModEntry.Config.Debug)
        {
            ModEntry.Log("Clearing today's visitor list, visitor count, and all other temp info...",
                ModEntry.Level);
        }
    }

    internal static void TitleReturn(object sender, ReturnedToTitleEventArgs e)
    {
        Content.ClearValues();

        ModEntry.Log(
            $"Removing cached information: HasAnyVisitors= {ModEntry.HasAnyVisitors}, TimeOfArrival={ModEntry.VContext?.TimeOfArrival}, CounterToday={ModEntry.CounterToday}, VisitorName={ModEntry.Visitor?.Name}. TodaysVisitors, NameAndLevel, FurnitureList, and RepeatedByLV cleared.", ModEntry.Level);
    }

    internal static void PlayerWarp(object sender, WarpedEventArgs e)
    {
        if (ModEntry.Config.Debug)
            ModEntry.Log($"Warped to new location. Info: \nName={e.NewLocation.Name}\nNameOrUniqueName={e.NewLocation.NameOrUniqueName},\nDisplayName={e.NewLocation.DisplayName}", LogLevel.Debug);

        if (ModEntry.Visitor == null)
            return;

        //if its forced they don't follow
        if (ModEntry.ForcedSchedule)
        {
            return;
        }

        if (!ModEntry.Config.WalkOnFarm)
            return;

        //check options
        var isFarmHouse = e.NewLocation.Equals(Utility.getHomeOfFarmer(Game1.player));
        var isGreenhouse = e.NewLocation.Name == "Greenhouse";
        var isFarm = e.NewLocation.Name == "Farm";
        var isShed = e.NewLocation.Name.Contains("Shed");

        //if new warp is not a valid option
        if (!isFarmHouse && !isFarm && !isGreenhouse && !isShed)
            return;

        //get config, and check if new location is allowed
        var cfg = ModEntry.Config;

        if(isShed && !cfg.Shed)
            return;
        
        if(isGreenhouse && !cfg.Greenhouse)
            return;

        //if we're returning to farm, check that npc could've been inside (or not)
        if (isFarm)
        {
            var wasFarmhouse = e.OldLocation.Equals(Utility.getHomeOfFarmer(Game1.player));
            //var wasBarnOrCoop = e.OldLocation.Name.Contains("Coop") || e.OldLocation.Name.Contains("Barn");
            var wasGreenhouse = e.OldLocation.Name == "Greenhouse";
            var wasShed = e.OldLocation.Name.Contains("Shed");
            var flag1 = wasShed && !cfg.Shed;
            var flag2 = wasGreenhouse && !cfg.Greenhouse;

            if (flag1 || flag2)
                return;

            if (!wasFarmhouse && !wasShed && !wasGreenhouse)
                return;
        }

        ModEntry.Log($"The new warp location is {e.NewLocation.NameOrUniqueName}", ModEntry.Level);

        var visit = ModEntry.Visitor;
        var context = ModEntry.VContext;

        if (context.IsGoingToSleep)
            return;

        context.ControllerTime++;

        if (ModEntry.Config.Debug)
        {
            ModEntry.Log($"Leaving {e.OldLocation.Name}...Warped to {e.NewLocation.Name}. isFarm = {e.NewLocation.IsFarm} , CanFollow = {ModEntry.Config.WalkOnFarm}, VisitorName = {visit.Name}", LogLevel.Info);
        }

        var door = Game1.player.Tile;
        context.IsOutside = false;

        if (ModEntry.Config.Debug)
            ModEntry.Log("Warped position: " + door, LogLevel.Debug);

        //reset certain things
        if (context.Idle)
            context.Idle = false;
        visit.Halt();
        visit.controller = null;
        visit.temporaryController = null;
        visit.CurrentDialogue?.Clear();
        visit.Dialogue?.Clear();
        if (visit.IsWalkingTowardPlayer)
            visit.IsWalkingTowardPlayer = false;

        //if farmhouse, get entry
        if (isFarmHouse)
        {
            var home = Utility.getHomeOfFarmer(Game1.player);
            door = home.getEntryLocation().ToVector2();
            door.Y-=2;
        }

        if (visit.controller is not null)
            visit.Halt();

        Game1.warpCharacter(visit, e.NewLocation, door);

        Point randomspot;

        if (isFarm) //if new location is farm
        {
            context.IsOutside = true;
            visit.faceDirection(2);
            door.X--;
            door.Y++;
            visit.controller = new PathFindController(visit, Game1.getFarm(), door.ToPoint(), 2);

            randomspot = door.ToPoint();
            //visit.moveTowardPlayer(2);
        }
        else //if it's anywhere else
        {
            context.IsOutside = false;
            visit.faceDirection(0);
            randomspot = Data.RandomTile(e.NewLocation, visit, 5).ToPoint();

            visit.controller = new PathFindController(visit, e.NewLocation, randomspot, 0);
        }

        if (ModEntry.Config.Debug)
            ModEntry.Log("Warped NPC. Pathing to " + randomspot);
    }

}