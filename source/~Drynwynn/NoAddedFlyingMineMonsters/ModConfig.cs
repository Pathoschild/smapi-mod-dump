/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Drynwynn/StardewValleyMods
**
*************************************************/

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
