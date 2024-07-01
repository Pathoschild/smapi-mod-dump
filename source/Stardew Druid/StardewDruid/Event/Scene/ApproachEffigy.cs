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

        public override void TriggerInterval()
        {

            base.TriggerInterval();

            int actionCycle = triggerCounter % 20;

            if (actors.Count == 0)
            {

                AddActor(0, origin - new Vector2(48, 48));

            }

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

                    Mod.instance.CastDisplay(Mod.instance.Config.riteButtons.ToString() + " with a tool in hand to perform a rite of the Weald");

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

            activeLimit = eventCounter + 302;

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

        public override bool AttemptReset()
        {

            if (Mod.instance.characters.ContainsKey(CharacterHandle.characters.Effigy))
            {

                Mod.instance.characters[CharacterHandle.characters.Effigy].currentLocation.characters.Remove(Mod.instance.characters[CharacterHandle.characters.Effigy]);

                Mod.instance.characters.Remove(CharacterHandle.characters.Effigy);

            }

            ResetToTrigger();

            return true;

        }

        public override void EventCompleted()
        {
            Mod.instance.questHandle.CompleteQuest(eventId);
        }

        public override void EventInterval()
        {

            if(Game1.activeClickableMenu != null)
            {

                activeLimit += 1;

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

                        texture = CharacterHandle.CharacterTexture(CharacterHandle.characters.Effigy),

                        scale = 4.25f,

                        motion = new Vector2(0, 0.32f),

                        rotationChange = 0.1f,

                        timeBasedMotion = true,

                    };

                    location.temporarySprites.Add(EffigyAnimation);

                    break;

                case 3:

                    RemoveActors();

                    CharacterHandle.CharacterLoad(CharacterHandle.characters.Effigy, StardewDruid.Character.Character.mode.scene);

                    narrators = new()
                    {
                        [0] = "The Forgotten Effigy",
                    };

                    companions.Add(0,Mod.instance.characters[CharacterHandle.characters.Effigy]);

                    companions[0].netStandbyActive.Set(false);

                    companions[0].currentLocation = location;

                    companions[0].Position = origin + new Vector2(64,0);

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

                case 10:

                    activeCounter = 100;

                    break;

                case 101:

                    DialogueSetups(companions[0], 2);

                    break;

                case 105:

                    activeCounter = 200;

                    break;

                case 201:

                    DialogueSetups(companions[0], 3);

                    companions[0].TargetEvent(300, companions[0].Position + new Vector2(0, 192));

                    break;

                case 205:

                    activeCounter = 300;

                    break;

                case 301:

                    eventComplete = true;

                    break;

            }

        }

        public override void EventScene(int index)
        {
            switch (index)
            {
                case 300:

                    if (!eventComplete)
                    {

                        activeCounter = Math.Max(300, activeCounter);

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

                    intro = "Meet me in the grove outside, and we will test your aptitude for the otherworld.";

                    DialogueDraw(npc, intro);

                    return;

            }

            List<Response> responseList = new();

            switch (dialogueId)
            {

                default: //introOne

                    responseList.Add(new Response("1a", "Who stuck you in the ceiling?"));
                    responseList.Add(new Response("1a", "I inherited this plot from my grandfather. His notes didn't say anything about a magic scarecrow."));
                    responseList.Add(new Response("1a", "(Say nothing)"));

                    break;

                case 2:

                    responseList.Add(new Response("2a", "I would love to know more about the traditions of my forebearers."));
                    responseList.Add(new Response("2a", "I want to be like the farmers of old and form a circle"));
                    responseList.Add(new Response("2a", "(Say nothing)"));

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

                case "1a":
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