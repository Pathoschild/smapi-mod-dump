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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;

using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.Almanac.Models;

internal class Chapter {

	#region Basic Properties

	/// <summary>
	/// Every chapter needs an identifier, but these IDs are only unique within
	/// the book they're in. These are not namespaced IDs, but simple case
	/// insensitive strings.
	/// </summary>
	public string? Id { get; set; }

	/// <summary>
	/// The QualifiedID is calculated based on the chapter's ID but also the ID
	/// of the book that the chapter is within. For example, if the chapter is
	/// within a book with the Id <c>MyCoolMod:Guide</c> and the chapter's Id is
	/// <c>Pizza</c> then the QualifiedID would be <c>MyCoolMod:Guide/Pizza</c>.
	/// </summary>
	[JsonIgnore]
	public NamespaceId? QualifiedID { get; internal set; }

	#endregion

	#region Visibility

	/// <summary>
	/// Whether or not the chapter should be displayed in the book.
	/// </summary>
	public bool Enable { get; set; } = true;

	/// <summary>
	/// A GameStateQuery that, if set, must evaluate to true for the chapter
	/// to be unlocked in the book. Locked chapters appear as
	/// greyed out and unavailable unless the chapter is secret.
	/// </summary>
	public string? Condition { get; set; } = null;

	/// <summary>
	/// If this is enabled, the chapter will not appear in the book at all
	/// until it is unlocked. This has no effect if the chapter has no
	/// condition set.
	/// </summary>
	public bool Secret { get; set; } = false;

	#endregion

	#region Appearance

	public string? Title { get; set; }

	public string? Description { get; set; }

	public ChapterStyle Style { get; set; } = ChapterStyle.Standard;

	#endregion

	#region Content

	public bool GenerateIntro { get; set; } = true;

	/// <summary>
	/// Chapters have a number of entries that content is separated amongst.
	/// Entries are displayed in the chapter's table of contents.
	/// </summary>
	public CaseInsensitiveDictionary<Entry> Entries { get; set; } = new();

	#endregion

}
