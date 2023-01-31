/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraCore.Framework.Harmonizer.HarmonyAttributes;

/// <summary>
/// Indicates the following patch should only be applied if a specific mod is not installed.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
internal class NotWithModAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotWithModAttribute"/> class.
    /// </summary>
    /// <param name="uniqueID">Unique ID of the mod.</param>
    public NotWithModAttribute(string uniqueID)
        => this.UniqueID = uniqueID;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotWithModAttribute"/> class.
    /// </summary>
    /// <param name="uniqueID">Unique ID of the mod.</param>
    /// <param name="minVersion">Minimum version for this attribute to apply to, inclusive.</param>
    /// <param name="maxVersion">Max version for this attribute to apply to, inclusive.</param>
    public NotWithModAttribute(string uniqueID, string? minVersion = null, string? maxVersion = null)
        : this(uniqueID)
    {
        this.MinVersion = minVersion;
        this.MaxVersion = maxVersion;
    }

    /// <summary>
    /// Gets the unique ID of the mod avoid patching with.
    /// </summary>
    internal string UniqueID { get; init; }

    internal string? MinVersion { get; init; }

    internal string? MaxVersion { get; init; }
}
