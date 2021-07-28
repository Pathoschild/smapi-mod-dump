/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.IO;

namespace TheLion.AwesomeProfessions
{
	internal class ProspectorWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			AwesomeProfessions.ProspectorHunt ??= new ProspectorHunt(AwesomeProfessions.I18n.Get("prospector.huntstarted"),
				AwesomeProfessions.I18n.Get("prospector.huntfailed"),
				AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "prospector.png")));

			if (AwesomeProfessions.ProspectorHunt.TreasureTile != null) AwesomeProfessions.ProspectorHunt.End();

			if (Game1.CurrentEvent == null && e.NewLocation is MineShaft) AwesomeProfessions.ProspectorHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}