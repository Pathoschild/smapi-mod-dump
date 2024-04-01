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
using System;
using System.Threading;

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

        public override void LoadOut()
        {
            loadedOut = true;
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            
            if (Context.IsMainPlayer && drawSlave)
            {
                
                foreach (NPC character in currentLocation.characters)
                {
                    
                    character.drawAboveAlwaysFrontLayer(b);
                
                }

            }

        }

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            
            if (textAboveHeadTimer > 0 && textAboveHead != null)
            {
                
                Point standingPixel = base.StandingPixel;
                
                Vector2 vector = Game1.GlobalToLocal(new Vector2(standingPixel.X, standingPixel.Y - 128f));
                
                if (textAboveHeadStyle == 0)
                {
                    vector += new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
                }
                
                Point tilePoint = base.TilePoint;

                SpriteText.drawStringWithScrollCenteredAt(b, textAboveHead, (int)vector.X, (int)vector.Y, "", textAboveHeadAlpha, textAboveHeadColor, 1, (float)(tilePoint.Y * 64) / 10000f + 0.001f + (float)tilePoint.X / 10000f);

            }

        }

        public override void behaviorOnFarmerPushing()
        {

            return;

        }

        public override bool checkAction(Farmer who, GameLocation l)
        {

            return false;

        }

        public override void update(GameTime time, GameLocation location)
        {
            
            if (Context.IsMainPlayer)
            {

                if (shakeTimer > 0)
                {
                    shakeTimer = 0;
                }

                if (textAboveHeadTimer > 0)
                {
                    if (textAboveHeadPreTimer > 0)
                    {
                        textAboveHeadPreTimer -= time.ElapsedGameTime.Milliseconds;
                    }
                    else
                    {
                        textAboveHeadTimer -= time.ElapsedGameTime.Milliseconds;
                        if (textAboveHeadTimer > 500)
                        {
                            textAboveHeadAlpha = Math.Min(1f, textAboveHeadAlpha + 0.1f);
                        }
                        else
                        {
                            textAboveHeadAlpha = Math.Max(0f, textAboveHeadAlpha - 0.04f);
                        }
                    }
                }

                updateEmote(time);

            }

            return;

        }

    }

}
