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

/// <inheritdoc />
internal sealed class CustomFieldsStorageOptions : DictionaryStorageOptions
{
    private readonly Func<bool, Dictionary<string, string>> getData;

    /// <summary>Initializes a new instance of the <see cref="CustomFieldsStorageOptions" /> class.</summary>
    /// <param name="getData">Get the custom field data.</param>
    public CustomFieldsStorageOptions(Func<bool, Dictionary<string, string>> getData) => this.getData = getData;

    /// <inheritdoc />
    protected override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) =>
        this.getData(false).TryGetValue(key, out value);

    /// <inheritdoc />
    protected override void SetValue(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            this.getData(false).Remove(key);
            return;
        }

        this.getData(true)[key] = value;
    }
}