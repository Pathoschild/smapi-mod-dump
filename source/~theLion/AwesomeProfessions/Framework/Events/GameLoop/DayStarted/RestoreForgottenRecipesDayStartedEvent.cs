/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class RestoreForgottenRecipesDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    public override void OnDayStarted(object sender, DayStartedEventArgs e)
    {
        var forgottenRecipes = ModEntry.Data.Read("ForgottenRecipes").ToDictionary<string, int>(",", ";");
        if (!forgottenRecipes.Any())
        {
            ModEntry.Subscriber.Unsubscribe(GetType());
            return;
        }

        for (var i = forgottenRecipes.Count - 1; i >= 0; --i)
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

        ModEntry.Data.Write("ForgottenRecipes", forgottenRecipes.Any()
            ? forgottenRecipes.ToString(",", ";")
            : null);
        ModEntry.Subscriber.Unsubscribe(GetType());
    }
}