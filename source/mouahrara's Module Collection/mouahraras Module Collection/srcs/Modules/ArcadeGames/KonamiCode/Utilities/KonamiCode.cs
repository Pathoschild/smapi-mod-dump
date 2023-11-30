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
		private static readonly PerScreen<bool>	InfiniteLivesMode = new(() => false);
		private static readonly PerScreen<int>	Step = new(() => 0);

		internal static void Reset()
		{
			SetInfiniteLivesMode(false);
			SetStep(0);
		}

		internal static void SetInfiniteLivesMode(bool value)
		{
			InfiniteLivesMode.Value = value;
		}

		internal static bool GetInfiniteLivesMode()
		{
			return InfiniteLivesMode.Value;
		}

		internal static void SetStep(int value)
		{
			Step.Value = value;
		}

		internal static int GetStep()
		{
			return Step.Value;
		}

		internal static void ReceiveKeyPressPostfix(Keys k)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || GetInfiniteLivesMode())
				return;

			if (GetStep() == 0 && (k.Equals(Keys.Up) || k.Equals(Keys.W)))
			{
				SetStep(1);
				return;
			}
			else if (GetStep() == 1 && (k.Equals(Keys.Up) || k.Equals(Keys.W)))
			{
				SetStep(2);
				return;
			}
			else if (GetStep() == 2 && (k.Equals(Keys.Down) || k.Equals(Keys.S)))
			{
				SetStep(3);
				return;
			}
			else if (GetStep() == 3 && (k.Equals(Keys.Down) || k.Equals(Keys.S)))
			{
				SetStep(4);
				return;
			}
			else if (GetStep() == 4 && (k.Equals(Keys.Left) || k.Equals(Keys.A)))
			{
				SetStep(5);
				return;
			}
			else if (GetStep() == 5 && (k.Equals(Keys.Right) || k.Equals(Keys.D)))
			{
				SetStep(6);
				return;
			}
			else if (GetStep() == 6 && (k.Equals(Keys.Left) || k.Equals(Keys.A)))
			{
				SetStep(7);
				return;
			}
			else if (GetStep() == 7 && (k.Equals(Keys.Right) || k.Equals(Keys.D)))
			{
				SetStep(8);
				return;
			}
			else if (GetStep() == 8 && (k.Equals(Keys.B) || k.Equals(Keys.E)))
			{
				SetStep(9);
				return;
			}
			else if (GetStep() == 9 && (k.Equals(Keys.A) || k.Equals(Keys.X)))
			{
				SetInfiniteLivesMode(true);
				Game1.playSound("Cowboy_Secret");
				return;
			}
			else
			{
				SetStep(0);
				return;
			}
		}
	}
}
