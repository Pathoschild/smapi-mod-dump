/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ToolbarIcons.Framework.Services;

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Services;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewMods.ToolbarIcons.Framework.Models;

/// <summary>Responsible for handling assets provided by this mod.</summary>
internal sealed class AssetHandler : BaseService<AssetHandler>
{
    /// <summary>Initializes a new instance of the <see cref="AssetHandler" /> class.</summary>
    /// <param name="eventSubscriber">Dependency used for subscribing to events.</param>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    /// <param name="themeHelper">Dependency used for swapping palettes.</param>
    public AssetHandler(IEventSubscriber eventSubscriber, ILog log, IManifest manifest, IThemeHelper themeHelper)
        : base(log, manifest)
    {
        // Init
        this.ArrowsPath = this.ModId + "/Arrows";
        this.IconPath = this.ModId + "/Icons";
        this.DataPath = this.ModId + "/Data";
        themeHelper.AddAssets([this.IconPath, this.ArrowsPath]);

        // Events
        eventSubscriber.Subscribe<AssetRequestedEventArgs>(this.OnAssetRequested);
    }

    /// <summary>Gets the game path to Arrows Texture asset.</summary>
    public string ArrowsPath { get; }

    /// <summary>Gets the game path to Icons Texture asset.</summary>
    public string IconPath { get; }

    /// <summary>Gets the game path to Toolbar Data asset.</summary>
    public string DataPath { get; }

    private void OnAssetRequested(AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo(this.IconPath))
        {
            e.LoadFromModFile<Texture2D>("assets/icons.png", AssetLoadPriority.Exclusive);
            return;
        }

        if (e.Name.IsEquivalentTo(this.ArrowsPath))
        {
            e.LoadFromModFile<Texture2D>("assets/arrows.png", AssetLoadPriority.Exclusive);
            return;
        }

        if (e.Name.IsEquivalentTo(this.DataPath))
        {
            e.LoadFrom(static () => new Dictionary<string, ToolbarIconData>(), AssetLoadPriority.Exclusive);
        }
    }
}