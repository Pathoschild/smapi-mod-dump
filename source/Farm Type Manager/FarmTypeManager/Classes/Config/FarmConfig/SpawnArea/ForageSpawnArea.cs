/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //a subclass of "SpawnArea" specifically for forage generation, providing the ability to override each area's seasonal forage items
        private class ForageSpawnArea : SpawnArea
        {
            //this subclass was added in version 1.2; defaults are used here to automatically fill it in with SMAPI's json interface

            public object[] SpringItemIndex { get; set; } = null;
            public object[] SummerItemIndex { get; set; } = null;
            public object[] FallItemIndex { get; set; } = null;
            public object[] WinterItemIndex { get; set; } = null;

            //default constructor, providing settings for Forest Farm style forage placement
            public ForageSpawnArea()
                : base()
            {
                MapName = "Farm";
                MinimumSpawnsPerDay = 0;
                MaximumSpawnsPerDay = 3;
                IncludeTerrainTypes = new string[] { "Grass", "Diggable" };
            }
        }
    }
}