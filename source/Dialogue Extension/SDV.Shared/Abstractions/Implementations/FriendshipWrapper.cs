/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using JetBrains.Annotations;
using Netcode;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class FriendshipWrapper : IFriendshipWrapper
  {
    public int Points { get; set; }
    public int GiftsThisWeek { get; set; }
    public int GiftsToday { get; set; }
    public IWorldDateWrapper LastGiftDate { get; set; }
    public bool TalkedToToday { get; set; }
    public bool ProposalRejected { get; set; }
    public IWorldDateWrapper WeddingDate { get; set; }
    public IWorldDateWrapper NextBirthingDate { get; set; }
    public FriendshipStatus Status { get; set; }
    public long Proposer { get; set; }
    public bool RoommateMarriage { get; set; }
    public NetFields NetFields { get; }
    public int DaysMarried { get; }
    public int CountdownToWedding { get; }
    public int DaysUntilBirthing { get; }
    public void Clear()
    {
    }

    public bool IsDating() => false;

    public bool IsEngaged() => false;

    public bool IsMarried() => false;

    public bool IsDivorced() => false;

    public bool IsRoommate() => false;

    public FriendshipWrapper([NotNull] Friendship item) => GetBaseType = item;

    public Friendship GetBaseType { get; }
  }

}
