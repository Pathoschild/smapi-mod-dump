/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Xml.Serialization;
using StardewValley;

namespace MoreBuildings.Buildings.MiniSpa
{
    [XmlType("Mods_spacechase0_MiniSpaLocation")]
    public class MiniSpaLocation : GameLocation
    {
        public MiniSpaLocation()
            : base("Maps\\MiniSpa", "MiniSpa") { }

        protected override void resetLocalState()
        {
            Game1.player.changeIntoSwimsuit();
            Game1.player.swimming.Value = true;
        }

        public override int getExtraMillisecondsPerInGameMinuteForThisLocation()
        {
            return 7000;
        }
    }
}
