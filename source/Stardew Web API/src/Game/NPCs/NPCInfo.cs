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
using System.Text.Json.Serialization;

namespace StardewWebApi.Game.NPCs;

public class NPCInfo
{
    protected readonly NPC _npc;

    protected NPCInfo(NPC npc)
    {
        _npc = npc;
    }

    public static NPCInfo? FromNPC(NPC? npc)
    {
        return npc switch
        {
            not null => NPCUtilities.GetNPCType(npc) switch
            {
                NPCType.Villager => new VillagerInfo(npc),
                NPCType.Pet => new PetInfo(npc),
                _ => new NPCInfo(npc),
            },
            _ => null
        };
    }

    public static NPCInfo? FromNPCName(string name)
    {
        return FromNPC(NPCUtilities.GetNPCByName(name));
    }

    public string Name => _npc.Name;
    public string DisplayName => _npc.displayName;
    public int Age => _npc.Age;
    public string CurrentLocation => _npc.currentLocation.Name;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NPCType Type => NPCUtilities.GetNPCType(_npc);

    public NPCStub CreateStub()
    {
        return NPCStub.FromNPCInfo(this);
    }
}