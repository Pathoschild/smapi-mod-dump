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

namespace StardewWebApi.Game.NPCs;

public class NPCStub
{
    private NPCStub(string name, NPCType type)
    {
        Name = name;
        Type = type;
    }

    public static NPCStub FromNPC(NPC npc)
    {
        return new(npc.Name, NPCUtilities.GetNPCType(npc));
    }

    public static NPCStub FromNPCInfo(NPCInfo npc)
    {
        return new(npc.Name, npc.Type);
    }

    public string Name { get; }
    public NPCType Type { get; }
    public string Url => $"/api/v1/npcs/name/{Name}";
}

public static class NPCStubExtensions
{
    public static NPCStub CreateStub(this NPC npc)
    {
        return NPCStub.FromNPC(npc);
    }
}