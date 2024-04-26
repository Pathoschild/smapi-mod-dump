/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley.GameData.WildTrees;

namespace CustomTapperFramework;

public class ExtendedTapItemData : WildTreeTapItemData {
  // If specified, only applies this rule if the tap object is of this ID.
  public string SourceId = null;
  public bool RecalculateOnCollect = false;
}

public class TapperModel {
  public bool AlsoUseBaseGameRules = false;
  public List<ExtendedTapItemData> TreeOutputRules { get; set; } = null;
  public List<ExtendedTapItemData> FruitTreeOutputRules { get; set; } = null;
  public List<ExtendedTapItemData> GiantCropOutputRules { get; set; } = null;
}
