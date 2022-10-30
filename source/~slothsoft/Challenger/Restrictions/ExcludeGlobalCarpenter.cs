/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Events;
using Slothsoft.Challenger.Models;

// ReSharper disable InconsistentNaming

namespace Slothsoft.Challenger.Restrictions;

public class ExcludeGlobalCarpenter : IRestriction {
    
    private readonly string _displayName;
    private readonly string[] _excludedBluePrintNames;

    public ExcludeGlobalCarpenter(string displayName,  params string[] excludedBluePrintNames) {
        _displayName = displayName;
        _excludedBluePrintNames = excludedBluePrintNames;
    }

    public string GetDisplayText() {
        return CommonHelpers.ToListString(_displayName);
    }

    public void Apply() {
        GlobalCarpenterChanger.AddExcludedBluePrintNames(_excludedBluePrintNames);
    }

    public void Remove() {
        GlobalCarpenterChanger.RemoveExcludedBluePrintNames(_excludedBluePrintNames);
    }
}