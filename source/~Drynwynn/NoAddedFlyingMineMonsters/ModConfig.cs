using DsStardewLib.Config;
using StardewModdingAPI;

namespace NoAddedFlyingMineMonsters
{
  class ModConfig : HarmonyConfig
  {
    public bool HarmonyDebug { get; set; } = false;
    public bool HarmonyLoad { get; set; } = true;

    public bool NoRandomMonsters { get; set; } = true;
    public SButton NoRandomMonstersButton { get; set; } = SButton.None;
  }
}
