/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/desto-git/sdv-mods
**
*************************************************/

namespace GiftDecline
{
	using Common;
	using StardewModdingAPI;

	/// <summary>Mod Configuration helper.</summary>
	internal static class ConfigHelper
	{
		/// <summary>Identifier to send and receive this mod's config in multiplayer.</summary>
		public const string Key = "Config";

		/// <summary>Mod Configuration settings. This is overwritten by the host config in multiplayer.</summary>
		public static ModConfig Config { get; set; }

		/// <summary>Mod Configuration settings.
		/// This one is kept so it can be restored when leaving the multiplayer session.
		/// </summary>
		private static ModConfig LocalConfig { get; set; }

		/// <summary>Initialize the helper.</summary>
		/// <param name="helper">Instance to use for reading the config file.</param>
		public static void Init(IModHelper helper)
		{
			Config = helper.ReadConfig<ModConfig>();
			if (Config.ResetEveryXDays < 0)
			{
				throw new System.Exception("Error in config.json: \"ResetEveryXDays\" must be at least 0.");
			}

			if (Config.MaxReduction < 1)
			{
				throw new System.Exception("Error in config.json: \"MaxReduction\" must be at least 1.");
			}

			if (Config.ReduceAfterXGifts < 1)
			{
				throw new System.Exception("Error in config.json: \"ReduceAfterXGifts\" must be at least 1.");
			}

			LocalConfig = helper.ReadConfig<ModConfig>();
		}

		/// <summary>Send the configuration to all peers.</summary>
		public static void SyncWithPeers()
		{
			MultiplayerHelper.SendMessage(Config, Key);
		}

		/// <summary>Restore the configuration to the local one.</summary>
		public static void RestoreLocalConfig()
		{
			Logger.Trace("Restoring local configuration");
			Config = LocalConfig;
		}
	}
}