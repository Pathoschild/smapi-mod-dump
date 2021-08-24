/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.IO;
using TheLion.Stardew.Professions.Framework.TreasureHunt;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ProspectorWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			ModEntry.ProspectorHunt ??= new ProspectorHunt(ModEntry.I18n.Get("prospector.huntstarted"),
				ModEntry.I18n.Get("prospector.huntfailed"),
				ModEntry.Content.Load<Texture2D>(Path.Combine("assets", "prospector.png")));

			if (ModEntry.ProspectorHunt.TreasureTile != null) ModEntry.ProspectorHunt.End();

			if (Game1.CurrentEvent == null && e.NewLocation is MineShaft) ModEntry.ProspectorHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}