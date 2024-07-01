/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using StardewValley.GameData;
using System.Collections.Generic;

namespace Selph.StardewMods.ExtraAnimalConfig;

public class AnimalProduceExtensionData {
  public GenericSpawnItemData ItemQuery;
  public string HarvestTool;
  public string ProduceTexture;
  public Dictionary<string, string> SkinProduceTexture = new Dictionary<string, string>();
}

public class AnimalExtensionData {
  public float MalePercentage = -1;
  public Dictionary<string, AnimalProduceExtensionData> AnimalProduceExtensionData = new Dictionary<string, AnimalProduceExtensionData>();
  public string FeedItemId;
  public List<AnimalSpawnData> AnimalSpawnList = null;
}

public class EggExtensionData {
  public List<AnimalSpawnData> AnimalSpawnList = new List<AnimalSpawnData>();
}

public class AnimalSpawnData {
  public string Id;
  public string AnimalId;
  public string Condition;
}
