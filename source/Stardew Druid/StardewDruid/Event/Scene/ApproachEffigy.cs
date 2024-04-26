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

                    CastVoice("Farmer", duration: 3000);

                    break;

                case 4:

                    CastVoice("You come at last", duration: 3000);

                    break;

                case 7:

                    CastVoice("I'm in the ceiling", duration: 3000);

                    break;

                case 10:

                    CastVoice("Stand here and perform the rite", duration: 3000);

                    Mod.instance.CastMessage(Mod.instance.Config.riteButtons.ToString() + " with a tool in hand to perform a rite of the Weald", -1);

                    break;

                case 13:

                    CastVoice("As the first farmer did long ago", duration: 3000);

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

            Mod.instance.iconData.DecorativeIndicator(location, Game1.player.Position, IconData.decorations.weald);

            location.playSound("discoverMineral");

            ModUtility.AnimateRockfalls(location, Game1.player.Tile);

        }

        public override void EventAbort()
        {

            CharacterData.CharacterRemove(CharacterData.characters.effigy);

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

                    location.playSound("boulderBreak");

                    location.playSound("boulderBreak");

                    Vector2 EffigyPosition = origin + new Vector2(128, -640);

                    TemporaryAnimatedSprite EffigyAnimation = new(0, 1000f, 1, 1, EffigyPosition - new Vector2(36, 72), false, false)
                    {

                        sourceRect = new(0, 0, 32, 32),

                        texture = CharacterData.CharacterTexture(CharacterData.characters.effigy),

                        scale = 4.25f,

                        motion = new Vector2(0, 0.64f),

                        timeBasedMotion = true,

                    };

                    location.temporarySprites.Add(EffigyAnimation);

                    break;

                case 2:

                    RemoveActors();

                    location.playSound("boulderBreak");

                    location.playSound("boulderBreak");

                    CharacterData.CharacterLoad(CharacterData.characters.effigy, StardewDruid.Character.Character.mode.scene);

                    narrators = new()
                    {
                        [0] = new("The Forgotten Effigy", Microsoft.Xna.Framework.Color.Green),

                    }; ;

                    companions.Add(0,Mod.instance.characters[CharacterData.characters.effigy]);

                    companions[0].netStandbyActive.Set(false);

                    companions[0].currentLocation = location;

                    companions[0].Position = origin + new Vector2(128,64);

                    companions[0].currentLocation.characters.Add(companions[0]);

                    companions[0].LookAtTarget(Game1.player.Position);

                    companions[0].eventName = eventId;

                    DialogueCue(0, "Well done");

                    DialogueLoad(0, 1);

                    break;

                case 4:

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

                    intro = "So the successor appears. I am the Effigy of the First Farmer, and the sole remnant of my circle of Druids.";

                    break;

                case 2:

                    intro = "The Effigy: ^That is a matter for another time. The lineage of valley farmers was once aligned with the otherworld. If you intend to become a Stardew Druid, you will need to learn many lessons.";

                    break;

                case 3:


                    intro = "Meet me in the grove outside, and your journey will begin.";

                    npc.CurrentDialogue.Push(new(npc, "0", intro));

                    Game1.drawDialogue(npc);

                    return;

            }

            List<Response> responseList = new();

            switch (dialogueId)
            {

                default: //introOne

                    responseList.Add(new Response("1a", "Who stuck you in the ceiling?"));
                    responseList.Add(new Response("1b", "(Say nothing)"));

                    break;

                case 2:

                    responseList.Add(new Response("2a", "(start journey) Ok. What is the first lesson?"));
                    responseList.Add(new Response("2b", "(Say nothing)"));

                    break;

            }

            GameLocation.afterQuestionBehavior questionBehavior = new(DialogueResponses);

            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);

            return;

        }

        public override void DialogueResponses(Farmer visitor, string dialogueId)
        {
            
            Mod.instance.Monitor.Log(dialogueId,LogLevel.Debug);
            
            switch (dialogueId)
            {

                default:

                    activeCounter = Math.Max(100, activeCounter);

                    dialogueCounter++;

                    break;

                case "2a":

                    activeCounter = Math.Max(200, activeCounter);

                    dialogueCounter++;

                    break;

            }

        }

    }

}