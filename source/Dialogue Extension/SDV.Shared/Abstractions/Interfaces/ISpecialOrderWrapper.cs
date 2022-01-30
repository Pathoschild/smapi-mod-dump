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
  public interface ISpecialOrderWrapper : IQuestWrapper
  {
    NetFields NetFields { get; }
    void SetDuration(SpecialOrder.QuestDuration duration);
    void OnFail();
    int GetCompleteObjectivesCount();
    void ConfirmCompleteDonations();
    void UpdateDonationCounts();
    bool HighlightAcceptableItems(IItemWrapper item);
    int GetAcceptCount(IItemWrapper item);
    bool IsIslandOrder();
    string MakeLocalizationReplacements(string data);
    string Parse(string data);
    ISpecialOrderDataWrapper GetData();
    void InitializeNetFields();
    bool UsesDropBox(string box_id);
    int GetMinimumDropBoxCapacity(string box_id);
    void Update();
    void RemoveFromParticipants();
    void MarkForRemovalIfEmpty();
    void HostHandleQuestEnd();
    void AddSpecialRule(string rule);
    void RemoveSpecialRule(string rule);
    void Fail();
    void AddObjective(IOrderObjectiveWrapper objective);
    void CheckCompletion();
    string ToString();
  }
}