/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

// Decompiled with JetBrains decompiler
// Type: BetterFarmAnimalVariety.Framework.Decorators.FarmAnimal
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Paritee.StardewValley.Core.Characters;
using StardewValley;
using Game = Paritee.StardewValley.Core.Utilities.Game;
using Object = Paritee.StardewValley.Core.Objects.Object;

namespace BetterFarmAnimalVariety.Framework.Decorators
{
  public class FarmAnimal : Decorator
  {
    public FarmAnimal(StardewValley.FarmAnimal original)
      : base(original)
    {
    }

    public StardewValley.FarmAnimal GetOriginal()
    {
      return GetOriginal<StardewValley.FarmAnimal>();
    }

    public void Reload(StardewValley.Buildings.Building building)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.Reload(GetOriginal(), building);
    }

    public string GetTypeString()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetType(GetOriginal());
    }

    public long GetUniqueId()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetUniqueId(GetOriginal());
    }

    public bool IsVanilla()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.IsVanilla(GetOriginal());
    }

    public int RollProduce(int seed, StardewValley.Farmer farmer = null, double deluxeProduceLuck = 0.0)
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.RollProduce(GetOriginal(), seed, farmer,
        deluxeProduceLuck);
    }

    public int GetCurrentProduce()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetCurrentProduce(GetOriginal());
    }

    public int GetPrice()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetPrice(GetOriginal());
    }

    public void SetCurrentProduce(int produceIndex)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.SetCurrentProduce(GetOriginal(), produceIndex);
    }

    public void UpdateFromData(string type)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.UpdateFromData(GetOriginal(), type);
    }

    public string GetDefaultType()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetDefaultType(GetOriginal());
    }

    public void AddToBuilding(StardewValley.Buildings.Building building)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.AddToBuilding(GetOriginal(), building);
    }

    public string GetRandomTypeFromProduce(Dictionary<string, List<string>> restrictions)
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetRandomTypeFromProduce(GetOriginal(), restrictions) ??
             Paritee.StardewValley.Core.Characters.FarmAnimal.GetDefaultBarnDwellerType();
    }

    public void AssociateParent(StardewValley.FarmAnimal parent)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.AssociateParent(GetOriginal(), parent);
    }

    public bool HasHome()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.HasHome(GetOriginal());
    }

    public bool IsEating()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.IsEating(GetOriginal());
    }

    public byte GetFullness()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetFullness(GetOriginal());
    }

    public byte GetHappiness()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetHappiness(GetOriginal());
    }

    public void SetFindGrassPathController(GameLocation location)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.SetFindGrassPathController(GetOriginal(), location);
    }

    public void ReturnHome()
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.ReturnHome(GetOriginal());
    }

    public void SetFindHomeDoorPathController(GameLocation location)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.SetFindHomeDoorPathController(GetOriginal(), location);
    }

    public bool IsBaby()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.IsBaby(GetOriginal());
    }

    public bool CanFindProduce()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.CanFindProduce(GetOriginal());
    }

    public void AnimateFindingProduce()
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.AnimateFindingProduce(GetOriginal());
    }

    public void FindProduce(StardewValley.Farmer farmer)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.FindProduce(GetOriginal(), farmer);
    }

    public int GetFriendship()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetFriendship(GetOriginal());
    }

    public void SetHome(StardewValley.Buildings.Building building)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.SetHome(GetOriginal(), building);
    }

    public bool MakesSound()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.MakesSound(GetOriginal());
    }

    public string GetSound()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetSound(GetOriginal());
    }

    public string GetDisplayType()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetDisplayType(GetOriginal());
    }

    public string GetDisplayHouse()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetDisplayHouse(GetOriginal());
    }

    public bool CanLiveIn(StardewValley.Buildings.Building building)
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.CanLiveIn(GetOriginal(), building);
    }

    public bool CanBeNamed()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.CanBeNamed(GetOriginal());
    }

    public bool HasName()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.HasName(GetOriginal());
    }

    public string GetName()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetName(GetOriginal());
    }

    public bool IsCoopDweller()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.IsCoopDweller(GetOriginal());
    }

    public bool HasController()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.HasController(GetOriginal());
    }

    public Rectangle GetBoundingBox()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetBoundingBox(GetOriginal());
    }

    public Vector2 GetTileLocation()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetTileLocation(GetOriginal());
    }

    public int GetFacingDirection()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetFacingDirection(GetOriginal());
    }

    public bool IsAProducer()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.IsAProducer(GetOriginal());
    }

    public bool HasProduceThatMatchesAtLeastOne(int[] targets)
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.HasProduceThatMatchesAtLeastOne(GetOriginal(), targets);
    }

    public int GetDefaultProduce()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetDefaultProduce(GetOriginal());
    }

    public int GetDeluxeProduce()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetDeluxeProduce(GetOriginal());
    }

    public bool IsCurrentlyProducingDeluxe()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.IsCurrentlyProducingDeluxe(GetOriginal());
    }

    public bool IsType(Livestock type)
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.IsType(GetOriginal(), type);
    }

    public long GetOwnerId()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetOwnerId(GetOriginal());
    }

    public StardewValley.Farmer GetOwner()
    {
      return Game.GetFarmer(GetOwnerId());
    }

    public byte GetDaysToLay(StardewValley.Farmer farmer)
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetDaysToLay(GetOriginal(), farmer);
    }

    public byte GetDaysSinceLastLay()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetDaysSinceLastLay(GetOriginal());
    }

    public int GetMeatIndex()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetMeatIndex(GetOriginal());
    }

    public StardewValley.Buildings.Building GetHome()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetHome(GetOriginal());
    }

    public void SetDaysSinceLastLay(byte days)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.SetDaysSinceLastLay(GetOriginal(), days);
    }

    public Object.Quality RollProduceQuality(
      StardewValley.Farmer farmer,
      int seed)
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.RollProduceQuality(GetOriginal(), farmer, seed);
    }

    public void SetProduceQuality(Object.Quality quality)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.SetProduceQuality(GetOriginal(), quality);
    }

    public bool LaysProduce()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.LaysProduce(GetOriginal());
    }

    public int GetProduceQuality()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetProduceQuality(GetOriginal());
    }

    public void SetPauseTimer(int timer)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.SetPauseTimer(GetOriginal(), timer);
    }

    public int GetPauseTimer()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetPauseTimer(GetOriginal());
    }

    public void SetHitGlowTimer(int timer)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.SetHitGlowTimer(GetOriginal(), timer);
    }

    public int GetHitGlowTimer()
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.GetHitGlowTimer(GetOriginal());
    }
  }
}