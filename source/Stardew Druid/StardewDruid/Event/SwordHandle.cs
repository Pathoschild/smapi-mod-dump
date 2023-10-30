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
using StardewDruid.Cast;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace StardewDruid.Event
{
    internal class SwordHandle
    {

        private int swordIndex;

        private readonly Quest questData;

        private readonly Rite riteData;

        private readonly Vector2 targetVector;

        public readonly Mod mod;

        public SwordHandle(Mod Mod, Vector2 target, Rite rite, Quest quest)
        {

            targetVector = target;

            mod = Mod;

            riteData = rite;

            questData = quest;

        }

        public void EventTrigger()
        {

            switch (questData.name)
            {

                case "swordStars":

                    SwordStars();

                    break;

                case "swordWater":

                    SwordWater();

                    break;

                default: // CastEarth

                    SwordEarth();

                    break;

            }

        }

        public void SwordEarth()
        {

            string effigyQuestion = "Voices in the Rustle of the Leaves: " +
                "^You refer to the one destined to bring balance to the for- rest... between the light...of Spring... the dark... of Winter ...you believe it's this... farmer?";

            List<Response> effigyChoices = new()
            {
                new Response("trigger", "I've come to pay homage to the Kings of Oak and Holly."),

                new Response("none", "(say nothing)")
            };

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerEarth);

            riteData.castLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour);

        }

        public void AnswerEarth(Farmer effigyVisitor, string effigyAnswer)
        {

            switch (effigyAnswer)
            {
                case "trigger":

                    Game1.activeClickableMenu = new DialogueBox("Two voices speak in unison: \"...Arise...\"");

                    DelayedAction.functionAfterDelay(ThrowEarth, 2000);

                    mod.CompleteQuest("swordEarth");

                    mod.UpdateEffigy("swordEarth");

                    break;

                default:

                    Game1.activeClickableMenu = new DialogueBox("The voices disperse into a rustle of laughter and whispered chants.");

                    break;

            }

            return;

        }

        public void ThrowEarth()
        {

            DialogueBox dialogueBox = Game1.activeClickableMenu as DialogueBox;

            if (dialogueBox != null)
            {

                dialogueBox.closeDialogue();

            }

            //---------------------- throw Forest Sword

            swordIndex = 15;

            int delayThrow = 600;

            Vector2 triggerVector = questData.vectorList["triggerVector"];

            ThrowSword(triggerVector + new Vector2(0, -4), delayThrow);

            //----------------------- cast animation

            ModUtility.AnimateGrowth(riteData.castLocation, triggerVector + new Vector2(-3, -3));
            ModUtility.AnimateGrowth(riteData.castLocation, triggerVector + new Vector2(-3, -4));
            ModUtility.AnimateGrowth(riteData.castLocation, triggerVector + new Vector2(-2, -5));
            ModUtility.AnimateGrowth(riteData.castLocation, triggerVector + new Vector2(-1, -6));
            ModUtility.AnimateGrowth(riteData.castLocation, triggerVector + new Vector2(0, -7));
            ModUtility.AnimateGrowth(riteData.castLocation, triggerVector + new Vector2(1, -7));
            ModUtility.AnimateGrowth(riteData.castLocation, triggerVector + new Vector2(2, -7));
            ModUtility.AnimateGrowth(riteData.castLocation, triggerVector + new Vector2(3, -6));
            ModUtility.AnimateGrowth(riteData.castLocation, triggerVector + new Vector2(4, -5));
            ModUtility.AnimateGrowth(riteData.castLocation, triggerVector + new Vector2(5, -4));
            ModUtility.AnimateGrowth(riteData.castLocation, triggerVector + new Vector2(5, -3));

        }

        public void SwordWater()
        {

            string effigyQuestion = "Voices in the Breaking of the Waves: " +
                "^The river sings again... smish... the spring is clean... smashh... the farmer is friend to the water";

            List<Response> effigyChoices = new()
            {
                new Response("trigger", "I harken to the Voice Beyond the Shore."),

                new Response("none", "(say nothing)")
            };

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerWater);

            riteData.castLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour);

        }

        public void AnswerWater(Farmer effigyVisitor, string effigyAnswer)
        {

            switch (effigyAnswer)
            {
                case "trigger":

                    Game1.activeClickableMenu = new DialogueBox("Voice Beyond the Shore: \"And I Hear Your Voice.\"");

                    riteData.castLocation.playSound("thunder_small");

                    DelayedAction.functionAfterDelay(ThrowWater, 2000);

                    mod.CompleteQuest("swordWater");

                    mod.UpdateEffigy("swordWater");

                    break;

                default:

                    Game1.activeClickableMenu = new DialogueBox("The voices disperse into the gentle rolling of the waves.");

                    break;

            }

            return;

        }

        public void ThrowWater()
        {

            DialogueBox dialogueBox = Game1.activeClickableMenu as DialogueBox;

            if (dialogueBox != null)
            {

                dialogueBox.closeDialogue();

            }

            //---------------------- throw Neptune Glaive

            swordIndex = 14;

            int delayThrow = 600;

            Vector2 originVector = targetVector + new Vector2(3, 4);

            ThrowSword(originVector, delayThrow);

            //----------------------- strike animations

            ModUtility.AnimateBolt(riteData.castLocation, originVector);

        }


        public void SwordStars()
        {

            string effigyQuestion = "Voices in the Roiling Flames: " +
                "Farmer. Shadow slayer. Unburnt one." +
                "^Will you walk under the light?" +
                "^Will you walk over the fire?";

            List<Response> effigyChoices = new()
            {
                new Response("trigger", "May the Lights Beyond the Expanse shine upon me."),

                new Response("none", "(say nothing)")
            };

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerStars);

            riteData.castLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour);

        }

        public void AnswerStars(Farmer effigyVisitor, string effigyAnswer)
        {

            switch (effigyAnswer)
            {
                case "trigger":

                    Game1.activeClickableMenu = new DialogueBox("Voices in the Roiling Flames: \"...Star Caller...\"");

                    DelayedAction.functionAfterDelay(ThrowStars, 2000);

                    mod.CompleteQuest("swordStars");

                    mod.UpdateEffigy("swordStars");

                    break;

                default:

                    Game1.activeClickableMenu = new DialogueBox("The voices disperse into the churning of the lava.");

                    break;

            }

            return;

        }

        public void ThrowStars()
        {

            DialogueBox dialogueBox = Game1.activeClickableMenu as DialogueBox;

            if (dialogueBox != null)
            {

                dialogueBox.closeDialogue();

            }

            //---------------------- throw Lava Katana

            swordIndex = 9;

            int delayThrow = 600;

            Vector2 originVector = targetVector + new Vector2(5, 0);

            ThrowSword(originVector, delayThrow);

            //---------------------- meteor animation

            ModUtility.AnimateMeteor(riteData.castLocation, originVector, true);

            //mod.RemoveTrigger("swordStars");

            //mod.UpdateQuest("swordStars", true);

        }


        public void ThrowSword(Vector2 originVector, int delayThrow = 200)
        {
            /*
             * compensate       compensate for downward arc // 555 seems a nice substitute for 0.001 compounded 1000 times
             * 
             * motion           the movement of the animation every millisecond
             * 
             * acceleration     positive Y movement every millisecond creates a downward arc
             * 
             */

            int swordOffset = swordIndex % 8;

            int swordRow = (swordIndex - swordOffset) / 8;

            Rectangle swordRectangle = new(swordOffset * 16, swordRow * 16, 16, 16);

            Vector2 targetPosition = new(originVector.X * 64, originVector.Y * 64 - 96);

            Vector2 playerPosition = riteData.caster.Position;

            float animationInterval = 1000f;

            float motionX = (playerPosition.X - targetPosition.X) / 1000;

            float compensate = 0.555f;

            float motionY = (playerPosition.Y - targetPosition.Y) / 1000 - compensate;

            float animationSort = originVector.X * 1000 + originVector.Y + 20;

            TemporaryAnimatedSprite throwAnimation = new("TileSheets\\weapons", swordRectangle, animationInterval, 1, 0, targetPosition, flicker: false, flipped: false, animationSort, 0f, Color.White, 4f, 0f, 0f, 0.2f)
            {

                motion = new Vector2(motionX, motionY),

                acceleration = new Vector2(0f, 0.001f),

                timeBasedMotion = true,

                endFunction = CatchSword,

                delayBeforeAnimationStart = delayThrow,

            };

            riteData.castLocation.temporarySprites.Add(throwAnimation);

        }

        public void CatchSword(int EndBehaviour)
        {

            Item targetSword = new MeleeWeapon(swordIndex);

            riteData.caster.addItemByMenuIfNecessaryElseHoldUp(targetSword);

        }

    }
}
