/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ToolAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ToolAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ToolAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Provide($"{Manifest.UniqueID}/RadioactiveTools", new ModTextureProvider(ProvideRadioactiveTools));
    }

    #region provider callbacks

    /// <summary>Provides the texture for Radioactive Tools.</summary>
    private static string ProvideRadioactiveTools()
    {
        var path = "assets/sprites/objects/RadioactiveTools";
        if (ModHelper.ModRegistry.IsLoaded("Gweniaczek.Grandpas_Tools"))
        {
            path += "_Gwen";
        }

        path += ".png";
        return path;
    }

    #endregion provider callbacks
}
