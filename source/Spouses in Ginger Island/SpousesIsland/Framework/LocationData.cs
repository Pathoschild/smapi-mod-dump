/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

namespace SpousesIsland.Framework
{
        public class Location1Data
    {
        public string Name { get; set; }
        public int Time { get; set; }
        public string Position { get; set; }
        public string Dialogue { get; set; }

        public Location1Data()
        {
        }

        public Location1Data(Location1Data loc1)
        {
            Name = loc1.Name;
            Time = loc1.Time;
            Position = loc1.Position;
            Dialogue = loc1.Dialogue;
        }
    }

    public class Location2Data
    {
        public string Name;
        public int Time;
        public string Position;
        public string Dialogue;

        public Location2Data()
        {
        }

        public Location2Data(Location2Data loc2)
        {
            Name = loc2.Name;
            Time = loc2.Time;
            Position = loc2.Position;
            Dialogue = loc2.Dialogue;
        }
    }

    public class Location3Data
    {
        public string Name;
        public int Time;
        public string Position;
        public string Dialogue;

        public Location3Data()
        {
        }

        public Location3Data(Location1Data loc3)
        {
            Name = loc3.Name;
            Time = loc3.Time;
            Position = loc3.Position;
            Dialogue = loc3.Dialogue;
        }
    }
}
