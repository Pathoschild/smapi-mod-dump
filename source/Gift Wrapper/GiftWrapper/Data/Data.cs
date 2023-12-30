/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/GiftWrapper
**
*************************************************/

using System.Collections.Generic;

namespace GiftWrapper.Data
{
	/// <summary>
	/// Model for mod feature data.
	/// </summary>
	public record Data
	{
		/// <summary>
		/// Map of audio cue IDs to asset paths.
		/// </summary>
		public Dictionary<string, string[]> Audio;
		/// <summary>
		/// Various mod data definitions.
		/// </summary>
		public Definitions Definitions;
		/// <summary>
		/// List conditional shop entries to sell wrap items.
		/// </summary>
		public Shop[] Shops;
		/// <summary>
		/// Map of models for visual styles of wrapped gift items.
		/// </summary>
		public Dictionary<string, Style> Styles;
		/// <summary>
		/// Model for UI data, used for theme definitions.
		/// </summary>
		public UI UI;
	}
}
