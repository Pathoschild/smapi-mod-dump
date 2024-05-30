/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ToolbarIcons.Framework.Services.Factory;

using StardewMods.Common.Interfaces;
using StardewMods.Common.Services.Integrations.FauxCore;

/// <inheritdoc />
internal sealed class ApiFactory : IApiFactory
{
    private readonly IEventManager eventManager;
    private readonly IIconRegistry iconRegistry;
    private readonly ToolbarManager toolbarManager;

    /// <summary>Initializes a new instance of the <see cref="ApiFactory" /> class.</summary>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="iconRegistry">Dependency used for registering and retrieving icons.</param>
    /// <param name="toolbarManager">Dependency used for adding or removing icons on the toolbar.</param>
    public ApiFactory(IEventManager eventManager, IIconRegistry iconRegistry, ToolbarManager toolbarManager)
    {
        this.eventManager = eventManager;
        this.iconRegistry = iconRegistry;
        this.toolbarManager = toolbarManager;
    }

    /// <inheritdoc />
    public object CreateApi(IModInfo modInfo) =>
        new ToolbarIconsApi(modInfo, this.eventManager, this.iconRegistry, this.toolbarManager);
}