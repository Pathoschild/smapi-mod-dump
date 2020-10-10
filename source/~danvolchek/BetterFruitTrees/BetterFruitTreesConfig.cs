/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace BetterFruitTrees
{
    internal class BetterFruitTreesConfig
    {
        public bool Disable_Fruit_Tree_Junimo_Harvesting { get; set; } = false;

        public bool Wait_To_Harvest_Fruit_Trees_Until_They_Have_Three_Fruits__Then_Harvest_All_Three_At_Once
        {
            get;
            set;
        } = false;

        public bool Allow_Placing_Fruit_Trees_Outside_Farm { get; set; } = false;
        public bool Allow_Dangerous_Planting { get; set; } = false;
    }
}
