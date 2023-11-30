/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.DayStarted;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class AchievementUnlockedDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="AchievementUnlockedDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal AchievementUnlockedDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        string title =
            _I18n.Get("prestige.achievement.title" +
                              (Game1.player.IsMale ? ".male" : ".female"));
        if (Game1.player.achievements.Contains(title.GetDeterministicHashCode()))
        {
            this.Disable();
            return;
        }

        Game1.player.achievements.Add(title.GetDeterministicHashCode());
        Game1.playSound("achievement");
        Game1.addHUDMessage(new HUDMessage(title, true));
        this.Disable();
    }
}
