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
using TheLion.Stardew.Professions.Integrations;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class StaticGameLaunchedEvent : GameLaunchedEvent
	{
		//

		/// <inheritdoc />
		public override void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// add Generic Mod Config Menu integration
			new GenericModConfigMenuIntegrationForAwesomeTools(
				getConfig: () => ModEntry.Config,
				reset: () =>
				{
					ModEntry.Config = new();
					ModEntry.ModHelper.WriteConfig(ModEntry.Config);
				},
				saveAndApply: () => { ModEntry.ModHelper.WriteConfig(ModEntry.Config); },
				log: ModEntry.Log,
				modRegistry: ModEntry.ModHelper.ModRegistry,
				manifest: ModEntry.Manifest
			).Register();
		}
	}
}