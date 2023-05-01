/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Enchantments;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class CurseUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CurseUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CurseUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        if (!Game1.game1.ShouldTimePass() || !e.IsMultipleOf(300))
        {
            return;
        }

        if (player.CurrentTool is not MeleeWeapon weapon || !weapon.hasEnchantmentOfType<CursedEnchantment>())
        {
            this.Disable();
            return;
        }

        var dot = (weapon.Read<int>(DataKeys.CursePoints) / 10f) * WeaponsModule.Config.RuinBladeDotMultiplier;
        player.health = (int)Math.Max(player.health - dot, 1);
    }
}
