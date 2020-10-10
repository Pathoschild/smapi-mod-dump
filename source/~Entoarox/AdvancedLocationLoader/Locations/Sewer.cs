/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.Xml.Serialization;

namespace Entoarox.AdvancedLocationLoader.Locations
{
    [XmlType("ALLSewer")]
    public class Sewer : StardewValley.Locations.Sewer
    {
        /*********
        ** Public methods
        *********/
        public Sewer() { }

        public Sewer(string mapPath, string name)
            : base(mapPath, name) { }


        /*********
        ** Protected methods
        *********/
        protected override void resetLocalState()
        {
            base.resetLocalState();
            this.characters.Clear();
        }
    }
}
