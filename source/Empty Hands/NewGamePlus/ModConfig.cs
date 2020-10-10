/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/quicksilverfox/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGamePlus
{
    public class NewGamePlusConfig
    {
        public Dictionary<string, bool> config = new Dictionary<string, bool>()
        {
            ["professions"] = true,
            ["experience"] = false,
            ["stardrops"] = true,
            ["crafting_recipes"] = true,
            ["cooking_recipes"] = true,
            ["newgame_tools"] = true,
            ["newgame_money"] = true,
            ["newgame_assets"] = true,
        };
        
        public bool GetConfig(string key, bool defValue = true)
        {
            if (config.ContainsKey(key))
                return config[key];
            return defValue;
        }

        public string separator = "DO NOT EDIT FOLLOWING LINES MANUALLY";

        public Dictionary<string, Object> flags = new Dictionary<string, Object>();
        public SortedSet<int> Professions { get; set; } = new SortedSet<int>();
        public int[] Experience = new int[6];
        public List<string> Stardrops { get; set; } = new List<string>();
        public List<string> CraftingRecipes { get; set; } = new List<string>();
        public List<string> CookingRecipes { get; set; } = new List<string>();

        public void SetFlag(string key, Object value)
        {
            flags.Remove(key);
            flags.Add(key, value);
        }

        public void SetFlagIfGreater(string key, decimal value)
        {
            if (flags.ContainsKey(key))
                SetFlag(key, Math.Max(value, int.Parse(GetFlag(key, 0).ToString())));
            else
                SetFlag(key, value);
        }

        public Object GetFlag(string key, Object def = null)
        {
            if (flags.ContainsKey(key))
                return flags[key];
            return def;
        }

        public decimal GetFlagDecimal(string key, decimal dflt = 0)
        {
            decimal ret = dflt;
            if (flags.ContainsKey(key))
                decimal.TryParse(flags[key].ToString(), out ret);
            return ret;
        }

        public bool GetFlagBoolean(string key, bool dflt = false)
        {
            bool ret = dflt;
            if (flags.ContainsKey(key))
                return Boolean.TryParse(flags[key].ToString(), out ret);
            return ret;
        }
    }
}
