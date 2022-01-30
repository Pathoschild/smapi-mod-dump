/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/


namespace ModdedMinecarts.Assets
{
    internal class Minecart
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Direction { get; set; }
        public string[] Flags { get; set; }


        public Minecart(string name, string location, int x, int y, int dir)
        {
            Name = name;
            Location = location;
            PosX = x;
            PosY = y;
            Direction = dir;
        }
    }
}
