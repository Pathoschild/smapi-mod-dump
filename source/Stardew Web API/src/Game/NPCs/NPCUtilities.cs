/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley;
using StardewValley.Characters;

namespace StardewWebApi.Game.NPCs;

public enum NPCType
{
    Unknown,
    Villager,
    Pet,
    Horse,
    Child,
    Junimo
}

public static class NPCUtilities
{
    public static List<NPC> GetAllNPCs()
    {
        return Utility.getAllCharacters().Distinct().ToList();
    }

    public static NPCType GetNPCType(NPC npc)
    {
        return npc.IsVillager
            ? NPCType.Villager
            : npc switch
            {
                Pet => NPCType.Pet,
                Horse => NPCType.Horse,
                Child => NPCType.Child,
                _ => NPCType.Unknown
            };
    }

    public static List<NPC> GetAllNPCsOfType(NPCType type)
    {
        return GetAllNPCs().Where(npc =>
        {
            return type switch
            {
                NPCType.Villager => npc.IsVillager,
                NPCType.Pet => npc is Pet,
                NPCType.Horse => npc is Horse,
                _ => false,
            };
        }).ToList();
    }

    public static NPC? GetNPCByName(string name)
    {
        var nameLower = name.ToLower();
        NPC? foundNpc = null;

        Utility.ForEachCharacter((npc) =>
        {
            if (npc.Name.ToLower() == nameLower)
            {
                foundNpc = npc;
                return false;
            }
            return true;
        });

        return foundNpc;
    }

    public static NPC? GetNPCByNameAndType(string name, NPCType type)
    {
        var nameLower = name.ToLower();
        NPC? foundNpc = null;

        Utility.ForEachCharacter((npc) =>
        {
            if (npc.Name.ToLower() == nameLower && GetNPCType(npc) == type)
            {
                foundNpc = npc;
                return false;
            }
            return true;
        });

        return foundNpc;
    }

    public static List<NPC> GetNPCsByBirthday(Season season, int day)
    {
        return GetNPCsByBirthday(season.ToString(), day);
    }

    public static List<NPC> GetNPCsByBirthday(string season, int day)
    {
        var npcs = new List<NPC>();

        season = season.ToLower();

        foreach (var location in Game1.locations)
        {
            foreach (var npc in location.characters)
            {
                if (npc.Birthday_Season is not null
                    && npc.Birthday_Season?.ToLower() == season
                    && npc.Birthday_Day == day
                )
                {
                    npcs.Add(npc);
                }
            }
        }

        return npcs;
    }
}