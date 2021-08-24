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
using System.Collections.Generic;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeModMessageReceivedEvent : ModMessageReceivedEvent
	{
		/// <inheritdoc/>
		public override void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
		{
			if (e.FromModID != ModEntry.UniqueID) return;

			var key = e.ReadAs<int>();
			if (!ModEntry.ActivePeerSuperModes.ContainsKey(key))
				ModEntry.ActivePeerSuperModes[key] = new HashSet<long>();

			switch (e.Type)
			{
				case "SuperModeActivated":
					ModEntry.ActivePeerSuperModes[key].Add(e.FromPlayerID);
					break;
				case "SuperModeDeactivated":
					ModEntry.ActivePeerSuperModes[key].Remove(e.FromPlayerID);
					break;
			};
		}
	}
}
