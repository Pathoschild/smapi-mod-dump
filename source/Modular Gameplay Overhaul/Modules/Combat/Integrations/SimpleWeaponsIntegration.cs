/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("dengdeng.simpleweapons", "Simple Weapons")]
[ModConflict("Taiyo.VanillaTweaks")]
internal sealed class SimpleWeaponsIntegration : ModIntegration<SimpleWeaponsIntegration>
{
    /// <summary>Initializes a new instance of the <see cref="SimpleWeaponsIntegration"/> class.</summary>
    internal SimpleWeaponsIntegration()
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

        ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
        Log.D("[CMBT]: Registered the Simple Weapons integration.");
        return true;
    }
}
