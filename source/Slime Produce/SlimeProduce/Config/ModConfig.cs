/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andraemon/SlimeProduce
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimeProduce
{
    public class ModConfig
    {
        public bool EnableSpecialWhiteSlimeDrops { get; set; }
        public bool EnableSpecialPurpleSlimeDrops { get; set; }
        public bool EnableSpecialTigerSlimeDrops { get; set; }
        public bool EnableSpecialColorDrops { get; set; }
        public int SpecialColorMinDrop { get; set; }
        public int SpecialColorMaxDrop { get; set; }
        public double SpecialColorDropChance { get; set; }
        public List<DropTable> DropTables { get; set; }

        public ModConfig()
        {
            EnableSpecialWhiteSlimeDrops = true;
            EnableSpecialPurpleSlimeDrops = true;
            EnableSpecialTigerSlimeDrops = true;
            EnableSpecialColorDrops = false;
            SpecialColorDropChance = 0.05;
            SpecialColorMinDrop = 1;
            SpecialColorMaxDrop = 1;
            DropTables = new List<DropTable>
            {
                new DropTable(new ColorRange(new int[] { 0, 79 }, new int[] { 0, 79 }, new int[] { 0, 79 }),
                new List<ItemDrop>() { new ItemDrop(382, 5, 10), new ItemDrop(553, 1, 2, 0.05), new ItemDrop(539, 1, 2, 0.05) }),
                new DropTable(new ColorRange(new int[] { 201, 255 }, new int[] { 181, 255 }, new int[] { 0, 49 }),
                new List<ItemDrop>() { new ItemDrop(384, 10, 20) }),
                new DropTable(new ColorRange(new int[] { 221, 255 }, new int[] { 91, 149 }, new int[] { 0, 49 }),
                new List<ItemDrop>() { new ItemDrop(378, 10, 20) }),
                new DropTable(new ColorRange(new int[] { 151, 255 }, new int[] { 151, 255 }, new int[] { 151, 255 }),
                new List<ItemDrop>() { new ItemDrop(390, 20, 40) })
            };
        }
    }
}
