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
// Type: BetterFarmAnimalVariety.Framework.Decorators.AnimalHouse
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using StardewValley;

namespace BetterFarmAnimalVariety.Framework.Decorators
{
  internal class AnimalHouse : Decorator
  {
    public AnimalHouse(StardewValley.AnimalHouse original)
      : base(original)
    {
    }

    public StardewValley.AnimalHouse GetOriginal()
    {
      return GetOriginal<StardewValley.AnimalHouse>();
    }

    public Object GetIncubatorWithEggReadyToHatch()
    {
      return Paritee.StardewValley.Core.Locations.AnimalHouse.GetIncubatorWithEggReadyToHatch(GetOriginal());
    }

    public void ResetIncubator()
    {
      Paritee.StardewValley.Core.Locations.AnimalHouse.ResetIncubator(GetOriginal());
    }

    public void ResetIncubator(Object incubator)
    {
      Paritee.StardewValley.Core.Locations.AnimalHouse.ResetIncubator(GetOriginal(), incubator);
    }

    public bool IsEggReadyToHatch()
    {
      return Paritee.StardewValley.Core.Locations.AnimalHouse.IsEggReadyToHatch(GetOriginal());
    }

    public bool IsFull()
    {
      return Paritee.StardewValley.Core.Locations.AnimalHouse.IsFull(GetOriginal());
    }

    public StardewValley.Buildings.Building GetBuilding()
    {
      return Paritee.StardewValley.Core.Locations.AnimalHouse.GetBuilding(GetOriginal());
    }

    public void SetIncubatorHatchEvent()
    {
      Paritee.StardewValley.Core.Locations.AnimalHouse.SetIncubatorHatchEvent(GetOriginal());
    }
  }
}