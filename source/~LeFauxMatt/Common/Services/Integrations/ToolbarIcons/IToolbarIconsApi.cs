/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Services.Integrations.ToolbarIcons;

using StardewMods.FauxCore.Common.Services.Integrations.FauxCore;
#else
namespace StardewMods.Common.Services.Integrations.ToolbarIcons;

using StardewMods.Common.Services.Integrations.FauxCore;
#endif

using Microsoft.Xna.Framework;
using StardewValley.Menus;

/// <summary>Public api to add icons above or below the toolbar.</summary>
public interface IToolbarIconsApi
{
    /// <summary>Event triggered when any toolbar icon is pressed.</summary>
    [Obsolete("Use Subscribe(Action<IIconPressedEventArgs>) and Unsubscribe(Action<IIconPressedEventArgs>) instead.")]
    public event EventHandler<string> ToolbarIconPressed;

    /// <summary>Adds an icon next to the <see cref="Toolbar" />.</summary>
    /// <param name="id">A unique identifier for the icon.</param>
    /// <param name="texturePath">The path to the texture icon.</param>
    /// <param name="sourceRect">The source rectangle of the icon.</param>
    /// <param name="hoverText">Text to appear when hovering over the icon.</param>
    public void AddToolbarIcon(string id, string texturePath, Rectangle? sourceRect, string? hoverText);

    /// <summary>Adds an icon next to the <see cref="Toolbar" />.</summary>
    /// <param name="icon">The icon to add.</param>
    /// <param name="hoverText">Text to appear when hovering over the icon.</param>
    public void AddToolbarIcon(IIcon icon, string? hoverText);

    /// <summary>Removes an icon.</summary>
    /// <param name="id">A unique identifier for the icon.</param>
    public void RemoveToolbarIcon(string id);

    /// <summary>Removes an icon.</summary>
    /// <param name="icon">The icon to remove.</param>
    public void RemoveToolbarIcon(IIcon icon);

    /// <summary>Subscribes to an event handler.</summary>
    /// <param name="handler">The event handler to subscribe.</param>
    void Subscribe(Action<IIconPressedEventArgs> handler);

    /// <summary>Unsubscribes an event handler from an event.</summary>
    /// <param name="handler">The event handler to unsubscribe.</param>
    void Unsubscribe(Action<IIconPressedEventArgs> handler);
}