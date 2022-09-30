/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bwdy/SDVModding
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Events;
using System;
using System.Reflection;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace MinecartPatcher
{
	public class MCPModHooks : ModHooks
	{
		ModHooks alias;
		ModEntry modEntry;

		public MCPModHooks(ModEntry mod)
		{
			modEntry = mod;
			var field = typeof(Game1).GetField("hooks", BindingFlags.Static | BindingFlags.NonPublic);
			alias = (ModHooks)field.GetValue(null);
			field.SetValue(null, this);
		}

		public override void OnGame1_PerformTenMinuteClockUpdate(Action action)
		{
			alias.OnGame1_PerformTenMinuteClockUpdate(action);
		}

		public override void OnGame1_NewDayAfterFade(Action action)
		{
			alias.OnGame1_NewDayAfterFade(action);
		}

		public override void OnGame1_ShowEndOfNightStuff(Action action)
		{
			alias.OnGame1_ShowEndOfNightStuff(action);
		}

		public override void OnGame1_UpdateControlInput(ref KeyboardState keyboardState, ref MouseState mouseState, ref GamePadState gamePadState, Action action)
		{
			alias.OnGame1_UpdateControlInput(ref keyboardState, ref mouseState, ref gamePadState, action);
		}

		public override void OnGameLocation_ResetForPlayerEntry(GameLocation location, Action action)
		{
			alias.OnGameLocation_ResetForPlayerEntry(location, action);
		}

		public override bool OnGameLocation_CheckAction(GameLocation location, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, Func<bool> action)
		{
			var mc = modEntry.FindMinecart(location, new Vector2(tileLocation.X, tileLocation.Y));
			if (mc != null)
			{
				modEntry.OnMinecartActivation(mc, location, new Vector2(tileLocation.X, tileLocation.Y));
				return true;
			}
			return alias.OnGameLocation_CheckAction(location, tileLocation, viewport, who, action);
		}

		public override FarmEvent OnUtility_PickFarmEvent(Func<FarmEvent> action)
		{
			return alias.OnUtility_PickFarmEvent(action);
		}

		public override Task StartTask(Task task, string id)
		{
			return alias.StartTask(task, id);
		}

		public override Task<T> StartTask<T>(Task<T> task, string id)
		{
			return alias.StartTask<T>(task, id);
		}
	}
}
