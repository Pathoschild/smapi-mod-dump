/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System;
using StardewModdingAPI.Events;

namespace StardewTests.Harness.Events;

public class TestContentEvents : IContentEvents {
    public event EventHandler<AssetRequestedEventArgs>? AssetRequested;
    public event EventHandler<AssetsInvalidatedEventArgs>? AssetsInvalidated;
    public event EventHandler<AssetReadyEventArgs>? AssetReady;
    public event EventHandler<LocaleChangedEventArgs>? LocaleChanged;
}