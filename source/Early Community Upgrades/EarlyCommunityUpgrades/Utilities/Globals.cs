/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/EarlyCommunityUpgrades
**
*************************************************/

using StardewModdingAPI;

namespace EarlyCommunityUpgrades
{
	class Globals
	{
		public static IManifest Manifest { get; set; }
		public static ModConfig Config { get; set; }
		public static IModHelper Helper { get; set; }
		public static IMonitor Monitor { get; set; }

		/// <summary>
		/// A shortcut to the translation API
		/// </summary>
		/// <param name="key">The translation key</param>
		/// <param name="tokens">Tokens to replace in the translation</param>
		/// <returns>The retrieved translation</returns>
		public static string GetTranslation(string key, object tokens = null)
		{
			if (tokens == null)
			{
				return Helper.Translation.Get(key);
			}
			return Helper.Translation.Get(key, tokens);
		}
	}
}
