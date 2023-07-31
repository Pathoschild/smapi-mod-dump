/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework;
using AchtuurCore.Utility;
using JunimoBeacon.Extensions;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace JunimoBeacon;

/// <summary>
/// Group that consists of all adjacent <see cref="JunimoHut"/> and <see cref="JunimoBeacon"/>
/// </summary>
internal class JunimoGroup
{
    public const int MaxJunimoPerGroup = 5;

    private TrailParticle BeaconAddParticle;
    public JunimoHut Hut { get; set; }
    public Color Color { get; set; }
    public List<JunimoBeacon> Beacons { get; set; }

    /// <summary>
    /// Number of connected beacons to the hut in this group
    /// </summary>
    public int NumConnectedBeacons => Beacons.Count;

    public JunimoGroup(JunimoHut hut)
    {
        this.Hut = hut;

        this.AddInRangeBeacons();

        this.Color = ColorHelper.GetUniqueColor(alpha: 230);

        Vector2 targetPosition = new Vector2(this.Hut.tileX.Value + 1, this.Hut.tileY.Value) * Game1.tileSize;
        BeaconAddParticle = new TrailParticle(
            Vector2.Zero,
            targetPosition, // aim for one tile above the middle of the hut
            10,
            this.Color,
            Vector2.One * 10f
        );

        BeaconAddParticle.SetTrailColors(new List<Color>() { Color.Red, Color.Green, Color.Blue });
    }

    /// <summary>
    /// Try and add a beacon if it is in range
    /// </summary>
    /// <param name="beacon"></param>
    public void TryAddBeacon(JunimoBeacon beacon)
    {
        if (this.IsInRange(beacon.Tile))
        {
            this.Beacons.Add(beacon);

            AddInRangeBeacons();

            // play particle effect
            TrailParticle particle = (TrailParticle)BeaconAddParticle.Clone();
            particle.SetTrailColors(new List<Color>() { Color.Red, Color.Green, Color.Blue });
            particle.SetInitialPosition(beacon.Tile * Game1.tileSize);
            particle.Start();
        }
    }

    public void TryAddBeacon(SObject beacon_object)
    {
        if (TypeChecker.isType<JunimoBeacon>(beacon_object))
        {
            JunimoBeacon beacon = new JunimoBeacon(beacon_object);
            this.TryAddBeacon(beacon);
        }
    }

    /// <summary>
    /// Try and remove beacon with <paramref name="beacon_object"/> as its underlying object
    /// </summary>
    /// <param name="beacon_object"></param>
    public void TryRemoveBeacon(SObject beacon_object)
    {
        this.Beacons = this.Beacons.Where(b => !b.SObjectEquals(beacon_object)).ToList();

        // Remove other beacons that would be out of range
        RemoveOutOfRangeBeacons();
    }

    public bool IsInRange(Vector2 beacon_position)
    {
        Rectangle hut_rect = this.Hut.GetTileRangeAsRect();
        Rectangle beacon_rect = JunimoBeacon.GetBeaconRangeAsRect(beacon_position, expand: 1);

        return hut_rect.Intersects(beacon_rect) || // beacon_position in range of hut
            this.Beacons.Any(beacon =>
                beacon.Tile != beacon_position // not same beacon
                && beacon.GetBeaconRangeAsRect(expand: 0).Intersects(beacon_rect) // beacon_position in range
            );
    }

    public void AddInRangeBeacons()
    {
        // Add beacons directly in range of hut
        this.Beacons = JunimoBeacon.GetBeaconsOnFarm()
            .Where(beacon => beacon.IsInRangeOfJunimoHut(this.Hut))
            .ToList();

        // Add beacons that become in range due to previously added beacons
        int old_count;
        int current_count;
        do
        {
            old_count = this.Beacons.Count;

            IEnumerable<JunimoBeacon> newBeacons = JunimoBeacon.GetBeaconsOnFarm()
                .Where(beacon => !this.ContainsBeacon(beacon))
                .Where(beacon => this.IsInRange(beacon.Tile));

            this.Beacons.AddRange(newBeacons);
            current_count = this.Beacons.Count;

        } while (old_count != current_count);
    }

    /// <summary>
    /// Remove beacons from this group that are out of range
    /// </summary>
    public void RemoveOutOfRangeBeacons()
    {
        // If all beacons out of range __of hut__, remove them all
        // If two beacons are still in range of each other, but neither are connected to the hut
        // They would still count as being in range, if only the latter check is done
        if (this.Beacons.All(beacon => !beacon.IsInRangeOfJunimoHut(this.Hut)))
            this.Beacons = new List<JunimoBeacon>();
        // Else only remove beacons that are out of range of group
        else
            this.Beacons = this.Beacons.Where(beacon => this.IsInRange(beacon.Tile)).ToList();
    }

    /// <summary>
    /// Get all tiles this group encompasses, includes <see cref="JunimoHut"/> range and <see cref="JunimoBeacon"/>s ranges
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Vector2> GetTiles()
    {
        foreach (Vector2 tile in this.Hut.GetTileRangeAsRect().GetTiles())
            yield return tile;

        foreach (Vector2 tile in this.GetBeaconTiles())
            yield return tile;

    }

    public IEnumerable<Vector2> GetBeaconTiles()
    {
        foreach (JunimoBeacon beacon in this.Beacons)
        {
            yield return beacon.Tile;

            foreach (Vector2 tile in beacon.GetTilesInRange())
                yield return tile;
        }
    }

    public bool ContainsBeacon(JunimoBeacon beacon)
    {
        return this.Beacons.Any(b => beacon.SObjectEquals(b));
    }

    /// <summary>
    /// Returns maximum number of Junimos this group should have, which is the smallest of <see cref="MaxJunimoPerGroup"/> and <c>3 + <see cref="NumConnectedBeacons"/></c>
    /// </summary>
    /// <returns></returns>
    public int GetMaximumJunimoCount()
    {
        return Math.Max(MaxJunimoPerGroup, 3 + this.NumConnectedBeacons);
    }
}
