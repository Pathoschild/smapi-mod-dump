using HarmonyLib;

using NeverEndingAdventure.HarmonyPatches;
using NeverEndingAdventure.Utils;

using StardewModdingAPI;

namespace NeverEndingAdventure;

/// <inheritdoc/>
internal sealed class ModEntry : Mod
{
    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        // this is the entry point to your mod.
        // SMAPI calls this method when it loads your mod.

        Log.Monitor = this.Monitor; // this binds SMAPI's logging to the Log class so you can use it.

        // applying harmony patches.
        Harmony harmony = new(this.ModManifest.UniqueID);
        UntimedSO.ApplyPatch(harmony, helper);
    }
}
