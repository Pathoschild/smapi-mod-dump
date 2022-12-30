/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Overhaul.Modules.Rings.Integrations;
using DaLion.Shared.Extensions.SMAPI;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuCore
{
    /// <summary>Register the Rings menu.</summary>
    private void RegisterRings()
    {
        this
            .AddPage(OverhaulModule.Rings.Namespace, () => "Ring Settings")

            .AddCheckbox(
                () => "Rebalanced Rings",
                () => "Improves certain underwhelming rings.",
                config => config.Rings.RebalancedRings,
                (config, value) =>
                {
                    config.Rings.RebalancedRings = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                })
            .AddCheckbox(
                () => "Craftable Gemstone Rings",
                () => "Adds new combat recipes for crafting gemstone rings.",
                config => config.Rings.CraftableGemRings,
                (config, value) =>
                {
                    config.Rings.CraftableGemRings = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
                })
            .AddCheckbox(
                () => "Craftable Glow and Magnet Rings",
                () => "Adds new mining recipes for crafting glow and magnet rings.",
                config => config.Rings.CraftableGlowAndMagnetRings,
                (config, value) =>
                {
                    config.Rings.CraftableGlowAndMagnetRings = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                })
            .AddCheckbox(
                () => "Immersive Glowstone Recipe",
                () => "Replaces the glowstone ring recipe with one that makes sense.",
                config => config.Rings.ImmersiveGlowstoneRecipe,
                (config, value) =>
                {
                    config.Rings.ImmersiveGlowstoneRecipe = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                })
            .AddCheckbox(
                () => "The One Iridium Band",
                () => "Replaces the Iridium Band recipe and effect. Adds new forge mechanics.",
                config => config.Rings.TheOneInfinityBand,
                (config, value) =>
                {
                    if (value && !ModHelper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
                    {
                        Log.W("Cannot enable The One Iridium Band because this feature requires Json Assets which is not installed.");
                        return;
                    }

                    config.Rings.TheOneInfinityBand = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
                    if (value && !Globals.InfinityBandIndex.HasValue && JsonAssetsIntegration.Instance?.IsRegistered == false)
                    {
                        JsonAssetsIntegration.Instance.Register();
                    }
                })
            .AddCheckbox(
                () => "Enable Gemstone Resonance",
                () => "Allows gemstones to harmonize and resonate in close proximity of each other.",
                config => config.Rings.EnableResonance,
                (config, value) =>
                {
                    config.Rings.EnableResonance = value;
                });
    }
}
