/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Tells the GMCM generator that a property should only appear in the title screen of the menu.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMTitleScreenOnlyAttribute : Attribute
{
}
