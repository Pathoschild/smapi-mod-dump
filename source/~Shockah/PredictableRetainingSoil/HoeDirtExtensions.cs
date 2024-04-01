/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewValley.TerrainFeatures;

namespace Shockah.PredictableRetainingSoil;

public static class HoeDirtExtensions
{
	private static readonly string Key = $"{typeof(HoeDirtExtensions).Namespace!}::RetainingSoilDaysLeft";

	public static int GetRetainingSoilDaysLeft(this HoeDirt instance)
		=> instance.modData.TryGetValue(Key, out var stringData) && int.TryParse(stringData, out var value) ? value : 0;

	public static void SetRetainingSoilDaysLeft(this HoeDirt instance, int value)
		=> instance.modData[Key] = value.ToString();
}