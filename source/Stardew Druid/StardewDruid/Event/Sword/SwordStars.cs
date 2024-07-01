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
using StardewDruid.Character;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.GameData.Characters;
using StardewValley.Menus;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Tiles;

namespace StardewDruid.Event.Sword
{
    internal class SwordStars : EventHandle
    {

        public int swordIndex;

        public SwordStars()
        {

        }

        public override void EventSetup(Vector2 target, string id, bool trigger = false)
        {

            origin = target;

            eventId = id;

            staticEvent = true;

            location = Game1.getLocationFromName(Mod.instance.questHandle.quests[eventId].triggerLocation);

            if (location == null)
            {

                return;

            }

            ReturnToStatic();

            Mod.instance.RegisterEvent(this, eventId);

            if (!Mod.instance.characters.ContainsKey(CharacterHandle.characters.Revenant))
            {

                CharacterHandle.CharacterLoad(CharacterHandle.characters.Revenant, Character.Character.mode.home);

            }

        }

        public override void ReturnToStatic()
        {

            base.ReturnToStatic();

            Mod.instance.characters[CharacterHandle.characters.Revenant].SwitchToMode(Character.Character.mode.random, Game1.player);

        }

        public override void StaticInterval()
        {

            switch (staticCounter)
            {

                case 0:

                    if(Mod.instance.characters[CharacterHandle.characters.Revenant].modeActive != Character.Character.mode.scene)
                    {
                        Mod.instance.characters[CharacterHandle.characters.Revenant].SwitchToMode(Character.Character.mode.scene, Game1.player);

                        Mod.instance.characters[CharacterHandle.characters.Revenant].eventName = eventId;

                        companions[0] = Mod.instance.characters[CharacterHandle.characters.Revenant];

                        narrators[0] = "The Revenant";

                        voices[0] = Mod.instance.characters[CharacterHandle.characters.Revenant];

                    }

                    if (Vector2.Distance(origin, Mod.instance.characters[CharacterHandle.characters.Revenant].Position) >= 64)
                    {

                        Mod.instance.characters[CharacterHandle.characters.Revenant].TargetEvent(0, new Vector2(27, 16) * 64, true);

                    }
                    else
                    {

                        DialogueLoad(0, 1);

                        Mod.instance.characters[CharacterHandle.characters.Revenant].TargetIdle(2000);

                        Mod.instance.characters[CharacterHandle.characters.Revenant].netStandbyActive.Set(true);

                        switch ((int)Game1.currentGameTime.TotalGameTime.TotalSeconds % 30)
                        {

                            case 4:

                                DialogueCue(0, "My Lady of Fortune");

                                break;

                            case 7:

                                DialogueCue(0, "High Priestess of Yoba");

                                break;

                            case 10:

                                DialogueCue(0, "Deliverer of Destiny");

                                break;

                            case 13:


                                DialogueCue(0, "Relieve me of my tireless vigil");

                                break;

                            case 16:

                                DialogueCue(0, "Lead me to the afterlife");

                                break;

                            case 19:

                                DialogueCue(0, "At least, shut the bats up");

                                break;

                            case 22:

                                DialogueCue(0, "Your unwilling servant");

                                break;

                        }

                    }

                    break;

                case 1:

                    DialogueSetups(Mod.instance.characters[CharacterHandle.characters.Revenant], 2);

                    staticTimer = 10;

                    break;

                case 2:

                    DialogueSetups(Mod.instance.characters[CharacterHandle.characters.Revenant], 3);

                    staticTimer = 10;

                    break;

                case 3:

                    DialogueSetups(Mod.instance.characters[CharacterHandle.characters.Revenant], 4);

                    staticTimer = 10;

                    break;

                case 4:

                    DialogueCue(0, "Alright then. Take this.");

                    staticCounter++;

                    break;

                case 5:

                    companions[0].LookAtTarget(Game1.player.Position);

                    companions[0].TargetIdle(6000);

                    swordIndex = 3;

                    StardewValley.Tools.MeleeWeapon holyBlade = new(swordIndex.ToString());

                    holyBlade.enchantments.Add(new BugKillerEnchantment());

                    holyBlade.enchantments.Add(new CrusaderEnchantment());

                    RubyEnchantment powerEnchantment = new();

                    powerEnchantment.SetLevel(holyBlade, 3);

                    holyBlade.enchantments.Add(powerEnchantment);

                    ThrowHandle swordThrow = new(Game1.player, companions[0].Position, holyBlade);

                    swordThrow.register();

                    staticTimer = 2;

                    break;

                case 6:

                    DialogueCue(0, "A sword for a holy warrior");

                    staticTimer = 2;

                    break;

                case 7:

                    companions[0].clearTextAboveHead();

                    Mod.instance.characters[CharacterHandle.characters.Revenant].SwitchToMode(Character.Character.mode.random, Game1.player);

                    Mod.instance.characters[CharacterHandle.characters.Revenant].modeActive = Character.Character.mode.home;

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

                default:

                    intro = "The Revenant: Well now, a fellow human. Welcome to the Chapel of the Stars.";

                    break;

                case 2:

                    intro = "The Revenant: Ah... it was I that gifted that lantern you carry to the masked scarecrow, back when he used to visit. So you're the successor he has been waiting for. " +
                        "Did he tell you the risks? Did he tell you what you could lose by this path?";

                    break;

                case 3:

                    intro = "The Revenant: Well, I'll be the first to admit that being a Holy Warrior of the Star Guardians has been pretty fun. Until the Fates cursed me with unlife. If you follow this path, you'll face your own reckoning one day.";

                    break;

                case 4:

                    intro = "The Revenant: Well, if the Effigy believes in you, I suppose that counts for something. Seems to me he never does things on a whim. I accept you as guardian in training.";

                    break;

            }

            List<Response> responseList = new();

            switch (dialogueId)
            {

                default:

                    responseList.Add(new Response("1a", "Master holy warrior, I have come, bearing the lantern of your order, to learn the ways of the Stars.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("1a", "Human? You don't even have a face."));
                    responseList.Add(new Response("1b", "(Say nothing)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;
                case 2:

                    responseList.Add(new Response("2a", "He told me this is a necessary step towards obtaining the power the circle needs to defend our home.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("2a", "I think I've exhausted all his dialogue."));
                    responseList.Add(new Response("2b", "(Say nothing)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;

                case 3:

                    responseList.Add(new Response("3a", "I will do what needs to be done to lead the circle of Druids. I am not afraid.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("3a", "I don't know, you seem pretty upbeat about everything, maybe the unlife would suit me."));
                    responseList.Add(new Response("3b", "(Say nothing)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;
                case 4:

                    responseList.Add(new Response("4a", "Thank you master warrior.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("4a", "You and the Effigy seem pretty desperate, so I'll help you out just this once."));
                    responseList.Add(new Response("4b", "(Say nothing)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

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

                    companions[0].TargetIdle(6000);

                    companions[0].LookAtTarget(Game1.player.Position);

                    companions[0].clearTextAboveHead();

                    staticTimer = 0;

                    staticCounter = Math.Max(1, staticCounter);

                    break;

                case "2a":

                    staticTimer = 0;

                    staticCounter = Math.Max(2, staticCounter);

                    break;

                case "3a":

                    staticTimer = 0;

                    staticCounter = Math.Max(3, staticCounter);

                    break;

                case "4a":

                    staticTimer = 0;

                    staticCounter = Math.Max(4, staticCounter);

                    break;

                case "1b":
                case "2b":
                case "3b":
                case "4b":

                    Game1.activeClickableMenu = new DialogueBox("The figure returns to its vigil.");

                    ReturnToStatic();

                    break;

            }

        }

    }

}