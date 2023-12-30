/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using StardewModdingAPI.Utilities;
using StardewValley;
using Microsoft.Xna.Framework.Input;

namespace mouahrarasModuleCollection.ArcadeGames.KonamiCode.Utilities
{
	internal class KonamiCodeUtility
	{
		private static readonly PerScreen<bool>	infiniteLivesMode = new(() => false);
		private static readonly PerScreen<int>	step = new(() => 0);

		internal static void Reset()
		{
			InfiniteLivesMode = false;
			Step = 0;
		}

		internal static bool InfiniteLivesMode
		{
			get => infiniteLivesMode.Value;
			set => infiniteLivesMode.Value = value;
		}

		internal static int Step
		{
			get => step.Value;
			set => step.Value = value;
		}

		internal static void ReceiveKeyPressPostfix(Keys k)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || InfiniteLivesMode)
				return;

			if (Step == 0 && (k.Equals(Keys.Up) || k.Equals(Keys.W)))
			{
				Step = 1;
				return;
			}
			else if (Step == 1 && (k.Equals(Keys.Up) || k.Equals(Keys.W)))
			{
				Step = 2;
				return;
			}
			else if (Step == 2 && (k.Equals(Keys.Down) || k.Equals(Keys.S)))
			{
				Step = 3;
				return;
			}
			else if (Step == 3 && (k.Equals(Keys.Down) || k.Equals(Keys.S)))
			{
				Step = 4;
				return;
			}
			else if (Step == 4 && (k.Equals(Keys.Left) || k.Equals(Keys.A)))
			{
				Step = 5;
				return;
			}
			else if (Step == 5 && (k.Equals(Keys.Right) || k.Equals(Keys.D)))
			{
				Step = 6;
				return;
			}
			else if (Step == 6 && (k.Equals(Keys.Left) || k.Equals(Keys.A)))
			{
				Step = 7;
				return;
			}
			else if (Step == 7 && (k.Equals(Keys.Right) || k.Equals(Keys.D)))
			{
				Step = 8;
				return;
			}
			else if (Step == 8 && (k.Equals(Keys.B) || k.Equals(Keys.E)))
			{
				Step = 9;
				return;
			}
			else if (Step == 9 && (k.Equals(Keys.A) || k.Equals(Keys.X)))
			{
				InfiniteLivesMode = true;
				Game1.playSound("Cowboy_Secret");
				return;
			}
			else
			{
				Step = 0;
				return;
			}
		}
	}
}
