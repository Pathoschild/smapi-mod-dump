/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace StardewMods.FuryCore.Events;

using System;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewMods.FuryCore.Interfaces;
using StardewMods.FuryCore.Models.CustomEvents;
using StardewMods.FuryCore.Services;
using StardewValley;

/// <inheritdoc />
internal class MenuComponentPressed : SortedEventHandler<ClickableComponentPressedEventArgs>
{
    private readonly Lazy<MenuComponents> _menuComponents;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MenuComponentPressed" /> class.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="services">Provides access to internal and external services.</param>
    public MenuComponentPressed(IModHelper helper, IModServices services)
    {
        this.Helper = helper;
        this._menuComponents = services.Lazy<MenuComponents>();
        this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
    }

    private MenuComponents Components
    {
        get => this._menuComponents.Value;
    }

    private IModHelper Helper { get; }

    [EventPriority(EventPriority.High)]
    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (this.HandlerCount == 0 || !this.Components.Components.Any())
        {
            return;
        }

        if (e.Button is not SButton.MouseLeft or SButton.MouseRight && !(e.Button.IsActionButton() || e.Button.IsUseToolButton()))
        {
            return;
        }

        var (x, y) = Game1.getMousePosition(true);
        var component = this.Components.Components.FirstOrDefault(component => component.Component?.containsPoint(x, y) == true);
        if (component is null)
        {
            return;
        }

        Game1.playSound("drumkit6");
        this.InvokeAll(new(
            e.Button,
            component,
            () => this.Helper.Input.Suppress(e.Button),
            () => this.Helper.Input.IsSuppressed(e.Button)));

        if (Game1.activeClickableMenu.currentlySnappedComponent is not null && Game1.options.SnappyMenus)
        {
            Game1.activeClickableMenu.setCurrentlySnappedComponentTo(component.Id);
            Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
        }
    }
}