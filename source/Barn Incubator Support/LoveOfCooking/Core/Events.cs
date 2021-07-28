/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System;

namespace LoveOfCooking
{
	public class CookingExperienceGainedEventArgs : EventArgs
	{
		internal CookingExperienceGainedEventArgs(int value)
		{
			Value = value;
		}

		public int Value { get; }
	}

	public class Events
	{
		public static event EventHandler CookingExperienceGained;

		internal static void InvokeOnCookingExperienceGained(int experienceGained)
		{
			if (CookingExperienceGained == null)
				return;

			CookingExperienceGained.Invoke(null, new CookingExperienceGainedEventArgs(value: experienceGained));
		}
	}
}
