/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Extensions.Stardew;

#region using directives

using Microsoft.Xna.Framework;
using ModData;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="TerrainFeatures"/> class.</summary>
public static class TerrainFeatureExtensions
{
    /// <summary>Get the tile distance between the terrain feature and a target <see cref="Building"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="@object">The target object.</param>
    public static double DistanceTo(this TerrainFeature terrain, Building building) =>
        (terrain.currentTileLocation - new Vector2(building.tileX.Value, building.tileY.Value)).Length();

    /// <summary>Get the tile distance between the terrain feature and a target <see cref="Character"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="character">The target character.</param>
    public static double DistanceTo(this TerrainFeature terrain, Character character) =>
        (terrain.currentTileLocation - character.getTileLocation()).Length();

    /// <summary>Get the tile distance between the terrain feature and a target <see cref="SObject"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="@object">The target object.</param>
    public static double DistanceTo(this TerrainFeature terrain, SObject @object) =>
        (terrain.currentTileLocation - @object.TileLocation).Length();

    /// <summary>Get the tile distance between this and another terrain feature in the same <see cref="GameLocation"/>.</summary>
    /// <param name="terrain">The target terrain feature.</param>
    public static double DistanceTo(this TerrainFeature terrain, TerrainFeature other) =>
        (terrain.currentTileLocation - other.currentTileLocation).Length();

    /// <summary>Find the closest <see cref="Building"/> to this terrain feature in the current <see cref="GameLocation"/> of the specified type.</summary>
    /// <param name="candidates">The candidate buildings, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestBuilding<T>(this TerrainFeature terrain, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : Building
    {
        if (terrain.currentLocation is not BuildableGameLocation buildable) return null;

        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? buildable.buildings.OfType<T>().Where(t => predicate(t)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        T? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = terrain.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="Farmer"/> to this terrain feature in the current <see cref="GameLocation"/>.</summary>
    /// <param name="candidates">The candidate farmers, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static Farmer? GetClosestFarmer(this TerrainFeature terrain, IEnumerable<Farmer>? candidates = null,
        Func<Farmer, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? terrain.currentLocation.farmers.Where(f => predicate(f)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        Farmer? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = terrain.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="SObject"/> to this terrain feature in the current <see cref="GameLocation"/> of the specified type.</summary>
    /// <param name="candidates">The candidate objects, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestObject<T>(this TerrainFeature terrain, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : SObject
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            terrain.currentLocation.Objects.Values.OfType<T>().Where(o => predicate(o)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        T? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = terrain.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="NPC"/> to this terrain feature in the current <see cref="GameLocation"/> of the specified type.</summary>
    /// <param name="candidates">The candidate NPCs, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestNPC<T>(this TerrainFeature terrain, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : NPC
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            terrain.currentLocation.characters.OfType<T>().Where(t => predicate(t)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        T? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = terrain.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest terrain feature to this one in the current <see cref="GameLocation"/> of the specified type.</summary>
    /// <param name="candidates">The candidate terrain features, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestTerrainFeature<T>(this TerrainFeature terrain, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : TerrainFeature
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? terrain.currentLocation.terrainFeatures.Values.OfType<T>()
            .Where(t => predicate(t)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        T? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = terrain.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <inheritdoc cref="ModDataIO.Read"/>
    public static string Read(this TerrainFeature feature, string field, string defaultValue = "", string modId = "") =>
        ModDataIO.Read(feature, field, defaultValue, modId);

    /// <inheritdoc cref="ModDataIO.Read{T}"/>
    public static T Read<T>(this TerrainFeature feature, string field, T defaultValue = default, string modId = "") where T : struct =>
        ModDataIO.Read(feature, field, defaultValue, modId);

    /// <inheritdoc cref="ModDataIO.Write"/>
    public static void Write(this TerrainFeature building, string field, string? value) =>
        ModDataIO.Write(building, field, value);

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists"/>
    public static void WriteIfNotExists(this TerrainFeature feature, string field, string? value) =>
        ModDataIO.WriteIfNotExists(feature, field, value);

    /// <inheritdoc cref="ModDataIO.Append"/>
    public static void Append(this TerrainFeature feature, string field, string value, string separator = ",") =>
        ModDataIO.Append(feature, field, value, separator);

    /// <inheritdoc cref="ModDataIO.Increment{T}"/>
    public static void Increment<T>(this TerrainFeature feature, string field, T amount) where T : struct =>
        ModDataIO.Increment(feature, field, amount);

    /// <summary>Increment the value of a numeric field in the building's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment(this TerrainFeature feature, string field) =>
        ModDataIO.Increment(feature, field, 1);
}