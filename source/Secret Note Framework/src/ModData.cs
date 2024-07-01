/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/SecretNoteFramework
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace ichortower.SNF
{
    internal class ModData
    {
        private static string _dataKey = $"{SNF.ModId}/NotesSeen";
        private static Dictionary<Farmer, HashSet<string>> _cache = new();
        private static Dictionary<Farmer, Timer> _writeTimers = new();
        private static int _writeWindow = 100;

        public static void ClearCache()
        {
            Log.Trace("Clearing seen-note cache");
            _cache.Clear();
        }

        public static void Load(Farmer who)
        {
            if (_cache.ContainsKey(who)) {
                return;
            }
            _cache[who] = new HashSet<string>();
            string serial;
            if (!who.modData.TryGetValue(_dataKey, out serial)) {
                return;
            }
            string[] items = serial.Trim('[',']').Split(",")
                    .Select(s => s.Trim('"')).ToArray();
            _cache[who].UnionWith(items);
        }

        private static void Write(Farmer who)
        {
            if (_writeTimers.ContainsKey(who)) {
                _writeTimers[who].Stop();
                _writeTimers[who].Dispose();
                _writeTimers.Remove(who);
            }
            Timer t = new(_writeWindow);
            t.Elapsed += delegate(object sender, ElapsedEventArgs e) {
                string serial = "[" + string.Join(",", _cache[who].ToArray()
                        .Select(s => $"\"{s}\"")) + "]";
                who.modData[_dataKey] = serial;
                t.Stop();
                t.Dispose();
                _writeTimers.Remove(who);
            };
            t.AutoReset = false;
            t.Enabled = true;
            _writeTimers[who] = t;
        }

        public static bool HasNote(Farmer who, string id)
        {
            Load(who);
            return _cache[who].Contains(id);
        }

        public static bool AddNote(Farmer who, string id)
        {
            Load(who);
            bool ret = _cache[who].Add(id);
            if (ret) {
                Write(who);
            }
            return ret;
        }

        public static bool RemoveNote(Farmer who, string id)
        {
            Load(who);
            bool ret = _cache[who].Remove(id);
            if (ret) {
                Write(who);
            }
            return ret;
        }

        /*
         * Returns the given farmer's seen mod notes in CP token format
         * (comma-separated list of strings).
         */
        public static string NotesAsToken(Farmer who)
        {
            Load(who);
            return string.Join(", ", _cache[who].ToArray());
        }
    }
}
