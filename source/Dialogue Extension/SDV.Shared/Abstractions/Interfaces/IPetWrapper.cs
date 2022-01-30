/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using Microsoft.Xna.Framework;

namespace SDV.Shared.Abstractions
{
  public interface IPetWrapper : INPCWrapper
  {
    int CurrentBehavior { get; set; }
    void OnPetAnimationEvent(string animation_event);
    string getPetTextureName();
    void reloadBreedSprite();
    void warpToFarmHouse(IFarmerWrapper who);
    void UpdateSleepingOnBed();
    void GrantLoveMailIfNecessary();
    void setAtFarmPosition();
    void playContentSound();
    void hold(IFarmerWrapper who);
    void RunState(GameTime time);
    void OnNewBehavior();
  }
}