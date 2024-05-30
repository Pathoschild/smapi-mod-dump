/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.Player.InventoryChanged;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="RascalInventoryChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class RascalInventoryChangedEvent(EventManager? manager = null)
    : InventoryChangedEvent(manager ?? ProfessionsMod.EventManager)
{
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

            var index = player.getIndexOfInventoryItem(item);
            if (player.HasProfession(Profession.Rascal) &&
                (slingshot.AttachmentSlotsCount != 2 || slingshot.attachments.Length != 2))
            {
                var replacement = ItemRegistry.Create<Slingshot>(slingshot.QualifiedItemId);
                replacement.AttachmentSlotsCount = 2;
                player.Items[index] = replacement;
            }
            else if (!player.HasProfession(Profession.Rascal) &&
                     (slingshot.AttachmentSlotsCount == 2 || slingshot.attachments.Length == 2))
            {
                var replacement = ItemRegistry.Create<Slingshot>(slingshot.QualifiedItemId);

                if (slingshot.attachments[0] is { } ammo1)
                {
                    replacement.attachments[0] = (SObject)ammo1.getOne();
                    replacement.attachments[0].Stack = ammo1.Stack;
                }

                if (slingshot.attachments[1] is { } ammo2)
                {
                    var drop = (SObject)ammo2.getOne();
                    drop.Stack = ammo2.Stack;
                    if (!player.addItemToInventoryBool(drop))
                    {
                        Game1.createItemDebris(drop, player.getStandingPosition(), -1, player.currentLocation);
                    }
                }

                player.Items[index] = replacement;
            }
        }
    }
}
