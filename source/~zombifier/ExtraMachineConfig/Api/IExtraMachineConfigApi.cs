/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using StardewValley.GameData.Machines;
using System.Collections.Generic;

namespace Selph.StardewMods.ExtraMachineConfig;

public interface IExtraMachineConfigApi {
  IList<(string, int)> GetExtraRequirements(MachineItemOutput outputData);
  IList<(string, int)> GetExtraTagsRequirements(MachineItemOutput outputData);
  IList<MachineItemOutput> GetExtraOutputs(MachineItemOutput outputData);
}
