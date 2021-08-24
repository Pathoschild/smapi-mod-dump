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
using System.IO;
using TheLion.Stardew.Professions.Framework.TreasureHunt;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ScavengerWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			ModEntry.ScavengerHunt ??= new ScavengerHunt(ModEntry.I18n.Get("scavenger.huntstarted"),
				ModEntry.I18n.Get("scavenger.huntfailed"),
				ModEntry.Content.Load<Texture2D>(Path.Combine("assets", "scavenger.png")));

			if (ModEntry.ScavengerHunt.TreasureTile != null) ModEntry.ScavengerHunt.End();

			if (Game1.CurrentEvent == null && e.NewLocation.IsOutdoors && !(e.NewLocation.IsFarm || e.NewLocation.NameOrUniqueName.Equals("Town")))
				ModEntry.ScavengerHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}