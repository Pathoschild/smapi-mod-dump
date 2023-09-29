/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ExplosiveUpdateTickedEvent : UpdateTickedEvent
{
    private WeakReference<ExplosiveEnchantment>? _explosive;

    /// <summary>Initializes a new instance of the <see cref="ExplosiveUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ExplosiveUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        var explosive = (Game1.player.CurrentTool as MeleeWeapon)?.GetEnchantmentOfType<ExplosiveEnchantment>();
        if (explosive is null)
        {
            this.Disable();
            return;
        }

        this._explosive = new WeakReference<ExplosiveEnchantment>(explosive, false);
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        this._explosive = null;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (this._explosive is null || !this._explosive.TryGetTarget(out var explosive))
        {
            this.Disable();
            return;
        }

        explosive.Update();
    }
}
