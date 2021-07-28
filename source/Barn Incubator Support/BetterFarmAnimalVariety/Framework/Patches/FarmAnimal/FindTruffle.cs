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
// Type: BetterFarmAnimalVariety.Framework.Patches.FarmAnimal.FindTruffle
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using Microsoft.Xna.Framework;
using Paritee.StardewValley.Core.Locations;
using Paritee.StardewValley.Core.Utilities;
using StardewValley;
using Game = Paritee.StardewValley.Core.Utilities.Game;

namespace BetterFarmAnimalVariety.Framework.Patches.FarmAnimal
{
  internal class FindTruffle : Patch
  {
    public static bool Prefix(ref StardewValley.FarmAnimal __instance, ref Farmer who)
    {
      var moddedAnimal = new Decorators.FarmAnimal(__instance);
      AttemptToSpawnProduce(ref moddedAnimal, Game.GetMasterPlayer());
      if (ShouldStopFindingProduce(ref moddedAnimal))
        moddedAnimal.SetCurrentProduce(-1);
      return false;
    }

    private static bool ShouldStopFindingProduce(ref Decorators.FarmAnimal moddedAnimal)
    {
      return Random.Seed((int) (moddedAnimal.GetUniqueId() / 2L +
                                Game.GetDaysPlayed() +
                                Game.GetTimeOfDay())).NextDouble() >
             moddedAnimal.GetFriendship() / 1500.0;
    }

    private static bool AttemptToSpawnProduce(ref Decorators.FarmAnimal moddedAnimal, Farmer who)
    {
      var translatedVector2 =
        StardewValley.Utility.getTranslatedVector2(moddedAnimal.GetTileLocation(), moddedAnimal.GetFacingDirection(),
          1f);
      var currentProduce = moddedAnimal.GetCurrentProduce();
      if (!Paritee.StardewValley.Core.Characters.FarmAnimal.IsProduceAnItem(currentProduce))
        return false;
      var @object = new StardewValley.Object(Vector2.Zero, currentProduce, null, false, true, false, true)
      {
        Quality = moddedAnimal.GetProduceQuality()
      };
      Location.SpawnObject(Game.GetFarm(), translatedVector2, @object);
      return true;
    }
  }
}