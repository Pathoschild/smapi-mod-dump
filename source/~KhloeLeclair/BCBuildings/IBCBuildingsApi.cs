/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;

using StardewValley;

namespace Leclair.Stardew.BCBuildings;

public interface IBlueprintPopulationEventArgs {
	Dictionary<string, BluePrint> Blueprints { get; }
}

public interface IBCBuildingsApi {

	/// <summary>
	/// Add a blueprint to the BC: Buildings menu, by name.
	/// </summary>
	/// <param name="name">The name of the blueprint to add</param>
	/// <param name="displayCondition">An optional game state query to control
	/// visibility of the blueprint in the menu.
	/// <see href="https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Game_state_queries" /></param>
	/// <param name="buildCondition">An optional game state query to control
	/// whether or not players can construct a blueprint. If the condition is
	/// set and doesn't pass, the building will be grayed out and unavailable
	/// to construct, with a message displayed to the user that a condition
	/// has not been fulfilled.</param>
	void AddBlueprint(string name, string? displayCondition, string? buildCondition);

	/// <summary>
	/// Add a blueprint to the BC: Buildings menu.
	/// </summary>
	/// <param name="blueprint">The blueprint to add</param>
	/// <param name="displayCondition">An optional game state query to control
	/// visibility of the blueprint in the menu.
	/// <see href="https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Game_state_queries" /></param>
	/// <param name="buildCondition">An optional game state query to control
	/// whether or not players can construct a blueprint. If the condition is
	/// set and doesn't pass, the building will be grayed out and unavailable
	/// to construct, with a message displayed to the user that a condition
	/// has not been fulfilled.</param>
	void AddBlueprint(BluePrint blueprint, string? displayCondition, string? buildCondition);

	/// <summary>
	/// Remove a blueprint from the BC: Buildings menu, by name.
	/// </summary>
	/// <param name="name">The name of the blueprint to remove</param>
	void RemoveBlueprint(string name);

	/// <summary>
	/// Remove a blueprint from the BC: Buildings menu.
	/// </summary>
	/// <param name="blueprint">The blueprint to remove</param>
	void RemoveBlueprint(BluePrint blueprint);

	/// <summary>
	/// Override the source rectangle used for drawing the building in the
	/// BC: Buildings menu.
	/// </summary>
	/// <param name="name">The name of the blueprint to change</param>
	/// <param name="rect">The rectangle of the texture to use</param>
	void SetTextureSourceRect(string name, Rectangle? rect);

	/// <summary>
	/// This event is emitted when discovering blueprints for display in the
	/// menu. Additional logic to control the visibility of a blueprint can
	/// be performed here, assuming game state queries aren't enough.
	/// </summary>
	event EventHandler<IBlueprintPopulationEventArgs>? OnBlueprintPopulation;

}
