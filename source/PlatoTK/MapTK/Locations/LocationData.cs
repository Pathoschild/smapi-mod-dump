/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

namespace MapTK.Locations
{
    internal class LocationData
    {
        public string Name { get; set; }

        public string MapPath { get; set; }

        public string Type { get; set; } = "default";

        public bool Save { get; set; } = false;

        public bool Farm { get; set; } = false;

        public bool Greenhouse { get; set; } = false;

        public string Season { get; set; } = "auto";
    }
}
