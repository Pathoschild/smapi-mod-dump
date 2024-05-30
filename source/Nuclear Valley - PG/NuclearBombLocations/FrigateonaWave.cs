/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ApryllForever/NuclearBombLocations
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;








namespace NuclearBombLocations
{
    [XmlType("Mods_ApryllForever_NuclearBombLocations_FrigateOnAWave")]
    internal class FrigateOnAWave : Furniture
    {

        public FrigateOnAWave() { }


            public HashSet<Vector2> lightGlowPositionList = new HashSet<Vector2>();

         

          

            public void DoInit()
            {
                this.name = "ApryllForever.NuclearBombCP_FrigateOnAWave";
                this.furniture_type.Value = 8;
               // this.defaultSourceRect.Value = this.sourceRect.Value = data.GetTexture().Rect ?? new Rectangle(0, 0, data.GetTexture().Texture.Width, data.GetTexture().Texture.Height);
                this.boundingBox.Value = new Rectangle(1, 1, 1, 1);

            }

            protected override void initNetFields()
            {
                base.initNetFields();
            }

          

            protected override string loadDisplayName()
            {
                return "Frigate On A Wave";
            }

            public override string getDescription()
            {
                return "";
            }

            public override string getCategoryName()
            {
                return "decor";
            }

          

         

           

            /// <inheritdoc />
        

            public override bool DoesTileHaveProperty(int tile_x, int tile_y, string property_name, string layer_name, ref string property_value)
            {
                var currConfig = "";
                Vector2 key = new Vector2((int)(tile_x - this.tileLocation.X), (int)(tile_y - this.tileLocation.Y));
               
                    if (Game1.player != null)
                    {
                       // property_value = currConfig.TileProperties[key][layer_name][property_name];
                        return true;
                    }
                
                return false;
            }



}
}
