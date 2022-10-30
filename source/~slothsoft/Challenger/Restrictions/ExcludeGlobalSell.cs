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

public class ExcludeGlobalSell : IRestriction {
    
    private readonly string _displayName;
    private readonly int[] _allowedCategories;

    public ExcludeGlobalSell(string displayName,  params int[] allowedCategories) {
        _displayName = displayName;
        _allowedCategories = allowedCategories;
    }

    public string GetDisplayText() {
        return CommonHelpers.ToListString(_displayName);
    }

    public void Apply() {
        GlobalSellChanger.AddAllowedCategories(_allowedCategories);
    }

    public void Remove() {
        GlobalSellChanger.RemoveAllowedCategories(_allowedCategories);
    }
}