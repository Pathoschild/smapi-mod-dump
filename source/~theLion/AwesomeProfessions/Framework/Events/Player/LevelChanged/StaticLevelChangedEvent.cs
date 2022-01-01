/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using JetBrains.Annotations;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

[UsedImplicitly]
internal class StaticLevelChangedEvent : LevelChangedEvent
{
    /// <inheritdoc />
    public override void OnLevelChanged(object sender, LevelChangedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;

        if (e.NewLevel == 0)
        {
            // clean up rogue events and data on skill reset
            ModEntry.Subscriber.CleanUpRogueEvents();
            ModEntry.Data.CleanUpRogueData();
        }
        else
        {
            ModEntry.Subscriber.Subscribe(new RestoreForgottenRecipesDayStartedEvent());
        }
    }
}