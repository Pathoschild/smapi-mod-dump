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
using StardewValley.Quests;

namespace SDV.Shared.Abstractions
{
  public class SpecialOrderWrapper : QuestWrapper, ISpecialOrderWrapper
  {
    public SpecialOrderWrapper(Quest item) : base(item)
    {
    }
    
    public NetFields NetFields { get; }
    public void SetDuration(SpecialOrder.QuestDuration duration)
    {
    }

    public void OnFail()
    {
    }

    public int GetCompleteObjectivesCount() => 0;

    public void ConfirmCompleteDonations()
    {
    }

    public void UpdateDonationCounts()
    {
    }

    public bool HighlightAcceptableItems(IItemWrapper item) => false;

    public int GetAcceptCount(IItemWrapper item) => 0;

    public bool IsIslandOrder() => false;

    public string MakeLocalizationReplacements(string data) => null;

    public string Parse(string data) => null;

    public ISpecialOrderDataWrapper GetData() => null;

    public void InitializeNetFields()
    {
    }

    public bool UsesDropBox(string box_id) => false;

    public int GetMinimumDropBoxCapacity(string box_id) => 0;

    public void Update()
    {
    }

    public void RemoveFromParticipants()
    {
    }

    public void MarkForRemovalIfEmpty()
    {
    }

    public void HostHandleQuestEnd()
    {
    }

    public void AddSpecialRule(string rule)
    {
    }

    public void RemoveSpecialRule(string rule)
    {
    }

    public void Fail()
    {
    }

    public void AddObjective(IOrderObjectiveWrapper objective)
    {
    }

    public void CheckCompletion()
    {
    }
  }
}
