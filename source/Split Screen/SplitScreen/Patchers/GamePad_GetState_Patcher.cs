/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/SplitScreen
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace SplitScreen.Patchers
{
	[HarmonyPatch(typeof(Microsoft.Xna.Framework.Input.GamePad))]
	[HarmonyPatch("GetState")]
	[HarmonyPatch(new Type[] { typeof(PlayerIndex), typeof(GamePadDeadZone)})]//The GetState(PlayerIndex playerIndex) calls this method, so only need to edit one method
	class GamePad_GetState_Patcher
	{
		public static GamePadState Postfix(GamePadState g, PlayerIndex playerIndex, GamePadState __result)
		{
			if (playerIndex.Equals(PlayerIndex.One) && PlayerIndexController._PlayerIndex != PlayerIndex.One)
				return PlayerIndexController.GetRawGamePadState();
			else return __result;
		}
	}
}
