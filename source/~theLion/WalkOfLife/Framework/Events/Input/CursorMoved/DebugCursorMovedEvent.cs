/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class DebugCursorMovedEvent : CursorMovedEvent
	{
		internal static ICursorPosition CursorPosition { get; set; }

		/// <inheritdoc />
		public override void OnCursorMoved(object sender, CursorMovedEventArgs e)
		{
			CursorPosition = e.NewPosition;
		}
	}
}