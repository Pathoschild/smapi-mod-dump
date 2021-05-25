/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Harmony patch base class.</summary>
	internal abstract class BasePatch : IPatch
	{
		protected static IMonitor Monitor { get; private set; }
		protected static ILHelper Helper { get; private set; }

		/// <summary>Initialize the ILHelper.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal static void Init(IMonitor monitor)
		{
			Monitor = monitor;
			Helper = new ILHelper(monitor);
		}

		/// <inheritdoc/>
		public abstract void Apply(HarmonyInstance harmony);
	}
}