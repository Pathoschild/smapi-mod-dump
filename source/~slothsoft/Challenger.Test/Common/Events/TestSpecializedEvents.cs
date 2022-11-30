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

public class TestSpecializedEvents : ISpecializedEvents{
    public event EventHandler<LoadStageChangedEventArgs>? LoadStageChanged;
    public event EventHandler<UnvalidatedUpdateTickingEventArgs>? UnvalidatedUpdateTicking;
    public event EventHandler<UnvalidatedUpdateTickedEventArgs>? UnvalidatedUpdateTicked;
}