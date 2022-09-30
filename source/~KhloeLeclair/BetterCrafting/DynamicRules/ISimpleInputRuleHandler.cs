/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

/// <summary>
/// ISimpleInputRuleHandler is an <see cref="IDynamicRuleHandler"/> that only
/// has a single text input for configuring it. This allows you to create
/// basic rules without needing to implement a configuration interface.
/// </summary>
public interface ISimpleInputRuleHandler : IDynamicRuleHandler {

	/// <summary>
	/// If set to a string, this string will be displayed alongside the
	/// text editor added to the rule editor for this rule. This text will
	/// be rendered as formatted text, using the format described in the
	/// authoring guide for Almanac at <see href="https://github.com/KhloeLeclair/StardewMods/blob/main/Almanac/author-guide.md#rich-text"/>
	/// </summary>
	string? HelpText { get; }

}
