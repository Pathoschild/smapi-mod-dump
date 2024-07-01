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
using static StardewDruid.Data.IconData;

namespace StardewDruid.Event.Sword
{
    internal class SwordMists : EventHandle
    {

        private int swordIndex;

        public SwordMists()
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

            Mod.instance.iconData.DecorativeIndicator(location, Game1.player.Position, IconData.decorations.mists, 4f, new());

            location.playSound("thunder_small");

        }

        public override void EventInterval()
        {

            activeCounter++;

            switch (activeCounter)
            {

                case 1:

                    narrators = new()
                    {
                        [0] = "Murmurs on the waves",
                        [1] = "Voice Beyond the Shore",
                    };

                    AddActor(0,origin + new Vector2(-64,256));
                    AddActor(1,origin + new Vector2(192, 320));

                    DialogueCue(0, "See who comes before the Lady");

                    break;

                case 4:

                    DialogueCue(0, "The one who cleansed the spring");

                    break;

                case 7:

                    DialogueCue(0, "The one who made the river sing again");

                    break;

                case 10:

                    DialogueSetups(null, 1);

                    break;

                case 101:

                    DialogueSetups(null, 2);

                    break;

                case 201:

                    location.playSound("thunder_small");

                   // Mod.instance.iconData.AnimateBolt(location, origin + new Vector2(-128,-256));
                    Mod.instance.spellRegister.Add(new(origin + new Vector2(-128, -256), 128, IconData.impacts.puff, new()) { type = SpellHandle.spells.bolt });

                    DialogueSetups(null, 3);

                    break;

                case 301:

                    //---------------------- throw Neptune Glaive

                    location.playSound("thunder_small");

                    //Mod.instance.iconData.AnimateBolt(location, origin + new Vector2(64, 320));
                    Mod.instance.spellRegister.Add(new(origin + new Vector2(64, 320), 128, IconData.impacts.puff, new()) { type = SpellHandle.spells.bolt });

                    DialogueCue(1, "My blessing is yours");

                    break;

                case 302:

                    swordIndex = 14;

                    if (Mod.instance.Helper.ModRegistry.IsLoaded("DaLion.Overhaul"))
                    {
                        swordIndex = 7;
                    }

                    ThrowHandle swordThrow = new(Game1.player, origin + new Vector2(64,320), new StardewValley.Tools.MeleeWeapon(swordIndex.ToString()));

                    swordThrow.register();

                    Mod.instance.iconData.ImpactIndicator(location, origin + new Vector2(64, 320), IconData.impacts.fish, 4f, new());

                    break;

                case 304:

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

                    intro = "Murmurs of the waves: We thank you for restoring our sacred waters. Though you are young. And dry. This is unexpected.";

                    break;

                case 2:

                    intro = "Murmurs of the waves: Speak, friend. She listens.";

                    break;

                case 3:

                    intro = "The Voice Beyond the Shore: I hear you, successor.";

                    break;

            }

            List<Response> responseList = new();

            switch (dialogueId)
            {

                default:

                    responseList.Add(new Response("1a", "I harken to the Voice Beyond the Shore, as I was called.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("1a", "Creepy voices. Creepy voices everywhere. And I never have something to record them."));
                    responseList.Add(new Response("1b", "(Say nothing)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;
                case 2:

                    responseList.Add(new Response("2a", "Dear Lady, you once blessed the first farmer. I am their successor.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("2a", "So, is this like a prayer? Do I close my eyes."));
                    responseList.Add(new Response("2b", "(Say nothing)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;
                case 3:

                    responseList.Add(new Response("3a", "My Lady, I will be your champion in the realm before the shore.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("3a", "How? I'm not even on a radio."));
                    responseList.Add(new Response("3b", "(Say nothing)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

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

                    Game1.activeClickableMenu = new DialogueBox("The voices disperse into the gentle rolling of the waves.");

                    ResetToTrigger();

                    break;


            }

        }

    }

}