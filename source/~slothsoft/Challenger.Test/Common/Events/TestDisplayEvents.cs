/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System;
using StardewModdingAPI.Events;

namespace ChallengerTest.Common.Events;

public class TestDisplayEvents : IDisplayEvents {
    public event EventHandler<MenuChangedEventArgs>? MenuChanged;
    public event EventHandler<RenderingEventArgs>? Rendering;
    public event EventHandler<RenderedEventArgs>? Rendered;
    public event EventHandler<RenderingWorldEventArgs>? RenderingWorld;
    public event EventHandler<RenderedWorldEventArgs>? RenderedWorld;
    public event EventHandler<RenderingActiveMenuEventArgs>? RenderingActiveMenu;
    public event EventHandler<RenderedActiveMenuEventArgs>? RenderedActiveMenu;
    public event EventHandler<RenderingHudEventArgs>? RenderingHud;
    public event EventHandler<RenderedHudEventArgs>? RenderedHud;
    public event EventHandler<WindowResizedEventArgs>? WindowResized;
}