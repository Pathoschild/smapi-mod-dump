using Harmony;
using Microsoft.Xna.Framework.Input;
using SplitScreen.Mice;
using System;

namespace SplitScreen.Patchers
{
	//Usually ignores mouse when window is inactive (Don't use Game1.IsActive, that is always set to true)

	[HarmonyPatch(typeof(Microsoft.Xna.Framework.Input.Mouse))]
	[HarmonyPatch("GetState")]
	[HarmonyPatch(new Type[] { })]
	public class Mouse_GetState_Patcher
	{
		public static MouseState Postfix(MouseState m, MouseState __result)
		{
			if (MultipleMiceManager.HasAttachedMouse() || !Utils.TrueIsWindowActive())
			{
				if (Utils.IsMouseLocked() && MultipleMiceManager.HasAttachedMouse())
					return MultipleMiceManager.GetAttachedMouseState() ?? default(MouseState);
				else
				{
					return new MouseState(FakeMouse.X, FakeMouse.Y, 0, 
						ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
				}
			}
			else return __result;
		}		
	}
}
