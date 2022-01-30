/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

namespace SDV.Shared.Abstractions
{
  public interface IChildWrapper : INPCWrapper
  {
    bool isInCrib();
    void toss(IFarmerWrapper who);
    void performToss(IFarmerWrapper who);
    void doneTossing(IFarmerWrapper who);
    void tenMinuteUpdate();
    int GetChildIndex();
    void toddlerReachedDestination(ICharacterWrapper c, IGameLocationWrapper l);
    void resetForPlayerEntry(IGameLocationWrapper l);
  }
}