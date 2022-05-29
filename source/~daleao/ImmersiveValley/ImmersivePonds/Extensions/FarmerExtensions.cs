/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Ponds.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

using Common.Extensions;
using Common.Extensions.Stardew;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Get the tile distance between the farmer and any building in the <see cref="GameLocation"/>.</summary>
    /// <param name="building">The target building.</param>
    public static double DistanceToBuilding(this Farmer farmer, Building building)
    {
        return (farmer.getTileLocation() - new Vector2(building.tileX.Value, building.tileY.Value)).Length();
    }

    /// <summary>Find the closest building to this instance in the current <see cref="GameLocation"/>. </summary>
    /// <typeparam name="T">A subtype of <see cref="Building"/>.</typeparam>
    /// <param name="distanceToClosestBuilding">The distance to the returned building, or <see cref="double.MaxValue"/> if none was found.</param>
    /// <param name="candidates">The candidate buildings, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates (ignore candidates for which the predicate returns <c>True</c>.</param>
    [CanBeNull]
    public static T GetClosestBuilding<T>(this Farmer farmer, out double distanceToClosestBuilding,
        IEnumerable<T> candidates = null, Func<T, bool> predicate = null) where T : Building
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? Game1.getFarm().buildings.OfType<T>().Where(c => predicate(c)).ToArray();
        distanceToClosestBuilding = double.MaxValue;
        if (candidatesArr.Length == 0) return null;

        if (candidatesArr.Length == 1)
        {
            distanceToClosestBuilding = farmer.DistanceToBuilding(candidatesArr[0]);
            return candidatesArr[0];
        }

        T closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = farmer.DistanceToBuilding(candidate);
            if (distanceToThisCandidate >= distanceToClosestBuilding) continue;

            closest = candidate;
            distanceToClosestBuilding = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Read a string from the <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(this Farmer farmer, DataField field, string defaultValue = "")
    {
        return Game1.MasterPlayer.modData.Read($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Read a field from the <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataAs<T>(this Farmer farmer, DataField field, T defaultValue = default)
    {
        return Game1.MasterPlayer.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(this Farmer farmer, DataField field, string value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Write/{field}",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            return;
        }

        Game1.player.modData.Write($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.D($"[ModData]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(this Farmer farmer, DataField field, string value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Write/{field}",
                new[] { ModEntry.Manifest.UniqueID },
                new[] { Game1.MasterPlayer.UniqueMultiplayerID }); // request the main player
        else Game1.player.WriteData(field, value);

        return false;
    }

    /// <summary>Append a string to an existing string field in the <see cref="ModDataDictionary"/>, or initialize to the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void AppendData(this Farmer farmer, DataField field, string value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Append/{field}",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            return;
        }

        var current = Game1.player.ReadData(field);
        Game1.player.WriteData(field, string.IsNullOrEmpty(current) ? value : current + ',' + value);
        Log.D($"[ModData]: Appended {farmer.Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(this Farmer farmer, DataField field, T amount)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(amount, $"RequestUpdateData/Increment/{field}",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            return;
        }

        Game1.player.modData.Increment($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(this Farmer farmer, DataField field)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(1, $"RequestUpdateData/Increment/{field}",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            return;
        }

        Game1.player.modData.Increment($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by 1.");
    }
}