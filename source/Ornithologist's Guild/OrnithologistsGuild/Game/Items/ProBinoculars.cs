/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System.Xml.Serialization;
using StardewValley;

namespace OrnithologistsGuild.Game.Items
{
    [XmlType("Mods_Ivy_OrnithologistsGuild_ProBinoculars")]
    public class ProBinoculars : Binoculars
    {
        public ProBinoculars(): base(ModEntry.DGAContentPack.Find("ProBinoculars"), 10)
        {
        }

        public ProBinoculars(string arg) : this()
        {
            // Required for DGA
        }

        public override Item getOne()
        {
            var ret = new ProBinoculars();
            ret.Quality = this.Quality;
            ret.Stack = 1;
            ret.Price = this.Price;
            ret.ObjectColor = this.ObjectColor;
            ret._GetOneFrom(this);
            return ret;
        }
    }
}

