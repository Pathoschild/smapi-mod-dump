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

public class TestPlayerEvents : IPlayerEvents {
    public event EventHandler<InventoryChangedEventArgs>? InventoryChanged;
    public event EventHandler<LevelChangedEventArgs>? LevelChanged;
    public event EventHandler<WarpedEventArgs>? Warped;
}