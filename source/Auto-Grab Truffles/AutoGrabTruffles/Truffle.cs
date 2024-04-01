/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/qixing-jk/QiXingAutoGrabTruffles
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley;

namespace AutoGrabTruffles;

public class Truffle
{
	public enum Quality
	{
		Base = 0,
		Silver = 1,
		Gold = 2,
		Iridium = 4
	}

	public Dictionary<Quality, int> Summary = new Dictionary<Quality, int>();

	public Queue<StardewValley.Object> Queue = new Queue<StardewValley.Object>();

	private static ModConfig Config;

	public Truffle(ModConfig config)
	{
		Config = config;
	}

	public static bool IsValid(StardewValley.Object truffle)
	{
		if (!truffle.bigCraftable.Value)
		{
			return truffle.ParentSheetIndex == 430;
		}
		return false;
	}

	public static StardewValley.Object UpdateData(StardewValley.Object truffle)
	{
		Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)truffle.TileLocation.X + (int)truffle.TileLocation.Y * 777);
		if (Config.ApplyGathererBonus && Game1.player.professions.Contains(13) && random.NextDouble() < 0.2)
		{
			truffle.Stack++;
		}
		if (Config.ApplyBotanistBonus && Game1.player.professions.Contains(16))
		{
			truffle.Quality = 4;
		}
		else if (random.NextDouble() < (double)Game1.player.ForagingLevel / 30.0)
		{
			truffle.Quality = 2;
		}
		else if (random.NextDouble() < (double)Game1.player.ForagingLevel / 15.0)
		{
			truffle.Quality = 1;
		}
		return truffle;
	}

	public void InitializeSummary()
	{
		Summary.Clear();
		foreach (Quality quality in Enum.GetValues(typeof(Quality)))
		{
			Summary.Add(quality, 0);
		}
	}

	public void Enqueue(StardewValley.Object truffle)
	{
		Queue.Enqueue(truffle);
	}

	public bool TryDequeue(out StardewValley.Object truffle)
	{
		return Queue.TryDequeue(out truffle);
	}
}
