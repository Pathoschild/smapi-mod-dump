/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using System;
using SObject = StardewValley.Object;

namespace Shockah.InAHeartbeat;

public sealed class QualityBasedConfig<T>
{
	[JsonProperty] public T Regular { get; internal set; }
	[JsonProperty] public T Silver { get; internal set; }
	[JsonProperty] public T Gold { get; internal set; }
	[JsonProperty] public T Iridium { get; internal set; }

	public QualityBasedConfig(T regular, T silver, T gold, T iridium)
	{
		this.Regular = regular;
		this.Silver = silver;
		this.Gold = gold;
		this.Iridium = iridium;
	}

	public T GetForQuality(int quality)
		=> quality switch
		{
			SObject.bestQuality => Iridium,
			SObject.highQuality => Gold,
			SObject.medQuality => Silver,
			_ => Regular
		};
}

public static class QualityBasedConfigExt
{
	public static T GetMin<T>(this QualityBasedConfig<T> config) where T : IComparable<T>
	{
		static T Min(T a, T b)
			=> a.CompareTo(b) <= 0 ? a : b;

		return Min(Min(config.Regular, config.Silver), Min(config.Gold, config.Iridium));
	}

	public static T GetMax<T>(this QualityBasedConfig<T> config) where T : IComparable<T>
	{
		static T Max(T a, T b)
			=> a.CompareTo(b) >= 0 ? a : b;

		return Max(Max(config.Regular, config.Silver), Max(config.Gold, config.Iridium));
	}
}