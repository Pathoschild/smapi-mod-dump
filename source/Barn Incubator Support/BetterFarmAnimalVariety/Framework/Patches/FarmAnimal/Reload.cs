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
// Type: BetterFarmAnimalVariety.Framework.Patches.FarmAnimal.Reload
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using BetterFarmAnimalVariety.Framework.Helpers;
using StardewValley.Buildings;
using FarmAnimals = BetterFarmAnimalVariety.Framework.SaveData.FarmAnimals;

namespace BetterFarmAnimalVariety.Framework.Patches.FarmAnimal
{
  internal class Reload : Patch
  {
    public static bool Prefix(ref StardewValley.FarmAnimal __instance, ref Building home)
    {
      var moddedAnimal = new Decorators.FarmAnimal(__instance);
      if (!moddedAnimal.HasName())
        return true;
      moddedAnimal.SetHome(home);
      Mod.ReadSaveData<FarmAnimals>("farm-animals").OverwriteFarmAnimal(ref moddedAnimal, null);
      return false;
    }
  }
}