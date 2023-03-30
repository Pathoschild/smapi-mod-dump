/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;

namespace DrawFishPondsOverGrass;

/// <inheritdoc/>
internal sealed class ModEntry : Mod
{
    /// <summary>
    /// Gets the logger for this file.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");
    }

    /// <summary>
    /// Applies and logs this mod's harmony patches.
    /// </summary>
    /// <param name="harmony">My harmony instance.</param>
    public void ApplyPatches(Harmony harmony)
    {
        try
        {
            harmony.PatchAll(typeof(ModEntry).Assembly);
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Mod crashed while applying harmony patches\n\n{ex}", LogLevel.Error);
        }

        // no snitch necessary, this mod entirely uses ForEachMatch patches.
        // harmony.Snitch(this.Monitor, this.ModManifest.UniqueID, transpilersOnly: true);
    }
}