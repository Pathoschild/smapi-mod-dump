/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Overhaul.Modules.Arsenal.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ComboButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ComboButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ComboButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ArsenalModule.Config.Weapons.EnableComboHits;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!ArsenalModule.Config.Weapons.EnableComboHits)
        {
            return;
        }

        var player = Game1.player;
        if (!Context.IsWorldReady || Game1.activeClickableMenu is not null || !e.Button.IsUseToolButton() ||
            player.CurrentTool is not MeleeWeapon weapon || weapon.isScythe())
        {
            return;
        }

        var hitStep = ArsenalModule.State.ComboHitStep;
        if (hitStep == ComboHitStep.Idle)
        {
            return;
        }

        var finalHitStep = weapon.GetFinalHitStep();
        if (hitStep >= finalHitStep)
        {
            ModHelper.Input.Suppress(e.Button);
            return;
        }

        if (!ArsenalModule.State.FarmerAnimating)
        {
            return;
        }

        ModHelper.Input.Suppress(e.Button);

        var type = weapon.type.Value;
        if (type == MeleeWeapon.club && hitStep == finalHitStep - 1)
        {
            player.QueueSmash(weapon);
        }
        else if ((int)hitStep % 2 == 0)
        {
            player.QueueForwardSwipe(weapon);
        }
        else
        {
            player.QueueReverseSwipe(weapon);
        }
    }
}
