/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Netcode;
using StardewValley.TerrainFeatures;
using System.Runtime.CompilerServices;

namespace Shockah.PredictableRetainingSoil;

public static class HoeDirtExtensions
{
	private class Holder
	{
		internal static ConditionalWeakTable<HoeDirt, Holder> Values { get; private set; } = new();

		public NetInt RetainingSoilDaysLeft { get; private set; } = new(0);
	}

	internal static NetInt GetRetainingSoilDaysLeftNetField(this HoeDirt instance)
		=> Holder.Values.GetOrCreateValue(instance).RetainingSoilDaysLeft;

	public static int GetRetainingSoilDaysLeft(this HoeDirt instance)
		=> Holder.Values.TryGetValue(instance, out var holder) ? holder.RetainingSoilDaysLeft.Value : 0;

	public static void SetRetainingSoilDaysLeft(this HoeDirt instance, int value)
		=> Holder.Values.GetOrCreateValue(instance).RetainingSoilDaysLeft.Set(value);
}