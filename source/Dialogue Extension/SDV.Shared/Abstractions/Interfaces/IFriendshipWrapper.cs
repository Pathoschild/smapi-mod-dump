/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using Netcode;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public interface IFriendshipWrapper : IWrappedType<Friendship>
  {
    int Points { get; set; }
    int GiftsThisWeek { get; set; }
    int GiftsToday { get; set; }
    IWorldDateWrapper LastGiftDate { get; set; }
    bool TalkedToToday { get; set; }
    bool ProposalRejected { get; set; }
    IWorldDateWrapper WeddingDate { get; set; }
    IWorldDateWrapper NextBirthingDate { get; set; }
    FriendshipStatus Status { get; set; }
    long Proposer { get; set; }
    bool RoommateMarriage { get; set; }
    NetFields NetFields { get; }
    int DaysMarried { get; }
    int CountdownToWedding { get; }
    int DaysUntilBirthing { get; }
    void Clear();
    bool IsDating();
    bool IsEngaged();
    bool IsMarried();
    bool IsDivorced();
    bool IsRoommate();
  }
}