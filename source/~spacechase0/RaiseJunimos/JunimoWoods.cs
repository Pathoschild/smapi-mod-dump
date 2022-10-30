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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;

namespace RaiseJunimos
{
    [XmlType("Mods_spacechase0_JunimoWoods")]
    public class JunimoWoods : GameLocation
    {
        public readonly NetCollection<RaisableJunimo> junimos = new();

        public JunimoWoods()
        {
        }

        public JunimoWoods( IModContentHelper content )
        : base(content.GetInternalAssetName("assets/JunimoWoods.tmx").BaseName, "Custom_JunimoWoods")
        {
        }

        protected override void initNetFields()
        {
            NetFields.AddField(junimos);
        }

        protected override void resetLocalState()
        {
            ignoreOutdoorLighting.Value = true;
            base.resetLocalState();
            /*
            Game1.drawLighting = true;
            Game1.ambientLight = new Color(65, 170, 65);
            Game1.outdoorLight = Game1.ambientLight;
            */
        }

        protected override void updateCharacters(GameTime time)
        {
            base.updateCharacters(time);
            foreach (var junimo in junimos)
            {
                junimo.currentLocation = this;
                junimo.update(time, this);
            }
        }

        protected override void drawCharacters(SpriteBatch b)
        {
            base.drawCharacters(b);
            foreach (var junimo in junimos)
            {
                junimo.draw(b);
            }
        }
    }
}
