/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/


using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.MoveToConnected {
	public class ModConfig {

		// Connections

		/// <summary>
		/// Whether we should check diagonal tiles for connections, or if we should only check cardinal directions.
		/// </summary>
		public bool AllowDiagonalConnection { get; set; } = true;

		/// <summary>
		/// The names of objects and terrain features that are valid connectors.
		/// </summary>
		public CaseInsensitiveHashSet ValidConnectors { get; set; } = new CaseInsensitiveHashSet(new string[] {
			"Wood Path",
			"Stone Path"
		});


		// Keybinds



		// Performance

		public int MaxInventories { get; set; } = 50;
		public int MaxCheckedDistance { get; set; } = 100;
		public int MaxCheckedTiles { get; set; } = 1000;

	}
}
