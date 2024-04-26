/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;

namespace StardewDruid.Event.Sword
{
    internal class SwordWeald : EventHandle
    {

        private int swordIndex;

        public SwordWeald()
        {

        }

        public override void EventSetup(Vector2 target, string id, bool trigger = false)
        {

            base.EventSetup(target, id, trigger);

            origin = new Vector2(12, 8) * 64;
        
        }

        public override void EventRemove()
        {
            
            if (eventComplete)
            {
                
                Mod.instance.questHandle.CompleteQuest(eventId);

            }

            base.EventRemove();

        }


        public override void EventActivate()
        {

            base.EventActivate();

            Mod.instance.iconData.DecorativeIndicator(location, Game1.player.Position, IconData.decorations.weald);

            location.playSound("discoverMineral");

        }

        public override void EventInterval()
        {

            activeCounter++;

            switch (activeCounter)
            {

                case 1:

                    narrators = new()
                    {
                        [0] = new("Rustling in the woodland", Microsoft.Xna.Framework.Color.LightGreen),
                        [1] = new("Whispers on the wind", Microsoft.Xna.Framework.Color.LightBlue),
                        [2] = new("Sighs of the earth", Microsoft.Xna.Framework.Color.LightCyan),
                    };

                    AddActor(0,origin + new Vector2(-180, -32));
                    AddActor(1,origin + new Vector2(180, -32));
                    AddActor(2,origin + new Vector2(0, -180));

                    actors[0].doEmote(8);

                    actors[1].doEmote(8);

                    actors[2].doEmote(8);

                    break;

                case 3:

                    DialogueCue(0,"something treads the old paths");

                    break;

                case 5:

                    DialogueCue(2,"aye, a mortal");

                    break;

                case 7:

                    DialogueCue(1,"sent by the gardener");

                    break;

                case 9:

                    DialogueCue(0,"it seeks the forgotten ways");

                    break;

                case 11:

                    DialogueCue(2,"(grumble)");

                    break;

                case 13:

                    DialogueSetups(null, 1);

                    break;

                case 101:

                    DialogueSetups(null, 2);

                    break;

                case 201:

                    DialogueSetups(null, 3);

                    break;

                case 301:

                    DialogueCue(0, "arise");
                    CastVoice("arise", 1);
                    CastVoice("arise", 2);

                    //---------------------- throw Forest Sword

                    Mod.instance.iconData.CursorIndicator(location, origin - new Vector2(128,128), IconData.cursors.weald);

                    location.playSound("discoverMineral");

                    swordIndex = 15;

                    if (Mod.instance.Helper.ModRegistry.IsLoaded("DaLion.Overhaul"))
                    {
                        swordIndex = 44;
                    }

                    int delayThrow = 600;

                    Vector2 triggerVector = ModUtility.PositionToTile(origin) - new Vector2(1, 2);

                    new Throw().ThrowSword(Game1.player, swordIndex, triggerVector, delayThrow);

                    //----------------------- cast animation

                    Color animateColor = new(0.8f, 1, 0.8f, 1);

                    ModUtility.AnimateSparkles(Mod.instance.rite.castLocation, triggerVector + new Vector2(-3, -3), animateColor);
                    ModUtility.AnimateSparkles(Mod.instance.rite.castLocation, triggerVector + new Vector2(-3, -4), animateColor);
                    ModUtility.AnimateSparkles(Mod.instance.rite.castLocation, triggerVector + new Vector2(-2, -5), animateColor);
                    ModUtility.AnimateSparkles(Mod.instance.rite.castLocation, triggerVector + new Vector2(-1, -6), animateColor);
                    ModUtility.AnimateSparkles(Mod.instance.rite.castLocation, triggerVector + new Vector2(0, -7), animateColor);
                    ModUtility.AnimateSparkles(Mod.instance.rite.castLocation, triggerVector + new Vector2(1, -7), animateColor);
                    ModUtility.AnimateSparkles(Mod.instance.rite.castLocation, triggerVector + new Vector2(2, -7), animateColor);
                    ModUtility.AnimateSparkles(Mod.instance.rite.castLocation, triggerVector + new Vector2(4, -5), animateColor);
                    ModUtility.AnimateSparkles(Mod.instance.rite.castLocation, triggerVector + new Vector2(5, -4), animateColor);
                    ModUtility.AnimateSparkles(Mod.instance.rite.castLocation, triggerVector + new Vector2(5, -3), animateColor);

                    break;

                case 303:

                    eventComplete = true;

                    expireEarly = true;

                    break;

            }

        }


        public override void DialogueSetups(StardewDruid.Character.Character npc, int dialogueId)
        {

            string intro;

            switch (dialogueId)
            {

                default:

                    intro = "Voices in union: ^...Farmer...";

                    break;

                case 2:

                    intro = "Sighs of the Earth: The monarchs remain dormant, their realm untended. Who are you to claim the inheritance of the broken circle?";

                    break;

                case 3:

                    intro = "Rustling in the Woodland: It is not an easy path, the one tread by a squire of the Two Kings. Are you ready to serve?";

                    break;

            }

            List<Response> responseList = new();

            switch (dialogueId)
            {

                default:

                    responseList.Add(new Response("1a", "I've come to pay homage to the Kings of Oak and Holly.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("1b", "(Say nothing)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;
                case 2:

                    responseList.Add(new Response("2a", "The valley is my home now. I will attend to it.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("2b", "(This is a bit much)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;
                case 3:

                    responseList.Add(new Response("3a", "I will serve the Weald like the druids of yore.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("3b", "(Maybe not)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;

            }

            GameLocation.afterQuestionBehavior questionBehavior = new(DialogueResponses);

            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);


        }

        public override void DialogueResponses(Farmer visitor, string dialogueId)
        {
            Mod.instance.Monitor.Log(dialogueId, LogLevel.Debug);
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

                case "3a":

                    activeCounter = Math.Max(300, activeCounter);

                    dialogueCounter++;

                    break;

                case "1b":
                case "2b":
                case "3b":

                    Game1.activeClickableMenu = new DialogueBox("The voices disperse into a rustle of laughter and whispered chants.");

                    ResetToTrigger();

                    break;


            }

        }

    }

}