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
    private readonly Dictionary<string, string> customFields;

    /// <summary>Initializes a new instance of the <see cref="CustomFieldsStorageOptions" /> class.</summary>
    /// <param name="customFields">The custom fields.</param>
    public CustomFieldsStorageOptions(Dictionary<string, string>? customFields) =>
        this.customFields = customFields ?? new Dictionary<string, string>();

    /// <inheritdoc />
    protected override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) =>
        this.customFields.TryGetValue(key, out value);

    /// <inheritdoc />
    protected override void SetValue(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            this.customFields.Remove(key);
            return;
        }

        this.customFields[key] = value;
    }
}