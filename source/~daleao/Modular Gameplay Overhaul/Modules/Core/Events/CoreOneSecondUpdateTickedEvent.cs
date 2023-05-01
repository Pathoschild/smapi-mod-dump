/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class CoreOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CoreOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CoreOneSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => !Data.InitialSetupComplete;

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (Game1.ticks <= 1 || Game1.currentGameTime == null || Game1.activeClickableMenu is not TitleMenu ||
            TitleMenu.subMenu is ConfirmationDialog)
        {
            return;
        }

        Log.I("Opening GMCM for initial setup.");
        GenericModConfigMenu.Instance!.ModApi!.OpenModMenu(Manifest);
        Data.InitialSetupComplete = true;
        ModHelper.Data.WriteJsonFile("data.json", Data);
        GenericModConfigMenu.Instance.Reload();
        this.Manager.Unmanage(this);
    }
}
