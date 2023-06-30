/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Slingshots.Integrations;

#region using directives

using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.Archery;

#endregion using directives

[ModRequirement("PeacefulEnd.Archery", "Archery", "2.1.0")]
internal sealed class ArcheryIntegration : ModIntegration<ArcheryIntegration, IArcheryApi>
{
    /// <summary>Initializes a new instance of the <see cref="ArcheryIntegration"/> class.</summary>
    internal ArcheryIntegration()
        : base("PeacefulEnd.Archery", "Archery", "2.1.0", ModHelper.ModRegistry)
    {
    }

    internal Lazy<MethodInfo> GetAmmoModel { get; } = new(() => "Archery.Framework.Objects.InstancedObject".ToType()
        .RequireMethod("GetModel")
        .MakeGenericMethod("Archery.Framework.Models.Weapons.AmmoModel".ToType()));

    internal Lazy<MethodInfo> GetWeaponModel { get; } = new(() => "Archery.Framework.Objects.InstancedObject".ToType()
        .RequireMethod("GetModel")
        .MakeGenericMethod("Archery.Framework.Models.Weapons.WeaponModel".ToType()));
}
