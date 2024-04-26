/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/BuildableGingerIslandFarm
**
*************************************************/

namespace BuildableGingerIslandFarm.Utilities
{
	internal class Compatibility
	{
		internal static readonly bool IsIslandOverhaulLoaded = ModEntry.Helper.ModRegistry.IsLoaded("Lnh.IslandOverhaul");
		internal static readonly bool IsModestMapsGingerIslandFarmLoaded = ModEntry.Helper.ModRegistry.IsLoaded("InkubusMods.ModestGinger");
	}
}
