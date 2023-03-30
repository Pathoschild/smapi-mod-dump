/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Enchantments;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.SpaceCore;

#endregion using directives

[RequiresMod("spacechase0.SpaceCore", "SpaceCore", "1.8.3")]
internal sealed class SpaceCoreIntegration : ModIntegration<SpaceCoreIntegration, ISpaceCoreApi>
{
    private SpaceCoreIntegration()
        : base("spacechase0.SpaceCore", "SpaceCore", "1.8.3", ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.AssertLoaded();

        this.ModApi.RegisterSerializerType(typeof(LavaEnchantment));
        this.ModApi.RegisterSerializerType(typeof(ObsidianEnchantment));
        this.ModApi.RegisterSerializerType(typeof(BlessedEnchantment));
        this.ModApi.RegisterSerializerType(typeof(CursedEnchantment));
        this.ModApi.RegisterSerializerType(typeof(InfinityEnchantment));

        return true;
    }
}
