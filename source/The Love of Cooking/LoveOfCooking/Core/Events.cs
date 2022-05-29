/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;

namespace LoveOfCooking
{
	public class CookingExperienceGainedEventArgs : EventArgs
	{
		public int Value { get; }
		internal CookingExperienceGainedEventArgs(int value)
		{
			this.Value = value;
		}
	}

	public class BushShakenEventArgs : EventArgs
	{
		public CustomBush Bush;
		internal BushShakenEventArgs(CustomBush bush)
		{
			this.Bush = bush;
		}
	}

	public class BushToolUsedEventArgs : EventArgs
	{
		public CustomBush Bush;
		public Tool Tool;
		public int Explosion;
		public Vector2 TileLocation;
		public GameLocation Location;
		internal BushToolUsedEventArgs(CustomBush bush, Tool tool, int explosion, Vector2 tileLocation, GameLocation location)
		{
			this.Bush = bush;
			this.Tool = tool;
			this.Explosion = explosion;
			this.TileLocation = tileLocation;
			this.Location = location;
		}
	}


	public static class Events
	{
		public static event EventHandler CookingExperienceGained;
		public static event EventHandler BushShaken;
		public static event EventHandler BushToolUsed;

		internal static void InvokeOnCookingExperienceGained(int experienceGained)
		{
			if (CookingExperienceGained is null)
				return;

			CookingExperienceGained.Invoke(
				sender: null,
				e: new CookingExperienceGainedEventArgs(value: experienceGained));
		}

		internal static void InvokeOnBushShaken(CustomBush bush)
		{
			if (BushShaken is null)
				return;

			BushShaken.Invoke(
				sender: null,
				e: new BushShakenEventArgs(bush: bush));
		}

		internal static void InvokeOnBushToolUsed(CustomBush bush, Tool tool, int explosion, Vector2 tileLocation, GameLocation location)
		{
			if (BushToolUsed is null)
				return;

			BushToolUsed.Invoke(
				sender: null,
				e: new BushToolUsedEventArgs(bush: bush, tool: tool, explosion: explosion, tileLocation: tileLocation, location: location));
		}
	}
}
