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
using StardewValley;
using StardewValley.Tools;
using System;

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

        public float throwFade;

        public float throwHeight;

        public float throwScale;

        public bool dontInventorise;

        public Throw()
        {
        }

        public Throw(Farmer Player, Vector2 Position, int ObjectIndex, int ObjectQuality = 0)
        {

            objectIndex = ObjectIndex;

            objectQuality = ObjectQuality;

            if(objectQuality == 3)
            {

                objectQuality = 2;

            }
            else if(objectQuality > 4)
            {

                objectQuality = 4;

            }
            else if(objectQuality < 0)
            {

                objectQuality = 0;

            }

            objectInstance = new StardewValley.Object(objectIndex.ToString(), 1, false, -1, objectQuality);

            targetPlayer = Player;

            throwPosition = Position;

            catchPosition = Player.Position;

            throwFade = 0.001f;

            throwHeight = 1;

            throwScale = 3f;

        }

        public Throw(Farmer Player, Vector2 Catch, StardewValley.Object Extract, Vector2 Throw)
        {

            objectIndex = Extract.ParentSheetIndex;

            targetPlayer = Player;

            throwPosition = Throw;

            objectInstance = Extract;

            catchPosition = Catch;

            throwFade = 0.001f;

            throwHeight = 1;

            throwScale = 4f;

        }

        public void UpdateQuality(int updateQuality)
        {

            objectInstance = new StardewValley.Object( objectIndex.ToString(), 1, false, -1, updateQuality);

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

            Rectangle targetRectangle = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, objectIndex, 16, 16);

            float animationInterval = 1000f;

            float xOffset = (catchPosition.X - throwPosition.X);

            float yOffset = (catchPosition.Y - throwPosition.Y - 64);

            float motionX = xOffset / 1000;

            float compensate = 0.505f * throwHeight;

            float motionY = (yOffset / 1000) - compensate;

            TemporaryAnimatedSprite throwAnimation = new("Maps\\springobjects", targetRectangle, animationInterval, 1, 0, throwPosition, flicker: false, flipped: false, throwPosition.Y / 10000f, throwFade, Color.White, throwScale, 0f, 0f, 0f);

            throwAnimation.motion = new Vector2(motionX, motionY);

            throwAnimation.acceleration = new Vector2(0f, 0.001f * throwHeight);

            throwAnimation.timeBasedMotion = true;

            throwAnimation.endFunction = InventoriseObject;

            targetPlayer.currentLocation.temporarySprites.Add(throwAnimation);

        }

        public void AnimateObject(int delay = 0)
        {
            Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, objectIndex, 16, 16);

            float num1 = 1000f;

            float num2 = catchPosition.X - throwPosition.X;

            float num3 = catchPosition.Y - throwPosition.Y;

            float num4 = num2 / 1000f;

            float num5 = 0.505f * throwHeight;

            float num6 = num3 / 1000f - num5;

            this.itemDebris = true;

            TemporaryAnimatedSprite throwAnimation = new("Maps\\springobjects", standardTileSheet, num1, 1, 0, throwPosition, false, false, 999f, 0.0f, Color.White, throwScale, 0.0f, 0.0f, 0.0f, false);

            throwAnimation.motion = new Vector2(num4, num6);

            throwAnimation.acceleration = new Vector2(0.0f, 0.001f * throwHeight);

            throwAnimation.timeBasedMotion = true;

            throwAnimation.delayBeforeAnimationStart = delay;

            throwAnimation.endFunction = InventoriseObject;

            targetPlayer.currentLocation.temporarySprites.Add(throwAnimation);

        }

        public void InventoriseObject(int endBehaviour)
        {

            if (dontInventorise)
            {

                return;

            }

            if (itemDebris && objectInstance is StardewValley.Object)
            {

                Game1.createItemDebris(objectInstance.getOne(), catchPosition + new Vector2(32, 32), -1);

                return;

            }

            if (!targetPlayer.addItemToInventoryBool(objectInstance)) // if unable to add to inventory spawn as debris
            {

                Vector2 spawnVector = targetPlayer.Tile;

                if (objectInstance is StardewValley.Object)
                {

                    Game1.createObjectDebris( objectInstance.ParentSheetIndex.ToString(), (int)spawnVector.X, (int)spawnVector.Y,-1,objectQuality);

                }
                else
                {

                    targetPlayer.dropItem(objectInstance);

                }

                objectInstance = null;

            }

        }

        public void ThrowSword(Farmer player, int swordIndex, Vector2 originVector, int delayThrow = 200)
        {
            this.targetPlayer = player;
            this.objectIndex = swordIndex;
            int num1 = this.objectIndex % 8;
            int num2 = (this.objectIndex - num1) / 8;
            Rectangle rectangle = new(num1 * 16, num2 * 16, 16, 16);
            Vector2 vector2 = new(originVector.X * 64f, (float)(originVector.Y * 64.0 - 96.0));
            Vector2 position = targetPlayer.Position;
            float num3 = 1000f;
            float num4 = (float)((position.X - (double)vector2.X) / 1000.0);
            float num5 = 0.555f;
            float num6 = (float)((position.Y - (double)vector2.Y) / 1000.0) - num5;
            float num7 = (float)(originVector.X * 1000.0 + originVector.Y + 20.0);
            targetPlayer.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\weapons", rectangle, num3, 1, 0, vector2, false, false, num7, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.2f, false)
            {
                motion = new Vector2(num4, num6),
                acceleration = new Vector2(0.0f, 1f / 1000f),
                timeBasedMotion = true,
                endFunction = CatchSword,
                delayBeforeAnimationStart = delayThrow
            });
        }

        public void CatchSword(int EndBehaviour)
        {
            this.targetPlayer.addItemByMenuIfNecessaryElseHoldUp(new MeleeWeapon("(W)" + objectIndex.ToString()), null);
        }

    }

}
