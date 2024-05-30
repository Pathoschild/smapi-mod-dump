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
using StardewDruid.Character;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewDruid.Journal;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;


namespace StardewDruid.Event.Scene
{
    public class ApproachEffigy : EventHandle
    {

        public ApproachEffigy()
        {

        }

        public override bool TriggerMarker()
        {
            
            if (!base.TriggerMarker())
            {

                return false;

            }

            AddActor(0,origin - new Vector2(48, 48));

            return true;

        }

        public override void TriggerInterval()
        {

            base.TriggerInterval();

            int actionCycle = triggerCounter % 20;

            switch (actionCycle)
            {
                case 1:

                    CastVoice(0, "Farmer", duration: 3000);

                    break;

                case 4:

                    CastVoice(0, "You come at last", duration: 3000);

                    break;

                case 7:

                    CastVoice(0, "I'm in the ceiling", duration: 3000);

                    break;

                case 10:

                    CastVoice(0, "Stand here and perform the rite", duration: 3000);

                    Mod.instance.CastMessage(Mod.instance.Config.riteButtons.ToString() + " with a tool in hand to perform a rite of the Weald", -1);

                    break;

                case 13:

                    CastVoice(0, "As the first farmer did long ago", duration: 3000);

                    break;

            }

        }

        public override void EventActivate()
        {

            TriggerRemove();

            eventActive = true;

            location = Game1.player.currentLocation;

            Mod.instance.RegisterEvent(this, eventId, true);

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 600.0;

            ModUtility.AnimateHands(Game1.player, Game1.player.FacingDirection, 600);

            Mod.instance.iconData.DecorativeIndicator(location, Game1.player.Position, IconData.decorations.weald, 4f, new());

            location.playSound("discoverMineral");

            Mod.instance.rite.CastRockfall(true);
            Mod.instance.rite.CastRockfall(true);
            Mod.instance.rite.CastRockfall(true);
            Mod.instance.rite.CastRockfall(true);
            Mod.instance.rite.CastRockfall(true);
            Mod.instance.rite.CastRockfall(true);

        }

        public override void EventAbort()
        {

            CharacterData.CharacterRemove(CharacterData.characters.Effigy);

            base.EventAbort();

        }

        public override void EventRemove()
        {

            if (eventComplete)
            {

                Mod.instance.questHandle.CompleteQuest(eventId);

            }

            base.EventRemove();

        }

        public override void EventInterval()
        {

            if(Game1.activeClickableMenu != null)
            {
                
                expireTime++;

                return;

            }

            activeCounter++;

            switch (activeCounter)
            {

                // ------------------------------------------
                // 1 Beginning
                // ------------------------------------------

                case 1:

                    Vector2 EffigyPosition = origin + new Vector2(128, -640);

                    TemporaryAnimatedSprite EffigyAnimation = new(0, 2000f, 1, 1, EffigyPosition - new Vector2(36, 72), false, false)
                    {

                        sourceRect = new(0, 0, 32, 32),

                        texture = CharacterData.CharacterTexture(CharacterData.characters.Effigy),

                        scale = 4.25f,

                        motion = new Vector2(0, 0.32f),

                        rotationChange = 0.1f,

                        timeBasedMotion = true,

                    };

                    location.temporarySprites.Add(EffigyAnimation);

                    break;

                case 3:

                    RemoveActors();

                    CharacterData.CharacterLoad(CharacterData.characters.Effigy, StardewDruid.Character.Character.mode.scene);

                    narrators = new()
                    {
                        [0] = new("The Forgotten Effigy", Microsoft.Xna.Framework.Color.Green),

                    }; ;

                    companions.Add(0,Mod.instance.characters[CharacterData.characters.Effigy]);

                    companions[0].netStandbyActive.Set(false);

                    companions[0].currentLocation = location;

                    companions[0].Position = origin + new Vector2(128,64);

                    companions[0].currentLocation.characters.Add(companions[0]);

                    companions[0].LookAtTarget(Game1.player.Position);

                    companions[0].eventName = eventId;

                    voices[0] = companions[0];

                    Mod.instance.iconData.ImpactIndicator(location, companions[0].Position, IconData.impacts.impact, 6f, new());

                    location.playSound("explosion");

                    break;

                case 4:

                    DialogueCue(0, "Well done");

                    DialogueLoad(0, 1);

                    break;

                case 6:

                    DialogueNext(companions[0]);

                    break;

                case 101:

                    DialogueSetups(companions[0], 2);

                    break;

                case 201:

                    DialogueSetups(companions[0], 3);

                    companions[0].TargetEvent(203, companions[0].Position + new Vector2(0, 192));

                    break;

                case 203:

                    eventComplete = true;

                    expireEarly = true;

                    break;

            }

        }

        public override void EventScene(int index)
        {
            switch (index)
            {
                case 203:

                    if (!eventComplete)
                    {

                        activeCounter = 202;

                        EventInterval();

                    }

                    break;


            }
        }

        public override void DialogueSetups(StardewDruid.Character.Character npc, int dialogueId)
        {
            
            string intro;

            switch (dialogueId)
            {

                default: // introOne

                    intro = "Forgotten Effigy: So a successor appears. I am the Effigy, crafted by the First Farmer, sustained by the powers of the elderborn, and bored.";

                    break;

                case 2:

                    intro = "Forgotten Effigy: It is difficult to explain the course of events that led to my predicament. First know that the lineage of valley farmers you belong to was once aligned with the otherworld. They formed a circle of Druids.";

                    break;

                case 3:

                    intro = "An interesting proposition. Meet me in the grove outside, and we will test your aptitude for the otherworld.";

                    DialogueDraw(npc, intro);

                    return;

            }

            List<Response> responseList = new();

            switch (dialogueId)
            {

                default: //introOne

                    responseList.Add(new Response("1a", "Who stuck you in the ceiling?"));
                    responseList.Add(new Response("1a", "I inherited this plot from my grandfather. His notes didn't say anything about a magic scarecrow."));
                    responseList.Add(new Response("1b", "(Say nothing)"));

                    break;

                case 2:

                    responseList.Add(new Response("2a", "I want to be like the farmers of old and form a circle"));
                    responseList.Add(new Response("2a", "Do you think I could become...(dramatic pause)... a Stardew Druid?"));
                    responseList.Add(new Response("2b", "(Say nothing)"));

                    break;

            }

            GameLocation.afterQuestionBehavior questionBehavior = new(DialogueResponses);

            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);

            return;

        }

        public override void DialogueResponses(Farmer visitor, string dialogueId)
        {

            switch (dialogueId)
            {

                default:

                    activeCounter = Math.Max(100, activeCounter);

                    break;

                case "2a":

                    activeCounter = Math.Max(200, activeCounter);

                    break;

            }

        }

    }

}