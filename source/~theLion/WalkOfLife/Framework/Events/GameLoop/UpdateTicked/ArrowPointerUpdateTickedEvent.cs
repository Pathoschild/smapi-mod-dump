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
using TheLion.Stardew.Professions.Framework.Util;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ArrowPointerUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc />
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			HUD.Pointer ??= new();
			if (e.Ticks % 4 == 0) HUD.Pointer.Bob();
		}
	}
}