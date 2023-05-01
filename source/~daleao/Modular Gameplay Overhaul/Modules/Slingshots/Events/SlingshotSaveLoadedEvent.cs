/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Slingshots.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class SlingshotSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        if (Game1.options.useLegacySlingshotFiring)
        {
            SlingshotsModule.Config.BullseyeReplacesCursor = false;
            ModHelper.WriteConfig(ModEntry.Config);
            Log.W(
                "[SLNGS]: Bullseye cursor settings is not compatible with pull-back firing mode. Please change to hold-and-release to use this option.");
        }

        if (!SlingshotsModule.Config.EnableAutoSelection)
        {
            return;
        }

        var player = Game1.player;
        var index = player.Read(DataKeys.SelectableSlingshot, -1);
        if (index < 0)
        {
            return;
        }

        var item = player.Items[index];
        if (item is not Slingshot slingshot)
        {
            return;
        }

        SlingshotsModule.State.AutoSelectableSlingshot = slingshot;
    }
}
