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
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using xTile.Dimensions;
using static StardewValley.Minigames.TargetGame;

namespace StardewDruid.Event
{
    public class TriggerHandle
    {

        public readonly Map.Quest questData;

        public int activeCounter;

        public GameLocation targetLocation;

        public Vector2 targetVector;

        public List<TemporaryAnimatedSprite> targets;

        public List<TemporaryAnimatedSprite> animationList;

        public Color animationColor;

        public bool animationRotate;

        public List<StardewDruid.Character.Character> actors;

        public Vector2 voicePosition;

        public TriggerHandle(GameLocation location, Map.Quest quest)
        {

            questData = quest;

            targetLocation = location;

            targets = new List<TemporaryAnimatedSprite>();

            animationList = new List<TemporaryAnimatedSprite>();

            targetVector = new(0);

            animationRotate = false;

            animationColor = new Color(0, 1f, 0, 1f);

            if(quest.triggerColour != Color.White)
            {

                animationColor = quest.triggerColour;

            }

            actors = new();

            voicePosition = new(0);

        }

        public virtual bool SetMarker()
        {

            if (questData.triggerVector != Vector2.Zero)
            {

                targetVector = questData.triggerVector;

                return true;

            }

            Vector2 specialTrigger = QuestData.SpecialVector(targetLocation, questData.name);

            if (specialTrigger != new Vector2(-1))
            {

                targetVector = specialTrigger;

                return true;

            }

            return false;

        }

        public virtual bool CheckMarker(Rite rite)
        {

            if (questData.triggerBlessing != null)
            {

                if (rite.castType != questData.triggerBlessing)
                {

                    return false;

                }

            }

            if (questData.startTime != 0)
            {

                if (Game1.timeOfDay < questData.startTime)
                {

                    return false;

                }

            }

            if (questData.triggerAnywhere || Vector2.Distance(rite.castVector, targetVector) <= 3f)
            {

                Mod.instance.triggerList.Remove(questData.name);

                Mod.instance.locationPoll["trigger"] = null;

                ModUtility.AnimateHands(Game1.player, Game1.player.FacingDirection, 500);

                Game1.player.currentLocation.playSound("yoba");

                QuestData.QuestHandle(targetVector, rite, questData);

                return true;

            }

            return false;

        }

        public void RemoveAnimations()
        {
            
            if (targets.Count > 0)
            {

                foreach (TemporaryAnimatedSprite targetSprite in targets)
                {

                    targetLocation.temporarySprites.Remove(targetSprite);

                }

            }

            if (animationList.Count > 0)
            {

                foreach (TemporaryAnimatedSprite animatedSprite in animationList)
                {

                    targetLocation.temporarySprites.Remove(animatedSprite);

                }

            }

        }

        public void RemoveActors()
        {

            if (actors.Count > 0)
            {

                foreach (StardewDruid.Character.Character actor in actors)
                {

                    actor.currentLocation.characters.Remove(actor);

                }

                actors.Clear();

            }

        }

        public virtual void EventRemove()
        {

            RemoveActors();

            RemoveAnimations();

        }

        public virtual void EventInterval()
        {

            activeCounter++;

            float activeCycle = activeCounter % 5;

            if (activeCycle != 1)
            {

                return;

            }

            Vector2 animationPosition = targetVector * 64 - new Vector2(32, 32);

            Vector2 animationMotion = new Vector2(0, -0.3f);

            Vector2 animationAcceleration = new Vector2(0f, 0.002f);

            if (targets.Count > 0)
            {

                if (targetLocation.temporarySprites.Contains(targets.First()))
                {

                    targets.First().reset();

                    targets.First().motion = animationMotion;

                    return;

                }

                targets.Clear();

            }

            float animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString()) + 2;

            TemporaryAnimatedSprite targetAnimation = new(0, 5000f, 1, 1, animationPosition, false, false)
            {

                sourceRect = new(0, 0, 64, 64),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Target.png")),

                layerDepth = animationSort,

                scale = 2f,

                motion = animationMotion,

                acceleration = animationAcceleration,

                color = animationColor,

                rotation = animationRotate ? 3.16f : 0f,

            };

            targetLocation.temporarySprites.Add(targetAnimation);

            targets.Add(targetAnimation);

            /*if (activeCounter % 10 >= 5)
            {

                activeCycle = 5 - activeCycle;

            }

            TemporaryAnimatedSprite newAnimation = new(
                "LooseSprites\\Cursors",
                new Rectangle(395, 497, 3, 8),
                500f,
                1,
                1,
                targetVector * 64 - new Vector2(8, 2 * activeCycle),
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
                new Rectangle(395, 497, 3, 8),
                500f,
                1,
                1,
                targetVector * 64 - new Vector2(8, 2 * activeCycle + 1),
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

            targetLocation.temporarySprites.Add(secondAnimation);*/

        }

        public void CastVoice(string message, int duration = 2000)
        {

            if (actors.Count <= 0)
            {

                StardewDruid.Character.Character disembodied = CharacterData.DisembodiedVoice(targetLocation, voicePosition);

                targetLocation.characters.Add(disembodied);

                actors.Add(disembodied);

            }

            actors[0].showTextAboveHead(message, duration: duration);

        }

    }

}
