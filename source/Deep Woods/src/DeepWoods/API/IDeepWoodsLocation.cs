/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace DeepWoodsMod.API
{
    public interface IDeepWoodsLocation
    {
        // returns the seed used to generate the map
        // this seed is guaranteed to be the same on server and all clients
        // use this to seed a new Random when you need pseudorandom stuff synchronized between client and server
        // (hint: the Name for each DeepWoodsLocation is always "DeepWoods_"+Seed, except for the root level ("DeepWoods"))
        int Seed { get; }
        // returns the parent DeepWoods (null if no parent exists)
        IDeepWoodsLocation ParentDeepWoods { get; }
        // the location of the entrance in tile indices (x, y)
        Location EnterLocation { get; set; }
        // the size of the map in number of tiles (width x height)
        Tuple<int, int> MapSize { get; set; }
        // if true, this level is a clearing
        bool IsClearing { get; set; }
        // if true, this level will never get lost - this affects parent levels for obvious reasons as well!
        bool CanGetLost { get; set; }
        // from which side this level is entered (0 = left, 1 = top, 2 = right, 3 = bottom)
        int EnterSide { get; }
        // the level of this location (1 = root level, higher = deeper in the DeepWoods)
        int Level { get; }
        // true if this location has no connection to the root level anymore
        bool IsLost { get; }
        // value between 0.0 and 1.0, can be used together with Game1.dailyLuck to modify difficulty
        double LuckLevel { get; }
        // value between 0 and 10, can be used to modify difficulty
        int CombatLevel { get; }
        // returns all exits from this level
        IEnumerable<IDeepWoodsExit> Exits { get; }
        // returns all resourceclumps in this level
        ICollection<ResourceClump> ResourceClumps { get; }
        // returns the baubles in this level
        ICollection<Vector2> Baubles { get; }
        // returns the leaves in this level
        ICollection<WeatherDebris> WeatherDebris { get; }
        // true if this level has a custom map provided by a mod
        bool IsCustomMap { get; }
    }
}
