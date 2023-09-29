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

using DaLion.Overhaul;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ArtfulParryUpdateTickedEvent : UpdateTickedEvent
{
    private int _timer;

    /// <summary>Initializes a new instance of the <see cref="ArtfulParryUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArtfulParryUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    private static int BuffId { get; } = (Manifest.UniqueID + "Parry").GetHashCode();

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._timer = 300;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (this._timer-- <= 0)
        {
            CombatModule.State.DidArtfulParry = false;
            this.Disable();
            return;
        }

        Game1.buffsDisplay.addOtherBuff(
            new Buff(
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                "Artful",
                I18n.Enchantments_Artful_Name())
            {
                which = BuffId,
                sheetIndex = 20,
                millisecondsDuration = 0,
                description = I18n.Enchantments_Artful_Parry(),
            });
    }
}
