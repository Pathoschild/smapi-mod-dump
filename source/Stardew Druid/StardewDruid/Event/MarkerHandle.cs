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
using StardewDruid.Cast;
using StardewDruid.Map;
using StardewDruid.Monster;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
namespace StardewDruid.Event
{
    public class MarkerHandle
    {
        
        public readonly Mod mod;

        public readonly Quest questData;

        public int activeCounter;

        public GameLocation targetLocation;

        public Vector2 targetVector;

        public List<TemporaryAnimatedSprite> animationList;

        public MarkerHandle(Mod Mod,GameLocation location,StardewDruid.Map.Quest quest)
        {
            
            mod = Mod;

            questData = quest;

            targetLocation = location;

            animationList = new List<TemporaryAnimatedSprite>();

            targetVector = new(0);

        }

        public virtual void EventTrigger()
        {

        }

        public virtual void EventRemove()
        {

            if(animationList.Count > 0)
            {

                foreach(TemporaryAnimatedSprite animatedSprite in animationList)
                {

                    targetLocation.temporarySprites.Remove(animatedSprite);

                }

            }

        }

        public virtual void EventInterval()
        {

            activeCounter++;

            float activeCycle = (activeCounter % 5);
            
            if(activeCounter % 10 >= 5)
            {

                activeCycle = 5 - activeCycle;

            }

            TemporaryAnimatedSprite newAnimation = new(
                "LooseSprites\\Cursors",
                new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8),
                500f,
                1,
                1,
                (targetVector * 64) - new Vector2(8,2*activeCycle),
                flicker: false,
                flipped: false,
                0.0001f,
                0f,
                Color.White,
                5f,
                0,
                0f,
                0f
            );

            targetLocation.temporarySprites.Add(newAnimation);

            TemporaryAnimatedSprite secondAnimation = new(
                "LooseSprites\\Cursors",
                new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8),
                500f,
                1,
                1,
                (targetVector * 64) - new Vector2(8, (2*activeCycle)+1),
                flicker: false,
                flipped: false,
                0.0001f,
                0f,
                Color.White,
                5f,
                0,
                0f,
                0f
            )
            {

                delayBeforeAnimationStart = 500

            };

            targetLocation.temporarySprites.Add(secondAnimation);

            //animationList.Add(newAnimation);

            /*if (activeCounter != 1)
            {

                if (activeCounter == 100)
                {

                    activeCounter = 0;

                }

                return;

            }
            TemporaryAnimatedSprite newAnimation = new(
                "TileSheets\\furniture",
                new Rectangle(368,800,48,32),
                99999f,
                1,
                1,
                targetVector * 64 + new Vector2(-24,8),
                flicker: false,
                flipped: false,
                0.0001f,
                0f,
                Color.White*0.25f,
                3f,
                0f,
                0f,
                0f
            );

            targetLocation.temporarySprites.Add(newAnimation);

            animationList.Add(newAnimation);*/

        }

    }
}
