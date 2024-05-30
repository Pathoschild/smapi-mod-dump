/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;

public interface IVAECraftingRecipeData {

	List<IVAEIngredientData> Ingredients { get; }

}

public interface IVAEIngredientMatcher : IIngredientMatcher {

	IVAEIngredientData Data { get; }

}

public enum VAEIngredientType {
	Item,
	ContextTag,
}

public interface IVAEIngredientData {

	VAEIngredientType Type { get; }

	string Value { get; }

}
