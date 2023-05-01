/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class GatlingButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="GatlingButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal GatlingButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!e.Button.IsUseToolButton())
        {
            return;
        }

        if (EnchantmentsModule.State.DoublePressTimer > 0)
        {
            EnchantmentsModule.State.GatlingModeEngaged = true;
            this.Manager.Enable<GatlingButtonReleasedEvent>();
        }
        else
        {
            EnchantmentsModule.State.DoublePressTimer = 18;
            this.Manager.Enable<GatlingUpdateTickedEvent>();
        }
    }
}
