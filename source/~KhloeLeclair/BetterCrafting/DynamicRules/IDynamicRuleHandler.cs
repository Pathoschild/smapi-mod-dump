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

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

/// <summary>
/// IDynamicRuleHandler instances handle the logic of determining whether or
/// not any given <see cref="IRecipe"/> matches a dynamic rule, and thus
/// whether the recipe should be displayed in a category using rules.
///
/// It also handles anything necessary for displaying a user interface to
/// the user for editing the rule's configuration.
/// </summary>
public interface IDynamicRuleHandler {

	#region Display

	/// <summary>
	/// The name of the dynamic rule, to be displayed to the user when
	/// editing a category.
	/// </summary>
	string DisplayName { get; }

	/// <summary>
	/// A description of what the dynamic rule matches, to be displayed
	/// to the user when hovering over the rule in the interface to
	/// add a new rule.
	/// </summary>
	string Description { get; }

	/// <summary>
	/// The source texture for an icon to display alongside this dynamic rule.
	/// </summary>
	Texture2D Texture { get; }

	/// <summary>
	/// The source area for an icon to display alongside this dynamic rule.
	/// </summary>
	Rectangle Source { get; }

	/// <summary>
	/// Whether or not this dynamic rule should be allowed to be added to a
	/// category multiple times.
	/// </summary>
	bool AllowMultiple { get; }

	#endregion

	#region Editing

	/// <summary>
	/// Whether or not this dynamic rule has a custom editor.
	/// </summary>
	bool HasEditor { get; }

	/// <summary>
	/// WIP! This currently does not function. In the future, this will obtain
	/// a new editor child menu that will be rendered within the rule editor.
	/// </summary>
	/// <param name="parent">The rule editor</param>
	/// <param name="data">The data of the rule being edited</param>
	IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data);

	#endregion

	#region Processing

	/// <summary>
	/// This method is called before a dynamic rule is executed, allowing the
	/// rule to parse its configuration into a state object that can be
	/// re-used when checking recipes against the rule.
	/// </summary>
	/// <param name="data">The data of the rule</param>
	/// <returns>A custom state object, or null if no state is required</returns>
	object? ParseState(IDynamicRuleData data);

	/// <summary>
	/// This method checks whether a recipe matches this dynamic rule or not.
	/// </summary>
	/// <param name="recipe">The recipe being checked.</param>
	/// <param name="item">The item output of the recipe being checked.</param>
	/// <param name="state">The state object returned from <see cref="ParseState(IDynamicRuleData)"/></param>
	bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state);

	#endregion
}

public abstract class DynamicTypeHandler<T> : IDynamicRuleHandler, IExtraInfoRuleHandler {

	#region Display

	public abstract string DisplayName { get; }

	public abstract string Description { get; }

	public abstract Texture2D Texture { get; }

	public abstract Rectangle Source { get; }

	public abstract bool AllowMultiple { get; }

	public abstract bool HasEditor { get; }

	public abstract IFlowNode[]? GetExtraInfo(T? state);

	public abstract IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data);

	#endregion

	#region Processing

	public abstract T? ParseStateT(IDynamicRuleData type);

	public object? ParseState(IDynamicRuleData type) {
		return ParseStateT(type);
	}

	public IFlowNode[]? GetExtraInfo(object? state) {
		T? tstate = state is T ts ? ts : default;
		return GetExtraInfo(tstate);
	}

	public abstract bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, T? state);

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		T? tstate = state is T ts ? ts : default;
		return DoesRecipeMatch(recipe, item, tstate);
	}

	#endregion

}
