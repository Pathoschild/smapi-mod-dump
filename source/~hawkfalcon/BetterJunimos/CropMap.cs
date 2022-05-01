/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;

namespace BetterJunimos {
    public static class CropTypes {
        public const string Trellis = "trellis";
        public const string Ground = "ground";
        public const string Flower = "flower";
    }

    public class Maps {
        internal CropMap GetCropMapForHut(JunimoHut hut) {
            var pos = new Vector2(hut.tileX.Value, hut.tileY.Value);
            return HutCropMap.TryGetValue(pos, out CropMap map) ? map : null;
        }

        internal string GetCropForPos(JunimoHut hut, Vector2 pos) {
            return GetCropMapForHut(hut)?.CropTypeAt(hut, pos);
        }

        internal void SetCropMapForHut(JunimoHut hut, CropMap map) {
            var pos = new Vector2(hut.tileX.Value, hut.tileY.Value);
            HutCropMap[pos] = map;
        }

        internal void ClearCropMapForHut(JunimoHut hut) {
            var pos = new Vector2(hut.tileX.Value, hut.tileY.Value);
            if (HutCropMap.ContainsKey(pos)) {
                HutCropMap.Remove(pos);
            }
        }

        private static Dictionary<Vector2, CropMap> HutCropMap = new Dictionary<Vector2, CropMap>();
    }

    public class CropMap {
        public string[,] Map { get; set; }

        public string CropTypeAt(JunimoHut hut, Vector2 pos) {
            if (Map is null) {
                return null;
            }

            if (hut is null) {
                return null;
            }

            int radius = BetterJunimos.Config.JunimoHuts.MaxRadius;

            int dx = (int) pos.X - hut.tileX.Value;
            int dy = (int) pos.Y - hut.tileY.Value;

            int mx = radius - 1 + dx;
            int my = radius - 1 + dy;


            string ct;
            try {
                ct = Map[mx, my];
            }
            catch (IndexOutOfRangeException) {
                // BetterJunimos.SMonitor.Log($"CropTypeAt: [{mx} {my}] out of bounds", LogLevel.Warn);
                ct = null;
            }

            // BetterJunimos.SMonitor.Log($"CropTypeAt: hut [{hut.tileX} {hut.tileY}] pos [{pos.X} {pos.Y}] radius {radius} d [{dx} {dy}] m [{mx} {my}]: {ct}", LogLevel.Debug);
            return ct;
        }
    }
}