/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.JsonAssets;

#endregion using directives

[ModRequirement("ughitsmegan.ostrichmayoforJsonAssets", "[JA] Ostrich Mayo")]
internal sealed class OstrichMayoIntegration : ModIntegration<OstrichMayoIntegration, IJsonAssetsApi>
{
    /// <summary>Initializes a new instance of the <see cref="OstrichMayoIntegration"/> class.</summary>
    internal OstrichMayoIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        this.ModApi.IdsAssigned += this.OnIdsAssigned;
        Log.D("[PROFS]: Registered the Ostrich Mayo integration.");
        return true;
    }

    /// <summary>Gets Ostrich Mayo ID.</summary>
    private void OnIdsAssigned(object? sender, EventArgs e)
    {
        this.AssertLoaded();
        var index = this.ModApi.GetObjectId("Delight Mayonnaise");
        if (index == -1)
        {
            Log.W("[PROFS]: Failed to get ID for Ostrich Mayo from Json Assets.");
            return;
        }

        Log.D($"[PROFS]: Json Assets ID {index} has been assigned to Ostrich Mayo.");
        Sets.AnimalDerivedProductIds = Sets.AnimalDerivedProductIds.Add(index);
    }
}
