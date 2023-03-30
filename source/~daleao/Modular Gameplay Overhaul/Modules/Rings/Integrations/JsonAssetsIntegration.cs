/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.Integrations;

#region using directives

using System.IO;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.JsonAssets;

#endregion using directives

[RequiresMod("spacechase0.JsonAssets", "Json Assets", "1.10.7")]
internal sealed class JsonAssetsIntegration : ModIntegration<JsonAssetsIntegration, IJsonAssetsApi>
{
    internal JsonAssetsIntegration()
        : base("spacechase0.JsonAssets", "Json Assets", "1.10.7", ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (this.IsLoaded)
        {
            var directory = Path.Combine(ModHelper.DirectoryPath, "assets", "json-assets", "Rings");
            if (!Directory.Exists(directory))
            {
                return false;
            }

            var subDir = VanillaTweaksIntegration.Instance?.RingsCategoryEnabled == true
                ? "VanillaTweaks"
                : BetterRingsIntegration.Instance?.IsLoaded == true
                    ? "BetterRings" : "Vanilla";
            this.ModApi.LoadAssets(Path.Combine(directory, subDir), I18n);
            this.ModApi.IdsAssigned += this.OnIdsAssigned;
            return true;
        }

        RingsModule.Config.TheOneInfinityBand = false;
        ModHelper.WriteConfig(ModEntry.Config);
        return false;
    }

    /// <summary>Gets assigned IDs.</summary>
    private void OnIdsAssigned(object? sender, EventArgs e)
    {
        if (this.ModApi is null)
        {
            return;
        }

        Globals.GarnetIndex = this.ModApi.GetObjectId("Garnet");
        if (Globals.GarnetIndex == -1)
        {
            Log.W("[Rings]: Failed to get an ID for Garnet from Json Assets.");
        }

        Globals.GarnetRingIndex = this.ModApi.GetObjectId("Garnet Ring");
        if (Globals.GarnetRingIndex == -1)
        {
            Log.W("[Rings]: Failed to get an ID for Garnet Ring from Json Assets.");
        }

        Globals.InfinityBandIndex = this.ModApi.GetObjectId("Infinity Band");
        if (Globals.InfinityBandIndex == -1)
        {
            Log.W("[Rings]: Failed to get an ID for Infinity Band from Json Assets.");
        }

        Log.T("[Rings]: Done assigning IDs for custom items in the Rings module.");
    }
}
