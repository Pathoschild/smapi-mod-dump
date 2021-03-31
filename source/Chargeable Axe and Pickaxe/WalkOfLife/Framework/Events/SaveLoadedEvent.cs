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
	internal class SaveLoadedEvent : BaseEvent
	{
		private EventManager _Manager { get; }

		/// <summary>Construct an instance.</summary>
		internal SaveLoadedEvent(EventManager manager)
		{
			_Manager = manager;
		}

		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Hook(IModEvents listener)
		{
			listener.GameLoop.SaveLoaded += OnSaveLoaded;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Unhook(IModEvents listener)
		{
			listener.GameLoop.SaveLoaded -= OnSaveLoaded;
		}

		/// <summary>Raised after loading a save (including the first day after creating a new save), or connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			// load persisted mod data
			AwesomeProfessions.Data = AwesomeProfessions.ModHelper.Data.ReadSaveData<ProfessionsData>("thelion.AwesomeProfessions") ?? new ProfessionsData();
			BaseEvent.SetData(AwesomeProfessions.Data);
			BasePatch.SetData(AwesomeProfessions.Data);
			Utility.SetData(AwesomeProfessions.Data);

			// start treasure hunt managers
			AwesomeProfessions.ProspectorHunt = new ProspectorHunt(Config, Data, _Manager, AwesomeProfessions.I18n, AwesomeProfessions.ModHelper.Content);
			AwesomeProfessions.ScavengerHunt = new ScavengerHunt(Config, Data, _Manager, AwesomeProfessions.I18n, AwesomeProfessions.ModHelper.Content);
			
			// hook events for loaded save
			_Manager.SubscribeProfessionEventsForLocalPlayer();
		}
	}
}
