/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class RestoreForgottenRecipesDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RestoreForgottenRecipesDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal RestoreForgottenRecipesDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        var forgottenRecipes = Game1.player.Read(DataKeys.ForgottenRecipesDict)
            .ParseDictionary<string, int>();
        if (forgottenRecipes.Count == 0)
        {
            this.Disable();
            return;
        }

        for (var i = forgottenRecipes.Count - 1; i >= 0; i--)
        {
            var key = forgottenRecipes.ElementAt(i).Key;
            if (Game1.player.craftingRecipes.ContainsKey(key))
            {
                Game1.player.craftingRecipes[key] += forgottenRecipes[key];
                forgottenRecipes.Remove(key);
            }
            else if (Game1.player.cookingRecipes.ContainsKey(key))
            {
                Game1.player.cookingRecipes[key] += forgottenRecipes[key];
                forgottenRecipes.Remove(key);
            }
        }

        Game1.player.Write(
            DataKeys.ForgottenRecipesDict,
            forgottenRecipes.Count > 0 ? forgottenRecipes.Stringify() : null);
        this.Disable();
    }
}
