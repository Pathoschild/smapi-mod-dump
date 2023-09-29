/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
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
[AlwaysEnabledEvent]
internal sealed class CoreLateLoadOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CoreLateLoadOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CoreLateLoadOneSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (Game1.ticks <= 1 || Game1.currentGameTime == null || Game1.activeClickableMenu is not TitleMenu ||
            TitleMenu.subMenu is ConfirmationDialog)
        {
            return;
        }

        if (OverhaulModule.Professions._ShouldEnable &&
            Professions.Integrations.SpaceCoreIntegration.Instance?.IsRegistered != true)
        {
            return;
        }

        OverhaulModule.Core.RegisterIntegrations();
        if (GenericModConfigMenu.Instance?.IsRegistered != true || LocalData.InitialSetupComplete)
        {
            this.Manager.Unmanage(this);
            return;
        }

        Log.I("Launching GMCM for initial setup.");
        GenericModConfigMenu.Instance.ModApi!.OpenModMenu(Manifest);
        LocalData.InitialSetupComplete = true;
        ModHelper.Data.WriteJsonFile("data.json", LocalData);
        GenericModConfigMenu.Instance.Reload();
        this.Manager.Unmanage(this);
    }
}
