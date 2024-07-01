/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

namespace Selph.StardewMods.ExtraAnimalConfig;

using Selph.StardewMods.Common;

public sealed class AnimalExtensionDataAssetHandler : DictAssetHandler<AnimalExtensionData> {
  public AnimalExtensionDataAssetHandler() : base($"{ModEntry.UniqueId}/AnimalExtensionData", ModEntry.StaticMonitor) {}
}

public sealed class EggExtensionDataAssetHandler : DictAssetHandler<EggExtensionData> {
  public EggExtensionDataAssetHandler() : base($"{ModEntry.UniqueId}/EggExtensionData", ModEntry.StaticMonitor) {}
}
