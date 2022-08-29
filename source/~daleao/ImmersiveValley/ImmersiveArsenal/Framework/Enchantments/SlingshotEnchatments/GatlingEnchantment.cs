/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Enables auto-firing at lower firing speed..</summary>
[XmlType("Mods_DaLion_GatlingEnchantment")]
public class GatlingEnchantment : BaseSlingshotEnchantment
{
    public override string GetName() => ModEntry.i18n.Get("enchantments.gatling");
}