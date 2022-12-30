/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andraemon/SlimeProduce
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SObject = StardewValley.Object;

namespace SlimeProduce
{
    public class SlimeBall
    {
        public SlimeBall(SObject slimeBall)
        {
			Valid = StringyStuff.TryParseSlimeString(slimeBall.orderData.Value,
				out Color color, out bool isTiger, out bool isFirstGeneration, out int specialNumber);
            SlimeColor = color;
            IsTiger = isTiger;
			IsFirstGeneration = isFirstGeneration;
			SpecialNumber = specialNumber;
			TileLocation = slimeBall.TileLocation;
		}

		public Dictionary<int, int> GenerateDrops()
        {
			Dictionary<int, int> drops = new();

			if (!Valid) return drops;

			ModConfig config = ModEntry.Config;

			ColorRange purple = new(new int[] { 151, 255 }, new int[] { 0, 49 }, new int[] { 181, 255 });
			ColorRange white = new(new int[] { 231, 255 }, new int[] { 231, 255 }, new int[] { 231, 255 });

			Random r = new((int)(Game1.stats.DaysPlayed + Game1.uniqueIDForThisGame + TileLocation.X * 77 + TileLocation.Y * 777));

			// Handle special color drops
			if (config.EnableSpecialColorDrops && StardewMelon.ColoredObjects.ContainsKey(SlimeColor))
				if (r.NextDouble() < config.SpecialColorDropChance)
				{
					List<int> i = StardewMelon.ColoredObjects[SlimeColor];
					drops.Add(i[r.Next(i.Count)], r.Next(config.SpecialColorMinDrop, config.SpecialColorMaxDrop + 1));
				}

			// Handle color-specific drop behaviour
			if (config.EnableSpecialTigerSlimeDrops && IsTiger)
			{
				// Add sap and jade, in that order
				drops.Add(92, r.Next(15, 26));
				drops.Add(70, r.Next(1, 3));

				// Add ginger
				if (r.NextDouble() < 0.65)
					drops.Add(829, r.Next(4, 9));

				// Add dragon teeth, magma caps, and cinder shards, in that order
				if (IsFirstGeneration)
				{
					if (r.NextDouble() < 0.33)
						drops.Add(852, r.Next(1, 3));
					if (r.NextDouble() < 0.33)
						drops.Add(851, r.Next(1, 3));
					if (r.NextDouble() < 0.5)
						drops.Add(848, r.Next(5, 11));
				}
				
				return drops;
			}
			if (config.EnableSpecialWhiteSlimeDrops && white.Contains(SlimeColor))
			{
				// Add refined quartz and iron, in that order
				if (SlimeColor.R % 2 == 1)
				{
					drops.Add(338, r.Next(2, 5));

					if (SlimeColor.G % 2 == 1)
						drops[338] += r.Next(2, 5);
				}
				else drops.Add(380, r.Next(10, 21));

				// Add diamonds
				if (SlimeColor.R % 2 == 0 && SlimeColor.G % 2 == 0 && SlimeColor.B % 2 == 0 || SlimeColor == Color.White)
					drops.Add(72, r.Next(1, 3));
				
				return drops;
			}
			if (config.EnableSpecialPurpleSlimeDrops && purple.Contains(SlimeColor) && SpecialNumber % (IsFirstGeneration ? 4 : 2) == 0)
			{
				// Add iridium ore
				drops.Add(386, r.Next(5, 11));

				// Add iridium bars
				if (IsFirstGeneration && r.NextDouble() < 0.072)
					drops.Add(337, 1);

				return drops;
			}

			// Handle user-specified drop behaviour
			foreach (DropTable table in config.DropTables)
				if (table.colorRange.Contains(SlimeColor))
				{
					foreach (ItemDrop drop in table.itemDrops)
						if (r.NextDouble() < drop.dropChance)
							drops.Add(drop.parentSheetIndex, r.Next(drop.minDrop, drop.maxDrop + 1));

					return drops;
				}

			return drops;
        }

		public bool Valid;
        public Color SlimeColor;
		public bool IsTiger;
		public bool IsFirstGeneration;
		public int SpecialNumber;
		public Vector2 TileLocation;
    }
}
