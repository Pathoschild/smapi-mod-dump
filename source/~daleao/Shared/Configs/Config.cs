/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Configs;

/// <summary>Base class for a mod's config settings.</summary>
public abstract class Config
{
    /// <summary>Validate the config settings, replacing invalid values if necessary.</summary>
    /// <returns><see langword="true"/> if all config settings are valid and don't need rewriting, otherwise <see langword="false"/>.</returns>
    internal virtual bool Validate()
    {
        return true;
    }
}
