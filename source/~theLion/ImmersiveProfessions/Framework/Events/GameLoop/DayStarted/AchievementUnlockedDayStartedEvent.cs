/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

using Common.Extensions;
using AssetEditors;

#endregion using directives

internal class AchievementUnlockedDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object sender, DayStartedEventArgs e)
    {
        if (!ModEntry.ModHelper.Content.AssetEditors.ContainsType(typeof(AchivementsEditor)))
            ModEntry.ModHelper.Content.AssetEditors.Add(new AchivementsEditor());

        string name =
            ModEntry.ModHelper.Translation.Get("prestige.achievement.name." +
                                               (Game1.player.IsMale ? "male" : "female"));
        Game1.player.achievements.Add(name.GetDeterministicHashCode());
        Game1.playSound("achievement");
        Game1.addHUDMessage(new(name, true));

        Disable();
    }
}