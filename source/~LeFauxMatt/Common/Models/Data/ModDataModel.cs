/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Models.Data;

using StardewMods.FauxCore.Common.Interfaces.Data;
using StardewValley.Mods;

#else
namespace StardewMods.Common.Models.Data;

using StardewMods.Common.Interfaces.Data;
using StardewValley.Mods;
#endif

/// <inheritdoc />
internal sealed class ModDataModel : IDictionaryModel
{
    private readonly ModDataDictionary modData;

    /// <summary>Initializes a new instance of the <see cref="ModDataModel" /> class.</summary>
    /// <param name="modData">The mod data dictionary.</param>
    public ModDataModel(ModDataDictionary modData) => this.modData = modData;

    /// <inheritdoc />
    public bool ContainsKey(string key) => this.modData.ContainsKey(key);

    /// <inheritdoc />
    public void SetValue(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            this.modData.Remove(key);
            return;
        }

        this.modData[key] = value;
    }

    /// <inheritdoc />
    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value) =>
        this.modData.TryGetValue(key, out value);
}