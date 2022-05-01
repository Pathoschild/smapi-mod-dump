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
using System.Text;
using System.Threading.Tasks;

using StardewValley;

namespace Leclair.Stardew.Almanac {

	public enum LevelType {
		None,
		Mushroom,
		InfestedMonster,
		InfestedSlime,
		Quarry,
		InfestedQuarry,
		Dino
	};

	public class MineHelper {

		public static bool UnlockedQuarry() {
			return Game1.MasterPlayer.hasOrWillReceiveMail("VisitedQuarryMine");
		}

		public static int GetDifficulty(int floor) {
			if (floor == 77377)
				return 0;

			return floor > 120
				? Game1.netWorldState.Value.SkullCavesDifficulty
				: Game1.netWorldState.Value.MinesDifficulty;
		}

		public static LevelType GetLevelType(int floor, ulong seed, int date) {
			if (floor < 1 || floor > 77377)
				return LevelType.None;

			Random rnd;

			// Standard Mines
			if (floor >= 1 && floor <= 120) {
				// Elevator Floors have nothing.
				if (floor % 5 == 0)
					return LevelType.None;

				// Check for monster infestation first, since that
				// has a higher priority.
				rnd = new((date + 1) + floor * 100 + (int)seed / 2);

				if (rnd.NextDouble() < 0.044 && floor % 40 > 5 && floor % 40 < 30 && floor % 40 != 19) {
					if (rnd.NextDouble() < 0.5)
						return LevelType.InfestedMonster;
					return LevelType.InfestedSlime;
				}

				if (rnd.NextDouble() < 0.044 && UnlockedQuarry() && floor % 40 > 1) {
					// Infested Quarry, while possible, is a bit too much
					// data to show in our calendar view.
					//if (rnd.NextDouble() < 0.25)
					//	return LevelType.InfestedQuarry;
					return LevelType.Quarry;
				}

				// Now check for mushrooms.
				rnd = new((date - 1) * floor + 4 * floor + (int)seed / 2);

				// Duplicate randomness calls that are used before.
				// We don't care about their output.
				if (rnd.NextDouble() < 0.3 && floor > 2)
					rnd.NextDouble();

				rnd.NextDouble();

				if (rnd.NextDouble() < 0.035 && floor > 80)
					return LevelType.Mushroom;

				return LevelType.None;
			}

			// Skull Caves
			if (floor >= 121 && floor < 77377) {
				rnd = new((date + 1) + floor * 100 + (int)seed / 2);

				if (rnd.NextDouble() < 0.044 && floor % 40 > 5 && floor % 40 < 30 && floor % 40 != 19) {
					LevelType result;
					if (rnd.NextDouble() < 0.5)
						result = LevelType.InfestedMonster;
					else
						result = LevelType.InfestedSlime;

					if (rnd.NextDouble() < 0.5)
						return LevelType.Dino;

					return result;
				}
			}

			// Quarry
			if (floor == 77377)
				return LevelType.Quarry;

			return LevelType.None;
		}

	}
}
