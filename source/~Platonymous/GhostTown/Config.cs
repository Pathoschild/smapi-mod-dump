/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace GhostTown
{
    class Config
    {
        public bool desaturate { get; set; }
        public bool people { get; set; }
        public bool houses { get; set; }
        public bool animals { get; set; }
        public bool critters { get; set; }

        public Config()
        {
            desaturate = true;
            people = true;
            houses = true;
            animals = true;
            critters = true;
        }
    }
}
