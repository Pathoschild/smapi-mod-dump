/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Content;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetRequested"/> allowing dynamic enabling / disabling.</summary>
public abstract class AssetRequestedEvent : ManagedEvent
{
    private readonly Dictionary<string, List<IAssetEditor>> _editors = [];

    private readonly Dictionary<string, IAssetProvider> _providers = [];

    /// <summary>Initializes a new instance of the <see cref="AssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected AssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Content.AssetRequested += this.OnAssetRequested;
        this.Initialize();
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Content.AssetRequested -= this.OnAssetRequested;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnAssetRequestedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnAssetRequested"/>
    protected virtual void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (this._providers.TryGetValue(e.NameWithoutLocale.Name, out var provider))
        {
            provider.Provide(e);
        }

        if (this._editors.TryGetValue(e.NameWithoutLocale.Name, out var editors))
        {
            editors.ForEach(editor => editor.Edit(e));
        }
    }

    /// <summary>Initializes editors and providers.</summary>
    protected abstract void Initialize();

    /// <summary>Caches the specified <paramref name="editor"/> for the asset with the specified <paramref name="name"/>.</summary>
    /// <param name="name">The name of the asset.</param>
    /// <param name="editor">The <see cref="AssetEditor"/>.</param>
    protected void Edit(string name, IAssetEditor editor)
    {
        if (!this._editors.TryAdd(name, [editor]))
        {
            this._editors[name].Add(editor);
        }
    }

    /// <summary>Caches the specified <paramref name="provider"/> for the asset with the specified <paramref name="name"/>.</summary>
    /// <param name="name">The name of the asset.</param>
    /// <param name="provider">The <see cref="IAssetProvider"/>.</param>
    protected void Provide(string name, IAssetProvider provider)
    {
        this._providers[name] = provider;
    }
}
