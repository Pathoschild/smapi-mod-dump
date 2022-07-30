/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Globalization;
using AtraBase.Toolkit.Extensions;

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extensions to more easily interact with the ModData <see cref="ModDataDictionary" /> dictionary.
/// </summary>
/// <remarks>Inspired by https://github.com/spacechase0/StardewValleyMods/blob/main/SpaceShared/ModDataHelper.cs. </remarks>
public static class ModDataExtensions
{
    // Instead of storing a real bool, just store 0 or 1

    /// <summary>
    /// Gets a boolean value out of ModData.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultVal">default value.</param>
    /// <returns>Boolean value, or null if not found/not parseable.</returns>
    [return: NotNullIfNotNull("defaultVal")]
    public static bool? GetBool(this ModDataDictionary modData, string key, bool? defaultVal = null)
        => modData.TryGetValue(key, out string val) ? !(val == "0") : defaultVal;

    /// <summary>
    /// Sets a boolean value into modData.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="val">Value.</param>
    /// <param name="defaultVal">default value - not saved if matches.</param>
    public static void SetBool(this ModDataDictionary modData, string key, bool val, bool? defaultVal = null)
    {
        if (defaultVal == val)
        {
            modData.Remove(key);
        }
        else
        {
            modData[key] = val ? "1" : "0";
        }
    }

    /// <summary>
    /// Gets a float value out of ModData.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultVal">default value.</param>
    /// <returns>Float value, or null of not found/not parseable.</returns>
    [return: NotNullIfNotNull("defaultVal")]
    public static float? GetFloat(this ModDataDictionary modData, string key, float? defaultVal = null)
        => modData.TryGetValue(key, out string val) && float.TryParse(val, out float result) ? result : defaultVal;

    /// <summary>
    /// Sets a float value into modData. To reduce reads/writes, rounds.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="val">Value.</param>
    /// <param name="decimals">Decimal points to round to.</param>
    /// <param name="format">Format string.</param>
    /// <param name="defaultVal">default value - not saved if matches.</param>
    public static void SetFloat(this ModDataDictionary modData, string key, float val, int decimals = 2, string format = "G", float? defaultVal = null)
    {
        if (defaultVal is not null && val.WithinMargin(defaultVal.Value, 0.499f * (float)Math.Pow(0.1, -decimals)))
        {
            modData.Remove(key);
        }
        else
        {
            modData[key] = MathF.Round(val, decimals, MidpointRounding.ToEven).ToString(format, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Gets a int value out of ModData.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultVal">default value.</param>
    /// <returns>Int value, or null of not found/not parseable.</returns>
    [return: NotNullIfNotNull("defaultVal")]
    public static int? GetInt(this ModDataDictionary modData, string key, int? defaultVal = null)
        => modData.TryGetValue(key, out string val) && int.TryParse(val, out int result) ? result : defaultVal;

    /// <summary>
    /// Sets a int value into modData.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="val">Value.</param>
    /// /// <param name="format">Format string.</param>
    /// <param name="defaultVal">default value - not saved if matches.</param>
    public static void SetInt(this ModDataDictionary modData, string key, int val, string format = "G", int? defaultVal = null)
    {
        if (defaultVal is not null && defaultVal.Value == val)
        {
            modData.Remove(key);
        }
        else
        {
            modData[key] = val.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}