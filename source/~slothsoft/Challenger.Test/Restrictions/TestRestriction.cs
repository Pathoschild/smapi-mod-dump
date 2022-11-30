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
using Slothsoft.Challenger.Api;

namespace ChallengerTest.Restrictions; 

public class TestRestriction : IRestriction {

    public string DisplayText { get; set; } = "Test Restriction";
    public Action<TestRestriction> OnApply { get; set; } = _ => {};
    public Action<TestRestriction> OnRemove { get; set; } = _ => {};
    
    public string GetDisplayText() {
        return DisplayText;
    }

    public void Apply() {
        OnApply(this);
    }

    public void Remove() {
        OnRemove(this);
    }
}