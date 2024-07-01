/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Enchantments.Framework.Events;

#region using directives

using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="EnergizedUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class EnergizedUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? EnchantmentsMod.EventManager)
{
    private WeakReference<BaseEnchantment>? _instance;

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        switch (Game1.player.CurrentTool)
        {
            case MeleeWeapon weapon when weapon.GetEnchantmentOfType<EnergizedMeleeEnchantment>() is { } energized:
                this._instance = new WeakReference<BaseEnchantment>(energized, false);
                break;
            case Slingshot slingshot when slingshot.GetEnchantmentOfType<EnergizedSlingshotEnchantment>() is { } energized:
                this._instance = new WeakReference<BaseEnchantment>(energized, false);
                break;
            default:
                this.Disable();
                break;
        }
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        this._instance = null;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (this._instance is null || !this._instance.TryGetTarget(out var instance))
        {
            this.Disable();
            return;
        }

        switch (instance)
        {
            case EnergizedMeleeEnchantment energized:
                energized.Update(e.Ticks);
                break;
            case EnergizedSlingshotEnchantment energized:
                energized.Update(e.Ticks);
                break;
        }
    }
}
