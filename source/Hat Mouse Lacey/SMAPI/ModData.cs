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
using System.Linq;
using System.Collections.Generic;

namespace ichortower_HatMouseLacey
{
    /*
     * Keeps track of player data:
     *   - How mean you have been to Lacey in her heart events
     *   - Which hats you have shown her and received comments on
     *
     * Data is stored in Farmer.modData, but also kept in memory to avoid
     * thrashing too hard, since storing and retrieving data means serializing
     * to and from a string.
     */
    internal class LCModData
    {
        private static HashSet<string> _hatsShown = null!;
        private static int _crueltyScore = -1;
        private static string lc = ModEntry.LCInternalName;

        public static void ClearCache()
        {
            _hatsShown = null;
            _crueltyScore = -1;
        }

        public static void AddShownHat(string name)
        {
            if (HasShownHat(name)) {
                return;
            }
            _hatsShown.Add(name);
            string serial = "[" + String.Join(",", _hatsShown.ToArray()
                    .Select(s => $"\"{s}\"")) + "]";
            Game1.player.modData[$"{lc}/HatsShown"] = serial;
        }

        public static bool HasShownHat(string name)
        {
            Load();
            return _hatsShown.Contains(name);
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
