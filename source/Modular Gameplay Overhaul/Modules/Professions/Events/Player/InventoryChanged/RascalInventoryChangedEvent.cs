/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Player.InventoryChanged;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class RascalInventoryChangedEvent : InventoryChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RascalInventoryChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal RascalInventoryChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnInventoryChangedImpl(object? sender, InventoryChangedEventArgs e)
    {
        var player = e.Player;
        foreach (var item in e.Added)
        {
            if (item is not Slingshot slingshot)
            {
                continue;
            }

            if (player.HasProfession(Profession.Rascal) &&
                (slingshot.numAttachmentSlots.Value == 1 || slingshot.attachments.Length == 1))
            {
                slingshot.numAttachmentSlots.Value = 2;
                slingshot.attachments.SetCount(2);
            }
            else if (!player.HasProfession(Profession.Rascal) &&
                     (slingshot.numAttachmentSlots.Value == 2 || slingshot.attachments.Length == 2))
            {
                var replacement = ArcheryIntegration.Instance?.ModApi?.GetWeaponData(Manifest, slingshot) is { } bowData
                    ? (Slingshot)ArcheryIntegration.Instance.ModApi.CreateWeapon(Manifest, bowData.WeaponId)
                    : new Slingshot(slingshot.InitialParentTileIndex);

                if (slingshot.attachments[0] is { } ammo1)
                {
                    replacement.attachments[0] = (SObject)ammo1.getOne();
                    replacement.attachments[0].Stack = ammo1.Stack;
                }

                if (slingshot.attachments.Length > 1 && slingshot.attachments[1] is { } ammo2)
                {
                    var drop = (SObject)ammo2.getOne();
                    drop.Stack = ammo2.Stack;
                    if (!player.addItemToInventoryBool(drop))
                    {
                        Game1.createItemDebris(drop, player.getStandingPosition(), -1, player.currentLocation);
                    }
                }

                var index = player.getIndexOfInventoryItem(item);
                player.Items[index] = replacement;
            }
        }
    }
}
