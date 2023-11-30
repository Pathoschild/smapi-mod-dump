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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Cast
{
    internal class Throw
    {

        private Farmer targetPlayer;

        public StardewValley.Item objectInstance;

        public int objectIndex;

        public int objectQuality;

        public Vector2 throwPosition;

        public Vector2 catchPosition;

        public bool itemDebris;

        public Throw(Farmer Player, Vector2 Position, int ObjectIndex, int ObjectQuality)
        {
            
            objectIndex = ObjectIndex;

            objectQuality = ObjectQuality;

            objectInstance = new StardewValley.Object(objectIndex, 1, false, -1, objectQuality);

            targetPlayer = Player;

            throwPosition = Position;

            catchPosition = targetPlayer.Position;

        }
        
        public Throw(Farmer Player, Vector2 Catch, StardewValley.Object Extract, Vector2 Throw)
        {

            objectIndex = Extract.ParentSheetIndex;
            
            targetPlayer = Player;

            throwPosition = Throw;

            objectInstance = Extract;

            catchPosition = Catch;

        }

        public void ThrowObject()
        {

            /*
             * compensate       compensate for downward arc // 555 seems a nice substitute for 0.001 compounded 1000 times
             * 
             * motion           the movement of the animation every millisecond
             * 
             * acceleration     positive Y movement every millisecond creates a downward arc
             * 
             */

            Vector2 motionPlayer = targetPlayer.getMostRecentMovementVector() / 33.33f;

            Rectangle targetRectangle = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, objectIndex, 16, 16);

            float animationInterval = 1000f;

            float xOffset = (catchPosition.X - throwPosition.X);

            float yOffset = (catchPosition.Y - throwPosition.Y);

            float motionX =  xOffset / 1000;

            float compensate = 0.555f;

            float motionY = ( yOffset / 1000) - compensate;

            TemporaryAnimatedSprite throwAnimation = new("Maps\\springobjects", targetRectangle, animationInterval, 1, 0, throwPosition, flicker: false, flipped: false, throwPosition.Y / 10000f, 0.001f, Color.White, 3f, 0f, 0f, 0f)
            {

                motion = new Vector2(motionX, motionY) + motionPlayer,

                acceleration = new Vector2(0f, 0.001f),

                timeBasedMotion = true,

                endFunction = InventoriseObject,

            };

            targetPlayer.currentLocation.temporarySprites.Add(throwAnimation);

        }

        public void InventoriseObject(int endBehaviour)
        {

            if(itemDebris && objectInstance is StardewValley.Object)
            {

                Game1.createItemDebris(objectInstance.getOne(), catchPosition + new Vector2(32, 32), -1);

                return;

            }

            if (!targetPlayer.addItemToInventoryBool(objectInstance)) // if unable to add to inventory spawn as debris
            {

                Vector2 spawnVector = targetPlayer.getTileLocation();

                if(objectInstance is StardewValley.Object)
                {

                    Game1.createObjectDebris(objectInstance.ParentSheetIndex, (int)spawnVector.X, (int)spawnVector.Y);

                }
                else
                {

                    targetPlayer.dropItem(objectInstance);

                }


                objectInstance = null;

            }

        }


    }

}
