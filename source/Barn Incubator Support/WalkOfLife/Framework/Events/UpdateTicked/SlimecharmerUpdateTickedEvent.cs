/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.AwesomeProfessions.Framework.Events.UpdateTicked
{
	internal class SlimecharmerUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (AwesomeProfessions.slimeHealTimer > 0 && Game1.shouldTimePass())
				--AwesomeProfessions.slimeHealTimer;
		}
	}
}