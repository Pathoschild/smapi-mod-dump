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
using System.Text;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace SpousesIsland.Additions;

public static class Debugging
{
    private static void Log(string msg, LogLevel lv = LogLevel.Debug) => ModEntry.Mon.Log(msg, lv);
    public static void Print(string arg1, string[] arg2)
    {
        var sb = new StringBuilder("Spouses:\n");
        foreach (var spouse in ModEntry.ValidSpouses)
        {
            var npc = Game1.getCharacterFromName(spouse);
            
            sb.Append("                  - ");
            sb.Append(npc.displayName);
            sb.Append(":  ");
            foreach (var path in npc.Schedule.Values)
            {
                sb.Append(path.time);
                sb.Append(' ');
                sb.Append(path.targetLocationName);
                sb.Append(' ');
                sb.Append(path.targetTile);
                sb.Append('/');
            }
            sb[^1] = '\n';
        }
        Log(sb.ToString(), LogLevel.Info);
    }

    public static void PrintDetailed(string arg1, string[] arg2)
    {
        try
        {
            var sb = new StringBuilder("Spouses:\n");
            foreach (var spouse in ModEntry.ValidSpouses)
            {
                var npc = Game1.getCharacterFromName(spouse);

                sb.Append("                  - ");
                sb.Append(npc.displayName);
                sb.Append(":  ");
                Stack<Point> controller;
                try
                {
                    controller = npc.temporaryController.pathToEndPoint;
                    _ = controller.Peek();
                }
                catch (Exception)
                {
                    controller = npc.controller.pathToEndPoint;
                    _ = controller.Peek();
                }

                foreach (var path in controller)
                {
                    sb.Append(" [");
                    sb.Append(' ');
                    sb.Append(path.X);
                    sb.Append(' ');
                    sb.Append(path.Y);
                    sb.Append(' ');
                    sb.Append(']');
                    sb.Append(',');
                }

                sb[^1] = '\n';
            }

            Log(sb.ToString(), LogLevel.Info);
        }
        catch (Exception)
        {
            Log("No current pathfind.", LogLevel.Warn);
        }
    }
    
    public static void PrintFull(string arg1, string[] arg2)
    {
        var sb = new StringBuilder("Spouses:\n");
        foreach (var spouse in ModEntry.ValidSpouses)
        {
            var npc = Game1.getCharacterFromName(spouse);
            
            sb.Append("                  - ");
            sb.Append(npc.displayName);
            sb.Append(":  ");
            
            foreach (var (time, path) in npc.Schedule)
            {
            
                sb.Append("\n                  - ");
                sb.Append(time);
                sb.Append(": ");
                foreach (var point in path.route)
                {
                    sb.Append(' ');
                    sb.Append(point.X);
                    sb.Append(' ');
                    sb.Append(point.Y);
                    sb.Append(' ');
                    sb.Append(',');
                }
                sb[^1] = '.';
            }
        }
        Log(sb.ToString(), LogLevel.Info);
    }

    public static void GetNpcWarps(string arg1, string[] arg2)
    {
        var sb = new StringBuilder("Npc warps:\n");
        foreach (var w in Game1.player.currentLocation.warps)
        {
            if(w.npcOnly.Value == false)
                continue;
            
            sb.Append("                  - ");
            sb.Append(w.TargetName);
            sb.Append(' ');
            sb.Append(w.X);
            sb.Append(' ');
            sb.Append(w.Y);
            sb.Append('\n');
        }
        Log(sb.ToString(), LogLevel.Info);
    }
    
    public static void GetAllWarps(string arg1, string[] arg2)
    {
        var sb = new StringBuilder("All warps:\n");
        foreach (var w in Game1.player.currentLocation.warps)
        {
            sb.Append("                  - ");
            sb.Append(w.X);
            sb.Append(' ');
            sb.Append(w.Y);
            sb.Append(' ');
            sb.Append(w.TargetName);
            sb.Append(' ');
            sb.Append(w.TargetX);
            sb.Append(' ');
            sb.Append(w.TargetY);
            
            if(w.npcOnly.Value)
                sb.Append(" (NPC only)");

            sb.Append('\n');
        }
        Log(sb.ToString(), LogLevel.Info);
    }

    public static void IsMoving(string arg1, string[] arg2)
    {
        var sb = new StringBuilder("All warps:\n");
        const string subSeparator = "\n                   - ";
        foreach (var spouse in ModEntry.ValidSpouses)
        {
            var npc = Game1.getCharacterFromName(spouse);
            
            sb.Append("                  - ");
            sb.Append(npc.displayName);
            sb.Append(":  ");
            sb.Append(subSeparator);
            sb.Append("isMovingOnPathFindPath: ");
            sb.Append(npc.isMovingOnPathFindPath.Value);
            sb.Append(subSeparator);
            sb.Append("controller: ");
            sb.Append(npc.controller);
            sb.Append(subSeparator);
            sb.Append("temporaryController: ");
            sb.Append(npc.temporaryController);
            sb.Append(subSeparator);
            sb.Append("isMoving: ");
            sb.Append(npc.isMoving());
        }
        Log(sb.ToString(), LogLevel.Info);
    }

    public static void Speed(string arg1, string[] arg2)
    {
        var sb = new StringBuilder("Companion speed:\n");
        const string subSeparator = "\n                   - ";
        foreach (var spouse in ModEntry.ValidSpouses)
        {
            var npc = Game1.getCharacterFromName(spouse);
            
            sb.Append("                  - ");
            sb.Append(npc.displayName);
            sb.Append(":  ");
            sb.Append(subSeparator);
            sb.Append("Speed: ");
            sb.Append(npc.Speed);
            sb.Append(subSeparator);
            sb.Append("addedSpeed: ");
            sb.Append(npc.addedSpeed);
        }
        Log(sb.ToString(), LogLevel.Info);
    }
}