using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace MapUtilities.Pseudo3D
{
    public static class LevelHandler
    {
        public static Dictionary<Character, string> levels;

        public static string getLevelForCharacter(Character c)
        {
            if (levels.ContainsKey(c))
            {
                if(c.currentLocation != null && MapHandler.hasLevel(c.currentLocation, levels[c]))
                {
                    return levels[c];
                }
            }
            levels[c] = "Base";
            return levels[c];
        }

        public static string getLevelSuffixForCharacter(Character c)
        {
            if (!levels.ContainsKey(c))
                levels[c] = "Base";
            if(c.currentLocation != null && MapHandler.hasLevel(c.currentLocation, levels[c]))
            {
                if (levels[c].Equals("Base"))
                    return "";
                return "_" + levels[c];
            }
            return "";
        }

        public static void setLevelForCharacter(Character c, string level)
        {
            levels[c] = level;
        }

        public static void initialize()
        {
            levels = new Dictionary<Character, string>();
        }
    }
}
