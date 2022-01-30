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
  public interface IFarmAnimalWrapper : ICharacterWrapper
  {
    IBuildingWrapper home { get; set; }
    string displayHouse { get; set; }
    string displayType { get; set; }
    void reloadData();
    string shortDisplayType();
    bool isCoopDweller();
    Rectangle GetHarvestBoundingBox();
    Rectangle GetCursorPetBoundingBox();
    void reload(IBuildingWrapper home);
    int GetDaysOwned();
    void pet(IFarmerWrapper who, bool is_auto_pet = false);
    void farmerPushing();
    void doDive();
    void Poke();
    void setRandomPosition(IGameLocationWrapper location);
    void StopAllActions();
    void dayUpdate(IGameLocationWrapper environtment);
    int getSellPrice();
    bool isMale();
    string getMoodMessage();
    bool isBaby();
    void warpHome(IFarmWrapper f, IFarmAnimalWrapper a);

    void updateWhenNotCurrentLocation(
      IBuildingWrapper currentBuilding,
      GameTime time,
      IGameLocationWrapper environment);

    void updatePerTenMinutes(int timeOfDay, IGameLocationWrapper environment);
    void eatGrass(IGameLocationWrapper environment);
    void Eat(IGameLocationWrapper location);
    void GetNewFollowPosition();
    void hitWithWeapon(IMeleeWeaponWrapper t);
    void makeSound();
    bool CanHavePregnancy();
    bool SleepIfNecessary();
    bool updateWhenCurrentLocation(GameTime time, IGameLocationWrapper location);
    void UpdateRandomMovements();
    bool CanSwim();
    bool CanFollowAdult();
    void HandleHop();

    bool HandleCollision(Rectangle next_position);
    bool IsActuallySwimming();
    void Splash();
  }
}