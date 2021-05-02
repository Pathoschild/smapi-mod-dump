/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods/-/tree/master/WalkOfLife
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using System.IO;

namespace TheLion.AwesomeProfessions
{
	internal class ScavengerWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			AwesomeProfessions.ScavengerHunt ??= new ScavengerHunt(AwesomeProfessions.I18n.Get("scavenger.huntstarted"),
				AwesomeProfessions.I18n.Get("scavenger.huntfailed"),
				AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "scavenger.png")));

			if (AwesomeProfessions.ScavengerHunt.TreasureTile != null) AwesomeProfessions.ScavengerHunt.End();

			if (Game1.CurrentEvent == null && e.NewLocation.IsOutdoors && !(e.NewLocation.IsFarm || e.NewLocation.NameOrUniqueName.Equals("Town")))
				AwesomeProfessions.ScavengerHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}