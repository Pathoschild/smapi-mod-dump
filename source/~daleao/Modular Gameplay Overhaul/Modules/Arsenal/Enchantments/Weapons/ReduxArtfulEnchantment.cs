/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Enchantments;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Improves weapon special moves.</summary>
[XmlType("Mods_DaLion_ArtfulEnchantment")]
public class ReduxArtfulEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return "Artful";
    }
}
