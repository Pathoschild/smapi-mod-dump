/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Ranged;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Enchantments.Events;

#endregion using directives

/// <summary>Enables auto-firing at lower firing speed.</summary>
[XmlType("Mods_DaLion_GatlingEnchantment")]
public sealed class GatlingEnchantment : BaseSlingshotEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Get("enchantments.gatling.name");
    }

    /// <inheritdoc />
    protected override void _OnEquip(Farmer who)
    {
        base._OnEquip(who);
        EventManager.Enable<GatlingButtonPressedEvent>();
    }

    /// <inheritdoc />
    protected override void _OnUnequip(Farmer who)
    {
        base._OnUnequip(who);
        EventManager.Disable<GatlingButtonPressedEvent>();
    }
}
