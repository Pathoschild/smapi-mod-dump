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

using Newtonsoft.Json;

using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.Almanac.Models;

internal class Entry {

	#region Basic Properties

	/// <summary>
	/// Every entry needs an identifier, but these IDs are only unique within
	/// the chapter they're in. These are not namespaced IDs, but simple case
	/// insensitive strings.
	/// </summary>
	public string? Id { get; set; }

	/// <summary>
	/// The QualifiedID is calculated based on the entry's ID but also the
	/// QualifiedID of the chapter the entry is within. For example, if the
	/// entry is within a chapter with the QualifiedID <c>MyCoolMod:Guide/Pizza</c>
	/// and the entry's Id is <c>Crust</c> then the QualifiedId would be
	/// <c>MyCoolMod:Guide/Pizza/Crust</c>
	/// </summary>
	[JsonIgnore]
	public NamespaceId? QualifiedId { get; internal set; }

	#endregion

	#region Visibility

	/// <summary>
	/// Whether or not the entry should be displayed in the book.
	/// </summary>
	public bool Enable { get; set; } = true;

	/// <summary>
	/// A GameStateQuery that, if set, must evaluate to true for the entry
	/// to be unlocked in the book. Locked entries appear as greyed out
	/// and unavailable unless the entry is secret.
	/// </summary>
	public string? Condition { get; set; } = null;

	/// <summary>
	/// If this is enabled, the entry will not appear in the chapter at all
	/// until it is unlocked. This has no effect if the entry has no
	/// condition set.
	/// </summary>
	public bool Secret { get; set; } = false;

	#endregion

	#region Appearance

	#endregion

	#region Contents

	public Page[] Pages { get; set; } = Array.Empty<Page>();

	#endregion

}
