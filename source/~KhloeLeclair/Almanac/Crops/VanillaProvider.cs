/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using Leclair.Stardew.Common;

using Microsoft.Xna.Framework;

using StardewValley;

using SObject = StardewValley.Object;

namespace Leclair.Stardew.Almanac.Crops {
	public class VanillaProvider : ICropProvider {

		public int Priority => 0;

		public IEnumerable<CropInfo> GetCrops() {
			Dictionary<int, string> crops = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
			List<CropInfo> result = new();

			foreach (var entry in crops) {

				// TODO: Create Crop instances so we can
				// lean more on vanilla logic.

				string[] bits = entry.Value.Split('/');
				if (bits.Length < 5)
					continue;

				int sprite = Convert.ToInt32(bits[2]);
				int harvest = Convert.ToInt32(bits[3]);
				int regrow = Convert.ToInt32(bits[4]);

				bool trellisCrop = bits.Length > 7 && bits[7].Equals("true");

				if (!Game1.objectInformation.ContainsKey(harvest))
					continue;

				string[] seasons = bits[1].Split(' ');

				WorldDate startDate = null;
				WorldDate endDate = null;

				// TODO: Handle weird crops with a gap.

				foreach (string season in seasons) {
					WorldDate start = new(1, season, 1);
					WorldDate end = new(1, season, WorldDate.DaysPerMonth);

					if (startDate == null || startDate > start)
						startDate = start;
					if (endDate == null || endDate < end)
						endDate = end;
				}

				// Skip crops that are always active.
				if (startDate.SeasonIndex == 0 && startDate.DayOfMonth == 1 && endDate.SeasonIndex == (WorldDate.MonthsPerYear - 1) && endDate.DayOfMonth == WorldDate.DaysPerMonth)
					continue;

				// If the sprite is 23, it's a seasonal multi-seed
				// so we want to show that rather than the seed.
				Item item = new SObject(sprite == 23 ? entry.Key : harvest, 1).getOne();

				// Phases
				int[] phases = bits[0].Split(' ').Select(val => Convert.ToInt32(val)).ToArray();

				// Stupid hard coded bullshit.
				bool paddyCrop = harvest == 271 || harvest == 830;

				// Phase Sprites
				// We need an extra sprite for the finished crop,
				// as well as one for regrow if that's enabled.
				SpriteInfo[] sprites = new SpriteInfo[phases.Length + 1 + (regrow > 0 ? 1 : 0)];

				// Are we dealing with colors?
				Color? color = null;
				string[] colorbits = regrow <= 0 && bits.Length > 8 ? bits[8].Split(' ') : null;
				if (colorbits != null && colorbits.Length >= 4 && colorbits[0].Equals("true")) {
					// Technically there could be many colors, but we just use
					// the first one.
					color = new(
						Convert.ToByte(colorbits[1]),
						Convert.ToByte(colorbits[2]),
						Convert.ToByte(colorbits[3])
					);
				}

				for (int i = 0; i < sprites.Length; i++) {
					bool final = i == (sprites.Length - (regrow > 0 ? 2 : 1));

					sprites[i] = new(
						Game1.cropSpriteSheet,
						new Rectangle(
							Math.Min(240, (i + 1) * 16 + (sprite % 2 != 0 ? 128 : 0)),
							sprite / 2 * 16 * 2,
							16, 32
						),
						overlaySource: final && color.HasValue ? new Rectangle(
							Math.Min(240, (i + 2) * 16 + (sprite % 2 != 0 ? 128 : 0)),
							sprite / 2 * 16 * 2,
							16, 32
						) : null,
						overlayColor: final ? color : null
					);
				}

				// TODO: Real giant crop detection.
				bool giant = false;
				if (harvest == 190 || harvest == 254 || harvest == 276)
					giant = true;

				result.Add(new(
					Convert.ToString(entry.Key),
					item,
					item.DisplayName,
					SpriteHelper.GetSprite(item, ModEntry.instance.Helper),
					giant,
					trellisCrop,
					phases,
					regrow,
					paddyCrop,
					sprites,
					startDate,
					endDate
				));
			}

			return result;
		}
	}
}
