/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Layers;
using xTile.Tiles;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Buildings;
using System.Drawing;
using StardewValley.Objects;
using System.Runtime.Intrinsics.X86;
using StardewValley.GameData.Locations;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;
using System.Threading;
using System.Xml.Serialization;
using StardewDruid.Character;
using StardewDruid.Data;
using xTile.Dimensions;

namespace StardewDruid.Location
{

    public class Grove : GameLocation
    {

        //public List<WarpBack> warpBacks = new();

        public List<Location.WarpTile> warpSets = new();

        public List<Location.LocationTile> locationTiles = new();

        public List<Location.LocationTile> herbalTiles = new();

        public Dictionary<Vector2, CharacterHandle.characters> dialogueTiles = new();

        public Grove() { }

        public Grove(string Name)
            : base("Maps\\Shed", Name) 
        {

        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            foreach (LocationTile tile in locationTiles)
            {

                tile.Draw(b);

            }

            foreach (LocationTile tile in herbalTiles)
            {

                tile.Draw(b);

            }

        }

        

        public override void OnMapLoad(xTile.Map map)
        {

            /*
             Ground
            306
            351
              
            Ground line

            357|405|407
            381|XXX|379
            382|355|332

            406|xxx|404
            356|xxx|354

            C1 = 406
            C2 = 357
            C3 = 404
            C4 = 407
            C5 = 356
            C6 = 382
            C7 = 354
            C8 = 332

            E1 = 405
            E2 = 381
            E3 = 379
            E4 = 355

            Dark Ground
            380
             
             Tree line
                
            940 | 941 | 942 | 943 | 944 | 945
            965 | 966 | 967 | 968 | 969 | 970
            990 | 991 | 992 | 993 | 994 | 995
            1015 | 1016 | 1017 | 1018 | 1019 | 1020
            1040 | 1041 | 1042 | 1043 | 1044 | 1045
            1065 | 1066 | 1067 | 1068 | 1069 | 1070
            
            1019, 993, 967 invert corner

            Cave

            466|467|468|1633|1634|1635|467|468|467|468
            491|492|493|1658|1659|1660|492|493|492|493
            516|517|518|1683|XXXX|1685|517|518|517|518
            541|542|543|1708|XXXX|1710|542|543|542|543

            Paving
            947|948|949
            972|973|974
            XXX|998|999

            973, 999 are whole

            */

            xTile.Dimensions.Size tileSize = map.GetLayer("Back").TileSize;

            xTile.Map newMap = new(map.Id);

            Layer back = new("Back", newMap, new(56, 30), tileSize);

            newMap.AddLayer(back);

            Layer buildings = new("Buildings", newMap, new(56, 30), tileSize);

            newMap.AddLayer(buildings);

            Layer front = new("Front", newMap, new(56, 30), tileSize);

            newMap.AddLayer(front);

            Layer alwaysfront = new("AlwaysFront", newMap, new(56, 30), tileSize);

            newMap.AddLayer(alwaysfront);

            TileSheet outdoor = new(LocationData.druid_grove_name+"_outdoors", newMap, "Maps\\spring_outdoorsTileSheet", new(25,79), tileSize);
            
            newMap.AddTileSheet(outdoor); //map.TileSheets[1].ImageSource

            IsOutdoors = true;

            ignoreOutdoorLighting.Set(false);

            // -----------------------------------------------------
            // normal ground


            int G1 = 351;

            int E5 = 380;
            int C1 = 406;
            int C2 = 357;
            int C3 = 404;
            int C4 = 407;
            int C5 = 356;
            int C6 = 382;
            int C7 = 354;
            int C8 = 332;

            int E1 = 405;
            int E2 = 381;
            int E3 = 379;
            int E4 = 355;

            //int ENT = 265;
            int ENT = 154;
            int ENT2 = 154;

            int PAV2 = 948;
            int PAV4 = 972;
            int PAV5 = 973;
            int PAV6 = 974;
            int PAV8 = 998;
            int PAV9 = 999;

            Dictionary<int, List<List<int>>> groundCodes = new()
            {

                [3] = new() { new() { 16, E5 }, new() { 17, E5 }, new() { 18, E5 }, new() { 19, E5 }, new() { 20, E5 }, new() { 21, E5 }, new() { 22, E5 }, new() { 23, E5 }, new() { 24, E5 }, new() { 25, E5 }, new() { 26, E5 }, },
                [4] = new() { new() { 15, E5 }, new() { 16, C2 }, new() { 17, E1 }, new() { 18, E1 }, new() { 19, E1 }, new() { 20, E1 }, new() { 21, E1 }, new() { 22, E1 }, new() { 23, E1 }, new() { 24, E1 }, new() { 25, E1 }, new() { 26, C4 }, new() { 27, E5 }, },
                [5] = new() { new() { 14, E5 }, new() { 15, E5 }, new() { 16, E2 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, E3 }, new() { 27, E5 }, new() { 28, E5 }, },
                [6] = new() { new() { 13, E5 }, new() { 14, E5 }, new() { 15, C2 }, new() { 16, C1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, C3 }, new() { 27, C4 }, new() { 28, E5 }, new() { 29, E5 }, },
                [7] = new() { new() { 12, E5 }, new() { 13, C2 }, new() { 14, E1 }, new() { 15, C1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, C3 }, new() { 28, E1 }, new() { 29, C4 }, new() { 30, E5 }, new() { 39, ENT2 }, },
                [8] = new() { new() { 11, E5 }, new() { 12, E5 }, new() { 13, E2 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, E3 }, new() { 30, E5 }, new() { 31, E5 }, new() { 39, ENT }, },
                [9] = new() { new() { 10, E5 }, new() { 11, E5 }, new() { 12, C2 }, new() { 13, C1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, C3 }, new() { 30, C4 }, new() { 31, E5 }, new() { 32, E5 }, new() { 33, E5 }, new() { 34, E5 }, new() { 35, E5 }, new() { 36, E5 }, new() { 37, E5 }, new() { 38, E2 }, new() { 39, PAV9 }, new() { 40, E3 }, new() { 41, E5 }, new() { 42, E5 }, new() { 43, E5 }, new() { 44, E5 }, },
                [10] = new() { new() { 9, E5 }, new() { 10, C2 }, new() { 11, E1 }, new() { 12, C1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, G1 }, new() { 30, E3 }, new() { 31, E5 }, new() { 32, E5 }, new() { 33, E5 }, new() { 34, E5 }, new() { 35, C2 }, new() { 36, E1 }, new() { 37, E1 }, new() { 38, C1 }, new() { 39, PAV5 }, new() { 40, C3 }, new() { 41, E1 }, new() { 42, E1 }, new() { 43, C4 }, new() { 44, E5 }, },
                [11] = new() { new() { 9, E5 }, new() { 10, E2 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, C7 }, new() { 30, C8 }, new() { 31, E5 }, new() { 32, E5 }, new() { 33, E5 }, new() { 34, E5 }, new() { 35, E2 }, new() { 36, G1 }, new() { 37, G1 }, new() { 38, PAV4 }, new() { 39, PAV5 }, new() { 40, PAV6 }, new() { 41, G1 }, new() { 42, G1 }, new() { 43, E3 }, new() { 44, E5 }, },
                [12] = new() { new() { 9, E5 }, new() { 10, E2 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, E3 }, new() { 30, E5 }, new() { 34, E5 }, new() { 35, E2 }, new() { 36, G1 }, new() { 37, G1 }, new() { 38, G1 }, new() { 39, PAV8 }, new() { 40, G1 }, new() { 41, G1 }, new() { 42, G1 }, new() { 43, E3 }, new() { 44, E5 }, },
                [13] = new() { new() { 9, E5 }, new() { 10, E2 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, E3 }, new() { 30, E5 }, new() { 34, E5 }, new() { 35, E2 }, new() { 36, G1 }, new() { 37, G1 }, new() { 38, G1 }, new() { 39, G1 }, new() { 40, G1 }, new() { 41, G1 }, new() { 42, G1 }, new() { 43, E3 }, new() { 44, E5 }, },
                [14] = new() { new() { 9, E5 }, new() { 10, E2 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, E3 }, new() { 30, E5 }, new() { 34, E5 }, new() { 35, E2 }, new() { 36, G1 }, new() { 37, G1 }, new() { 38, G1 }, new() { 39, G1 }, new() { 40, G1 }, new() { 41, G1 }, new() { 42, G1 }, new() { 43, E3 }, new() { 44, E5 }, },
                [15] = new() { new() { 9, E5 }, new() { 10, E2 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, E3 }, new() { 30, E5 }, new() { 34, E5 }, new() { 35, E2 }, new() { 36, G1 }, new() { 37, G1 }, new() { 38, G1 }, new() { 39, G1 }, new() { 40, G1 }, new() { 41, G1 }, new() { 42, G1 }, new() { 43, E3 }, new() { 44, E5 }, },
                [16] = new() { new() { 9, E5 }, new() { 10, E2 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, E3 }, new() { 30, E5 }, new() { 34, E5 }, new() { 35, E2 }, new() { 36, G1 }, new() { 37, G1 }, new() { 38, G1 }, new() { 39, G1 }, new() { 40, G1 }, new() { 41, G1 }, new() { 42, G1 }, new() { 43, E3 }, new() { 44, E5 }, },
                [17] = new() { new() { 9, E5 }, new() { 10, E2 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, C3 }, new() { 30, C4 }, new() { 31, E5 }, new() { 32, E5 }, new() { 33, E5 }, new() { 34, E5 }, new() { 35, E2 }, new() { 36, G1 }, new() { 37, G1 }, new() { 38, G1 }, new() { 39, G1 }, new() { 40, G1 }, new() { 41, G1 }, new() { 42, G1 }, new() { 43, E3 }, new() { 44, E5 }, },
                [18] = new() { new() { 9, E5 }, new() { 10, C6 }, new() { 11, E4 }, new() { 12, C5 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, G1 }, new() { 30, E3 }, new() { 31, E5 }, new() { 32, E5 }, new() { 33, E5 }, new() { 34, E5 }, new() { 35, E2 }, new() { 36, G1 }, new() { 37, G1 }, new() { 38, G1 }, new() { 39, PAV2 }, new() { 40, G1 }, new() { 41, G1 }, new() { 42, G1 }, new() { 43, E3 }, new() { 44, E5 }, },
                [19] = new() { new() { 10, E5 }, new() { 11, E5 }, new() { 12, C6 }, new() { 13, C5 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, C7 }, new() { 30, C8 }, new() { 31, E5 }, new() { 32, E5 }, new() { 33, E5 }, new() { 34, E5 }, new() { 35, E2 }, new() { 36, G1 }, new() { 37, G1 }, new() { 38, PAV4 }, new() { 39, PAV5 }, new() { 40, PAV6 }, new() { 41, G1 }, new() { 42, G1 }, new() { 43, E3 }, new() { 44, E5 }, },
                [20] = new() { new() { 11, E5 }, new() { 12, E5 }, new() { 13, E2 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, E3 }, new() { 30, E5 }, new() { 31, E5 }, new() { 34, E5 }, new() { 35, C6 }, new() { 36, E4 }, new() { 37, E4 }, new() { 38, C5 }, new() { 39, PAV5 }, new() { 40, C7 }, new() { 41, E4 }, new() { 42, E4 }, new() { 43, C8 }, new() { 44, E5 }, },
                [21] = new() { new() { 12, E5 }, new() { 13, C6 }, new() { 14, E4 }, new() { 15, C5 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, G1 }, new() { 27, C7 }, new() { 28, E4 }, new() { 29, C8 }, new() { 30, E5 }, new() { 34, E5 }, new() { 35, E5 }, new() { 36, E5 }, new() { 37, E5 }, new() { 38, E2 }, new() { 39, PAV5 }, new() { 40, E3 }, new() { 41, E5 }, new() { 42, E5 }, new() { 43, E5 }, new() { 44, E5 }, },
                [22] = new() { new() { 12, E5 }, new() { 13, E5 }, new() { 14, E5 }, new() { 15, C6 }, new() { 16, C5 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, C7 }, new() { 27, C8 }, new() { 28, E5 }, new() { 29, E5 }, new() { 37, E5 }, new() { 38, E2 }, new() { 39, PAV5 }, new() { 40, E3 }, new() { 41, E5 }, },
                [23] = new() { new() { 14, E5 }, new() { 15, E5 }, new() { 16, E2 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, G1 }, new() { 21, G1 }, new() { 22, G1 }, new() { 23, G1 }, new() { 24, G1 }, new() { 25, G1 }, new() { 26, E3 }, new() { 27, E5 }, new() { 28, E5 }, new() { 37, E5 }, new() { 38, C6 }, new() { 39, PAV5 }, new() { 40, C8 }, new() { 41, E5 }, },
                [24] = new() { new() { 15, E5 }, new() { 16, C6 }, new() { 17, E4 }, new() { 18, E4 }, new() { 19, E4 }, new() { 20, E4 }, new() { 21, E4 }, new() { 22, E4 }, new() { 23, E4 }, new() { 24, E4 }, new() { 25, E4 }, new() { 26, C8 }, new() { 27, E5 }, },
                [25] = new() { new() { 16, E5 }, new() { 17, E5 }, new() { 18, E5 }, new() { 19, E5 }, new() { 20, E5 }, new() { 21, E5 }, new() { 22, E5 }, new() { 23, E5 }, new() { 24, E5 }, new() { 25, E5 }, new() { 26, E5 }, },

            };

            foreach(KeyValuePair<int, List<List<int>>> groundCode in groundCodes)
            {

                foreach(List<int> groundArray in groundCode.Value)
                {

                    back.Tiles[groundArray[0], groundCode.Key] = new StaticTile(back, outdoor, BlendMode.Alpha, groundArray[1]);

                }

            }


            int Grass = 0;

            int Stone = 1;

            Dictionary<int, List<List<int>>> typeCodes = new()
            {
                [3] = new() { new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, },
                [4] = new() { new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, },
                [5] = new() { new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, },
                [6] = new() { new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, },
                [7] = new() { new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Stone }, new() { 18, Stone }, new() { 19, Stone }, new() { 20, Stone }, new() { 21, Stone }, new() { 22, Stone }, new() { 23, Stone }, new() { 24, Stone }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 39, Stone }, },
                [8] = new() { new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Stone }, new() { 18, Stone }, new() { 19, Stone }, new() { 20, Stone }, new() { 21, Stone }, new() { 22, Stone }, new() { 23, Stone }, new() { 24, Stone }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 31, Grass }, new() { 39, Stone }, },
                [9] = new() { new() { 10, Grass }, new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Stone }, new() { 18, Stone }, new() { 19, Stone }, new() { 20, Stone }, new() { 21, Stone }, new() { 22, Stone }, new() { 23, Stone }, new() { 24, Stone }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 31, Grass }, new() { 32, Grass }, new() { 33, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Grass }, new() { 38, Stone }, new() { 39, Stone }, new() { 40, Stone }, new() { 41, Grass }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [10] = new() { new() { 10, Grass }, new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Stone }, new() { 18, Stone }, new() { 19, Stone }, new() { 20, Stone }, new() { 21, Stone }, new() { 22, Stone }, new() { 23, Stone }, new() { 24, Stone }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 31, Grass }, new() { 32, Grass }, new() { 33, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Grass }, new() { 38, Grass }, new() { 39, Stone }, new() { 40, Grass }, new() { 41, Grass }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [11] = new() { new() { 10, Grass }, new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Stone }, new() { 18, Stone }, new() { 19, Stone }, new() { 20, Stone }, new() { 21, Stone }, new() { 22, Stone }, new() { 23, Stone }, new() { 24, Stone }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 31, Grass }, new() { 32, Grass }, new() { 33, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Grass }, new() { 38, Stone }, new() { 39, Stone }, new() { 40, Stone }, new() { 41, Grass }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [12] = new() { new() { 10, Grass }, new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Grass }, new() { 38, Grass }, new() { 39, Stone }, new() { 40, Grass }, new() { 41, Grass }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [13] = new() { new() { 10, Grass }, new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Stone }, new() { 38, Stone }, new() { 39, Stone }, new() { 40, Stone }, new() { 41, Stone }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [14] = new() { new() { 10, Grass }, new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Stone }, new() { 38, Stone }, new() { 39, Stone }, new() { 40, Stone }, new() { 41, Stone }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [15] = new() { new() { 10, Grass }, new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Stone }, new() { 38, Stone }, new() { 39, Stone }, new() { 40, Stone }, new() { 41, Stone }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [16] = new() { new() { 10, Grass }, new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Stone }, new() { 38, Stone }, new() { 39, Stone }, new() { 40, Stone }, new() { 41, Stone }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [17] = new() { new() { 10, Grass }, new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 31, Grass }, new() { 32, Grass }, new() { 33, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Stone }, new() { 38, Stone }, new() { 39, Stone }, new() { 40, Stone }, new() { 41, Stone }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [18] = new() { new() { 10, Grass }, new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 31, Grass }, new() { 32, Grass }, new() { 33, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Grass }, new() { 38, Grass }, new() { 39, Stone }, new() { 40, Grass }, new() { 41, Grass }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [19] = new() { new() { 10, Grass }, new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 31, Grass }, new() { 32, Grass }, new() { 33, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Grass }, new() { 38, Stone }, new() { 39, Stone }, new() { 40, Stone }, new() { 41, Grass }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [20] = new() { new() { 11, Grass }, new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 31, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Grass }, new() { 38, Grass }, new() { 39, Stone }, new() { 40, Grass }, new() { 41, Grass }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [21] = new() { new() { 12, Grass }, new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 30, Grass }, new() { 34, Grass }, new() { 35, Grass }, new() { 36, Grass }, new() { 37, Grass }, new() { 38, Grass }, new() { 39, Stone }, new() { 40, Grass }, new() { 41, Grass }, new() { 42, Grass }, new() { 43, Grass }, new() { 44, Grass }, },
                [22] = new() { new() { 13, Grass }, new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 29, Grass }, new() { 37, Grass }, new() { 38, Grass }, new() { 39, Stone }, new() { 40, Grass }, new() { 41, Grass }, },
                [23] = new() { new() { 14, Grass }, new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, new() { 28, Grass }, new() { 37, Grass }, new() { 38, Grass }, new() { 39, Stone }, new() { 40, Grass }, new() { 41, Grass }, },
                [24] = new() { new() { 15, Grass }, new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, new() { 27, Grass }, },
                [25] = new() { new() { 16, Grass }, new() { 17, Grass }, new() { 18, Grass }, new() { 19, Grass }, new() { 20, Grass }, new() { 21, Grass }, new() { 22, Grass }, new() { 23, Grass }, new() { 24, Grass }, new() { 25, Grass }, new() { 26, Grass }, },

            };

            foreach (KeyValuePair<int, List<List<int>>> typeCode in typeCodes)
            {

                foreach (List<int> typeArray in typeCode.Value)
                {

                    back.Tiles[typeArray[0], typeCode.Key].TileIndexProperties.Add("Type", typeArray[1] == 0 ? "Grass" : "Stone");
                }

            }


            // ------------------------------------------------------
            // cave


            Dictionary<int, List<int>> caveIndices = new()
            {
                [0] = new() { 466, 467, 468, 1633, 1634, 1635, 467, 468, 467, 468, },
                [1] = new() { 491, 492, 493, 1658, 1659, 1660, 492, 493, 492, 493, },
                [2] = new() { 516, 517, 518, 1683, 0, 1685, 517, 518, 517, 518, },
                [3] = new() { 541, 542, 543, 1708, 0, 1710, 542, 543, 542, 543, },

            };
            int bX = 35; int bY = 6;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 4; j++)
                {

                    if (caveIndices[j][i] == 0) { continue; }

                    buildings.Tiles[bX + i, bY + j] = new StaticTile(buildings, outdoor, BlendMode.Alpha, caveIndices[j][i]);

                }

            }

            int B1 = 380;
            //int B2 = 351;

            int TF1 = 9999;
            int TF12 = 9999;

            Dictionary<int, List<List<int>>> borderCodes = new()
            {
                [2] = new() { new() { 15, B1 }, new() { 16, B1 }, new() { 17, B1 }, new() { 18, B1 }, new() { 19, B1 }, new() { 20, B1 }, new() { 21, B1 }, new() { 22, B1 }, new() { 23, B1 }, new() { 24, B1 }, new() { 25, B1 }, new() { 26, B1 }, new() { 27, B1 }, },
                [3] = new() { new() { 14, B1 }, new() { 15, B1 }, new() { 27, B1 }, new() { 28, B1 }, },
                [4] = new() { new() { 13, B1 }, new() { 14, B1 }, new() { 15, TF1 }, new() { 16, TF12 }, new() { 26, TF1 }, new() { 27, TF12 }, new() { 28, B1 }, new() { 29, B1 }, },
                [5] = new() { new() { 12, B1 }, new() { 13, B1 }, new() { 29, B1 }, new() { 30, B1 }, },
                [6] = new() { new() { 11, B1 }, new() { 12, B1 }, new() { 30, B1 }, new() { 31, B1 }, },
                [7] = new() { new() { 10, B1 }, new() { 11, B1 }, new() { 12, TF1 }, new() { 13, TF12 }, new() { 29, TF1 }, new() { 30, TF12 }, new() { 31, B1 }, new() { 32, B1 }, },
                [8] = new() { new() { 9, B1 }, new() { 10, B1 }, new() { 32, B1 }, new() { 33, B1 }, new() { 34, B1 }, new() { 45, B1 }, },
                [9] = new() { new() { 8, B1 }, new() { 9, B1 }, new() { 45, B1 }, },
                [10] = new() { new() { 8, B1 }, new() { 9, TF1 }, new() { 10, TF12 }, new() { 43, TF1 }, new() { 44, TF12 }, new() { 45, B1 }, },
                [11] = new() { new() { 8, B1 }, new() { 45, B1 }, },
                [12] = new() { new() { 8, B1 }, new() { 31, B1 }, new() { 32, B1 }, new() { 33, B1 }, new() { 45, B1 }, },
                [13] = new() { new() { 8, B1 }, new() { 31, B1 }, new() { 33, B1 }, new() { 45, B1 }, },
                [14] = new() { new() { 8, B1 }, new() { 31, B1 }, new() { 33, B1 }, new() { 45, B1 }, },
                [15] = new() { new() { 8, B1 }, new() { 31, B1 }, new() { 33, B1 }, new() { 45, B1 }, },
                [16] = new() { new() { 8, B1 }, new() { 31, B1 }, new() { 32, B1 }, new() { 33, B1 }, new() { 45, B1 }, },
                [17] = new() { new() { 8, B1 }, new() { 45, B1 }, },
                [18] = new() { new() { 8, B1 }, new() { 45, B1 }, },
                [19] = new() { new() { 8, B1 }, new() { 9, B1 }, new() { 45, B1 }, },
                [20] = new() { new() { 9, B1 }, new() { 10, B1 }, new() { 32, B1 }, new() { 33, B1 }, new() { 45, B1 }, },
                [21] = new() { new() { 10, B1 }, new() { 11, B1 }, new() { 31, B1 }, new() { 32, B1 }, new() { 33, B1 }, new() { 45, B1 }, },
                [22] = new() { new() { 11, B1 }, new() { 12, TF1 }, new() { 13, TF12 }, new() { 28, TF1 }, new() { 29, TF12 }, new() { 30, B1 }, new() { 31, B1 }, new() { 33, B1 }, new() { 34, B1 }, new() { 35, B1 }, new() { 36, B1 }, new() { 42, B1 }, new() { 43, B1 }, new() { 44, B1 }, new() { 45, B1 }, },
                [23] = new() { new() { 12, B1 }, new() { 13, B1 }, new() { 29, B1 }, new() { 30, B1 }, new() { 36, B1 }, new() { 42, B1 }, },
                [24] = new() { new() { 13, B1 }, new() { 14, B1 }, new() { 28, B1 }, new() { 29, B1 }, new() { 36, B1 }, new() { 37, B1 }, new() { 38, B1 }, new() { 39, B1 }, new() { 40, B1 }, new() { 41, B1 }, new() { 42, B1 }, },
                [25] = new() { new() { 14, B1 }, new() { 15, TF1 }, new() { 16, TF12 }, new() { 26, TF1 }, new() { 27, B1 }, new() { 28, B1 }, },
                [26] = new() { new() { 15, B1 }, new() { 16, B1 }, new() { 17, B1 }, new() { 18, B1 }, new() { 19, B1 }, new() { 20, B1 }, new() { 21, B1 }, new() { 22, B1 }, new() { 23, B1 }, new() { 24, B1 }, new() { 25, B1 }, new() { 26, B1 }, new() { 27, B1 }, },


            };

            foreach (KeyValuePair<int, List<List<int>>> borderCode in borderCodes)
            {

                for(int i = 0; i < borderCode.Value.Count; i++)
                {

                    List<int> borderArray = borderCode.Value[i];

                    if (borderArray[1] == 9999)
                    {
                        
                        buildings.Tiles[borderArray[0], borderCode.Key] = new StaticTile(back, outdoor, BlendMode.Alpha, groundCodes[borderCode.Key][i][1]);

                        continue;

                    }

                    buildings.Tiles[borderArray[0], borderCode.Key] = new StaticTile(back, outdoor, BlendMode.Alpha, borderArray[1]);

                }

            }

            // ------------------------------------------------------
            // canopy line
            /*
            940 | 941 | 942 | 943 | 944 | 945
            965 | 966 | 967 | 968 | 969 | 970
            990 | 991 | 992 | 993 | 994 | 995
            1015 | 1016 | 1017 | 1018 | 1019 | 1020
            1040 | 1041 | 1042 | 1043 | 1044 | 1045
            1065 | 1066 | 1067 | 1068 | 1069 | 1070

            947 | 948 | 949
            972 | 973 | 974
                | 998 |

            */

            int CV = 946;

            int N1 = 942;
            int N2 = 943;
            int N3 = 967;
            int N4 = 968;

            E1 = 994;
            E2 = 995;
            E3 = 1019;
            E4 = 1020;

            int W1 = 990;
            int W2 = 991;
            int W3 = 1015;
            int W4 = 1016;

            int S1 = 1042;
            int S2 = 1043;
            int S3 = 1067;
            int S4 = 1068;

            int NW1 = 940;
            int NW2 = 941;
            int NW3 = 942;
            int NW4 = 965;
            int NW5 = 966;
            int NW6 = 967;
            int NW7 = 990;
            int NW8 = 991;

            int NE1 = 943;
            int NE2 = 944;
            int NE3 = 945;
            int NE4 = 968;    
            int NE5 = 969;
            int NE6 = 970;
            int NE7 = 994;
            int NE8 = 995;

            int SE1 = 1019;
            int SE2 = 1020;
            int SE3 = 1043;
            int SE4 = 1044;
            int SE5 = 1045;
            int SE6 = 1068;
            int SE7 = 1069;
            int SE8 = 1070;

            int SW1 = 1015;
            int SW2 = 1016;
            int SW3 = 1040;
            int SW4 = 1041;
            int SW5 = 1042;
            int SW6 = 1065;
            int SW7 = 1066;
            int SW8 = 1067;

            int NC1 = 1015;
            int NC2 = 1016;
            int NC3 = 968;
            int NC4 = 992;

            int EC1 = 1019;
            int EC2 = 942;
            int EC3 = 993;
            int EC4 = 967;

            int WC1 = 1043;
            int WC2 = 1017;
            int WC3 = 1068;
            int WC4 = 991;

            int SC1 = 1018;
            int SC2 = 1042;
            int SC3 = 994;
            int SC4 = 995;


            Dictionary<int, List<List<int>>> canopyCodes = new()
            {
                [0] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, CV }, new() { 12, CV }, new() { 13, CV }, new() { 14, CV }, new() { 15, CV }, new() { 16, CV }, new() { 17, CV }, new() { 18, CV }, new() { 19, CV }, new() { 20, CV }, new() { 21, CV }, new() { 22, CV }, new() { 23, CV }, new() { 24, CV }, new() { 25, CV }, new() { 26, CV }, new() { 27, CV }, new() { 28, CV }, new() { 29, CV }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, CV }, new() { 36, CV }, new() { 37, CV }, new() { 38, CV }, new() { 39, CV }, new() { 40, CV }, new() { 41, CV }, new() { 42, CV }, new() { 43, CV }, new() { 44, CV }, new() { 45, CV }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [1] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, CV }, new() { 12, CV }, new() { 13, CV }, new() { 14, CV }, new() { 15, CV }, new() { 16, CV }, new() { 17, CV }, new() { 18, CV }, new() { 19, CV }, new() { 20, CV }, new() { 21, CV }, new() { 22, CV }, new() { 23, CV }, new() { 24, CV }, new() { 25, CV }, new() { 26, CV }, new() { 27, CV }, new() { 28, CV }, new() { 29, CV }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, CV }, new() { 36, CV }, new() { 37, CV }, new() { 38, CV }, new() { 39, CV }, new() { 40, CV }, new() { 41, CV }, new() { 42, CV }, new() { 43, CV }, new() { 44, CV }, new() { 45, CV }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [2] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, CV }, new() { 12, CV }, new() { 13, CV }, new() { 14, NW1 }, new() { 15, NW2 }, new() { 16, NW3 }, new() { 17, N1 }, new() { 18, N2 }, new() { 19, N2 }, new() { 20, N2 }, new() { 21, N1 }, new() { 22, N2 }, new() { 23, N2 }, new() { 24, N2 }, new() { 25, N1 }, new() { 26, NE1 }, new() { 27, NE2 }, new() { 28, NE3 }, new() { 29, CV }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, CV }, new() { 36, CV }, new() { 37, CV }, new() { 38, CV }, new() { 39, CV }, new() { 40, CV }, new() { 41, CV }, new() { 42, CV }, new() { 43, CV }, new() { 44, CV }, new() { 45, CV }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [3] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, CV }, new() { 12, CV }, new() { 13, CV }, new() { 14, NW4 }, new() { 15, NW5 }, new() { 16, NW6 }, new() { 17, N3 }, new() { 18, N4 }, new() { 19, N3 }, new() { 20, N4 }, new() { 21, N3 }, new() { 22, N4 }, new() { 23, N4 }, new() { 24, N4 }, new() { 25, N3 }, new() { 26, NE4 }, new() { 27, NE5 }, new() { 28, NE6 }, new() { 29, CV }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, CV }, new() { 36, CV }, new() { 37, CV }, new() { 38, CV }, new() { 39, CV }, new() { 40, CV }, new() { 41, CV }, new() { 42, CV }, new() { 43, CV }, new() { 44, CV }, new() { 45, CV }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [4] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, CV }, new() { 12, CV }, new() { 13, NW1 }, new() { 14, NW7 }, new() { 15, NW8 }, new() { 27, NE7 }, new() { 28, NE8 }, new() { 29, NE3 }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, CV }, new() { 36, CV }, new() { 37, CV }, new() { 38, CV }, new() { 39, CV }, new() { 40, CV }, new() { 41, CV }, new() { 42, CV }, new() { 43, CV }, new() { 44, CV }, new() { 45, CV }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [5] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, NW1 }, new() { 12, NW2 }, new() { 13, NW3 }, new() { 14, NC1 }, new() { 15, NC2 }, new() { 27, EC1 }, new() { 28, EC2 }, new() { 29, NE1 }, new() { 30, NE2 }, new() { 31, NE3 }, new() { 32, CV }, new() { 33, CV }, new() { 34, NW1 }, new() { 35, NW2 }, new() { 36, NW3 }, new() { 37, N1 }, new() { 38, N2 }, new() { 39, N1 }, new() { 40, N2 }, new() { 41, N1 }, new() { 42, N2 }, new() { 43, NE1 }, new() { 44, NE2 }, new() { 45, NE3 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [6] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, NW4 }, new() { 12, NW5 }, new() { 13, NW6 }, new() { 14, NC3 }, new() { 15, NC4 }, new() { 27, EC3 }, new() { 28, EC4 }, new() { 29, NE4 }, new() { 30, NE5 }, new() { 31, NE6 }, new() { 32, CV }, new() { 33, CV }, new() { 34, NW4 }, new() { 35, NW5 }, new() { 36, NW6 }, new() { 37, N3 }, new() { 38, N4 }, new() { 39, N3 }, new() { 40, N4 }, new() { 41, N3 }, new() { 42, N4 }, new() { 43, NE4 }, new() { 44, NE5 }, new() { 45, NE6 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [7] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, NW1 }, new() { 11, NW7 }, new() { 12, NW8 }, new() { 30, NE7 }, new() { 31, NE8 }, new() { 32, NE3 }, new() { 33, NW1 }, new() { 34, NW7 }, new() { 35, NW8 }, new() { 44, NE7 }, new() { 45, NE8 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [8] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, NW1 }, new() { 9, NW2 }, new() { 10, NW3 }, new() { 11, NC1 }, new() { 12, NC2 }, new() { 30, EC1 }, new() { 31, EC2 }, new() { 32, N2 }, new() { 33, N1 }, new() { 34, NC1 }, new() { 35, NC2 }, new() { 44, E1 }, new() { 45, E2 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [9] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, NW4 }, new() { 9, NW5 }, new() { 10, NW6 }, new() { 11, NC3 }, new() { 12, NC4 }, new() { 30, EC3 }, new() { 31, EC4 }, new() { 32, N4 }, new() { 33, N3 }, new() { 34, NC3 }, new() { 35, NC4 }, new() { 44, E3 }, new() { 45, E4 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [10] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, NW7 }, new() { 9, NW8 }, new() { 44, E3 }, new() { 45, E4 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [11] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, W1 }, new() { 9, W2 }, new() { 30, SC1 }, new() { 31, SC2 }, new() { 32, S1 }, new() { 33, WC1 }, new() { 34, WC2 }, new() { 44, E3 }, new() { 45, E4 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [12] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, W3 }, new() { 9, W4 }, new() { 30, SC3 }, new() { 31, SC4 }, new() { 32, S3 }, new() { 33, WC3 }, new() { 34, WC4 }, new() { 44, E1 }, new() { 45, E2 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [13] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, W3 }, new() { 9, W4 }, new() { 30, E1 }, new() { 31, E2 }, new() { 32, CV }, new() { 33, W1 }, new() { 34, W2 }, new() { 44, E3 }, new() { 45, E4 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [14] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, W3 }, new() { 9, W4 }, new() { 30, E3 }, new() { 31, E4 }, new() { 32, CV }, new() { 33, W3 }, new() { 34, W4 }, new() { 44, E1 }, new() { 45, E2 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [15] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, W1 }, new() { 9, W2 }, new() { 30, E3 }, new() { 31, E4 }, new() { 32, CV }, new() { 33, W3 }, new() { 34, W4 }, new() { 44, E1 }, new() { 45, E2 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [16] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, W1 }, new() { 9, W2 }, new() { 30, EC1 }, new() { 31, EC2 }, new() { 32, N2 }, new() { 33, NC1 }, new() { 34, NC2 }, new() { 44, E3 }, new() { 45, E4 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [17] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, W3 }, new() { 9, W4 }, new() { 30, EC3 }, new() { 31, EC4 }, new() { 32, N4 }, new() { 33, NC3 }, new() { 34, NC4 }, new() { 44, E3 }, new() { 45, E4 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [18] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, SW1 }, new() { 9, SW2 }, new() { 44, E3 }, new() { 45, E4 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [19] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, SW3 }, new() { 9, SW4 }, new() { 10, SW5 }, new() { 11, WC1 }, new() { 12, WC2 }, new() { 30, SC1 }, new() { 31, SC2 }, new() { 32, S2 }, new() { 33, WC1 }, new() { 34, WC2 }, new() { 44, E1 }, new() { 45, E2 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [20] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, SW6 }, new() { 9, SW7 }, new() { 10, SW8 }, new() { 11, WC3 }, new() { 12, WC4 }, new() { 30, SC3 }, new() { 31, SC4 }, new() { 32, S4 }, new() { 33, WC3 }, new() { 34, WC4 }, new() { 44, SE1 }, new() { 45, SE2 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [21] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, SW6 }, new() { 11, SW1 }, new() { 12, SW2 }, new() { 30, SE1 }, new() { 31, SE2 }, new() { 32, SE8 }, new() { 33, SW3 }, new() { 34, SW4 }, new() { 35, SW5 }, new() { 36, WC1 }, new() { 37, WC2 }, new() { 41, SC1 }, new() { 42, SC2 }, new() { 43, SE3 }, new() { 44, SE4 }, new() { 45, SE5 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [22] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, SW3 }, new() { 12, SW4 }, new() { 13, SW5 }, new() { 14, WC1 }, new() { 15, WC2 }, new() { 27, SC1 }, new() { 28, SC2 }, new() { 29, SE3 }, new() { 30, SE4 }, new() { 31, SE5 }, new() { 32, CV }, new() { 33, SW6 }, new() { 34, SW7 }, new() { 35, SW8 }, new() { 36, WC3 }, new() { 37, WC4 }, new() { 41, SC3 }, new() { 42, SC4 }, new() { 43, SE6 }, new() { 44, SE7 }, new() { 45, SE8 }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [23] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, SW6 }, new() { 12, SW7 }, new() { 13, SW8 }, new() { 14, WC3 }, new() { 15, WC4 }, new() { 27, SC3 }, new() { 28, SC4 }, new() { 29, SE6 }, new() { 30, SE7 }, new() { 31, SE8 }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, SW6 }, new() { 36, SW3 }, new() { 37, SW4 }, new() { 38, SW5 }, new() { 39, S1 }, new() { 40, SE3 }, new() { 41, SE4 }, new() { 42, SE5 }, new() { 43, SE8 }, new() { 44, CV }, new() { 45, CV }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [24] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, CV }, new() { 12, CV }, new() { 13, SW6 }, new() { 14, SW1 }, new() { 15, SW2 }, new() { 27, SE1 }, new() { 28, SE2 }, new() { 29, SE8 }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, CV }, new() { 36, SW6 }, new() { 37, SW7 }, new() { 38, SW8 }, new() { 39, S3 }, new() { 40, SE6 }, new() { 41, SE7 }, new() { 42, SE8 }, new() { 43, CV }, new() { 44, CV }, new() { 45, CV }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [25] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, CV }, new() { 12, CV }, new() { 13, CV }, new() { 14, SW3 }, new() { 15, SW4 }, new() { 16, SW5 }, new() { 17, S1 }, new() { 18, S2 }, new() { 19, S2 }, new() { 20, S2 }, new() { 21, S1 }, new() { 22, S2 }, new() { 23, S1 }, new() { 24, S2 }, new() { 25, S1 }, new() { 26, SE3 }, new() { 27, SE4 }, new() { 28, SE5 }, new() { 29, CV }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, CV }, new() { 36, CV }, new() { 37, CV }, new() { 38, CV }, new() { 39, CV }, new() { 40, CV }, new() { 41, CV }, new() { 42, CV }, new() { 43, CV }, new() { 44, CV }, new() { 45, CV }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [26] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, CV }, new() { 12, CV }, new() { 13, CV }, new() { 14, SW6 }, new() { 15, SW7 }, new() { 16, SW8 }, new() { 17, S3 }, new() { 18, S4 }, new() { 19, S4 }, new() { 20, S4 }, new() { 21, S3 }, new() { 22, S4 }, new() { 23, S4 }, new() { 24, S4 }, new() { 25, S3 }, new() { 26, SE6 }, new() { 27, SE7 }, new() { 28, SE8 }, new() { 29, CV }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, CV }, new() { 36, CV }, new() { 37, CV }, new() { 38, CV }, new() { 39, CV }, new() { 40, CV }, new() { 41, CV }, new() { 42, CV }, new() { 43, CV }, new() { 44, CV }, new() { 45, CV }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [27] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, CV }, new() { 12, CV }, new() { 13, CV }, new() { 14, CV }, new() { 15, CV }, new() { 16, CV }, new() { 17, CV }, new() { 18, CV }, new() { 19, CV }, new() { 20, CV }, new() { 21, CV }, new() { 22, CV }, new() { 23, CV }, new() { 24, CV }, new() { 25, CV }, new() { 26, CV }, new() { 27, CV }, new() { 28, CV }, new() { 29, CV }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, CV }, new() { 36, CV }, new() { 37, CV }, new() { 38, CV }, new() { 39, CV }, new() { 40, CV }, new() { 41, CV }, new() { 42, CV }, new() { 43, CV }, new() { 44, CV }, new() { 45, CV }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [28] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, CV }, new() { 12, CV }, new() { 13, CV }, new() { 14, CV }, new() { 15, CV }, new() { 16, CV }, new() { 17, CV }, new() { 18, CV }, new() { 19, CV }, new() { 20, CV }, new() { 21, CV }, new() { 22, CV }, new() { 23, CV }, new() { 24, CV }, new() { 25, CV }, new() { 26, CV }, new() { 27, CV }, new() { 28, CV }, new() { 29, CV }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, CV }, new() { 36, CV }, new() { 37, CV }, new() { 38, CV }, new() { 39, CV }, new() { 40, CV }, new() { 41, CV }, new() { 42, CV }, new() { 43, CV }, new() { 44, CV }, new() { 45, CV }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },
                [29] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, CV }, new() { 3, CV }, new() { 4, CV }, new() { 5, CV }, new() { 6, CV }, new() { 7, CV }, new() { 8, CV }, new() { 9, CV }, new() { 10, CV }, new() { 11, CV }, new() { 12, CV }, new() { 13, CV }, new() { 14, CV }, new() { 15, CV }, new() { 16, CV }, new() { 17, CV }, new() { 18, CV }, new() { 19, CV }, new() { 20, CV }, new() { 21, CV }, new() { 22, CV }, new() { 23, CV }, new() { 24, CV }, new() { 25, CV }, new() { 26, CV }, new() { 27, CV }, new() { 28, CV }, new() { 29, CV }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, CV }, new() { 36, CV }, new() { 37, CV }, new() { 38, CV }, new() { 39, CV }, new() { 40, CV }, new() { 41, CV }, new() { 42, CV }, new() { 43, CV }, new() { 44, CV }, new() { 45, CV }, new() { 46, CV }, new() { 47, CV }, new() { 48, CV }, new() { 49, CV }, new() { 50, CV }, new() { 51, CV }, new() { 52, CV }, new() { 53, CV }, new() { 54, CV }, new() { 55, CV }, },

            };

            foreach (KeyValuePair<int, List<List<int>>> canopyCode in canopyCodes)
            {

                foreach (List<int> canopyArray in canopyCode.Value)
                {

                    alwaysfront.Tiles[canopyArray[0], canopyCode.Key] = new StaticTile(back, outdoor, BlendMode.Alpha, canopyArray[1]);

                }

            }


            // ------------------------------------------------------
            // location tiles

            locationTiles = new()
            {
                // rock 1
                new(18,9,0,2,2),
                new(19,9,1,2,2),
                new(18,10,0,3,1,true),
                new(19,10,1,3,1,true),

                // rock 2
                new(20,7,2,0,3),
                new(21,7,3,0,3),
                new(20,8,2,1,2),
                new(21,8,3,1,2),
                new(20,9,2,2,1,true),
                new(21,9,3,2,1,true),

                // rock 3
                new(23,8,5,1,3),
                new(23,9,5,2,2),
                new(23,10,5,3,1,true),

                // summoning
                new(15,13,6,0,2),
                new(15,14,6,1,1,true),
                new(26,14,7,1,1,true),
                new(17,18,6,2,2),
                new(17,19,6,3,1,true),
                new(24,18,7,2,2),
                new(24,19,7,3,1,true),

            };

            HerbalTiles();

            buildings.Tiles[18, 10] = new StaticTile(buildings, outdoor, BlendMode.Alpha, back.Tiles[18, 10].TileIndex);

            buildings.Tiles[19, 10] = new StaticTile(buildings, outdoor, BlendMode.Alpha, back.Tiles[19, 10].TileIndex);

            buildings.Tiles[20, 9] = new StaticTile(buildings, outdoor, BlendMode.Alpha, back.Tiles[20, 9].TileIndex);

            buildings.Tiles[21, 9] = new StaticTile(buildings, outdoor, BlendMode.Alpha, back.Tiles[21, 9].TileIndex);

            buildings.Tiles[23, 10] = new StaticTile(buildings, outdoor, BlendMode.Alpha, back.Tiles[23, 10].TileIndex);

            buildings.Tiles[15, 14] = new StaticTile(buildings, outdoor, BlendMode.Alpha, back.Tiles[15, 14].TileIndex);

            buildings.Tiles[26, 14] = new StaticTile(buildings, outdoor, BlendMode.Alpha, back.Tiles[26, 14].TileIndex);

            buildings.Tiles[17, 19] = new StaticTile(buildings, outdoor, BlendMode.Alpha, back.Tiles[17, 19].TileIndex);

            buildings.Tiles[24, 19] = new StaticTile(buildings, outdoor, BlendMode.Alpha, back.Tiles[24, 19].TileIndex);

            buildings.Tiles[20, 24] = new StaticTile(buildings, outdoor, BlendMode.Alpha, back.Tiles[20, 24].TileIndex);

            buildings.Tiles[21, 24] = new StaticTile(buildings, outdoor, BlendMode.Alpha, back.Tiles[21, 24].TileIndex);

            buildings.Tiles[22, 24] = new StaticTile(buildings, outdoor, BlendMode.Alpha, back.Tiles[22, 24].TileIndex);

            this.map = newMap;

            // ------------------------------------------------------
            // terrain features

            int bush = 0;

            Dictionary<int, List<List<int>>> bushCodes = new()
            {
                [4] = new() { new() { 15, bush }, new() { 26, bush }, },

                [7] = new() { new() { 12, bush }, new() { 29, bush }, },

                [10] = new() { new() { 9, bush }, new() { 43, bush }, },

                [22] = new() { new() { 12, bush }, new() { 28, bush }, },

                [25] = new() { new() { 15, bush }, new() { 26, bush }, },
            };

            largeTerrainFeatures.Clear();

            foreach (KeyValuePair<int, List<List<int>>> bushCode in bushCodes)
            {

                foreach (List<int> bushArray in bushCode.Value)
                {

                    Vector2 bushVector = new(bushArray[0], bushCode.Key);

                    StardewValley.TerrainFeatures.Bush newBush = new(bushVector, 2, this);

                    largeTerrainFeatures.Add(newBush);

                }

            }

            //spawnObjects();

        }

        public void HerbalTiles(bool bowl = false)
        {
            
            if (bowl)
            {
                herbalTiles = new()
                {

                    // herbalism
                    new(20,23,8,2,1),
                    new(21,23,9,2,1),
                    new(22,23,10,2,1),
                    new(20,24,8,3,0,true),
                    new(21,24,9,3,0,true),
                    new(22,24,10,3,0,true),

                };

                return;

            }

            herbalTiles = new()
            {

                // herbalism
                new(20,23,8,2,1),
                new(21,23,9,2,1),
                new(22,23,10,2,1),
                new(20,24,8,3,0,true),
                new(21,24,9,3,0,true),
                new(22,24,10,3,0,true),

            };

        }

        public override bool CanItemBePlacedHere(Vector2 tile, bool itemIsPassable = false, CollisionMask collisionMask = CollisionMask.All, CollisionMask ignorePassables = ~CollisionMask.Objects, bool useFarmerTile = false, bool ignorePassablesExactly = false)
        {

            return false;

        }

        public override bool isActionableTile(int xTile, int yTile, Farmer who)
        {

            Vector2 actionTile = new(xTile, yTile);

            if (dialogueTiles.ContainsKey(actionTile) && Mod.instance.activeEvent.Count == 0)
            {

                return true;

            }

            return base.isActionableTile(xTile, yTile, who);

        }

        public override bool checkAction(xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {

            Vector2 actionTile = new(tileLocation.X, tileLocation.Y);

            if (dialogueTiles.ContainsKey(actionTile))
            {

                CharacterHandle.characters characterType = dialogueTiles[actionTile];

                if (!Mod.instance.dialogue.ContainsKey(characterType))
                {

                    Mod.instance.dialogue[characterType] = new(characterType);

                }

                Mod.instance.dialogue[characterType].DialogueApproach();

                return true;

            }

            return base.checkAction(tileLocation, viewport, who);
        
        }

        public void AddDialogueTiles()
        {

            if (dialogueTiles.Count > 0) { return; }

            dialogueTiles.Add(new(18, 10), CharacterHandle.characters.energies);

            dialogueTiles.Add(new(19, 10), CharacterHandle.characters.energies);

            dialogueTiles.Add(new(20, 9), CharacterHandle.characters.energies);

            dialogueTiles.Add(new(21, 9), CharacterHandle.characters.energies);

            dialogueTiles.Add(new(21, 23), CharacterHandle.characters.herbalism);

        }

        public override void updateWarps()
        {
            //warps.Clear();

            if(warpSets.Count > 0)
            {

                warps.Clear();

                foreach(WarpTile warpSet in warpSets)
                {

                    warps.Add(new Warp(warpSet.enterX, warpSet.enterY, warpSet.location, warpSet.exitX, warpSet.exitY, flipFarmer: false));

                }

                return;

            }

            Layer back = map.GetLayer("Back");

            int width = back.LayerWidth;

            int height = back.LayerHeight;

            Vector2 caveEntry = new(19,15);

            Vector2 farmEntry = new(43,10);

            GameLocation farmLocation = Game1.getFarm();

            string farmName = "Farm";

            string caveName = "FarmCave";

            for (int w = farmLocation.warps.Count - 1; w >= 0; w--)
            {

                Warp warp = farmLocation.warps[w];

                GameLocation caveLocation = Game1.getLocationFromName(warp.TargetName);

                if(caveLocation is FarmCave)
                {

                    caveEntry = new Vector2(warp.TargetX, warp.TargetY);

                    Vector2 farmExit = new Vector2(warp.X, warp.Y);

                    farmEntry = farmExit + new Vector2(0, 2);

                    Warp change = new((int)farmExit.X, (int)farmExit.Y, LocationData.druid_grove_name, 39, 20, false);

                    farmLocation.warps[w] = change;

                    for (int c = caveLocation.warps.Count - 1; c >= 0; c--)
                    {

                        Warp caveWarp = caveLocation.warps[c];

                        if (caveWarp.TargetName == farmLocation.Name)
                        {

                            Vector2 caveExit = new Vector2(caveWarp.X, caveWarp.Y);

                            Warp caveChange = new((int)caveExit.X, (int)caveExit.Y, LocationData.druid_grove_name, 39, 10, false);

                            caveLocation.warps[c] = caveChange;

                        }

                    }

                    break;
                }

            }

            // --------------------------------------
            // Grove warps

            warpSets.Add(new WarpTile(39, 8, caveName, (int)caveEntry.X, (int)caveEntry.Y));

            warpSets.Add(new WarpTile(38, 23, farmName, (int)farmEntry.X, (int)farmEntry.Y));

            warpSets.Add(new WarpTile(39, 23, farmName, (int)farmEntry.X, (int)farmEntry.Y));

            warpSets.Add(new WarpTile(40, 23, farmName, (int)farmEntry.X, (int)farmEntry.Y));

            warps.Add(new Warp(39, 8, caveName, (int)caveEntry.X, (int)caveEntry.Y, flipFarmer: false));

            warps.Add(new Warp(38, 23, farmName, (int)farmEntry.X, (int)farmEntry.Y, flipFarmer: false));

            warps.Add(new Warp(39, 23, farmName, (int)farmEntry.X, (int)farmEntry.Y, flipFarmer: false));

            warps.Add(new Warp(40, 23, farmName, (int)farmEntry.X, (int)farmEntry.Y, flipFarmer: false));

        }

    }

}
