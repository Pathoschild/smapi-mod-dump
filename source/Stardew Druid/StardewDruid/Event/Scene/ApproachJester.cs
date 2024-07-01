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
using StardewValley.GameData.Characters;
using StardewValley.Locations;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;


namespace StardewDruid.Event.Scene
{
    public class ApproachJester : EventHandle
    {

        public ApproachJester()
        {

        }

        public override void EventSetup(Vector2 target, string id, bool trigger = false)
        {

            origin = target;

            eventId = id;

            staticEvent = true;

            location = Game1.getLocationFromName(Mod.instance.questHandle.quests[eventId].triggerLocation);

            if(location == null)
            {

                return;

            }

            ReturnToStatic();

            Mod.instance.RegisterEvent(this, eventId);

        }

        public override void ReturnToStatic()
        {

            base.ReturnToStatic();

            CharacterHandle.CharacterLoad(CharacterHandle.characters.Jester, StardewDruid.Character.Character.mode.scene);

            CharacterMover.Warp(location, Mod.instance.characters[CharacterHandle.characters.Jester], origin);

            Mod.instance.characters[CharacterHandle.characters.Jester].netStandbyActive.Set(true);

            Mod.instance.characters[CharacterHandle.characters.Jester].eventName = eventId;

            DialogueLoad(Mod.instance.characters[CharacterHandle.characters.Jester], 1);

        }

        public override void StaticInterval()
        {

            switch (staticCounter)
            {

                // ------------------------------------------
                // 1 Beginning
                // ------------------------------------------

                case 1:

                    DialogueSetups(Mod.instance.characters[CharacterHandle.characters.Jester], 2);

                    break;

                case 2:

                    DialogueSetups(Mod.instance.characters[CharacterHandle.characters.Jester], 3);

                    break;

                case 3:

                    DialogueSetups(Mod.instance.characters[CharacterHandle.characters.Jester], 4);

                    break;

                case 4:

                    DialogueSetups(Mod.instance.characters[CharacterHandle.characters.Jester], 5);

                    break;

                case 5:

                    DialogueSetups(Mod.instance.characters[CharacterHandle.characters.Jester], 51);

                    break;

                case 6:

                    ThrowHandle throwRelic = new(Game1.player, origin, IconData.relics.jester_dice);

                    throwRelic.register();

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.jester_dice.ToString());

                    Mod.instance.questHandle.CompleteQuest(eventId);

                    eventComplete = true;

                    break;

            }

        }

        public override void DialogueSetups(StardewDruid.Character.Character npc, int dialogueId)
        {
            
            string intro;

            switch (dialogueId)
            {

                default: // introOne

                    intro = "(The strange cat looks at you expectantly)";

                    break;

                case 2:

                    intro = "Strange Cat: What I am is desperately lost. The patterns of this world are strange enough, and the behaviour of the creatures here, especially humans... is confuddling.";

                    break;

                case 3:

                    intro = "Strange Cat: Well farmer, you have the scent of destiny about you, and some otherworldly ability too. You must be the fabled acolyte of the celestials I was warned about.";

                    break;

                case 4:

                    intro = "Strange Cat: I propose a partnership. I teach you some of my special tricks, and you help me in my sacred quest. All I need to do is find the great Reaper of Fate and the world-threatening Star entity that he's hunting.";

                    break;

                case 5:

                    intro = "Great! Now the adventure people said there's a dark dungeon to the east of here, full of peril and evil skull-heads. " +
                        "That's exactly the place I want to go! It's just the thought of going alone... well... uh... who doesn't like to have friends with them on an epic journey like this? " +
                        "Come see me when you're ready to venture forth, my brave and loyal farmer. (New quest received)";

                    DialogueDraw(npc, intro);

                    staticCounter = Math.Max(6, staticCounter);

                    return;

                case 51:

                    intro = "Hehehe... I like you already! But you cannot escape this Fate, literally, and, well literally. " +
                        "Come see me when you're ready to explore the dungeon on the other side of this gap, I promise it will be worth your time! (The mountain bridge must be repaired in order to proceed)";

                    DialogueDraw(npc, intro);

                    staticCounter = Math.Max(6, staticCounter);

                    return;

            }

            List<Response> responseList = new();

            switch (dialogueId)
            {

                default: //introOne

                    responseList.Add(new Response("1a", "Greetings, you must be the representative from the Fae Court").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("1a", "Hello Kitty, are you far from home?").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("1b", "(You hold out your empty hands and shrug)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;

                case 2:

                    //responseList.Add(new Response("2a", "An otherworldly visitor might be disorientated by the natural laws of this world, laws that keep it ordered and safe."));
                    responseList.Add(new Response("2a", "That's interesting. Cats usually have an instinct for finding their way.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("2a", "Forest magic can really mess with one's outlook. It's wack."));
                    responseList.Add(new Response("2b", "(Say nothing and pretend the cat can't talk)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;

                case 3:

                    responseList.Add(new Response("3a", "I've received a few titles on my journey, but I prefer to be known as the Stardew Druid.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("3a", "Meteorites, lightning bolts? Nah, can't say we see much of those out here.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;

                case 4:

                    responseList.Add(new Response("4a", "A practitioner of mysteries? This is truly fortuitous. I accept your proposal."));
                    responseList.Add(new Response("4a", "Well I could use a big cat on the farm.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    
                    object bridge = typeof(Mountain).GetField("bridgeRestored", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Game1.getLocationFromName("Mountain"));

                    if (bridge.ToString() == "True")
                    {
                        
                        responseList.Add(new Response("4b", "I'm not making any deals with a strange cat on a bridge built by forest spirits!").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    }

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

                    Mod.instance.characters[CharacterHandle.characters.Jester].netStandbyActive.Set(false);

                    Mod.instance.characters[CharacterHandle.characters.Jester].LookAtTarget(Game1.player.Position, true);

                    location = Mod.instance.characters[CharacterHandle.characters.Jester].currentLocation;

                    staticCounter = Math.Max(1, staticCounter);

                    break;

                case "1b":

                    ReturnToStatic();

                    DialogueDraw(Mod.instance.characters[CharacterHandle.characters.Jester], "(The strange cat shrugs back)");

                    break;

                case "2a":

                    staticCounter = Math.Max(2, staticCounter);

                    break;

                case "2b":

                    ReturnToStatic();

                    DialogueDraw(Mod.instance.characters[CharacterHandle.characters.Jester], "(The strange cat pretends it saw something 'over there' and begins to ignore you)");

                    break;

                case "3a":

                    staticCounter = Math.Max(3, staticCounter);

                    break;

                case "4a":

                    staticCounter = Math.Max(4, staticCounter);

                    break;

                case "4b":

                    staticCounter = Math.Max(5, staticCounter);

                    break;
            
            }

        }

    }

}