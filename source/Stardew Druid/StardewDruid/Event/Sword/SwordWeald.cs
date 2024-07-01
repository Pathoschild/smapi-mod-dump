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

            ModUtility.AnimateHands(Game1.player,Game1.player.FacingDirection,600);

            Mod.instance.iconData.DecorativeIndicator(location, Game1.player.Position, IconData.decorations.weald, 4f, new());

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
                        [0] = "Rustling in the woodland",
                        [1] = "Whispers on the wind",
                        [2] = "Sighs of the earth",
                    };

                    AddActor(0,origin + new Vector2(-192, -32));
                    AddActor(1,origin + new Vector2(128, -96));
                    AddActor(2,origin + new Vector2(-64, -180));

                    DialogueCue(0,"Something treads the old paths");

                    CastVoice(1, "!");

                    CastVoice(2, "!");

                    break;

                case 4:

                    DialogueCue(2,"Aye, a mortal");

                    break;

                case 7:

                    DialogueCue(1,"Sent by the gardener");

                    break;

                case 10:

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

                    CastVoice(1, "arise");

                    CastVoice(2, "arise");

                    //---------------------- throw Forest Sword

                    Mod.instance.iconData.ImpactIndicator(location, origin, IconData.impacts.nature, 6f, new());

                    location.playSound("discoverMineral");

                    swordIndex = 15;

                    if (Mod.instance.Helper.ModRegistry.IsLoaded("DaLion.Overhaul"))
                    {
                        swordIndex = 44;
                    }

                    ThrowHandle swordThrow = new(Game1.player, origin - new Vector2(64,320), new StardewValley.Tools.MeleeWeapon(swordIndex.ToString()));

                    swordThrow.delay = 40;

                    swordThrow.register();

                    break;

                case 303:

                    eventComplete = true;

                    break;

            }

        }


        public override void DialogueSetups(StardewDruid.Character.Character npc, int dialogueId)
        {

            string intro;

            switch (dialogueId)
            {

                default:

                    intro = "Sighs of the Earth: What say you, farmer?";

                    break;

                case 2:

                    intro = "Whispers on the wind: The monarchs remain dormant, their realm untended. Who are you to claim the inheritance of the broken circle?";

                    break;

                case 3:

                    intro = "Rustling in the Woodland: It is not an easy path, the one tread by a squire of the Two Kings. Are you ready to serve?";

                    break;

            }

            List<Response> responseList = new();

            switch (dialogueId)
            {

                default:

                    responseList.Add(new Response("1a", "I seek the blessing of the Two Kings to reform the circle of Druids.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("1a", "Ok. Whoever's behind the rock, come on out."));
                    responseList.Add(new Response("1b", "(Say nothing)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;
                case 2:

                    responseList.Add(new Response("2a", "The valley is my home now. I want to care for and protect it.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("2a", "I'm being friendly and playing along with your little game. Just dont pull down my pants or anything."));
                    responseList.Add(new Response("2b", "(This is a bit much)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;
                case 3:

                    responseList.Add(new Response("3a", "I will serve the Weald like the druids of yore.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("3a", "Serve... tea?"));
                    responseList.Add(new Response("3b", "(Maybe not)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;

            }

            GameLocation.afterQuestionBehavior questionBehavior = new(DialogueResponses);

            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);


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

                case "3a":

                    activeCounter = Math.Max(300, activeCounter);

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