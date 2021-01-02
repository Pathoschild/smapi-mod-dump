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

namespace NewGamePlus.Framework
{
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        public readonly Dictionary<string, bool> Config = new Dictionary<string, bool>
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

        public string Separator = "FOLLOWING LINES ARE NOT MEANT TO BE EDITED MANUALLY";
        public Dictionary<string, object> flags = new Dictionary<string, object>();
        public SortedSet<int> Professions { get; set; } = new SortedSet<int>();
        public int[] Experience = new int[6];
        public List<string> Stardrops { get; set; } = new List<string>();
        public List<string> CraftingRecipes { get; set; } = new List<string>();
        public List<string> CookingRecipes { get; set; } = new List<string>();


        /*********
        ** Public methods
        *********/
        public bool GetConfig(string key, bool defValue = true)
        {
            if (Config.ContainsKey(key))
                return Config[key];
            return defValue;
        }

        public void SetFlag(string key, object value)
        {
            flags.Remove(key);
            if (value != null)
                flags.Add(key, Base64Encode(value.ToString()));
        }

        public void SetFlagIfGreater(string key, decimal value)
        {
            if (flags.ContainsKey(key))
                SetFlag(key, Math.Max(value, int.Parse(GetFlag(key, "0"))));
            else
                SetFlag(key, value);
        }

        public string GetFlag(string key, string def = null)
        {
            if (flags.ContainsKey(key))
                return Base64Decode(flags[key].ToString());
            return def;
        }

        public decimal GetFlagDecimal(string key, decimal dflt = 0)
        {
            decimal ret = dflt;
            if (flags.ContainsKey(key))
                decimal.TryParse(Base64Decode(flags[key].ToString()), out ret);
            return ret;
        }

        public bool GetFlagBoolean(string key, bool dflt = false)
        {
            bool ret = dflt;
            if (flags.ContainsKey(key))
                return bool.TryParse(Base64Decode(flags[key].ToString()), out ret);
            return ret;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
