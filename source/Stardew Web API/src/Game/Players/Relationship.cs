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
using StardewWebApi.Game.NPCs;

namespace StardewWebApi.Game.Players;

public class Relationship
{
    private readonly Friendship _friendship;

    private Relationship(NPCStub npc, Friendship friendship)
    {
        NPC = npc;
        _friendship = friendship;
    }

    public static Relationship FromFriendshipData(string name, Friendship friendship)
    {
        var npc = NPCUtilities.GetNPCByName(name)!.CreateStub();

        return new(npc, friendship);
    }

    public static int GetHeartsFromPoints(int points) => (int)Math.Floor(points / 250D);

    public NPCStub NPC { get; }
    public int Points => _friendship.Points;
    public int Hearts => GetHeartsFromPoints(Points);
    public bool HasBeenGivenGiftToday => GiftsGivenToday > 0;
    public int GiftsGivenToday => _friendship.GiftsToday;
    public int GiftsGivenThisWeek => _friendship.GiftsThisWeek;
    public Date? LastGiftDate => _friendship.LastGiftDate is not null ? new(_friendship.LastGiftDate) : null;
    public bool IsDating => _friendship.IsDating();
    public bool IsEngaged => _friendship.IsEngaged();
    public bool IsMarried => _friendship.IsMarried();
    public bool IsRoommate => _friendship.IsRoommate();
    public bool IsDivorced => _friendship.IsDivorced();
}