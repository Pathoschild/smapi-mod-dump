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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Delegates;

namespace SpousesIsland.Additions;

internal static class Information
{
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);

    internal static List<Child> PlayerChildren(Farmer player)
    {
        var children = new List<Child>();

        var asChild = player.getChildren();

        //get them from kid list if vanilla
        if (asChild?.Count is not 0 && asChild != null)
        {
            foreach (var kid in asChild)
            {
                children.Add(kid);
            }
        }
        
        /*
        if (!ModEntry.ModInfo.LittleNpcs)
            return children;

        var charas = Utility.getHomeOfFarmer(player)?.characters;
        var values = new List<NPC>();

        if (charas != null)
            foreach (var npc in charas)
            {
                //0 teen?, 1 adult, 2 child
                if (npc.Age != 2)
                {
                    continue;
                }

                values.Add(npc);
            }

        //if theres still none
        if (values.Count is 0)
        {
            return children;
        }

        //add. we could do a .ToList() but i'd rather be safe
        foreach (var child in values)
        {
            children.Add(child);
        }
        */
        return children;
    }

    internal static List<string> PlayerSpouses(string id)
    {
        var farmer = Game1.getFarmer(long.Parse(id));
        var spouses = PlayerSpouses(farmer);

        return spouses;
    }

    internal static List<string> PlayerSpouses(Farmer farmer)
    {
        List<string> spouses = new();

        foreach (var name in Game1.NPCGiftTastes.Keys)
        {
            //if universal taste OR unmet npc
            if (name.StartsWith("Universal_") || farmer?.friendshipData?.Keys.Contains(name) == false)
                continue;

            var isMarried = farmer?.friendshipData[name]?.IsMarried() ?? false;
            var isRoommate = farmer?.friendshipData[name]?.IsRoommate() ?? false;
            if (isMarried || isRoommate)
            {
                spouses.Add(name);
            }
        }
        return spouses;
    }

    public static List<string> AllowedSpouses(Farmer player)
    {
        var result = new List<string>();
        
        var married = GetAllSpouses(Game1.player);
        foreach (var who in married)
        {
            if (!Enabled(who))
                continue;
            
            result.Add(who);
        }

        return result;
    }

    internal static bool IsIntegrated(string name)
    {
        var result = name switch
        {
            "Abigail" => true,
            "Alex" => true,
            "Elliott" => true,
            "Emily" => true,
            "Haley" => true,
            "Harvey" => true,
            "Krobus" => true,
            "Leah" => true,
            "Maru" => true,
            "Penny" => true,
            "Sam" => true,
            "Sebastian" => true,
            "Shane" => true,
            "Claire" => true,
            "Lance" => true,
            "Olivia" => true,
            "Sophia" => true,
            "Victor" => true,
            "Wizard" => true,
            _ => false
        };

        return result;
    }
    /// <summary> 
    /// Returns integrated spouse's "Allowed" config. If not integrated, returns false.
    /// </summary>
    internal static bool Enabled(string name)
    {
        return ModEntry.Blacklist.Contains(name) == false;
    }

    /// <summary> 
    /// Obtains all married NPCs, for non-host in multiplayer.
    /// </summary>
    internal static List<string> GetAllSpouses(Farmer player)
    {
        List<string> spouses = new();

        foreach (var chara in player.friendshipData.Keys)
        {
            if (player.friendshipData[chara].IsMarried())
            {
                spouses.Add(chara);
            }
        }

        return spouses;
    }

    
    public static string LittleNpcName(Child child, string prefix)
    {
        var name = child.Name.Replace(' ', '_');
            
        return $"{prefix}{name}";
    }

    public static bool IslandVisitDay(string[] query, GameStateQueryContext context)
    {
        return ModEntry.IslandToday;
    }
}
