/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

namespace Leclair.Stardew.Almanac.Models;

internal class BookCollectionStatus {

	/// <summary>
	/// Whether or not the book should be displayed in the collection.
	/// </summary>
	public bool Enable { get; set; } = false;

	/// <summary>
	/// A GameStateQuery that, if set, must evaluate to true for the book
	/// to be unlocked in the collection. Locked books appear as
	/// greyed out and unavailable unless the book is secret.
	/// </summary>
	public string? Condition { get; set; } = null;

	/// <summary>
	/// If this is enabled, books will not appear in the collection at all
	/// until they are unlocked. This has no effect if the book has no
	/// condition set.
	/// </summary>
	public bool Secret { get; set; } = false;

}
