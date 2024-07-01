/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/HatMouseLacey
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace ichortower_HatMouseLacey
{
    /*
     * Keeps track of player data:
     *   - How mean you have been to Lacey in her heart events
     *   - Which hats you have shown her and received comments on
     *
     * Data is stored in Farmer.modData, but also kept in memory. When the
     * list of shown hats changes, a short timer delays the write to modData;
     * this is intended to reduce how many times the set gets serialized to
     * string, which should only matter during save data migration.
     */
    internal class LCModData
    {
        private static HashSet<string> _hatsShown = null!;
        private static int _crueltyScore = -1;
        private static string lc = HML.CPId;
        private static Timer _hatTimer = null;

        public static HashSet<string> HatsShown
        {
            get {
                return _hatsShown;
            }
        }

        public static void ClearCache()
        {
            _hatsShown = null;
            _crueltyScore = -1;
        }

        public static bool AddShownHat(string name)
        {
            Load();
            bool ret = _hatsShown.Add(name);
            if (ret) {
                WriteHatData();
            }
            return ret;
        }

        public static bool RemoveShownHat(string name)
        {
            Load();
            bool ret = _hatsShown.Remove(name);
            if (ret) {
                WriteHatData();
            }
            return ret;
        }

        public static bool HasShownHat(string name)
        {
            Load();
            return _hatsShown.Contains(name);
        }

        public static bool HasShownAnyHat()
        {
            Load();
            return _hatsShown.Count > 0;
        }

        public static bool ClearShownHats()
        {
            Load();
            bool ret = _hatsShown.Count > 0;
            _hatsShown.Clear();
            if (ret) {
                WriteHatData();
            }
            return ret;
        }

        private static void WriteHatData()
        {
            if (_hatTimer != null) {
                _hatTimer.Stop();
                _hatTimer.Dispose();
                _hatTimer = null;
            }
            // 100 ms is still very generous. i expect any batching from the
            // save conversion to be well within this limit
            _hatTimer = new Timer(100);
            _hatTimer.Elapsed += delegate(object sender, ElapsedEventArgs e) {
                string serial = "[" + String.Join(",", _hatsShown.ToArray()
                        .Select(s => $"\"{s}\"")) + "]";
                Game1.player.modData[$"{lc}/HatsShown"] = serial;
                _hatTimer.Stop();
                _hatTimer.Dispose();
                _hatTimer = null;
            };
            _hatTimer.AutoReset = false;
            _hatTimer.Enabled = true;
        }

        private static void Load()
        {
            if (_hatsShown != null) {
                return;
            }
            _hatsShown = new();
            string serial;
            if (!Game1.player.modData.TryGetValue($"{lc}/HatsShown", out serial)) {
                return;
            }
            string[] items = serial.Trim('[',']').Split(",")
                    .Select(s => s.Trim('"')).ToArray();
            foreach (var s in items) {
                _hatsShown.Add(s);
            }
        }

        public static int CrueltyScore
        {
            get {
                if (_crueltyScore == -1) {
                    try {
                        string sScore;
                        if (Game1.player.modData.TryGetValue($"{lc}/CrueltyScore", out sScore)) {
                            _crueltyScore = Convert.ToInt32(sScore);
                        }
                        else {
                            _crueltyScore = 0;
                        }
                    }
                    catch {
                        _crueltyScore = 0;
                    }
                }
                return _crueltyScore;
            }
            set {
                _crueltyScore = value;
                Game1.player.modData[$"{lc}/CrueltyScore"] = Convert.ToString(_crueltyScore);
            }
        }
    }
}
