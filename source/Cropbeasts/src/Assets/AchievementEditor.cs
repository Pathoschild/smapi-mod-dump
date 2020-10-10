/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace Cropbeasts.Assets
{
	internal class AchievementEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		public static readonly int Key = 79400200;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals ("Data\\Achievements");
		}

		public void Edit<T> (IAssetData asset)
		{
			var data = asset.AsDictionary<int, string> ().Data;
			string name = Helper.Translation.Get ("achievement.name");
			string description = Helper.Translation.Get ("achievement.description");
			data[Key] = $"{name}^{description}^true^-1^-1";
		}

		// Returns whether the player has attained the achievement for slaying
		// every type of cropbeast. Does not check for new attainment.
		public static bool HasAchievement ()
		{
			return Game1.player.achievements.Contains (Key);
		}

		// Checks for the player having attained the achievement for slaying
		// every type of cropbeast. Returns whether the achievement has been
		// attained, regardless of whether newly or previously.
		public static bool CheckAchievement (out int slain, out int total)
		{
			List<string> beastNames = Assets.MonsterEditor.List ();
			slain = beastNames.Count ((beastName) =>
				Game1.stats.getMonstersKilled (beastName) > 0);
			total = beastNames.Count;

			if (Game1.player.achievements.Contains (Key))
			{
				return true;
			}
			else if (slain >= total)
			{
				Game1.getAchievement (Key);
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool CheckAchievement ()
		{
			return CheckAchievement (out int _slain, out int _total);
		}
	}
}
