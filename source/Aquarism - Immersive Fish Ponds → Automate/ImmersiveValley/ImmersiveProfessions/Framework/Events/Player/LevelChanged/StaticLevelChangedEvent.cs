/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using Common.Events;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticLevelChangedEvent : LevelChangedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticLevelChangedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        AlwaysEnabled = true;
    }

    /// <inheritdoc />
    protected override void OnLevelChangedImpl(object? sender, LevelChangedEventArgs e)
    {
        if (e.Skill != SkillType.Combat || e.NewLevel % 5 != 0) return;

        if (e.NewLevel > e.OldLevel)
        {
            Game1.player.maxHealth += 5;
            Game1.player.health = Game1.player.maxHealth;
        }
        else if (e.NewLevel < e.OldLevel)
        {
            Game1.player.maxHealth -= 5;
            Game1.player.health = Math.Max(Game1.player.health, Game1.player.maxHealth);
        }
    }
}