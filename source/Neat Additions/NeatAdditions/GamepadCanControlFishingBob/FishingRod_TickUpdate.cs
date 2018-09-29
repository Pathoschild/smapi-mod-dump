using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Tools;
using System;
using static System.Math;

namespace NeatAdditions.GamepadCanControlFishingBob
{
	public class FishingRod_TickUpdate : IPatch
	{
		public string GetPatchName() => "fishing bob control for gamepads";

		public static void Postfix(FishingRod __instance, Farmer who)
		{
			if (!__instance.isReeling && !__instance.isFishing && who.UsingTool && __instance.castedButBobberStillInAir)
			{
				UseControllerInput(__instance, who);
			}
		}

		private static void UseControllerInput(FishingRod fishingRod, Farmer who)
		{
			//Get gamepad input
			float deadzone = 0.1f;
			GamePadState state = GamePad.GetState(PlayerIndex.One);

			Func<float, ButtonState, float> getFactor = (stick, button) => Max(Max(stick, 0), button == ButtonState.Pressed ? 1 : 0);
			float rightPressedFactor = getFactor(state.ThumbSticks.Left.X, state.DPad.Right);//0 if in center or anywhere to the left, 1 if completely right
			float leftPressedFactor = getFactor(-state.ThumbSticks.Left.X, state.DPad.Left);
			float upPressedFactor = getFactor(state.ThumbSticks.Left.Y, state.DPad.Up);
			float downPressedFactor = getFactor(-state.ThumbSticks.Left.Y, state.DPad.Down);
			
			#region Mostly vanilla code - StardewValley.Tools.FishingRod:1654 to 1682
			Vector2 motion = Vector2.Zero;
			if (downPressedFactor >= deadzone && who.FacingDirection != 2 && who.FacingDirection != 0)
			{
				motion.Y += 4f * downPressedFactor;
			}
			if (rightPressedFactor >= deadzone && who.FacingDirection != 1 && who.FacingDirection != 3)
			{
				motion.X += 2f * rightPressedFactor;
			}
			if (upPressedFactor >= deadzone && who.FacingDirection != 0 && who.FacingDirection != 2)
			{
				motion.Y -= 4f * upPressedFactor;
			}
			if (leftPressedFactor >= deadzone && who.FacingDirection != 3 && who.FacingDirection != 1)
			{
				motion.X -= 2f * leftPressedFactor;
			}
			if (who.IsLocalPlayer)
			{
				fishingRod.bobber.Set((Vector2)fishingRod.bobber + motion);
			}
			if (fishingRod.animations.Count > 0)
			{
				TemporaryAnimatedSprite temporaryAnimatedSprite = fishingRod.animations[0];
				temporaryAnimatedSprite.position += motion;
			}
			#endregion
		}

		public void Patch(HarmonyInstance harmony)
		{
			var original = new StardewValley.Tools.FishingRod().GetType().GetMethod("tickUpdate");
			var postfix = GetType().GetMethod("Postfix");
			harmony.Patch(original, null, new HarmonyMethod(postfix));
		}
	}
}
