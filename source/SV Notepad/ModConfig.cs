/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/hbc-mods/stardew-valley/sv-notepad
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace SVNotepad{
	class ModConfig{
		public KeybindList ToggleKey { get; set; } = KeybindList.Parse( "N" ); // Key to open the mod's menu
	}
}