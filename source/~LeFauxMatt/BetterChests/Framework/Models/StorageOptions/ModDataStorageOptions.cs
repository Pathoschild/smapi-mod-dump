/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Models.StorageOptions;

using StardewValley.Mods;

/// <inheritdoc />
internal sealed class ModDataStorageOptions : DictionaryStorageOptions
{
    private readonly ModDataDictionary modData;

    /// <summary>Initializes a new instance of the <see cref="ModDataStorageOptions" /> class.</summary>
    /// <param name="modData">The mod data dictionary.</param>
    public ModDataStorageOptions(ModDataDictionary modData) => this.modData = modData;

    /// <inheritdoc />
    protected override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) =>
        this.modData.TryGetValue(key, out value);

    /// <inheritdoc />
    protected override void SetValue(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            this.modData.Remove(key);
            return;
        }

        this.modData[key] = value;
    }
}