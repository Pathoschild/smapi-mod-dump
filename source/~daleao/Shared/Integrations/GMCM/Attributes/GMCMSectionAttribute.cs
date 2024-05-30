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

/// <summary>Assigns a GMCM property to a specific section in the current page.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMSectionAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMSectionAttribute"/> class.</summary>
    /// <param name="sectionTitleKey">The translation key for the section title.</param>
    public GMCMSectionAttribute(string sectionTitleKey)
    {
        this.SectionTitleKey = sectionTitleKey;
    }

    /// <summary>Gets the translation key for the section title.</summary>
    public string SectionTitleKey { get; }
}
