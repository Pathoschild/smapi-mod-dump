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

namespace TheLion.AwesomeProfessions
{
	/// <summary>Base class for harmony patches.</summary>
	internal abstract class BasePatch
	{
		protected static ProfessionsConfig Config { get; private set; }
		protected static ProfessionsData Data { get; private set; }
		protected static IMonitor Monitor { get; private set; }

		/// <summary>Construct an instance.</summary>
		internal BasePatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal abstract void Apply(HarmonyInstance harmony);

		/// <summary>Initialize static fields.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		public static void Init(ProfessionsConfig config, IMonitor monitor)
		{
			Config = config;
			Monitor = monitor;
		}

		/// <summary>Set mod data reference for patches.</summary>
		/// <param name="data">The mod persisted data.</param>
		public static void SetData(ProfessionsData data)
		{
			Data = data;
		}
	}
}
