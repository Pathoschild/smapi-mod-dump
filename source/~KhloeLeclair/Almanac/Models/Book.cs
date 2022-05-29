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

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;

using Leclair.Stardew.Common.Serialization.Converters;
using Leclair.Stardew.Common.Types;

using StardewModdingAPI;

namespace Leclair.Stardew.Almanac.Models;

internal class Book {

	#region Internals

	[JsonIgnore]
	internal IContentPack? ContentPack { get; set; } = null;

	#endregion

	#region Basic Properties

	/// <summary>
	/// Every book needs a unique ID.
	/// </summary>
	public NamespaceId? Id { get; set; }

	#endregion

	#region Extension

	/// <summary>
	/// Whether or not this book allows other books to extend it. By
	/// default, extension is allowed.
	/// </summary>
	public bool AllowExtension { get; set; } = true;

	/// <summary>
	/// If set, this book will be treated as an extension of another book
	/// rather than its own separate book. In that case, most properties
	/// of the book itself will be ignored and Almanac will only consider
	/// the book's contents.
	/// </summary>
	public NamespaceId? Extends { get; set; }

	#endregion

	#region Collection

	/// <summary>
	/// Details about the appearance of this book in the player's book
	/// collection menu.
	/// </summary>
	public BookCollectionStatus Collection { get; set; } = new() {
		Enable = true
	};

	/// <summary>
	/// Details about the appearance of this book in the in-game Library.
	/// </summary>
	public BookCollectionStatus Library { get; set; } = new();

	#endregion

	#region Appearance

	/// <summary>
	/// An optional theme that should be used for displaying this book. If
	/// set, this overrides the user's selected theme and forces a book to
	/// be displayed with this theme as long as the theme is loaded.
	/// </summary>
	public string? Theme { get; set; }

	/// <summary>
	/// An optional color applied to the cover of the book, and possibly
	/// parts of open book pages depending on the theme.
	/// </summary>
	[JsonConverter(typeof(ColorConverter))]
	public Color? Color { get; set; }

	/// <summary>
	/// A book's title is displayed to the user whenever the book needs to
	/// be described to the user, such as in tooltips when hovering over
	/// a cook in the collections menu or the library.
	/// </summary>
	public string? Title { get; set; }

	/// <summary>
	/// An optional description to display alongside the book's title when
	/// relevant, such as in tooltips in the collections menu.
	/// </summary>
	public string? Description { get; set; }

	#endregion

	#region Cover Page

	/// <summary>
	/// Books have a single page that acts as their cover. If no cover is
	/// specified, one will be generated based on the book's Id. This is
	/// less than ideal, so every book should have a cover.
	/// </summary>
	public Page? Cover { get; set; }

	#endregion

	#region Contents

	/// <summary>
	/// Books have a number of chapters that content is separated amongst.
	/// Chapters are displayed along a book's side as navigation tabs.
	/// </summary>
	public CaseInsensitiveDictionary<Chapter> Chapters { get; set; } = new();

	#endregion

}
