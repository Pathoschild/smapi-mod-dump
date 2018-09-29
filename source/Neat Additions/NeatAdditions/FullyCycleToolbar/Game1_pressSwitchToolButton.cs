using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using System;
using System.Reflection;

namespace NeatAdditions.FullyCycleToolbar
{
	public class Game1_pressSwitchToolButton : IPatch
	{
		public string GetPatchName() => "fully cycle toolbar";

		public static bool Prefix()
		{
			#region get whichWay
			MouseState mouseState = Mouse.GetState();
			int whichWay;

			if (mouseState.ScrollWheelValue <= Game1.oldMouseState.ScrollWheelValue)
				whichWay = ((mouseState.ScrollWheelValue < Game1.oldMouseState.ScrollWheelValue) ? 1 : 0);
			else
				whichWay = -1;

			if (Game1.options.gamepadControls && whichWay == 0)
				whichWay = ((!GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftTrigger)) ? 1 : (-1));

			if (Game1.options.invertScrollDirection)
				whichWay *= -1;

			whichWay /= Math.Abs(whichWay);
			#endregion

			if (Game1.player.CurrentToolIndex <= getIndexOfFirstItem() && whichWay < 0)
			{
				//Go back instead
				Game1.player.shiftToolbar(false);
				Game1.player.CurrentToolIndex = getIndexOfLastItem();
				return false;
			}

			if (Game1.player.CurrentToolIndex >= getIndexOfLastItem() && whichWay > 0)
			{
				//Go back instead
				Game1.player.shiftToolbar(true);
				Game1.player.CurrentToolIndex = getIndexOfFirstItem();
				return false;
			}

			return true;
		}

		private static int getIndexOfFirstItem()
		{
			for (int i = 0; i < 12; i++)
			{
				if (Game1.player.Items[i] != null)
					return i;

			}
			return 0;
		}
		private static int getIndexOfLastItem()
		{
			for (int i = 11; i >= 0; i--)
			{
				if (Game1.player.Items[i] != null)
					return i;

			}
			return 11;
		}

		public void Patch(HarmonyInstance harmony)
		{
			MethodBase original = Game1.game1.GetType().GetMethod("pressSwitchToolButton", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			var prefix = GetType().GetMethod("Prefix");
			harmony.Patch(original, new HarmonyMethod(prefix), null);
		}
	}
}
