/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Base class for dynamic events.</summary>
	internal abstract class BaseEvent
	{
		protected static ProfessionsConfig Config { get; private set; }
		protected static ProfessionsData Data { get; private set; }

		/// <summary>Construct an instance.</summary>
		internal BaseEvent() { }

		/// <summary>Hook this event to an event handler.</summary>
		/// <param name="handler">Interface to the SMAPI event handler.</param>
		public abstract void Hook(IModEvents handler);

		/// <summary>Unhook this event from an event handler.</summary>
		/// <param name="handler">Interface to the SMAPI event handler.</param>
		public abstract void Unhook(IModEvents handler);

		/// <summary>Initialize static fields.</summary>
		/// <param name="config">The mod settings.</param>
		public static void Init(ProfessionsConfig config)
		{
			Config = config;
		}

		/// <summary>Set mod data reference for patches.</summary>
		/// <param name="data">The mod persisted data.</param>
		public static void SetData(ProfessionsData data)
		{
			Data = data;
		}
	}
}
