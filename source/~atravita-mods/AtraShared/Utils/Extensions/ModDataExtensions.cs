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
using System.Runtime.CompilerServices;

using AtraBase.Toolkit;
using AtraBase.Toolkit.Extensions;

using Microsoft.Xna.Framework;

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extensions to more easily interact with the ModData <see cref="ModDataDictionary" /> dictionary.
/// </summary>
/// <remarks>Inspired by https://github.com/spacechase0/StardewValleyMods/blob/main/SpaceShared/ModDataHelper.cs. </remarks>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Reviewed.")]
public static class ModDataExtensions
{
    /// <summary>
    /// Gets whether two modData dictionaries contain exactly the same modData.
    /// </summary>
    /// <param name="self">This modData.</param>
    /// <param name="other">That modData.</param>
    /// <returns>True if they match, false otherwise.</returns>
    public static bool ModDataMatches(this ModDataDictionary? self, ModDataDictionary? other)
    {
        if (self?.Count() is null or 0)
        {
            return other?.Count() is null or 0;
        }
        if (other?.Count() is null or 0)
        {
            return false;
        }
        return self.Count() == other.Count()
                && self.Pairs.All((kvp) => other.TryGetValue(kvp.Key, out var val) && val == kvp.Value);
    }

    /// <summary>
    /// Copies all modData out of the other dictionary into this one.
    /// </summary>
    /// <param name="self">This modData.</param>
    /// <param name="other">That modData.</param>
    public static void CopyModDataFrom(this ModDataDictionary? self, ModDataDictionary? other)
    {
        if (self is null || other is null)
        {
            return;
        }

        foreach ((string key, string value) in other.Pairs)
        {
            self[key] = value;
        }
    }

    // Instead of storing a real bool, just store 0 or 1
    private const string TrueValue = "1";
    private const string FalseValue = "0";

    /// <summary>
    /// Gets a boolean value out of ModData.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultVal">default value.</param>
    /// <returns>Boolean value, or null if not found/not parseable.</returns>
    [MethodImpl(TKConstants.Hot)]
    [return: NotNullIfNotNull("defaultVal")]
    public static bool? GetBool(this ModDataDictionary modData, string key, bool? defaultVal = null)
        => modData.TryGetValue(key, out string val)
            ? val != FalseValue
            : defaultVal;

    /// <summary>
    /// Sets a boolean value into modData.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="val">Value.</param>
    /// <param name="defaultVal">default value - not saved if matches.</param>
    [MethodImpl(TKConstants.Hot)]
    public static void SetBool(this ModDataDictionary modData, string key, bool val, bool? defaultVal = null)
    {
        if (defaultVal == val)
        {
            modData.Remove(key);
        }
        else
        {
            modData[key] = val ? TrueValue : FalseValue;
        }
    }

    /// <summary>
    /// Gets a float value out of ModData.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultVal">default value.</param>
    /// <returns>Float value, or null of not found/not parseable.</returns>
    [MethodImpl(TKConstants.Hot)]
    [return: NotNullIfNotNull("defaultVal")]
    public static float? GetFloat(this ModDataDictionary modData, string key, float? defaultVal = null)
        => modData.TryGetValue(key, out string val) && float.TryParse(val, NumberStyles.Number, CultureInfo.InvariantCulture, out float result)
            ? result
            : defaultVal;

    /// <summary>
    /// Sets a float value into modData. To reduce reads/writes, rounds.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="val">Value.</param>
    /// <param name="decimals">Decimal points to round to.</param>
    /// <param name="format">Format string.</param>
    /// <param name="defaultVal">default value - not saved if matches.</param>
    [MethodImpl(TKConstants.Hot)]
    public static void SetFloat(this ModDataDictionary modData, string key, float val, int decimals = 2, string format = "G", float? defaultVal = null)
    {
        if (defaultVal is not null && val.WithinMargin(defaultVal.Value, 0.499f * (float)Math.Pow(0.1, -decimals)))
        {
            modData.Remove(key);
        }
        else
        {
            modData[key] = MathF.Round(val, decimals, MidpointRounding.ToEven)
                                .ToString(format, provider: CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Gets a int value out of ModData.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultVal">default value.</param>
    /// <returns>Int value, or null of not found/not parseable.</returns>
    [MethodImpl(TKConstants.Hot)]
    [return: NotNullIfNotNull("defaultVal")]
    public static int? GetInt(this ModDataDictionary modData, string key, int? defaultVal = null)
        => modData.TryGetValue(key, out string val) && int.TryParse(val, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int result) ? result : defaultVal;

    /// <summary>
    /// Sets a int value into modData.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="val">Value.</param>
    /// /// <param name="format">Format string.</param>
    /// <param name="defaultVal">default value - not saved if matches.</param>
    [MethodImpl(TKConstants.Hot)]
    public static void SetInt(this ModDataDictionary modData, string key, int val, string format = "G", int? defaultVal = null)
    {
        if (defaultVal is not null && defaultVal.Value == val)
        {
            modData.Remove(key);
        }
        else
        {
            modData[key] = val.ToString(format, provider: CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Gets an enum value from modData.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultValue">Default Value.</param>
    /// <returns>Value if found, default value if not.</returns>
    [MethodImpl(TKConstants.Hot)]
    public static TEnum GetEnum<TEnum>(this ModDataDictionary modData, string key, TEnum defaultValue)
        where TEnum : struct, Enum
        => modData.TryGetValue(key, out string val) && Enum.TryParse(val, out TEnum ret)
            ? ret
            : defaultValue;

    /// <summary>
    /// Sets an enum value into ModData.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="value">Value to set.</param>
    [MethodImpl(TKConstants.Hot)]
    public static void SetEnum<TEnum>(this ModDataDictionary modData, string key, TEnum value)
        where TEnum : struct, Enum
        => modData[key] = value.ToString("D");

    /// <summary>
    /// Removes an enum from the modData.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    [MethodImpl(TKConstants.Hot)]
    public static void RemoveEnum(this ModDataDictionary modData, string key)
        => modData.Remove(key);

    // colors are stored as the hex representation of their packed value.
    // While I could do just two chars, it'll make it harder to debug
    // and some mods will print out the values of the moddata
    // making arbitrary unicode characters probably unwise.

    /// <summary>
    /// Gets a color value out of ModData.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultVal">default value.</param>
    /// <returns>Int value, or null of not found/not parseable.</returns>
    [MethodImpl(TKConstants.Hot)]
    [return: NotNullIfNotNull("defaultVal")]
    public static Color? GetColor(this ModDataDictionary modData, string key, Color? defaultVal = null)
        => modData.TryGetValue(key, out string? color) && uint.TryParse(color, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint result)
            ? new Color(result)
            : defaultVal;

    /// <summary>
    /// Sets a color value.
    /// </summary>
    /// <param name="modData">ModData.</param>
    /// <param name="key">Key.</param>
    /// <param name="color">Color value to set.</param>
    /// <param name="defaultVal">default value - not saved if matches.</param>
    [MethodImpl(TKConstants.Hot)]
    public static void SetColor(this ModDataDictionary modData, string key, Color color, Color? defaultVal = null)
    {
        if (defaultVal is not null & color == defaultVal)
        {
            modData.Remove(key);
        }
        else
        {
            modData[key] = color.PackedValue.ToString("X", CultureInfo.InvariantCulture);
        }
    }
}