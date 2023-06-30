/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Utility;
using JunimoBeacon.Extensions;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace JunimoBeacon;

internal class JunimoBeacon
{

    public static readonly string ItemName = "Junimo Beacon";

    public static int? ID = null;

    /// <summary>
    /// Actual object that this beacon represents
    /// </summary>
    private readonly SObject _object;

    /// <summary>
    /// Tile this beacon has been place on
    /// </summary>
    public Vector2 Tile { get; private set; }

    public JunimoBeacon(SObject statueObject)
    {
        this._object = statueObject;
        Tile = statueObject.TileLocation;
    }

    public IEnumerable<Vector2> GetTilesInRange()
    {
        foreach (Vector2 tile in GetBeaconRangeTiles(this.Tile, expand: 0))
            yield return tile;
    }

    public Rectangle GetBeaconRangeAsRect(int expand = 1)
    {
        return GetBeaconRangeAsRect(new Vector2(Tile.X, Tile.Y), expand);
    }

    public static Rectangle GetBeaconRangeAsRect(Vector2 beacon_pos, int expand = 1)
    {
        int range = ModEntry.Instance.Config.BeaconRange;
        return new Rectangle(
            (int)beacon_pos.X - range - expand,
            (int)beacon_pos.Y - range - expand,
            2 * range + 2 * expand + 1,
            2 * range + 2 * expand + 1
        );
    }

    public static IEnumerable<Vector2> GetBeaconRangeTiles(Vector2 beacon_pos, int expand = 1)
    {
        foreach (Vector2 tile in GetBeaconRangeAsRect(beacon_pos, expand).GetTiles())
            yield return tile;
    }


    /// <summary>
    /// Returns whether this <see cref="JunimoBeacon"/> is in range of <paramref name="hut"/>
    /// </summary>
    /// <param name="hut"></param>
    /// <returns></returns>
    public bool IsInRangeOfJunimoHut(JunimoHut hut)
    {

        Rectangle hutRange = hut.GetTileRangeAsRect();


        // Create rectangle around beacon that extends to Range + 1
        // + 1 is done so the range of the beacon and hut only have to touch, and not intersect
        Rectangle beaconRange = GetBeaconRangeAsRect(expand: 1);

        return hutRange.Intersects(beaconRange);
    }

    public static IEnumerable<JunimoBeacon> GetBeaconsOnFarm()
    {
        Farm farm = Game1.getFarm();

        foreach (SObject sobject in farm.objects.Values)
        {
            if (sobject.ParentSheetIndex == JunimoBeacon.ID)
                yield return new JunimoBeacon(sobject);
        }
    }

    internal bool SObjectEquals(SObject beacon_object)
    {
        return beacon_object == this._object;
    }
}
