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
using StardewValley;

namespace TheLion.AwesomeProfessions
{
	internal class StaticSaveLoadedEvent : SaveLoadedEvent
	{
		/// <summary>Raised after loading a save (including the first day after creating a new save), or connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		public override void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			// load persisted mod data
			AwesomeProfessions.Data = Game1.player.modData;

			// verify mod data and initialize assets and helpers
			foreach (var professionIndex in Game1.player.professions) Utility.InitializeModData(professionIndex);

			// subcribe events for loaded save
			AwesomeProfessions.EventManager.SubscribeEventsForLocalPlayer();
		}
	}
}