/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Models;

// ReSharper disable InconsistentNaming

namespace Slothsoft.Challenger.Restrictions;

public class RenameVanillaObject : IRestriction {
    public record VanillaObject(
        int ParentSheetIndex, 
        SObject.PreserveType? PreserveType = null,
        int? PreservedParentSheetIndex = null
    );
    
    private const string HarmonySuffix = ".RenameVanillaObject";
    
    private static Harmony? _harmony;
    private static readonly IDictionary<VanillaObject, string> _appliedVanillaObjectToDisplayName = new Dictionary<VanillaObject, string>();
    
    private static bool LoadDisplayName(SObject __instance, ref string __result) {
        var vanillaObject = new VanillaObject(
            __instance.ParentSheetIndex,
            __instance.preserve.Value,
            __instance.preservedParentSheetIndex.Value
        );
        if (_appliedVanillaObjectToDisplayName.ContainsKey(vanillaObject)) {
            __result = _appliedVanillaObjectToDisplayName[vanillaObject];
            return false;
        }
        return true;
    }

    private readonly IModHelper _modHelper;
    private readonly IDictionary<VanillaObject, string> _vanillaObjectToDisplayName;

    public RenameVanillaObject(IModHelper modHelper, VanillaObject vanillaObject, string displayName)
        : this(modHelper, new Dictionary<VanillaObject, string> {
            { vanillaObject, displayName },
        }) {
    }
    
    public RenameVanillaObject(IModHelper modHelper, IDictionary<VanillaObject, string> vanillaObjectToDisplayName) {
        _modHelper = modHelper;
        _vanillaObjectToDisplayName = vanillaObjectToDisplayName;
    }

    public string GetDisplayText() {
        return CommonHelpers.ToListString(
            _vanillaObjectToDisplayName.Values.Select(v => _modHelper.Translation.Get("RenameVanillaObject.DisplayText", new { NewObject = v } ).ToString())
            );
    }
    
    public void Apply() {
        if (_harmony == null) {
            // If harmony is not null, it was instanciated by this instance or another one
            _harmony = new Harmony(ChallengerMod.Instance.ModManifest.UniqueID + HarmonySuffix);
            _harmony.Patch(
                original: AccessTools.Method(
                    typeof(SObject),
                    "loadDisplayName"
                ),
                prefix: new HarmonyMethod(typeof(RenameVanillaObject), nameof(LoadDisplayName))
            );
        }
        
        foreach (var keyValuePair in _vanillaObjectToDisplayName) {
            _appliedVanillaObjectToDisplayName[keyValuePair.Key] = keyValuePair.Value;
        }
    }

    public void Remove() {
        foreach (var keyValuePair in _vanillaObjectToDisplayName) {
            _appliedVanillaObjectToDisplayName.Remove(keyValuePair.Key);
        }

        if (_appliedVanillaObjectToDisplayName.Count == 0 && _harmony != null) {
            // if there is nothing to replace, we don't need harmony any longer
            _harmony.UnpatchAll(ChallengerMod.Instance.ModManifest.UniqueID + HarmonySuffix);
            _harmony = null;
        }
    }
}