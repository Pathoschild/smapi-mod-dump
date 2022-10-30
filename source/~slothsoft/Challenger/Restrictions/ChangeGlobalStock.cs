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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Events;
using Slothsoft.Challenger.Models;

// ReSharper disable InconsistentNaming

namespace Slothsoft.Challenger.Restrictions;

public class ChangeGlobalStock : IRestriction {

    public static ChangeGlobalStock ExcludeSalables(string displayName,  Func<ISalable, bool> excludeFilter) {
        return new ChangeGlobalStock(displayName, stock => {
            foreach (var keyValuePair in stock) {
                if (excludeFilter.Invoke(keyValuePair.Key)) {
                    stock.Remove(keyValuePair.Key);
                }
            }
        });
    }
    
    public static ChangeGlobalStock AddRiceInFirstSpring(IModHelper modHelper) {
        return new ChangeGlobalStock(modHelper.Translation.Get("ChangeGlobalStock.AddRiceInFirstSpring"), stock => {
            if (Game1.year == 1 && Game1.currentSeason == Seasons.Spring) {
                var originalStock = stock.ToDictionary(entry => entry.Key, entry => entry.Value);
             
                stock.Clear();
                
                var riceSeeds = new StardewValley.Object(Vector2.Zero, SeedIds.Rice, 1);
                stock.Add(riceSeeds, new[]
                {
                    40,
                    int.MaxValue
                });
                
                foreach (var entry in originalStock) {
                    stock.Add(entry.Key, entry.Value);
                }
            }

        });
    }
    
    private readonly string _displayName;
    private readonly Action<IDictionary<ISalable, int[]>> _changer;

    public ChangeGlobalStock(string displayName,  Action<IDictionary<ISalable, int[]>> changer) {
        _displayName = displayName;
        _changer = changer;
    }

    public string GetDisplayText() {
        return CommonHelpers.ToListString(_displayName);
    }

    public void Apply() {
        GlobalStockChanger.AddChanger(_changer);
    }

    public void Remove() {
        GlobalStockChanger.RemoveChanger(_changer);
    }
}