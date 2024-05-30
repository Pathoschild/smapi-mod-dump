/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Content;

#region using directives

using StardewModdingAPI.Events;

#endregion using directivese

/// <summary>Generates a new instance of the <see cref="AssetEditor"/> record.</summary>
/// <param name="Apply">A delegate callback for applying edits to the content asset.</param>
/// <param name="Priority">The priority for an asset edit when multiple apply for the same asset.</param>
public record AssetEditor(Action<IAssetData> Apply, AssetEditPriority Priority = AssetEditPriority.Default) : IAssetEditor
{
    /// <inheritdoc />
    public void Edit(AssetRequestedEventArgs e)
    {
        e.Edit(this.Apply, this.Priority);
    }
}
