/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace StardewDruid.Character
{
    public class Actor : StardewDruid.Character.Character
    {
        public bool drawSlave;

        public Actor()
        {

        }

        public Actor(Vector2 position, string map, string Name)
          : base(position, map, Name)
        {
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (Context.IsMainPlayer && drawSlave)
            {
                foreach (NPC character in currentLocation.characters)
                {
                    //f (!(character is StardewDruid.Character.Actor))
                        character.drawAboveAlwaysFrontLayer(b);
                }
                //this.drawAboveAlwaysFrontLayer(b);
            }
            //base.draw(b, alpha);
        }

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            if (textAboveHeadTimer > 0 && textAboveHead != null)
            {
                
                Vector2 vector = Game1.GlobalToLocal(new Vector2(getStandingX(), getStandingY() -128f));

                SpriteText.drawStringWithScrollCenteredAt(b, textAboveHead, (int)vector.X, (int)vector.Y, "", textAboveHeadAlpha, textAboveHeadColor, 1, 999f);
            
            }

        }

    }

}
