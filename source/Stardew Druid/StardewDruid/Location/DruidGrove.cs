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

namespace StardewDruid.Location
{
    public class DruidGrove : GameLocation
    {

        public List<WarpBack> warpBacks = new();

        public List<Warp> warpSets = new();

        public DruidGrove() { }

        public DruidGrove(string Name)
            : base("Maps\\Woods", Name) 
        {

        }

        public override void OnMapLoad(xTile.Map map)
        {

            /*for(int l = map.Layers.Count-1; l >= 0; l--)
            {
                Layer layer = map.Layers[l];

                map.RemoveLayer(layer);

            }*/

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

            Layer back = new("Back", newMap, new(36, 25), tileSize);

            newMap.AddLayer(back);

            Layer buildings = new("Buildings", newMap, new(36, 25), tileSize);

            newMap.AddLayer(buildings);

            Layer front = new("Front", newMap, new(36, 25), tileSize);

            newMap.AddLayer(front);

            Layer alwaysfront = new("AlwaysFront", newMap, new(36, 25), tileSize);

            newMap.AddLayer(alwaysfront);

            TileSheet outdoor = new("outdoors", newMap, map.TileSheets[1].ImageSource, map.TileSheets[1].SheetSize, map.TileSheets[1].TileSize);
            
            newMap.AddTileSheet(outdoor);

            // -----------------------------------------------------
            // normal ground

            /*List<List<int>> ground = new()
            {
                new(){3,9,15,},
                new(){4,9,15,},
                new(){5,7,17,},
                new(){6,7,17,},
                new(){7,5,19,},
                new(){8,5,19,},
                new(){9,3,21,},
                new(){10,3,21,},
                new(){11,3,21,},
                new(){12,3,21,},
                new(){13,3,21,},
                new(){14,3,21,},
                new(){15,3,21,},
                new(){16,5,19,},
                new(){17,5,19,},
                new(){18,7,17,},
                new(){19,7,17,},
                new(){21,9,15,},
                new(){22,9,15,},
                new(){9,22,25,},
                new(){15,22,25,},
                new(){8,29,29,},
                new(){9,26,32,},
                new(){10,26,32,},
                new(){11,26,32,},
                new(){12,26,32,},
                new(){13,26,32,},
                new(){14,26,32,},
                new(){15,26,32,},
                new(){16,26,32,},
                new(){17,26,32,},
            };*/

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

            int ENT = 265;

            Dictionary<int, List<List<int>>> groundCodes = new()
            {
                [1] = new() { new() { 8, E5 }, new() { 9, E5 }, new() { 10, E5 }, new() { 11, E5 }, new() { 12, E5 }, new() { 13, E5 }, new() { 14, E5 }, new() { 15, E5 }, new() { 16, E5 }, },
                [2] = new() { new() { 7, E5 }, new() { 8, C2 }, new() { 9, E1 }, new() { 10, E1 }, new() { 11, E1 }, new() { 12, E1 }, new() { 13, E1 }, new() { 14, E1 }, new() { 15, E1 }, new() { 16, C4 }, new() { 17, E5 }, },
                [3] = new() { new() { 6, E5 }, new() { 7, E5 }, new() { 8, E2 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, E3 }, new() { 17, E5 }, new() { 18, E5 }, },
                [4] = new() { new() { 5, E5 }, new() { 6, E5 }, new() { 7, C2 }, new() { 8, C1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, C3 }, new() { 17, C4 }, new() { 18, E5 }, new() { 19, E5 }, },
                [5] = new() { new() { 4, E5 }, new() { 5, C2 }, new() { 6, E1 }, new() { 7, C1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, C3 }, new() { 18, E1 }, new() { 19, C4 }, new() { 20, E5 }, },
                [6] = new() { new() { 3, E5 }, new() { 4, E5 }, new() { 5, E2 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, E3 }, new() { 20, E5 }, new() { 21, E5 }, new() { 29, ENT }, },
                [7] = new() { new() { 2, E5 }, new() { 3, E5 }, new() { 4, C2 }, new() { 5, C1 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, C3 }, new() { 20, C4 }, new() { 21, E5 }, new() { 22, E5 }, new() { 23, E5 }, new() { 24, E5 }, new() { 25, E5 }, new() { 26, E5 }, new() { 27, E5 }, new() { 28, E2 }, new() { 29, G1 }, new() { 30, E3 }, new() { 31, E5 }, new() { 32, E5 }, new() { 33, E5 }, new() { 34, E5 }, },
                [8] = new() { new() { 1, E5 }, new() { 2, C2 }, new() { 3, E1 }, new() { 4, C1 }, new() { 5, G1 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, E3 }, new() { 21, E5 }, new() { 22, E5 }, new() { 23, E5 }, new() { 24, E5 }, new() { 25, C2 }, new() { 26, E1 }, new() { 27, E1 }, new() { 28, C1 }, new() { 29, G1 }, new() { 30, C3 }, new() { 31, E1 }, new() { 32, E1 }, new() { 33, C4 }, new() { 34, E5 }, },
                [9] = new() { new() { 1, E5 }, new() { 2, E2 }, new() { 3, G1 }, new() { 4, G1 }, new() { 5, G1 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, C7 }, new() { 20, C8 }, new() { 21, E5 }, new() { 22, E5 }, new() { 23, E5 }, new() { 24, E5 }, new() { 25, E2 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, G1 }, new() { 30, G1 }, new() { 31, G1 }, new() { 32, G1 }, new() { 33, E3 }, new() { 34, E5 }, },
                [10] = new() { new() { 1, E5 }, new() { 2, E2 }, new() { 3, G1 }, new() { 4, G1 }, new() { 5, G1 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, E3 }, new() { 20, E5 }, new() { 24, E5 }, new() { 25, E2 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, G1 }, new() { 30, G1 }, new() { 31, G1 }, new() { 32, G1 }, new() { 33, E3 }, new() { 34, E5 }, },
                [11] = new() { new() { 1, E5 }, new() { 2, E2 }, new() { 3, G1 }, new() { 4, G1 }, new() { 5, G1 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, E3 }, new() { 20, E5 }, new() { 24, E5 }, new() { 25, E2 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, G1 }, new() { 30, G1 }, new() { 31, G1 }, new() { 32, G1 }, new() { 33, E3 }, new() { 34, E5 }, },
                [12] = new() { new() { 1, E5 }, new() { 2, E2 }, new() { 3, G1 }, new() { 4, G1 }, new() { 5, G1 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, E3 }, new() { 20, E5 }, new() { 24, E5 }, new() { 25, E2 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, G1 }, new() { 30, G1 }, new() { 31, G1 }, new() { 32, G1 }, new() { 33, E3 }, new() { 34, E5 }, },
                [13] = new() { new() { 1, E5 }, new() { 2, E2 }, new() { 3, G1 }, new() { 4, G1 }, new() { 5, G1 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, E3 }, new() { 20, E5 }, new() { 24, E5 }, new() { 25, E2 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, G1 }, new() { 30, G1 }, new() { 31, G1 }, new() { 32, G1 }, new() { 33, E3 }, new() { 34, E5 }, },
                [14] = new() { new() { 1, E5 }, new() { 2, E2 }, new() { 3, G1 }, new() { 4, G1 }, new() { 5, G1 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, E3 }, new() { 20, E5 }, new() { 24, E5 }, new() { 25, E2 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, G1 }, new() { 30, G1 }, new() { 31, G1 }, new() { 32, G1 }, new() { 33, E3 }, new() { 34, E5 }, },
                [15] = new() { new() { 1, E5 }, new() { 2, E2 }, new() { 3, G1 }, new() { 4, G1 }, new() { 5, G1 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, C3 }, new() { 20, C4 }, new() { 21, E5 }, new() { 22, E5 }, new() { 23, E5 }, new() { 24, E5 }, new() { 25, E2 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, G1 }, new() { 30, G1 }, new() { 31, G1 }, new() { 32, G1 }, new() { 33, E3 }, new() { 34, E5 }, },
                [16] = new() { new() { 1, E5 }, new() { 2, C6 }, new() { 3, E4 }, new() { 4, C5 }, new() { 5, G1 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, G1 }, new() { 20, E3 }, new() { 21, E5 }, new() { 22, E5 }, new() { 23, E5 }, new() { 24, E5 }, new() { 25, E2 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, G1 }, new() { 30, G1 }, new() { 31, G1 }, new() { 32, G1 }, new() { 33, E3 }, new() { 34, E5 }, },
                [17] = new() { new() { 2, E5 }, new() { 3, E5 }, new() { 4, C6 }, new() { 5, C5 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, C7 }, new() { 20, C8 }, new() { 21, E5 }, new() { 22, E5 }, new() { 23, E5 }, new() { 24, E5 }, new() { 25, E2 }, new() { 26, G1 }, new() { 27, G1 }, new() { 28, G1 }, new() { 29, G1 }, new() { 30, G1 }, new() { 31, G1 }, new() { 32, G1 }, new() { 33, E3 }, new() { 34, E5 }, },
                [18] = new() { new() { 3, E5 }, new() { 4, E5 }, new() { 5, E2 }, new() { 6, G1 }, new() { 7, G1 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, G1 }, new() { 18, G1 }, new() { 19, E3 }, new() { 20, E5 }, new() { 21, E5 }, new() { 24, E5 }, new() { 25, C6 }, new() { 26, E4 }, new() { 27, E4 }, new() { 28, C5 }, new() { 29, G1 }, new() { 30, C7 }, new() { 31, E4 }, new() { 32, E4 }, new() { 33, C8 }, new() { 34, E5 }, },
                [19] = new() { new() { 4, E5 }, new() { 5, C6 }, new() { 6, E4 }, new() { 7, C5 }, new() { 8, G1 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, G1 }, new() { 17, C7 }, new() { 18, E4 }, new() { 19, C8 }, new() { 20, E5 }, new() { 24, E5 }, new() { 25, E5 }, new() { 26, E5 }, new() { 27, E5 }, new() { 28, E2 }, new() { 29, G1 }, new() { 30, E3 }, new() { 31, E5 }, new() { 32, E5 }, new() { 33, E5 }, new() { 34, E5 }, },
                [20] = new() { new() { 5, E5 }, new() { 6, E5 }, new() { 7, C6 }, new() { 8, C5 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, C7 }, new() { 17, C8 }, new() { 18, E5 }, new() { 19, E5 }, new() { 27, E5 }, new() { 28, E2 }, new() { 29, G1 }, new() { 30, E3 }, new() { 31, E5 }, },
                [21] = new() { new() { 6, E5 }, new() { 7, E5 }, new() { 8, E2 }, new() { 9, G1 }, new() { 10, G1 }, new() { 11, G1 }, new() { 12, G1 }, new() { 13, G1 }, new() { 14, G1 }, new() { 15, G1 }, new() { 16, E3 }, new() { 17, E5 }, new() { 18, E5 }, new() { 27, E5 }, new() { 28, C6 }, new() { 29, E4 }, new() { 30, C8 }, new() { 31, E5 }, },
                [22] = new() { new() { 7, E5 }, new() { 8, C6 }, new() { 9, E4 }, new() { 10, E4 }, new() { 11, E4 }, new() { 12, E4 }, new() { 13, E4 }, new() { 14, E4 }, new() { 15, E4 }, new() { 16, C8 }, new() { 17, E5 }, },
                [23] = new() { new() { 8, E5 }, new() { 9, E5 }, new() { 10, E5 }, new() { 11, E5 }, new() { 12, E5 }, new() { 13, E5 }, new() { 14, E5 }, new() { 15, E5 }, new() { 16, E5 }, },


            };

            foreach(KeyValuePair<int, List<List<int>>> groundCode in groundCodes)
            {

                foreach(List<int> groundArray in groundCode.Value)
                {

                    back.Tiles[groundArray[0], groundCode.Key] = new StaticTile(back, outdoor, BlendMode.Alpha, groundArray[1]);

                }

            }

            /*for (int i = 3; i < 29; i++)
            {

                for (int j = 3; j < 27; j++)
                {
                    
                    back.Tiles[(int)i, (int)j] = new StaticTile(back, outdoor, BlendMode.Alpha, 351);
                    
                    back.Tiles[(int)i, (int)j].TileIndexProperties.Add("Type", "Grass");

                }

            }
            for (int i = 28; i < 37; i++)
            {

                for (int j = 20; j < 27; j++)
                {

                    back.Tiles[(int)i, (int)j] = new StaticTile(back, outdoor, BlendMode.Alpha, 351);

                    back.Tiles[(int)i, (int)j].TileIndexProperties.Add("Type", "Grass");

                }

            }*/


            // ------------------------------------------------------
            // ground line

            /*back.Tiles[2,2] = new StaticTile(back, outdoor, BlendMode.Alpha, 357);
            back.Tiles[2,27] = new StaticTile(back, outdoor, BlendMode.Alpha, 382);
            back.Tiles[29, 19] = new StaticTile(back, outdoor, BlendMode.Alpha, 404);
            back.Tiles[29,2] = new StaticTile(back, outdoor, BlendMode.Alpha, 407);
            back.Tiles[37, 19] = new StaticTile(back, outdoor, BlendMode.Alpha, 407);
            back.Tiles[37,27] = new StaticTile(back, outdoor, BlendMode.Alpha, 332);


            for (int i = 3; i < 28; i++){ back.Tiles[i, 2] = new StaticTile(back, outdoor, BlendMode.Alpha, 405); back.Tiles[i, 2].TileIndexProperties.Add("Type", "Grass"); }
            for (int i = 30; i < 37; i++) { back.Tiles[i, 19] = new StaticTile(back, outdoor, BlendMode.Alpha, 405); back.Tiles[i, 19].TileIndexProperties.Add("Type", "Grass"); }
            for (int i = 3; i < 37; i++) {  back.Tiles[i, 27] = new StaticTile(back, outdoor, BlendMode.Alpha, 355); back.Tiles[i, 27].TileIndexProperties.Add("Type", "Grass"); }

            for (int i = 1; i < 32; i++) { back.Tiles[i, 1] = new StaticTile(back, outdoor, BlendMode.Alpha, 380); back.Tiles[i, 1].TileIndexProperties.Add("Type", "Grass"); }
            for (int i = 1; i < 39; i++) { back.Tiles[i, 28] = new StaticTile(back, outdoor, BlendMode.Alpha, 380); back.Tiles[i, 28].TileIndexProperties.Add("Type", "Grass"); }


            for (int i = 3; i < 27; i++) { back.Tiles[2, i] = new StaticTile(back, outdoor, BlendMode.Alpha, 381); back.Tiles[2, i].TileIndexProperties.Add("Type", "Grass"); }
            for (int i = 3; i < 19; i++) { back.Tiles[29, i] = new StaticTile(back, outdoor, BlendMode.Alpha, 379); back.Tiles[29, i].TileIndexProperties.Add("Type", "Grass"); }
            for (int i = 20; i < 27; i++) { back.Tiles[37, i] = new StaticTile(back, outdoor, BlendMode.Alpha, 379); back.Tiles[37, i].TileIndexProperties.Add("Type", "Grass"); }

            for (int i = 1; i < 29; i++) { back.Tiles[1, i] = new StaticTile(back, outdoor, BlendMode.Alpha, 380); back.Tiles[1, i].TileIndexProperties.Add("Type", "Grass"); }
            for (int i = 1; i < 19; i++) { back.Tiles[30, i] = new StaticTile(back, outdoor, BlendMode.Alpha, 380); back.Tiles[38, i].TileIndexProperties.Add("Type", "Grass"); }
            for (int i = 20; i < 28; i++) { back.Tiles[38, i] = new StaticTile(back, outdoor, BlendMode.Alpha, 380); back.Tiles[38, i].TileIndexProperties.Add("Type", "Grass"); }

            back.Tiles[34, 16] = new StaticTile(back, outdoor, BlendMode.Alpha, 380);
            back.Tiles[34, 17] = new StaticTile(back, outdoor, BlendMode.Alpha, 380);
            back.Tiles[34, 18] = new StaticTile(back, outdoor, BlendMode.Alpha, 380);*/


            // ------------------------------------------------------
            // cave


            Dictionary<int, List<int>> caveIndices = new()
            {
                [0] = new() { 466, 467, 468, 1633, 1634, 1635, 467, 468, 467, 468, },
                [1] = new() { 491, 492, 493, 1658, 1659, 1660, 492, 493, 492, 493, },
                [2] = new() { 516, 517, 518, 1683, 0, 1685, 517, 518, 517, 518, },
                [3] = new() { 541, 542, 543, 1708, 0, 1710, 542, 543, 542, 543, },

            };
            int bX = 25; int bY = 4;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 4; j++)
                {

                    if (caveIndices[j][i] == 0) { continue; }

                    buildings.Tiles[bX + i, bY + j] = new StaticTile(buildings, outdoor, BlendMode.Alpha, caveIndices[j][i]);

                }

            }

            int B1 = 380;

            Dictionary<int, List<List<int>>> borderCodes = new()
            {
                [0] = new() { new() { 7, B1 }, new() { 8, B1 }, new() { 9, B1 }, new() { 10, B1 }, new() { 11, B1 }, new() { 12, B1 }, new() { 13, B1 }, new() { 14, B1 }, new() { 15, B1 }, new() { 16, B1 }, new() { 17, B1 }, },
                [1] = new() { new() { 6, B1 }, new() { 7, B1 }, new() { 17, B1 }, new() { 18, B1 }, },
                [2] = new() { new() { 5, B1 }, new() { 6, B1 }, new() { 18, B1 }, new() { 19, B1 }, },
                [3] = new() { new() { 4, B1 }, new() { 5, B1 }, new() { 19, B1 }, new() { 20, B1 }, },
                [4] = new() { new() { 3, B1 }, new() { 4, B1 }, new() { 20, B1 }, new() { 21, B1 }, },
                [5] = new() { new() { 2, B1 }, new() { 3, B1 }, new() { 21, B1 }, new() { 22, B1 }, },
                [6] = new() { new() { 1, B1 }, new() { 2, B1 }, new() { 22, B1 }, new() { 23, B1 }, new() { 24, B1 }, new() { 35, B1 }, },
                [7] = new() { new() { 0, B1 }, new() { 1, B1 }, new() { 35, B1 }, },
                [8] = new() { new() { 0, B1 }, new() { 35, B1 }, },
                [9] = new() { new() { 0, B1 }, new() { 35, B1 }, },
                [10] = new() { new() { 0, B1 }, new() { 21, B1 }, new() { 22, B1 }, new() { 23, B1 }, new() { 35, B1 }, },
                [11] = new() { new() { 0, B1 }, new() { 21, B1 }, new() { 23, B1 }, new() { 35, B1 }, },
                [12] = new() { new() { 0, B1 }, new() { 21, B1 }, new() { 23, B1 }, new() { 35, B1 }, },
                [13] = new() { new() { 0, B1 }, new() { 21, B1 }, new() { 23, B1 }, new() { 35, B1 }, },
                [14] = new() { new() { 0, B1 }, new() { 21, B1 }, new() { 22, B1 }, new() { 23, B1 }, new() { 35, B1 }, },
                [15] = new() { new() { 0, B1 }, new() { 35, B1 }, },
                [16] = new() { new() { 0, B1 }, new() { 35, B1 }, },
                [17] = new() { new() { 0, B1 }, new() { 1, B1 }, new() { 35, B1 }, },
                [18] = new() { new() { 1, B1 }, new() { 2, B1 }, new() { 22, B1 }, new() { 23, B1 }, new() { 35, B1 }, },
                [19] = new() { new() { 2, B1 }, new() { 3, B1 }, new() { 21, B1 }, new() { 22, B1 }, new() { 23, B1 }, new() { 35, B1 }, },
                [20] = new() { new() { 3, B1 }, new() { 4, B1 }, new() { 20, B1 }, new() { 21, B1 }, new() { 23, B1 }, new() { 24, B1 }, new() { 25, B1 }, new() { 26, B1 }, new() { 32, B1 }, new() { 33, B1 }, new() { 34, B1 }, new() { 35, B1 }, },
                [21] = new() { new() { 4, B1 }, new() { 5, B1 }, new() { 19, B1 }, new() { 20, B1 }, new() { 26, B1 }, new() { 32, B1 }, },
                [22] = new() { new() { 5, B1 }, new() { 6, B1 }, new() { 18, B1 }, new() { 19, B1 }, new() { 26, B1 }, new() { 27, B1 }, new() { 28, B1 }, new() { 29, B1 }, new() { 30, B1 }, new() { 31, B1 }, new() { 32, B1 }, },
                [23] = new() { new() { 6, B1 }, new() { 7, B1 }, new() { 17, B1 }, new() { 18, B1 }, },
                [24] = new() { new() { 7, B1 }, new() { 8, B1 }, new() { 9, B1 }, new() { 10, B1 }, new() { 11, B1 }, new() { 12, B1 }, new() { 13, B1 }, new() { 14, B1 }, new() { 15, B1 }, new() { 16, B1 }, new() { 17, B1 }, },

            };

            foreach (KeyValuePair<int, List<List<int>>> borderCode in borderCodes)
            {

                foreach (List<int> borderArray in borderCode.Value)
                {

                    buildings.Tiles[borderArray[0], borderCode.Key] = new StaticTile(back, outdoor, BlendMode.Alpha, borderArray[1]);

                }

            }

            /*
            for (int i = 0; i < 40; i++) { buildings.Tiles[i, 0] = new StaticTile(buildings, outdoor, BlendMode.Alpha, 380); }
            for (int i = 0; i < 40; i++) { buildings.Tiles[i, 29] = new StaticTile(buildings, outdoor, BlendMode.Alpha, 380);  }
            for (int i = 0; i < 30; i++) { buildings.Tiles[0, i] = new StaticTile(buildings, outdoor, BlendMode.Alpha, 380);  }
            for (int i = 0; i < 15; i++) { buildings.Tiles[30, i] = new StaticTile(buildings, outdoor, BlendMode.Alpha, 380);  }
            for (int i = 19; i < 30; i++) { buildings.Tiles[39, i] = new StaticTile(buildings, outdoor, BlendMode.Alpha, 380); }
            */



            // ------------------------------------------------------
            // canopy line
            /*
            940 | 941 | 942 | 943 | 944 | 945
            965 | 966 | 967 | 968 | 969 | 970
            990 | 991 | 992 | 993 | 994 | 995
            1015 | 1016 | 1017 | 1018 | 1019 | 1020
            1040 | 1041 | 1042 | 1043 | 1044 | 1045
            1065 | 1066 | 1067 | 1068 | 1069 | 1070
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
                [0] = new() { new() { 5, CV }, new() { 6, NW1 }, new() { 7, NW2 }, new() { 8, NW3 }, new() { 9, N1 }, new() { 10, N2 }, new() { 11, N2 }, new() { 12, N1 }, new() { 13, N2 }, new() { 14, N2 }, new() { 15, N1 }, new() { 16, NE1 }, new() { 17, NE2 }, new() { 18, NE3 }, new() { 19, CV }, },
                [1] = new() { new() { 5, CV }, new() { 6, NW4 }, new() { 7, NW5 }, new() { 8, NW6 }, new() { 9, N3 }, new() { 10, N4 }, new() { 11, N4 }, new() { 12, N3 }, new() { 13, N4 }, new() { 14, N4 }, new() { 15, N3 }, new() { 16, NE4 }, new() { 17, NE5 }, new() { 18, NE6 }, new() { 19, CV }, },
                [2] = new() { new() { 3, CV }, new() { 4, CV }, new() { 5, NW1 }, new() { 6, NW7 }, new() { 7, NW8 }, new() { 17, NE7 }, new() { 18, NE8 }, new() { 19, NE3 }, new() { 20, CV }, new() { 21, CV }, new() { 24, CV }, new() { 25, CV }, new() { 26, CV }, new() { 27, CV }, new() { 28, CV }, new() { 29, CV }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, new() { 33, CV }, new() { 34, CV }, new() { 35, CV }, },
                [3] = new() { new() { 2, CV }, new() { 3, NW1 }, new() { 4, NW2 }, new() { 5, NW3 }, new() { 6, NC1 }, new() { 7, NC2 }, new() { 17, EC1 }, new() { 18, EC2 }, new() { 19, NE1 }, new() { 20, NE2 }, new() { 21, NE3 }, new() { 22, CV }, new() { 23, CV }, new() { 24, NW1 }, new() { 25, NW2 }, new() { 26, NW3 }, new() { 27, N1 }, new() { 28, N2 }, new() { 29, N1 }, new() { 30, N2 }, new() { 31, N1 }, new() { 32, N2 }, new() { 33, NE1 }, new() { 34, NE2 }, new() { 35, NE3 }, },
                [4] = new() { new() { 2, CV }, new() { 3, NW4 }, new() { 4, NW5 }, new() { 5, NW6 }, new() { 6, NC3 }, new() { 7, NC4 }, new() { 17, EC3 }, new() { 18, EC4 }, new() { 19, NE4 }, new() { 20, NE5 }, new() { 21, NE6 }, new() { 22, CV }, new() { 23, CV }, new() { 24, NW4 }, new() { 25, NW5 }, new() { 26, NW6 }, new() { 27, N3 }, new() { 28, N4 }, new() { 29, N3 }, new() { 30, N4 }, new() { 31, N3 }, new() { 32, N4 }, new() { 33, NE4 }, new() { 34, NE5 }, new() { 35, NE6 }, },
                [5] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, NW1 }, new() { 3, NW7 }, new() { 4, NW8 }, new() { 20, NE7 }, new() { 21, NE8 }, new() { 22, NE3 }, new() { 23, NW1 }, new() { 24, NW7 }, new() { 25, NW8 }, new() { 34, NE7 }, new() { 35, NE8 }, },
                [6] = new() { new() { 0, NW1 }, new() { 1, NW2 }, new() { 2, NW3 }, new() { 3, NC1 }, new() { 4, NC2 }, new() { 20, EC1 }, new() { 21, EC2 }, new() { 22, N2 }, new() { 23, N1 }, new() { 24, NC1 }, new() { 25, NC2 }, new() { 34, E1 }, new() { 35, E2 }, },
                [7] = new() { new() { 0, NW4 }, new() { 1, NW5 }, new() { 2, NW6 }, new() { 3, NC3 }, new() { 4, NC4 }, new() { 20, EC3 }, new() { 21, EC4 }, new() { 22, N4 }, new() { 23, N3 }, new() { 24, NC3 }, new() { 25, NC4 }, new() { 34, E3 }, new() { 35, E4 }, },
                [8] = new() { new() { 0, NW7 }, new() { 1, NW8 }, new() { 34, E3 }, new() { 35, E4 }, },
                [9] = new() { new() { 0, W1 }, new() { 1, W2 }, new() { 20, SC1 }, new() { 21, SC2 }, new() { 22, S1 }, new() { 23, WC1 }, new() { 24, WC2 }, new() { 34, E3 }, new() { 35, E4 }, },
                [10] = new() { new() { 0, W3 }, new() { 1, W4 }, new() { 20, SC3 }, new() { 21, SC4 }, new() { 22, S3 }, new() { 23, WC3 }, new() { 24, WC4 }, new() { 34, E1 }, new() { 35, E2 }, },
                [11] = new() { new() { 0, W3 }, new() { 1, W4 }, new() { 20, E1 }, new() { 21, E2 }, new() { 22, CV }, new() { 23, W1 }, new() { 24, W2 }, new() { 34, E3 }, new() { 35, E4 }, },
                [12] = new() { new() { 0, W3 }, new() { 1, W4 }, new() { 20, E3 }, new() { 21, E4 }, new() { 22, CV }, new() { 23, W3 }, new() { 24, W4 }, new() { 34, E1 }, new() { 35, E2 }, },
                [13] = new() { new() { 0, W1 }, new() { 1, W2 }, new() { 20, E3 }, new() { 21, E4 }, new() { 22, CV }, new() { 23, W3 }, new() { 24, W4 }, new() { 34, E1 }, new() { 35, E2 }, },
                [14] = new() { new() { 0, W1 }, new() { 1, W2 }, new() { 20, EC1 }, new() { 21, EC2 }, new() { 22, N2 }, new() { 23, NC1 }, new() { 24, NC2 }, new() { 34, E3 }, new() { 35, E4 }, },
                [15] = new() { new() { 0, W3 }, new() { 1, W4 }, new() { 20, EC3 }, new() { 21, EC4 }, new() { 22, N4 }, new() { 23, NC3 }, new() { 24, NC4 }, new() { 34, E3 }, new() { 35, E4 }, },
                [16] = new() { new() { 0, SW1 }, new() { 1, SW2 }, new() { 34, E3 }, new() { 35, E4 }, },
                [17] = new() { new() { 0, SW3 }, new() { 1, SW4 }, new() { 2, SW5 }, new() { 3, WC1 }, new() { 4, WC2 }, new() { 20, SC1 }, new() { 21, SC2 }, new() { 22, S2 }, new() { 23, WC1 }, new() { 24, WC2 }, new() { 34, E1 }, new() { 35, E2 }, },
                [18] = new() { new() { 0, SW6 }, new() { 1, SW7 }, new() { 2, SW8 }, new() { 3, WC3 }, new() { 4, WC4 }, new() { 20, SC3 }, new() { 21, SC4 }, new() { 22, S4 }, new() { 23, WC3 }, new() { 24, WC4 }, new() { 34, SE1 }, new() { 35, SE2 }, },
                [19] = new() { new() { 0, CV }, new() { 1, CV }, new() { 2, SW6 }, new() { 3, SW1 }, new() { 4, SW2 }, new() { 20, SE1 }, new() { 21, SE2 }, new() { 22, SE8 }, new() { 23, SW3 }, new() { 24, SW4 }, new() { 25, SW5 }, new() { 26, WC1 }, new() { 27, WC2 }, new() { 31, SC1 }, new() { 32, SC2 }, new() { 33, SE3 }, new() { 34, SE4 }, new() { 35, SE5 }, },
                [20] = new() { new() { 2, CV }, new() { 3, SW3 }, new() { 4, SW4 }, new() { 5, SW5 }, new() { 6, WC1 }, new() { 7, WC2 }, new() { 17, SC1 }, new() { 18, SC2 }, new() { 19, SE3 }, new() { 20, SE4 }, new() { 21, SE5 }, new() { 22, CV }, new() { 23, SW6 }, new() { 24, SW7 }, new() { 25, SW8 }, new() { 26, WC3 }, new() { 27, WC4 }, new() { 31, SC3 }, new() { 32, SC4 }, new() { 33, SE6 }, new() { 34, SE7 }, new() { 35, SE8 }, },
                [21] = new() { new() { 2, CV }, new() { 3, SW6 }, new() { 4, SW7 }, new() { 5, SW8 }, new() { 6, WC3 }, new() { 7, WC4 }, new() { 17, SC3 }, new() { 18, SC4 }, new() { 19, SE6 }, new() { 20, SE7 }, new() { 21, SE8 }, new() { 22, CV }, new() { 23, CV }, new() { 24, CV }, new() { 25, SW6 }, new() { 26, SW3 }, new() { 27, SW4 }, new() { 28, SW5 }, new() { 29, S1 }, new() { 30, SE3 }, new() { 31, SE4 }, new() { 32, SE5 }, new() { 33, SE8 }, new() { 34, CV }, new() { 35, CV }, },
                [22] = new() { new() { 3, CV }, new() { 4, CV }, new() { 5, SW6 }, new() { 6, SW1 }, new() { 7, SW2 }, new() { 17, SE1 }, new() { 18, SE2 }, new() { 19, SE8 }, new() { 20, CV }, new() { 21, CV }, new() { 25, CV }, new() { 26, SW6 }, new() { 27, SW7 }, new() { 28, SW8 }, new() { 29, S3 }, new() { 30, SE6 }, new() { 31, SE7 }, new() { 32, SE8 }, new() { 33, CV }, },
                [23] = new() { new() { 5, CV }, new() { 6, SW3 }, new() { 7, SW4 }, new() { 8, SW5 }, new() { 9, S1 }, new() { 10, S2 }, new() { 11, S2 }, new() { 12, S1 }, new() { 13, S2 }, new() { 14, S2 }, new() { 15, S1 }, new() { 16, SE3 }, new() { 17, SE4 }, new() { 18, SE5 }, new() { 19, CV }, new() { 26, CV }, new() { 27, CV }, new() { 28, CV }, new() { 29, CV }, new() { 30, CV }, new() { 31, CV }, new() { 32, CV }, },
                [24] = new() { new() { 5, CV }, new() { 6, SW6 }, new() { 7, SW7 }, new() { 8, SW8 }, new() { 9, S3 }, new() { 10, S4 }, new() { 11, S4 }, new() { 12, S3 }, new() { 13, S4 }, new() { 14, S4 }, new() { 15, S3 }, new() { 16, SE6 }, new() { 17, SE7 }, new() { 18, SE8 }, new() { 19, CV }, },


            };

            foreach (KeyValuePair<int, List<List<int>>> canopyCode in canopyCodes)
            {

                foreach (List<int> canopyArray in canopyCode.Value)
                {

                    alwaysfront.Tiles[canopyArray[0], canopyCode.Key] = new StaticTile(back, outdoor, BlendMode.Alpha, canopyArray[1]);

                }

            }

            /*

            int cX = 0; int cY = 0; int cI = 940;

            alwaysfront.Tiles[cX,cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI);
            alwaysfront.Tiles[cX+1,cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI+1);
            alwaysfront.Tiles[cX+2,cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI+2);
            alwaysfront.Tiles[cX,cY+1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI+25);
            alwaysfront.Tiles[cX+1,cY+1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI+26);
            alwaysfront.Tiles[cX+2,cY+1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI+27);
            alwaysfront.Tiles[cX,cY+2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI+50);
            alwaysfront.Tiles[cX+1,cY+2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI+51);
            //alwaysfront.Tiles[cX+2,cY+2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI+52);

            cX = 27; cY = 0; cI = 943;

            alwaysfront.Tiles[cX, cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI);
            alwaysfront.Tiles[cX + 1, cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 1);
            alwaysfront.Tiles[cX + 2, cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 2);
            alwaysfront.Tiles[cX, cY + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 25);
            alwaysfront.Tiles[cX + 1, cY + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 26);
            alwaysfront.Tiles[cX + 2, cY + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 27);
            //alwaysfront.Tiles[cX, cY + 2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 50);
            alwaysfront.Tiles[cX + 1, cY + 2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 51);
            alwaysfront.Tiles[cX + 2, cY + 2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 52);

            alwaysfront.Tiles[28, 14] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, 1019);
            alwaysfront.Tiles[29, 14] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, 942);
            alwaysfront.Tiles[28, 15] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, 993);
            alwaysfront.Tiles[29, 15] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, 967);

            cX = 37; cY = 14; cI = 943;

            alwaysfront.Tiles[cX, cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI);
            alwaysfront.Tiles[cX + 1, cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 1);
            alwaysfront.Tiles[cX + 2, cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 2);
            alwaysfront.Tiles[cX, cY + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 25);
            alwaysfront.Tiles[cX + 1, cY + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 26);
            alwaysfront.Tiles[cX + 2, cY + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 27);
            //alwaysfront.Tiles[cX, cY + 2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 50);
            alwaysfront.Tiles[cX + 1, cY + 2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 51);
            alwaysfront.Tiles[cX + 2, cY + 2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 52);

            cX = 0; cY = 27; cI = 1015;

            alwaysfront.Tiles[cX, cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI);
            alwaysfront.Tiles[cX + 1, cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 1);
            //alwaysfront.Tiles[cX + 2, cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 2);
            alwaysfront.Tiles[cX, cY + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 25);
            alwaysfront.Tiles[cX + 1, cY + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 26);
            alwaysfront.Tiles[cX + 2, cY + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 27);
            alwaysfront.Tiles[cX, cY + 2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 50);
            alwaysfront.Tiles[cX + 1, cY + 2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 51);
            alwaysfront.Tiles[cX + 2, cY + 2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 52);

            cX = 37; cY = 27; cI = 1018;

            //alwaysfront.Tiles[cX, cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI);
            alwaysfront.Tiles[cX + 1, cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 1);
            alwaysfront.Tiles[cX + 2, cY] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 2);
            alwaysfront.Tiles[cX, cY + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 25);
            alwaysfront.Tiles[cX + 1, cY + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 26);
            alwaysfront.Tiles[cX + 2, cY + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 27);
            alwaysfront.Tiles[cX, cY + 2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 50);
            alwaysfront.Tiles[cX + 1, cY + 2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 51);
            alwaysfront.Tiles[cX + 2, cY + 2] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 52);


            cI = 942;
            for (int i = 3; i < 27; i += 2)
            {
                alwaysfront.Tiles[i,0] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI);
                alwaysfront.Tiles[i+1,0] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 1);
                alwaysfront.Tiles[i,1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 25);
                alwaysfront.Tiles[i+1,1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 26);
            }

            cI = 942;
            for (int i = 30; i < 37; i += 2)
            {
                alwaysfront.Tiles[i, 14] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI);
                alwaysfront.Tiles[i + 1, 14] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 1);
                alwaysfront.Tiles[i, 15] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 25);
                alwaysfront.Tiles[i + 1, 15] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 26);
            }

            cI = 990;
            for (int i = 3; i < 27; i += 2)
            {
                alwaysfront.Tiles[0,i] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI);
                alwaysfront.Tiles[1,i] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 1);
                alwaysfront.Tiles[0,i+1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 25);
                alwaysfront.Tiles[1,i+1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 26);
            }

            cI = 994;
            for (int i = 3; i < 14; i += 2)
            {
                alwaysfront.Tiles[28, i] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI);
                alwaysfront.Tiles[29, i] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 1);
                alwaysfront.Tiles[28, i + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 25);
                alwaysfront.Tiles[29, i + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 26);
            }

            cI = 994;
            for (int i = 17; i < 27; i += 2)
            {
                alwaysfront.Tiles[38, i] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI);
                alwaysfront.Tiles[39, i] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 1);
                alwaysfront.Tiles[38, i + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 25);
                alwaysfront.Tiles[39, i + 1] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 26);
            }

            cI = 1042;
            for (int i = 3; i < 37; i += 2)
            {
                alwaysfront.Tiles[i, 28] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI);
                alwaysfront.Tiles[i + 1, 28] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 1);
                alwaysfront.Tiles[i, 29] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 25);
                alwaysfront.Tiles[i + 1, 29] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, cI + 26);
            }*/

            newMap.LoadTileSheets(Game1.mapDisplayDevice);

            this.map = newMap;

        }

        public override void updateWarps()
        {
            warps.Clear();

            if(warpSets.Count > 0)
            {

                foreach(Warp warpSet in warpSets)
                {

                    warps.Add(warpSet);

                }

                return;

            }

            Mod.instance.Monitor.Log("madeGroveWarps", LogLevel.Debug);

            Layer back = map.GetLayer("Back");

            int width = back.LayerWidth;

            int height = back.LayerHeight;

            Vector2 caveEntry = Vector2.Zero;

            Vector2 farmExit = Vector2.Zero;

            Vector2 caveExit = Vector2.Zero;

            Vector2 farmEntry = Vector2.Zero;

            GameLocation farmLocation = Game1.getFarm();

            for (int w = farmLocation.warps.Count - 1; w >= 0; w--)
            {

                Warp warp = farmLocation.warps[w];

                if (warp.TargetName == "FarmCave")
                {

                    caveEntry = new Vector2(warp.TargetX, warp.TargetY);

                    farmExit = new Vector2(warp.X, warp.Y);

                    Warp change = new((int)farmExit.X, (int)farmExit.Y, LocationData.druid_grove_name, 29, 18, false);

                    WarpBack save = new()
                    {
                        warp = warp,
                        location = "Farm",
                        index = w,
                    };

                    warpBacks.Add(save);

                    farmLocation.warps[w] = change;

                }
            }

            GameLocation caveLocation = Game1.getLocationFromName("FarmCave");

            for (int w = caveLocation.warps.Count - 1; w >= 0; w--)
            {

                Warp warp = Game1.getLocationFromName("FarmCave").warps[w];

                if (warp.TargetName == "Farm")
                {

                    farmEntry = new Vector2(warp.TargetX, warp.TargetY);

                    caveExit = new Vector2(warp.X, warp.Y);

                    Warp change = new((int)caveExit.X, (int)caveExit.Y, LocationData.druid_grove_name, 29, 8, false);

                    WarpBack save = new()
                    {
                        warp = warp,
                        location = "FarmCave",
                        index = w,
                    };

                    warpBacks.Add(save);

                    caveLocation.warps[w] = change;

                }
            }

            // --------------------------------------
            // Grove warps

            warpSets.Add(new Warp(29, 6, "Farmcave", (int)caveEntry.X, (int)caveEntry.Y, flipFarmer: false));

            warpSets.Add(new Warp(28, 21, "Farm", (int)farmEntry.X, (int)farmEntry.Y, flipFarmer: false));

            warpSets.Add(new Warp(29, 21, "Farm", (int)farmEntry.X, (int)farmEntry.Y, flipFarmer: false));

            warpSets.Add(new Warp(30, 21, "Farm", (int)farmEntry.X, (int)farmEntry.Y, flipFarmer: false));

            foreach (Warp warpSet in warpSets)
            {

                warps.Add(warpSet);

            }

        }

    }

    public class WarpBack
    {

        public Warp warp;

        public string location;

        public int index;

    }

}
