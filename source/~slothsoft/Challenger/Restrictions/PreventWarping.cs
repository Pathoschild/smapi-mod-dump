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
using HarmonyLib;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Models;

// ReSharper disable InconsistentNaming

namespace Slothsoft.Challenger.Restrictions;

public class PreventWarping : IRestriction {
    public record WarpDirection(
        string From,
        string To
    );
    
    private const string HarmonySuffix = ".PreventWarping";
    
    private static Harmony? _harmony;
    private static readonly IDictionary<WarpDirection, Func<string?>> _appliedWarpDirections = new Dictionary<WarpDirection, Func<string?>>();
    
    private static bool WarpFarmer(LocationRequest locationRequest, int tileX, int tileY, int facingDirectionAfterWarp) {
        var warpDirection = new WarpDirection(
            Game1.currentLocation.Name,
            locationRequest.Name
        );
        if (_appliedWarpDirections.ContainsKey(warpDirection)) {
            // if the direction is in the list and the key returns an error, we prevent warping
            var preventionMessage = _appliedWarpDirections[warpDirection]();
            if (preventionMessage != null) {
                Game1.addHUDMessage(new HUDMessage(preventionMessage, HUDMessage.error_type));
                return false;
            }
        }
        return true;
    }

    private readonly string _displayName;
    private readonly Dictionary<WarpDirection, Func<bool>> _warpDirections;

    public PreventWarping(string displayName, Dictionary<WarpDirection, Func<bool>> warpDirections) {
        _displayName = displayName;
        _warpDirections = warpDirections;
    }

    public string GetDisplayText() {
        return CommonHelpers.ToListString(_displayName);
    }

    public void Apply() {
        if (_harmony == null) {
            // If harmony is not null, it was instanciated by this instance or another one
            _harmony = new Harmony(ChallengerMod.Instance.ModManifest.UniqueID + HarmonySuffix);
            _harmony.Patch(
                original: AccessTools.Method(
                    typeof(Game1),
                    nameof(Game1.warpFarmer),
                    new[] {
                        typeof(LocationRequest),
                        typeof(int),
                        typeof(int),
                        typeof(int),
                    }),
                prefix: new HarmonyMethod(typeof(PreventWarping), nameof(WarpFarmer))
            );
        }
        
        foreach (var direction in _warpDirections) {
            _appliedWarpDirections.Add(direction.Key, () => direction.Value() ? _displayName : null);
        }
    }

    public void Remove() {
        foreach (var direction in _warpDirections) {
            _appliedWarpDirections.Remove(direction.Key);
        }

        if (_appliedWarpDirections.Count == 0 && _harmony != null) {
            // if there is nothing to replace, we don't need harmony any longer
            _harmony.UnpatchAll(ChallengerMod.Instance.ModManifest.UniqueID + HarmonySuffix);
            _harmony = null;
        }
    }
}