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
    [XmlType("Mods_Ivy_OrnithologistsGuild_AntiqueBinoculars")]
    public class AntiqueBinoculars : Binoculars
    {
        public AntiqueBinoculars(): base(ModEntry.DGAContentPack.Find("AntiqueBinoculars"), 6)
        {
        }

        public AntiqueBinoculars(string arg) : this()
        {
            // Required for DGA
        }

        public override bool performUseAction(GameLocation location)
        {
            if (Game1.random.NextDouble() < 0.01)
            {
                Game1.addHUDMessage(new HUDMessage(I18n.Items_AntiqueBinoculars_Message(), HUDMessage.error_type));

                return true;
            }

            return base.performUseAction(location);
        }

        public override Item getOne()
        {
            var ret = new AntiqueBinoculars();
            ret.Quality = this.Quality;
            ret.Stack = 1;
            ret.Price = this.Price;
            ret.ObjectColor = this.ObjectColor;
            ret._GetOneFrom(this);
            return ret;
        }
    }
}

