/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;

using StardewModdingAPI.Events;

namespace StopRugRemoval.Framework.Niceties;

/// <summary>
/// Lets you consume a recipe object to teach yourself the recipe.
/// </summary>
internal static class RecipeConsumer
{
    internal static bool ConsumeRecipeIfNeeded(ButtonPressedEventArgs e, IInputHelper helper)
    {
        if (e.Button.IsActionButton() && Game1.player.ActiveObject?.IsRecipe == true
            && Game1.player.ActiveObject.ConsumeRecipeImpl())
        {
            Game1.playSound("newRecipe");
            Game1.player.ActiveObject = null;
            helper.Suppress(e.Button);
            return true;
        }
        return false;
    }
}
