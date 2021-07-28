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
// Type: BetterFarmAnimalVariety.Framework.Decorators.Farmer
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using StardewValley;

namespace BetterFarmAnimalVariety.Framework.Decorators
{
  public class Farmer : Decorator
  {
    public Farmer(StardewValley.Farmer original)
      : base(original)
    {
    }

    public StardewValley.Farmer GetOriginal()
    {
      return GetOriginal<StardewValley.Farmer>();
    }

    public long GetUniqueId()
    {
      return Paritee.StardewValley.Core.Characters.Farmer.GetUniqueId(GetOriginal());
    }

    public List<string> SanitizeBlueChickens(List<string> types)
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.SanitizeBlueChickens(types, GetOriginal());
    }

    public List<string> SanitizeAffordableTypes(List<string> types)
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.SanitizeAffordableTypes(types, GetOriginal());
    }

    public StardewValley.FarmAnimal CreateFarmAnimal(
      string type,
      string name = null,
      StardewValley.Buildings.Building building = null)
    {
      return Paritee.StardewValley.Core.Characters.FarmAnimal.CreateFarmAnimal(type, GetUniqueId(), name, building);
    }

    public bool IsCurrentLocation(GameLocation location)
    {
      return Paritee.StardewValley.Core.Characters.Farmer.IsCurrentLocation(GetOriginal(), location);
    }

    public bool CanAfford(int amount)
    {
      return Paritee.StardewValley.Core.Characters.Farmer.CanAfford(GetOriginal(), amount);
    }

    public void SpendMoney(int amount)
    {
      Paritee.StardewValley.Core.Characters.Farmer.SpendMoney(GetOriginal(), amount);
    }

    public bool HasProfession(Paritee.StardewValley.Core.Characters.Farmer.Profession profession)
    {
      return Paritee.StardewValley.Core.Characters.Farmer.HasProfession(GetOriginal(), profession);
    }
  }
}