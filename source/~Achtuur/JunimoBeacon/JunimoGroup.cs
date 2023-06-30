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
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace JunimoBeacon;

/// <summary>
/// Group that consists of all adjacent <see cref="JunimoHut"/> and <see cref="JunimoBeacon"/>
/// </summary>
internal class JunimoGroup
{
    private TrailParticle BeaconAddParticle;
    public JunimoHut Hut { get; set; }
    List<JunimoBeacon> Beacons { get; set; }

    /// <summary>
    /// Number of connected beacons to the hut in this group
    /// </summary>
    public int NumConnectedBeacons => Beacons.Count;

    public JunimoGroup(JunimoHut hut)
    {
        this.Hut = hut;
        this.Beacons = JunimoBeacon.GetBeaconsOnFarm()
            .Where(beacon => beacon.IsInRangeOfJunimoHut(this.Hut))
            .ToList();


        Vector2 targetPosition = new Vector2(this.Hut.tileX.Value + 1, this.Hut.tileY.Value) * Game1.tileSize;
        BeaconAddParticle = new TrailParticle(
            Vector2.Zero,
            targetPosition, // aim for one tile above the middle of the hut
            10,
            Color.Green,
            Vector2.One * 10f
        );

        BeaconAddParticle.SetTrailColors(new List<Color>() { Color.Red, Color.Green, Color.Blue });
    }


    public void TryAddBeacon(JunimoBeacon beacon)
    {
        if (this.IsInRange(beacon.Tile))
        {
            this.Beacons.Add(beacon);

            Hut.updateWhenFarmNotCurrentLocation(Game1.currentGameTime);

            // play particle effect
            TrailParticle particle = (TrailParticle)BeaconAddParticle.Clone();
            particle.SetTrailColors(new List<Color>() { Color.Red, Color.Green, Color.Blue });
            particle.SetInitialPosition(beacon.Tile * Game1.tileSize);
            particle.Start();
        }
    }

    public void TryRemoveBeacon(SObject beacon_object)
    {
        foreach (JunimoBeacon beacon in this.Beacons)
        {
            if (beacon.SObjectEquals(beacon_object))
                this.Beacons.Remove(beacon);
        }
    }

    public bool IsInRange(Vector2 beacon_position)
    {
        Rectangle hut_rect = this.Hut.GetTileRangeAsRect();
        Rectangle beacon_rect = JunimoBeacon.GetBeaconRangeAsRect(beacon_position, expand: 1);

        return hut_rect.Intersects(beacon_rect) ||
            this.Beacons.Any(beacon => beacon.GetBeaconRangeAsRect(expand: 0).Intersects(beacon_rect));
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
}
