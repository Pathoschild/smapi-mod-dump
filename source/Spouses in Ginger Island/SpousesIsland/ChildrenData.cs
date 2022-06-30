/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace SpousesIsland
{
    internal class ChildrenData
    {
        /*if bed is furniture*/
        internal static void ChildSDV(ChildSchedule ChildInfo, IAssetData asset)
        {
            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
            data["Mon"] = $"620 IslandFarmHouse 20 10 3/1100 {ChildInfo.L1} {ChildInfo.X1} {ChildInfo.Y1}/1400 {ChildInfo.L2} {ChildInfo.X2} {ChildInfo.Y2}/1700 {ChildInfo.L3} {ChildInfo.X3} {ChildInfo.Y3}/1900 IslandFarmHouse 15 12 0/2000 IslandFarmHouse 30 15 2";
            data["Tue"] = "GOTO Mon";
            data["Wed"] = "GOTO Mon";
            data["Thu"] = "GOTO Mon";
            data["Fri"] = "GOTO Mon";
            data["Sat"] = "GOTO Mon";
            data["Sun"] = "GOTO Mon";
        }

        /*if bed is mod's*/
        internal static void ChildMOD(ChildSchedule ChildInfo, IAssetData asset)
        {
            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
            data["Mon"] = $"620 IslandFarmHouse 20 10 3/1100 {ChildInfo.L1} {ChildInfo.X1} {ChildInfo.Y1}/1400 {ChildInfo.L2} {ChildInfo.X2} {ChildInfo.Y2}/1700 {ChildInfo.L3} {ChildInfo.X3} {ChildInfo.Y3}/1900 IslandFarmHouse 15 12 0/2000 IslandFarmHouse 30 15 2/2100 IslandFarmHouse 35 14 3";
            data["Tue"] = "GOTO Mon";
            data["Wed"] = "GOTO Mon";
            data["Thu"] = "GOTO Mon";
            data["Fri"] = "GOTO Mon";
            data["Sat"] = "GOTO Mon";
            data["Sun"] = "GOTO Mon";
        }
    }

    internal class ChildSchedule
    {
        public string L1 { get; set; }
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public string L2 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public string L3 { get; set; }
        public int X3 { get; set; }
        public int Y3 { get; set; }
    }
}