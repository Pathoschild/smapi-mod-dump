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
using System.IO;

namespace TheLion.AwesomeProfessions
{
	internal class ArrowPointerUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			Utility.ArrowPointer ??= new ArrowPointer(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "cursor.png")));
			if (e.Ticks % 4 == 0) Utility.ArrowPointer.Bob();
		}
	}
}