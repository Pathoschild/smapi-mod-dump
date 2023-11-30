/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System;
using Microsoft.Xna.Framework.Graphics;

namespace mouahrarasModuleCollection.ArcadeGames.PayToPlay.Managers
{
	internal static class AssetManager
	{
		public static Texture2D JourneyOfThePrairieKing;

		internal static void Apply()
		{
			if (ModEntry.Helper.Translation.Locale.Equals("fr-FR", StringComparison.OrdinalIgnoreCase))
				JourneyOfThePrairieKing = ModEntry.Helper.ModContent.Load<Texture2D>("assets/ArcadeGames/PayToPlay/JourneyOfThePrairieKing.fr-FR.png");
			else if (ModEntry.Helper.Translation.Locale.Equals("zh-CN", StringComparison.OrdinalIgnoreCase))
				JourneyOfThePrairieKing = ModEntry.Helper.ModContent.Load<Texture2D>("assets/ArcadeGames/PayToPlay/JourneyOfThePrairieKing.zh-CN.png");
			else
				JourneyOfThePrairieKing = ModEntry.Helper.ModContent.Load<Texture2D>("assets/ArcadeGames/PayToPlay/JourneyOfThePrairieKing.png");
		}
	}
}
