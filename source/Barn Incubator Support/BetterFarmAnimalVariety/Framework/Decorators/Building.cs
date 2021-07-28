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
// Type: BetterFarmAnimalVariety.Framework.Decorators.Building
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

namespace BetterFarmAnimalVariety.Framework.Decorators
{
  internal class Building : Decorator
  {
    public Building(StardewValley.Buildings.Building original)
      : base(original)
    {
    }

    public StardewValley.Buildings.Building GetOriginal()
    {
      return GetOriginal<StardewValley.Buildings.Building>();
    }

    public StardewValley.AnimalHouse GetIndoors()
    {
      return Paritee.StardewValley.Core.Locations.AnimalHouse.GetIndoors(GetOriginal());
    }

    public bool IsFull()
    {
      return Paritee.StardewValley.Core.Locations.AnimalHouse.IsFull(GetOriginal());
    }
  }
}