/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Enchantments.Gemstone;
using DaLion.Overhaul.Modules.Enchantments.Melee;
using DaLion.Overhaul.Modules.Enchantments.Ranged;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.SpaceCore;

#endregion using directives

[ModRequirement("spacechase0.SpaceCore", "SpaceCore", "1.12.0")]
internal sealed class SpaceCoreIntegration : ModIntegration<SpaceCoreIntegration, ISpaceCoreApi>
{
    /// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
    internal SpaceCoreIntegration()
        : base("spacechase0.SpaceCore", "SpaceCore", "1.12.0", ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.AssertLoaded();

        // melee
        this.ModApi.RegisterSerializerType(typeof(MeleeArtfulEnchantment));
        this.ModApi.RegisterSerializerType(typeof(CarvingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(CleavingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(EnergizedEnchantment));
        this.ModApi.RegisterSerializerType(typeof(ExplosiveEnchantment));
        this.ModApi.RegisterSerializerType(typeof(MammoniteEnchantment));
        this.ModApi.RegisterSerializerType(typeof(BloodthirstyEnchantment));
        this.ModApi.RegisterSerializerType(typeof(SteadfastEnchantment));
        this.ModApi.RegisterSerializerType(typeof(WabbajackEnchantment));

        // ranged
        this.ModApi.RegisterSerializerType(typeof(BaseSlingshotEnchantment));
        this.ModApi.RegisterSerializerType(typeof(RangedArtfulEnchantment));
        this.ModApi.RegisterSerializerType(typeof(MagnumEnchantment));
        this.ModApi.RegisterSerializerType(typeof(GatlingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(PreservingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(QuincyEnchantment));
        this.ModApi.RegisterSerializerType(typeof(SpreadingEnchantment));

        // gemstone
        this.ModApi.RegisterSerializerType(typeof(GarnetEnchantment));

        return true;
    }
}
