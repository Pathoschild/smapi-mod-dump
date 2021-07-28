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
// Type: BetterFarmAnimalVariety.Framework.Patches.Object.DayUpdate
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using BetterFarmAnimalVariety.Framework.Decorators;
using StardewValley;

namespace BetterFarmAnimalVariety.Framework.Patches.Object
{
  internal class DayUpdate
  {
    public static bool Prefix(ref StardewValley.Object __instance, ref GameLocation location)
    {
      if (!Paritee.StardewValley.Core.Objects.Object.IsAutoGrabber(__instance) || location == null ||
          !(location is StardewValley.AnimalHouse animalHouse))
        return true;
      new AutoGrabber(__instance).AutoGrabFromAnimals(animalHouse);
      return false;
    }
  }
}