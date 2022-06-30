/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Common.Data;

#region using directives

using Extensions;
using Extensions.Stardew;
using Multiplayer;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;
using System;

#endregion using directives

internal static class ModDataIO
{
    private static Broadcaster _Broadcaster = null!;
    private static string _modID = null!;

    public static void Init(IMultiplayerHelper helper, string modID)
    {
        _Broadcaster = new(helper, modID);
        _modID = modID;
    }

    #region farmer io

    /// <summary>Read from a field in the farmer's <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(Farmer farmer, string field, string defaultValue = "") =>
        Game1.MasterPlayer.modData.Read($"{_modID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);

    /// <summary>Read from a field, external to this mod, in the farmer's <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadDataExt(Farmer farmer, string field, string modId, string defaultValue = "") =>
        Game1.MasterPlayer.modData.Read($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);

    /// <summary>Read from a field in the farmer's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataAs<T>(Farmer farmer, string field, T defaultValue = default) where T : struct =>
        Game1.MasterPlayer.modData.ReadAs($"{_modID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);

    /// <summary>Read from a field, external to this mod, in the farmer's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataExtAs<T>(Farmer farmer, string field, string modId, T defaultValue = default) where T : struct =>
         Game1.MasterPlayer.modData.ReadAs($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);

    /// <summary>Write to a field in the farmer's <see cref="ModDataDictionary" />, or remove the field if supplied an empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(Farmer farmer, string field, string? value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(value ?? String.Empty, $"RequestUpdateData/Write/{field}");
            return;
        }

        Game1.player.modData.Write($"{_modID}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModDataIO]: Cleared {farmer.Name}'s {field}."
            : $"[ModDataIO]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field, external to this mod, in the farmer's <see cref="ModDataDictionary" />, or remove the field if supplied an empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteDataExt(Farmer farmer, string field, string modId, string? value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(value ?? string.Empty, $"RequestUpdateData/Write/{field}", modId);
            return;
        }

        Game1.player.modData.Write($"{modId}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModDataIO]: Cleared {farmer.Name}'s {field}."
            : $"[ModDataIO]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field in the farmer's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(Farmer farmer, string field, string? value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{_modID}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.V($"[ModDataIO]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            _Broadcaster.MessageHost(value ?? string.Empty, $"RequestUpdateData/Write/{field}");
        else WriteData(farmer, field, value);

        return false;
    }

    /// <summary>Write to a field, external to this mod, in the farmer's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataExtIfNotExists(Farmer farmer, string field, string modId, string? value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{modId}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.V($"[ModDataIO]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            _Broadcaster.MessageHost(value ?? string.Empty, $"RequestUpdateData/Write/{field}", modId);
        else WriteData(farmer, field, value);

        return false;
    }

    /// <summary>Append a string to an existing string field in the farmer's <see cref="ModDataDictionary"/>, or initialize to the given value.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="value">Value to append.</param>
    public static void AppendData(Farmer farmer, string field, string value, string separator = ",")
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(value, $"RequestUpdateData/Append/{field}");
            return;
        }

        var current = ReadData(farmer, field);
        if (current.Contains(value))
        {
            Log.V($"[ModDataIO]: {farmer.Name}'s {field} already contained {value}.");
        }
        else
        {
            WriteData(farmer, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
            Log.V($"[ModDataIO]: Appended {farmer.Name}'s {field} with {value}");
        }
    }

    /// <summary>Append a string to an existing string field, external to this mod, in the farmer's <see cref="ModDataDictionary"/>, or initialize to the given value.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="value">Value to append.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    public static void AppendDataExt(Farmer farmer, string field, string value, string modId, string separator = ",")
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(value, $"RequestUpdateData/Append/{field}", modId);
            return;
        }

        var current = ReadDataExt(farmer, field, modId);
        if (current.Contains(value))
        {
            Log.V($"[ModDataIO]: {farmer.Name}'s {field} already contained {value}.");
        }
        else
        {
            WriteDataExt(farmer, field, string.IsNullOrEmpty(current) ? value : current + separator + value, modId);
            Log.V($"[ModDataIO]: Appended {farmer.Name}'s {field} with {value}");
        }
    }

    /// <summary>Increment the value of a numeric field in the farmer's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(Farmer farmer, string field, T amount) where T : struct
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(amount.ToString()!, $"RequestUpdateData/Increment/{field}");
            return;
        }

        Game1.player.modData.Increment($"{_modID}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.V($"[ModDataIO]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field, external to this mod, in the farmer's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    public static void IncrementDataExt<T>(Farmer farmer, string field, T amount, string modId) where T : struct
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(amount.ToString()!, $"RequestUpdateData/Increment/{field}", modId);
            return;
        }

        Game1.player.modData.Increment($"{modId}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.V($"[ModDataIO]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the farmer's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(Farmer farmer, string field) where T : struct
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost("1", $"RequestUpdateData/Increment/{field}");
            return;
        }

        Game1.player.modData.Increment($"{_modID}/{farmer.UniqueMultiplayerID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModDataIO]: Incremented {farmer.Name}'s {field} by 1.");
    }

    /// <summary>Increment the value of a numeric field, external to this mod, in the farmer's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    public static void IncrementDataExt<T>(Farmer farmer, string field, string modId) where T : struct
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost("1", $"RequestUpdateData/Increment/{field}", modId);
            return;
        }

        Game1.player.modData.Increment($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModDataIO]: Incremented {farmer.Name}'s {field} by 1.");
    }

    #endregion farmer io

    #region building io

    /// <summary>Read a string from the building's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(Building building, string field, string defaultValue = "") =>
        building.modData.Read($"{_modID}/{field}", defaultValue);

    /// <summary>Read a field from the building's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataAs<T>(Building building, string field, T defaultValue = default) where T : struct =>
        building.modData.ReadAs($"{_modID}/{field}", defaultValue);

    /// <summary>Write to a field in the building's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(Building building, string field, string? value)
    {
        building.modData.Write($"{_modID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {building.GetType().Name}'s {field}."
            : $"[ModData]: Wrote {value} to {building.GetType().Name}'s {field}.");
    }

    /// <summary>Write to a field in the building's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(Building building, string field, string? value)
    {
        if (building.modData.ContainsKey($"{_modID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        WriteData(building, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the building's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void AppendData(Building building, string field, string value, string separator = ",")
    {
        var current = ReadData(building, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {building.GetType().Name}'s {field} already contained {value}.");

        WriteData(building, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {building.GetType().Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the building's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(Building building, string field, T amount) where T : struct
    {
        building.modData.Increment($"{_modID}/{field}", amount);
        Log.V($"[ModData]: Incremented {building.GetType().Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the building's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(Building building, string field) where T : struct
    {
        building.modData.Increment($"{_modID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {building.GetType().Name}'s {field} by 1.");
    }

    #endregion building io

    #region character io

    /// <summary>Read a string from the character's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(Character character, string field, string defaultValue = "") =>
        character.modData.Read($"{_modID}/{field}", defaultValue);

    /// <summary>Read a field from the character's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataAs<T>(Character character, string field, T defaultValue = default) where T : struct =>
        character.modData.ReadAs($"{_modID}/{field}", defaultValue);

    /// <summary>Write to a field in the character's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(Character character, string field, string? value)
    {
        character.modData.Write($"{_modID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {character.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {character.Name}'s {field}.");
    }

    /// <summary>Write to a field in the character's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(Character character, string field, string? value)
    {
        if (character.modData.ContainsKey($"{_modID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        WriteData(character, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the character's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void AppendData(Character character, string field, string value, string separator = ",")
    {
        var current = ReadData(character, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {character.Name}'s {field} already contained {value}.");

        WriteData(character, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {character.Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the character's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(Character character, string field, T amount) where T : struct
    {
        character.modData.Increment($"{_modID}/{field}", amount);
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the character's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(Character character, string field) where T : struct
    {
        character.modData.Increment($"{_modID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by 1.");
    }

    #endregion character io

    #region game location io

    /// <summary>Read a string from the game location's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(GameLocation location, string field, string defaultValue = "") =>
        location.modData.Read($"{_modID}/{field}", defaultValue);

    /// <summary>Read a field from the game location's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataAs<T>(GameLocation location, string field, T defaultValue = default) where T : struct =>
        location.modData.ReadAs($"{_modID}/{field}", defaultValue);

    /// <summary>Write to a field in the game location's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(GameLocation location, string field, string? value)
    {
        location.modData.Write($"{_modID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {location.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {location.Name}'s {field}.");
    }

    /// <summary>Write to a field in the game location's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(GameLocation location, string field, string? value)
    {
        if (location.modData.ContainsKey($"{_modID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        WriteData(location, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the game location's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void AppendData(GameLocation character, string field, string value, string separator = ",")
    {
        var current = ReadData(character, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {character.Name}'s {field} already contained {value}.");

        WriteData(character, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {character.Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the game location's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(GameLocation character, string field, T amount) where T : struct
    {
        character.modData.Increment($"{_modID}/{field}", amount);
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the game location's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(GameLocation character, string field) where T : struct
    {
        character.modData.Increment($"{_modID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by 1.");
    }

    #endregion game location io

    #region item io

    /// <summary>Read a string from the item's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(Item item, string field, string defaultValue = "") =>
        item.modData.Read($"{_modID}/{field}", defaultValue);

    /// <summary>Read a field from the item's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataAs<T>(Item item, string field, T defaultValue = default) where T : struct =>
        item.modData.ReadAs($"{_modID}/{field}", defaultValue);

    /// <summary>Write to a field in the item's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(Item item, string field, string? value)
    {
        item.modData.Write($"{_modID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {item.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {item.Name}'s {field}.");
    }

    /// <summary>Write to a field in the item's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(Item item, string field, string? value)
    {
        if (item.modData.ContainsKey($"{_modID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        WriteData(item, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the item's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void AppendData(Item item, string field, string value, string separator = ",")
    {
        var current = ReadData(item, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {item.Name}'s {field} already contained {value}.");

        WriteData(item, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {item.Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the item's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(Item item, string field, T amount) where T : struct
    {
        item.modData.Increment($"{_modID}/{field}", amount);
        Log.V($"[ModData]: Incremented {item.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the item's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(Item item, string field) where T : struct
    {
        item.modData.Increment($"{_modID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {item.Name}'s {field} by 1.");
    }

    #endregion item io

    #region tree io

    /// <summary>Read a string from the terrain feature's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(TerrainFeature feature, string field, string defaultValue = "") =>
        feature.modData.Read($"{_modID}/{field}", defaultValue);

    /// <summary>Read a field from the terrain feature's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataAs<T>(TerrainFeature feature, string field, T defaultValue = default) where T : struct =>
        feature.modData.ReadAs($"{_modID}/{field}", defaultValue);

    /// <summary>Write to a field in the terrain feature's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(TerrainFeature feature, string field, string? value)
    {
        feature.modData.Write($"{_modID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {feature.GetType().Name}'s {field}."
            : $"[ModData]: Wrote {value} to {feature.GetType().Name}'s {field}.");
    }

    /// <summary>Write to a field in the terrain feature's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(TerrainFeature feature, string field, string? value)
    {
        if (feature.modData.ContainsKey($"{_modID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        WriteData(feature, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the terrain feature's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void AppendData(TerrainFeature feature, string field, string value, string separator = ",")
    {
        var current = ReadData(feature, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {feature.GetType().Name}'s {field} already contained {value}.");

        WriteData(feature, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {feature.GetType().Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the terrain feature's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(TerrainFeature feature, string field, T amount) where T : struct
    {
        feature.modData.Increment($"{_modID}/{field}", amount);
        Log.V($"[ModData]: Incremented {feature.GetType().Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the terrain feature's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(TerrainFeature feature, string field) where T : struct
    {
        feature.modData.Increment($"{_modID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {feature.GetType().Name}'s {field} by 1.");
    }

    #endregion tree io
}