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
using ChallengerTest.Common.Events;
using StardewModdingAPI.Events;

namespace ChallengerTest.Common;

public class TestModEvents : IModEvents {
    public IContentEvents Content { get; } = new TestContentEvents();
    public IDisplayEvents Display { get; } = new TestDisplayEvents();
    public IGameLoopEvents GameLoop { get; } = new TestGameLoopEvents();
    public IInputEvents Input { get; } = new TestInputEvents();
    public IMultiplayerEvents Multiplayer { get; } = new TestMultiplayerEvents();
    public IPlayerEvents Player { get; } = new TestPlayerEvents();
    public IWorldEvents World { get; } = new TestWorldEvents();
    public ISpecializedEvents Specialized { get; } = new TestSpecializedEvents();
}