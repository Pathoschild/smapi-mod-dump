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
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class FarmAnimalWrapper : CharacterWrapper, IFarmAnimalWrapper
  {
    public FarmAnimalWrapper(Character character) : base(character)
    {
    }

    public IBuildingWrapper home { get; set; }
    public string displayHouse { get; set; }
    public string displayType { get; set; }
    public void reloadData()
    {
    }

    public string shortDisplayType() => null;

    public bool isCoopDweller() => false;

    public Rectangle GetHarvestBoundingBox() => default;

    public Rectangle GetCursorPetBoundingBox() => default;

    public void reload(IBuildingWrapper home)
    {
    }

    public int GetDaysOwned() => 0;

    public void pet(IFarmerWrapper who, bool is_auto_pet = false)
    {
    }

    public void farmerPushing()
    {
    }

    public void doDive()
    {
    }

    public void Poke()
    {
    }

    public void setRandomPosition(IGameLocationWrapper location)
    {
    }

    public void StopAllActions()
    {
    }

    public void dayUpdate(IGameLocationWrapper environtment)
    {
    }

    public int getSellPrice() => 0;

    public bool isMale() => false;

    public string getMoodMessage() => null;

    public bool isBaby() => false;

    public void warpHome(IFarmWrapper f, IFarmAnimalWrapper a)
    {
    }

    public void updateWhenNotCurrentLocation(IBuildingWrapper currentBuilding, GameTime time, IGameLocationWrapper environment)
    {
    }

    public void updatePerTenMinutes(int timeOfDay, IGameLocationWrapper environment)
    {
    }

    public void eatGrass(IGameLocationWrapper environment)
    {
    }

    public void Eat(IGameLocationWrapper location)
    {
    }

    public void GetNewFollowPosition()
    {
    }

    public void hitWithWeapon(IMeleeWeaponWrapper t)
    {
    }

    public void makeSound()
    {
    }

    public bool CanHavePregnancy() => false;

    public bool SleepIfNecessary() => false;

    public bool updateWhenCurrentLocation(GameTime time, IGameLocationWrapper location) => false;

    public void UpdateRandomMovements()
    {
    }

    public bool CanSwim() => false;

    public bool CanFollowAdult() => false;

    public void HandleHop()
    {
    }

    public bool HandleCollision(Rectangle next_position) => false;

    public bool IsActuallySwimming() => false;

    public void Splash()
    {
    }
  }
}