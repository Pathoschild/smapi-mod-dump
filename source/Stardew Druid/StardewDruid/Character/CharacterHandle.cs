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
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewDruid.Journal;
using StardewDruid.Location;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Locations;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace StardewDruid.Character
{
    public static class CharacterHandle
    {

        public enum locations
        {
            grove,
            farm,
            chapel,
            vault,
            court,
            archaeum,
        }

        public enum characters
        {
            none,

            // NPCS
            Effigy,
            Jester,
            Revenant,
            Buffin,
            Shadowtin,

            // map interaction
            energies,
            waves,
            herbalism,

            // event
            disembodied,
            Marlon,
            Gunther,
            Cuchulan,
            Morrigan,
        }

        public enum subjects
        {
            approach,
            quests,
            lore,
            relics,
            adventure,
            nevermind,
            attune,
        }

        public static string CharacterTitle(characters character)
        {
            switch (character)
            {
                case characters.Shadowtin:

                    return "Shadowtin, Treasure Hunter";

                case characters.Jester:

                    return "Jester, Envoy of the Fates";

                case characters.Buffin:

                    return "Buffin, Agent of Chaos";

                case characters.Revenant:

                    return "Revenant, Guardian of the Star";

                default:

                    return "Effigy, Last of the Circle";

            }

        }

        public static Vector2 CharacterStart(locations location)
        {

            switch (location)
            {
                case locations.court:

                    return new Vector2(17, 17) * 64;

                case locations.archaeum:

                    return new Vector2(26, 15) * 64;

                case locations.grove:

                    return new Vector2(36, 15) * 64;

                case locations.chapel:

                    return new Vector2(27, 19) * 64;

                case locations.farm:

                    FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);

                    if (homeOfFarmer != null)
                    {
                        Point frontDoorSpot = homeOfFarmer.getFrontDoorSpot();

                        return frontDoorSpot.ToVector2() + new Vector2(0, 128);

                    }

                    break;



            }

            return Vector2.Zero;

        }

        public static string CharacterLocation(locations location)
        {

            switch (location)
            {

                case locations.court:

                    return LocationData.druid_court_name;

                case locations.grove:

                    return LocationData.druid_grove_name;

                case locations.chapel:

                    return LocationData.druid_chapel_name;

                case locations.farm:

                    return "Farm";

            }

            return null;

        }

        public static Vector2 RoamTether(GameLocation location)
        {

            if (location is Farm)
            {

                return CharacterStart(locations.farm);

            }

            if (location is Grove)
            {

                return CharacterStart(locations.grove) + new Vector2(0, Mod.instance.randomIndex.Next(3) * 64);

            }

            if (location is Chapel)
            {

                return CharacterStart(locations.chapel);

            }

            if (location is Court)
            {

                return CharacterStart(locations.court);

            }

            return new(location.map.Layers[0].LayerWidth / 2, location.map.Layers[0].LayerHeight / 2);

        }

        public static locations CharacterHome(characters character)
        {
            switch (character)
            {

                case characters.Buffin:

                    return locations.court;

                case characters.Revenant:

                    return locations.chapel;

                default:

                    return locations.grove;

            }

        }

        public static void CharacterWarp(Character entity, locations destination, bool instant = false)
        {

            string destiny = CharacterLocation(destination);

            Vector2 position = CharacterStart(destination);

            CharacterMover mover = new(entity.characterType);

            mover.WarpSet(destiny, position, true);

            if (instant)
            {

                mover.Update();

                return;

            }

            Mod.instance.movers[entity.characterType] = mover;

        }

        public static void CharacterLoad(characters character, Character.mode mode)
        {

            if (!Context.IsMainPlayer)
            {

                return;

            }

            if (Mod.instance.characters.ContainsKey(character))
            {

                if(Mod.instance.characters[character].modeActive != mode)
                {

                    Mod.instance.characters[character].SwitchToMode(mode, Game1.player);

                }

                return;

            }

            switch (character)
            {

                case characters.Revenant:


                    Mod.instance.characters[characters.Revenant] = new Revenant(characters.Revenant);

                    Mod.instance.dialogue[characters.Revenant] = new(characters.Revenant);

                    Mod.instance.characters[characters.Revenant].SwitchToMode(mode, Game1.player);

                    break;

                case characters.Jester:


                    Mod.instance.characters[characters.Jester] = new Jester(characters.Jester);

                    Mod.instance.dialogue[characters.Jester] = new(characters.Jester);

                    Mod.instance.characters[characters.Jester].SwitchToMode(mode, Game1.player);

                    break;

                case characters.Buffin:


                    Mod.instance.characters[characters.Buffin] = new Buffin(characters.Buffin);

                    Mod.instance.dialogue[characters.Buffin] = new(characters.Buffin);

                    Mod.instance.characters[characters.Buffin].SwitchToMode(mode, Game1.player);

                    break;

                case characters.Shadowtin:


                    Mod.instance.characters[characters.Shadowtin] = new Shadowtin(characters.Shadowtin);

                    Mod.instance.dialogue[characters.Shadowtin] = new(characters.Shadowtin);

                    Mod.instance.characters[characters.Shadowtin].SwitchToMode(mode, Game1.player);

                    break;

                default:

                    character = characters.Effigy;

                    Mod.instance.characters[characters.Effigy] = new Effigy(characters.Effigy);

                    Mod.instance.dialogue[characters.Effigy] = new(characters.Effigy);

                    Mod.instance.characters[characters.Effigy].SwitchToMode(mode, Game1.player);

                    break;
            }
            
        }

        public static Texture2D CharacterTexture(characters character)
        {

            switch (character)
            {
                /*case characters.jester:
                case characters.shadowtin:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", CharacterNames()[character] + ".png"));*/

                default:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", character.ToString() + ".png"));

            }

        }

        public static Texture2D CharacterPortrait(characters character)
        {

            switch (character)
            {
                /*case characters.jester:
                case characters.shadowtin:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", CharacterNames()[character] + "Portrait.png"));*/
                case characters.Revenant:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "RevenantPortrait.png"));

                case characters.Jester:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "JesterPortrait.png"));

                case characters.Buffin:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "BuffinPortrait.png"));

                case characters.Shadowtin:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "ShadowtinPortrait.png"));

                default:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "EffigyPortrait.png"));

            }

        }

        public static CharacterDisposition CharacterDisposition(characters character)
        {

            CharacterDisposition disposition = new()
            {
                Age = 30,
                Manners = 2,
                SocialAnxiety = 1,
                Optimism = 0,
                Gender = 0,
                datable = false,
                Birthday_Season = "summer",
                Birthday_Day = 27,
                id = 18465001,
                speed = 2,

            };

            if (character == characters.Revenant)
            {

                disposition.id += 1;
                disposition.Birthday_Day = 15;

            }

            if (character == characters.Jester)
            {

                disposition.id += 2;
                disposition.Birthday_Season = "fall";

            }

            if (character == characters.Buffin)
            {

                disposition.id += 3;
                disposition.Birthday_Season = "spring";

            }

            if (character == characters.Shadowtin)
            {

                disposition.id += 4;
                disposition.Birthday_Season = "winter";

            }

            return disposition;

        }

        public static void CharacterQuery(characters character, string eventQuery = "CharacterFollow")
        {

            if (Context.IsMultiplayer)
            {

                QueryData queryData = new()
                {
                    name = character.ToString(),
                    longId = Game1.player.UniqueMultiplayerID,
                    value = eventQuery,
                };

                Mod.instance.EventQuery(queryData, "CharacterCommand");

            }

        }

        public static string DialogueString(characters character, subjects subject)
        {

            switch (subject)
            {

                case subjects.approach:

                    switch (character)
                    {

                        case characters.Effigy:

                            if (Mod.instance.save.milestone == QuestHandle.milestones.weald_weapon || Mod.instance.save.milestone == QuestHandle.milestones.mists_weapon)
                            {

                                return "The Effigy: Successor, remember to retain discipline in your training, and visit me tomorrow for new instruction";

                            }

                            return "The Effigy: Greetings, successor";

                        case characters.Revenant:

                            if (Mod.instance.save.milestone == QuestHandle.milestones.stars_weapon)
                            {

                                return "The Revenant: Fortumei bless you, warrior. Come see me again tomorrow, I might have more to teach you.";

                            }
                            return "The Revenant: Hail, adventurer.";

                        case characters.Jester:

                            if (Mod.instance.save.milestone == QuestHandle.milestones.fates_weapon)
                            {

                                return "The Jester of Fate: That's all for today friend. Now what can I find to rub up against...";

                            }

                            if (Mod.instance.save.milestone == QuestHandle.milestones.fates_enchant)
                            {

                                return "The Jester of Fate: Did you go see Buffin at the court of Fates and Chaos? She has the best stuff.";

                            }

                            return "The Jester of Fate: Hey friend.";

                        case characters.Buffin:

                            return "You would make a great servant of chaos, Farmer.";

                        case characters.Shadowtin:

                            return "(Shadowtin's ethereal eyes shine through a cold metal mask)";

                        case characters.energies:

                            return "Energies of the Weald: Squire (echo) squire squire squire...";

                        case characters.waves:

                            return "Murmurs of the Waves: Yes, friend";

                        case characters.herbalism:

                            return "A mortar and pestle used for crafting herbal remedies";


                    }

                    break;

                case subjects.lore:

                    foreach(LoreData.stories story in Mod.instance.questHandle.stories)
                    {

                        if (Mod.instance.questHandle.lores.ContainsKey(story))
                        {

                            if (Mod.instance.questHandle.lores[story].character == character)
                            {

                                return LoreData.RequestLore(character);

                            }

                        }

                    }

                    break;

                case subjects.relics:

                    switch (character)
                    {

                        case characters.energies:

                            int runestones = Mod.instance.relicsData.ProgressRelicQuest(RelicData.relicsets.runestones);

                            if (runestones == -1)
                            {

                                return null;

                            }

                            if (runestones == 4)
                            {

                                return "(relics) I've recovered all the missing runestones from the founding of the circle. How do I access their latent powers?";

                            }
                            else if (runestones >= 1)
                            {

                                return "(relics) I think I've found an artifact from the time of the circle's founding";

                            }

                            return null;

                        case characters.waves:

                            int avalant = Mod.instance.relicsData.ProgressRelicQuest(RelicData.relicsets.avalant);

                            if (avalant == -1)
                            {

                                return null;

                            }

                            if (avalant == 6)
                            {

                                return "(relics) I've found all the components of the ancient Avalant. Will this enable me to chart a course to the isle of mists?";

                            }
                            else if (avalant >= 1)
                            {

                                return "(relics) I fished out a fragment of a strange device. Is it familiar to you?";

                            }

                            return null;

                    }

                    break;

                case subjects.adventure:

                    switch (character)
                    {

                        case characters.Effigy:

                            if (Mod.instance.questHandle.IsComplete(QuestHandle.questEffigy) && Context.IsMainPlayer)
                            {

                                return "(adventure) I have a task for you.";

                            }

                            break;

                        case characters.Jester:

                            if (Context.IsMainPlayer)
                            {

                                return "(adventure) Let's talk adventure.";

                            }

                            break;                        
                        
                        case characters.Shadowtin:
                            
                            if (Context.IsMainPlayer)
                            {

                                return "(adventure) Lets talk about our partnership.";

                            }

                            break;

                        case characters.waves:

                            return "(warp) Why is the monument to the lady all the way out here?";

                    }

                    break;

                case subjects.attune:

                    switch (character)
                    {

                        case characters.energies:

                            return AttunementIntro(Rite.rites.weald);

                        case characters.waves:

                            return AttunementIntro(Rite.rites.mists);

                        case characters.Revenant:

                            return AttunementIntro(Rite.rites.stars);

                        case characters.Jester:

                            return AttunementIntro(Rite.rites.fates);

                        case characters.herbalism:

                            if (Mod.instance.questHandle.IsComplete(QuestHandle.herbalism))
                            {

                                return "(herbalism) Replenish potions";

                            }
                            if (Mod.instance.questHandle.IsGiven(QuestHandle.herbalism))
                            {

                                return "The bowl beckons. You reach out, tentatively, and touch its rim with your fingertip.";

                            }

                            return null;

                    }

                    break;


                case subjects.nevermind:

                    return "(nevermind)";

            }

            return null;

        }

        public static DialogueSpecial DialogueGenerator(characters character, subjects subject, int index = 0, int answer = 0)
        {

            DialogueSpecial generate = new();

            switch (subject)
            {

                case subjects.lore:

                    foreach (LoreData.stories story in Mod.instance.questHandle.stories)
                    {

                        if (Mod.instance.questHandle.lores.ContainsKey(story))
                        {

                            if (Mod.instance.questHandle.lores[story].character == character)
                            {

                                generate.intro = LoreData.CharacterLore(character);

                                generate.responses.Add(Mod.instance.questHandle.lores[story].question);

                                generate.answers.Add(Mod.instance.questHandle.lores[story].answer);

                            }

                        }

                    }

                    break;

                case subjects.relics:

                    switch (character)
                    {

                        case characters.energies:

                            int runestones = Mod.instance.relicsData.ProgressRelicQuest(RelicData.relicsets.runestones);

                            if (runestones == -1)
                            {

                                return generate;

                            }

                            if (runestones == 4)
                            {

                                generate.intro = "Whispering on the wind: (laughs) You already possess conduits for the energies represented in the runes. Well, I can't remember what the cat means. " +
                                    "Sighs of the Earth: The calico shamans served the ancient ones, the dragons. Their wild shape-shifting has been forgotten to time and ruin. The runestones are useless to you now.";

                                generate.responses.Add("Oh. Well the craftsmanship is rather neat. I think I'll use them to redecorate the craftroom at the community center.");

                                generate.answers.Add("Rustling in the woodland: When sourcing materials for your redecoration project, consider nearby sources and support your local woodlands! We have the best timber products on the market. " +
                                    "(New quest recieved)");

                                if (!Game1.MasterPlayer.mailReceived.Contains("JojaMember") && !(Game1.getLocationFromName("CommunityCenter") as CommunityCenter).areasComplete[1])
                                {
                                    Mod.instance.questHandle.AssignQuest(QuestHandle.relicWeald);
                                }

                                Mod.instance.relicsData.FinishRelicQuest(RelicData.relicsets.runestones);

                            }
                            else if (runestones >= 1)
                            {

                                generate.intro = "Rustling in the woodland: It is one of the runestones gifted to your forebearers, those tasked to govern in the Kings' stead. " +
                                    "The other stones belong to creatures that do not care for the old ways. To gather all four would be a boon to the Weald.";

                            }

                            return generate;

                        case characters.waves:

                            int avalant = Mod.instance.relicsData.ProgressRelicQuest(RelicData.relicsets.avalant);

                            if (avalant == -1)
                            {

                                return null;

                            }

                            if (avalant == 6)
                            {

                                generate.intro = "Murmurs of the Waves: Indeed. The finned faithful will be very pleased... to be rid of the ruinous device! " +
                                    "You see, it points towards the broken forgehall of the drowned city, deep within the abyssal trench, where mortals cannot tread. It is useless to you.";

                                generate.responses.Add("The parts appear to be quite sea-resistant. Perhaps I can use them to repair the community center fishtank.");

                                generate.answers.Add("Fish... Tank? We weren't aware the little creatures had armed themselves with war machines. " +
                                    "We will warn the guardians of the depths to prepare for a fish uprising! But do as you will with the Avalant. " +
                                    "(New Quest Received)");

                                if (!Game1.MasterPlayer.mailReceived.Contains("JojaMember") && !(Game1.getLocationFromName("CommunityCenter") as CommunityCenter).areasComplete[2])
                                {
                                    Mod.instance.questHandle.AssignQuest(QuestHandle.relicMists);
                                }

                                Mod.instance.relicsData.FinishRelicQuest(RelicData.relicsets.avalant);

                            }
                            else if (avalant >= 1)
                            {

                                generate.intro = "Murmurs of the Waves: The Avalant, to guide the blessed pilgrim through the judgements of the mists. " +
                                    "It was broken, but can be restored. The finned faithful carry the pieces from the sea to the sacred spring. " +
                                    "Continue to use the power of the lady to fish the waters, and you might collect all the scattered pieces.";

                            }

                            return generate;

                    }

                    break;

                case subjects.adventure:

                    switch (index)
                    {

                        case 0:


                            switch (character)
                            {

                                case characters.Effigy:


                                    if (!Mod.instance.questHandle.IsComplete(QuestHandle.questEffigy))
                                    {

                                        break;

                                    }

                                    generate.intro = "The Effigy: It is time to roam the wilds once more.";

                                    generate.responses.Add("(inventory) I want to check your inventory.");

                                    generate.answers.Add("0");

                                    if (Mod.instance.characters[character].modeActive != Character.mode.track)
                                    {

                                        generate.responses.Add("(follow) Come explore the valley with me.");

                                        generate.answers.Add("1");
                                    }

                                    if (Mod.instance.characters[character].modeActive != Character.mode.roam)
                                    {

                                        generate.responses.Add("(work) My farm would benefit from your gentle stewardship.");

                                        generate.answers.Add("2");

                                    }

                                    if (Mod.instance.characters[character].modeActive != Character.mode.home
                                        && Mod.instance.characters[character].currentLocation.Name != CharacterLocation(locations.grove))
                                    {

                                        generate.responses.Add("(rest) Thank you for everything you do for the circle.");

                                        generate.answers.Add("3");

                                    }

                                    generate.lead = true;

                                    return generate;                                
                                
                                
                                case characters.Jester:

                                    if (!Mod.instance.questHandle.IsComplete(QuestHandle.approachJester))
                                    {

                                        break;

                                    }

                                    generate.intro = "The Jester of Fate: I'm ready to explore the world."; ;

                                    generate.responses.Add("(inventory) I want to check your inventory.");

                                    generate.answers.Add("0");

                                    if (Mod.instance.characters[character].modeActive != Character.mode.track)
                                    {

                                        generate.responses.Add("(follow) Let us continue our quest.");

                                        generate.answers.Add("1");

                                    }

                                    if (Mod.instance.characters[character].modeActive != Character.mode.roam)
                                    {

                                        generate.responses.Add("(work) There's plenty going on on the farm.");

                                        generate.answers.Add("2");

                                    }

                                    if (Mod.instance.characters[character].modeActive != Character.mode.home
                                        && Mod.instance.characters[character].currentLocation.Name != CharacterLocation(locations.grove))
                                    {

                                        generate.responses.Add("(rest) The Druid's grove is where it's all happening.");

                                        generate.answers.Add("3");

                                    }

                                    generate.lead = true;

                                    return generate;

                                case characters.Shadowtin:

                                    if (!Mod.instance.questHandle.IsComplete(QuestHandle.challengeFates))
                                    {

                                        break;

                                    }

                                    generate.intro = "Shadowtin Bear: What do you propose?"; ;

                                    generate.responses.Add("(inventory) I want to check your inventory.");

                                    generate.answers.Add("0");

                                    if (Mod.instance.characters[character].modeActive != Character.mode.track)
                                    {

                                        generate.responses.Add("(follow) Let us continue our investigation.");

                                        generate.answers.Add("1");

                                    }

                                    if (Mod.instance.characters[character].modeActive != Character.mode.roam)
                                    {

                                        generate.responses.Add("(work) There's plenty of research material on the farm.");

                                        generate.answers.Add("2");

                                    }

                                    if (Mod.instance.characters[character].modeActive != Character.mode.home
                                        && Mod.instance.characters[character].currentLocation.Name != CharacterLocation(locations.grove))
                                    {

                                        generate.responses.Add("(rest) That's enough treasure hunting for now.");

                                        generate.answers.Add("3");

                                    }

                                    generate.lead = true;

                                    return generate;

                                case characters.waves:

                                    generate.intro = "Murmurs of the waves: Still yourself, and let the rolling motions of the mists carry you home.";

                                    generate.responses.Add("I feel... wet? Wait what... !");

                                    generate.answers.Add("10");

                                    generate.lead = true;

                                    return generate;

                            }

                            break;

                        case 1:


                            switch (answer)
                            {

                                case 0: // Open inventory

                                    OpenInventory(character);

                                    return generate;

                                case 1: // Follow player

                                    switch (character)
                                    {
                                        default:
                                        case characters.Effigy:

                                            generate.intro = "I will see how you exercise the authority of the sleeping kings.";

                                            break;

                                        case characters.Jester:

                                            generate.intro = "Lead the way, fateful one.";

                                            break;

                                        case characters.Shadowtin:

                                            generate.intro = "Indeed. How about we split the spoils fifty fifty.";

                                            break;

                                    }

                                    Mod.instance.characters[character].SwitchToMode(Character.mode.track, Game1.player);

                                    return generate;

                                case 2: // Work on farm

                                    switch (character)
                                    {
                                        default:
                                        case characters.Effigy:

                                            generate.intro = "I will take my place amongst the posts and furrows of my old master's home.";

                                            break;

                                        case characters.Jester:

                                            generate.intro = "Let's see who's around to bother.";

                                            break;

                                        case characters.Shadowtin:

                                            generate.intro = "Lets see how profitable this agricultural venture is.";

                                            break;
                                    }

                                    Mod.instance.characters[character].SwitchToMode(Character.mode.roam, Game1.player);

                                    return generate;

                                case 3: // Return to grove

                                    switch (character)
                                    {
                                        default:
                                        case characters.Effigy:

                                            generate.intro = "I will return to where I may hear the rumbling energies of the valley's leylines.";

                                            break;

                                        case characters.Jester:

                                            generate.intro = "(Jester grins) That's my favourite place to nap!";

                                            break;

                                        case characters.Shadowtin:

                                            generate.intro = "Good idea. I require a quiet, shaded place to ruminate.";

                                            break;
                                    }

                                    Mod.instance.characters[character].SwitchToMode(Character.mode.home, Game1.player);

                                    return generate;

                                case 10:

                                    Wand wand = new();

                                    wand.lastUser = Game1.player;

                                    wand.DoFunction(Game1.player.currentLocation, 0, 0, 0, Game1.player);

                                    return generate;

                            }

                            break;
                    }

                    break;

                case subjects.attune:

                    int toolIndex = Mod.instance.AttuneableWeapon();

                    int attuneUpdate;

                    switch (character)
                    {

                        case characters.energies:

                            attuneUpdate = AttunementUpdate(Rite.rites.weald);

                            switch (attuneUpdate)
                            {

                                case 0:

                                    generate.intro = "This " + Game1.player.CurrentTool.Name + " resists attunement";

                                    break;

                                case 1:

                                    generate.intro = "Sighs of the Earth: The " + Game1.player.CurrentTool.Name + " was crafted from materials blessed by the Lords of the Weald. " +
                                    "It will serve your purposes as squire just fine, but its allegiance will always be to the Two Kings.";

                                    break;

                                case 2:

                                    generate.intro = "Sighs of the Earth: This " + Game1.player.CurrentTool.Name + " will no longer serve the Two Kings";

                                    break;

                                case 3:

                                    generate.intro = "Sighs of the Earth: This " + Game1.player.CurrentTool.Name + " will serve the Two Kings";

                                    break;

                            }

                            return generate;

                        case characters.waves:

                            attuneUpdate = AttunementUpdate(Rite.rites.mists);

                            switch (attuneUpdate)
                            {

                                case 0:

                                    generate.intro = "This " + Game1.player.CurrentTool.Name + " resists attunement";

                                    break;

                                case 1:

                                    generate.intro = "Murmurs of the Waves: The " + Game1.player.CurrentTool.Name + " is from a time before the Lady, " +
                                        "before the mists swirled around the isle, before the city it was forged in was lost to the storm.";


                                    break;

                                case 2:

                                    generate.intro = "Murmurs of the Waves: This " + Game1.player.CurrentTool.Name + " will no longer serve the Lady Beyond the Shore";

                                    break;

                                case 3:

                                    generate.intro = "Murmurs of the Waves: This " + Game1.player.CurrentTool.Name + " will serve the Lady Beyond the Shore";

                                    break;

                            }

                            return generate;

                        case characters.Revenant:

                            attuneUpdate = AttunementUpdate(Rite.rites.stars);

                            switch (attuneUpdate)
                            {

                                case 0:

                                    generate.intro = "This " + Game1.player.CurrentTool.Name + " resists attunement";

                                    break;

                                case 1:

                                    generate.intro = "There used to be a lot of warriors in our order. Now all that remains of them are the " + Game1.player.CurrentTool.Name + "'s that never tarnish.";

                                    break;

                                case 2:

                                    generate.intro = "This " + Game1.player.CurrentTool.Name + " will no longer serve the Lights of the Great Expanse";

                                    break;

                                case 3:

                                    generate.intro = "This " + Game1.player.CurrentTool.Name + " will serve the Lights of the Great Expanse";

                                    break;

                            }

                            return generate;

                        case characters.Jester:

                            attuneUpdate = AttunementUpdate(Rite.rites.fates);

                            switch (attuneUpdate)
                            {

                                case 0:

                                    generate.intro = "This " + Game1.player.CurrentTool.Name + " resists attunement";

                                    break;

                                case 1:

                                    generate.intro = "I don't think that " + Game1.player.CurrentTool.Name + " is of this world, Farmer. " +
                                        "I think it was made by one of the Morticians to reap the wayward souls of mortals. " +
                                        "Or maybe they wanted to cut the heads off of flowers. Morticians are melancholic.";

                                    break;

                                case 2:

                                    generate.intro = "This " + Game1.player.CurrentTool.Name + " means nothing to the Fates now, Farmer";

                                    break;

                                case 3:

                                    generate.intro = "This " + Game1.player.CurrentTool.Name + " will please the High Priestess";

                                    break;

                            }

                            return generate;

                        case characters.herbalism:

                            if (Mod.instance.questHandle.IsComplete(QuestHandle.herbalism))
                            {

                                generate.intro = "Potion stock has been refilled based on available ingredients.";

                                Mod.instance.herbalData.MassBrew();

                                return generate;

                            }

                            switch (index)
                            {

                                case 0:

                                    generate.intro = "Your fingertip traces the smoothened edge. The bowl seems pleased, as if the face reflected in the shiny inner surface is happier than your own.";

                                    generate.responses.Add("You rub your palms around the curvature of the bowl.");

                                    generate.answers.Add("0");

                                    generate.responses.Add("You use a firm grip to grasp the mortar. It's so strong.");

                                    generate.answers.Add("1");

                                    generate.responses.Add("Your face goes into the bowl.");

                                    generate.answers.Add("2");

                                    generate.lead = true;

                                    break;

                                case 1:

                                    generate.intro = "A soft sound of contentment fills your ear. You're startled. You look back and forth but find no other creature but yourself at the stone table. " +
                                        "Even stranger, you have acquired the forgotten knowledge of herbalism, with all the craft secrets of the druids that stood before this bowl in ages past. " +
                                        "With a bit of amateur effort, three stoppered flasks, each with a small amount of herbal remedy, dangle from your belt. " +
                                        "The bowl is pleased. (Herbalism journal unlocked)";

                                    Mod.instance.questHandle.CompleteQuest(QuestHandle.herbalism);

                                    break;

                            }

                            return generate;

                    }

                    break;

            }

            return generate;

        }

        public static void OpenInventory(characters character)
        {

            if (!Mod.instance.chests.ContainsKey(character))
            {

                Chest newChest = new();

                if (Mod.instance.save.chests.ContainsKey(character))
                {

                    foreach (ItemData item in Mod.instance.save.chests[character])
                    {

                        newChest.Items.Add(new StardewValley.Object(item.id, item.stack, quality: item.quality));


                    }

                }

                Mod.instance.chests[character] = newChest;

            }

            Mod.instance.chests[character].ShowMenu();

        }

        public static string AttunementIntro(Rite.rites compare)
        {

            int toolIndex = Mod.instance.AttuneableWeapon();

            if (toolIndex == -1 || toolIndex == 999)
            {

                return null;

            }

            Dictionary<int, Rite.rites> comparison = SpawnData.WeaponAttunement(true);

            if (comparison.ContainsKey(toolIndex))
            {

                if (comparison[toolIndex] == compare)
                {


                    return "Is there a special quality to this " + Game1.player.CurrentTool.Name + "?";

                }
                else
                {

                    return null;

                }

            }

            if (Mod.instance.save.attunement.ContainsKey(toolIndex))
            {

                if (Mod.instance.save.attunement[toolIndex] == compare)
                {


                    return "(detune) Can I reclaim this " + Game1.player.CurrentTool.Name + "?";

                }

            }

            return "(attune) I want to dedicate this " + Game1.player.CurrentTool.Name + " to the "+compare.ToString();

        }

        public static int AttunementUpdate(Rite.rites compare)
        {
            
            int toolIndex = Mod.instance.AttuneableWeapon();

            if (toolIndex == -1 || toolIndex == 999)
            {

                return 0;

            }

            Dictionary<int, Rite.rites> comparison = SpawnData.WeaponAttunement(true);

            if (comparison.ContainsKey(toolIndex))
            {

                if (comparison[toolIndex] == compare)
                {

                    return 1;

                }
                else
                {

                    return 0;

                }

            }

            if (Mod.instance.save.attunement.ContainsKey(toolIndex))
            {

                if (Mod.instance.save.attunement[toolIndex] == compare)
                {

                    Mod.instance.iconData.ImpactIndicator(Game1.player.currentLocation, Game1.player.Position, IconData.impacts.nature, 6f, new());

                    Game1.player.currentLocation.playSound("yoba");

                    Mod.instance.DetuneWeapon();

                    return 2;

                }

                //return 0;

            }

            Mod.instance.iconData.ImpactIndicator(Game1.player.currentLocation, Game1.player.Position, IconData.impacts.nature, 6f, new());

            Game1.player.currentLocation.playSound("yoba");

            Mod.instance.AttuneWeapon(compare);

            return 3;

        }


    }

    public class CharacterMover
    {

        public CharacterHandle.characters character;

        public enum moveType
        {

            from,
            to,
            remove,

        }

        public moveType type;

        public string locale;

        public Vector2 position;

        public bool animate;

        public CharacterMover(CharacterHandle.characters CharacterType)
        {

            character = CharacterType;

        }

        public void Update()
        {

            Character entity = Mod.instance.characters[character];

            GameLocation target;

            if (Mod.instance.locations.ContainsKey(locale))
            {

                target = Mod.instance.locations[locale];

            }
            else
            {

                target = Game1.getLocationFromName(locale);

            }

            switch (type)
            {

                case moveType.from:

                    target.characters.Remove(entity);

                    break;

                case moveType.to:

                    Warp(target, entity, position);

                    break;

                case moveType.remove:

                    RemoveAll(entity);

                    break;

            }


        }

        public void WarpSet(string Target, Vector2 Position, bool Animate = true)
        {

            type = moveType.to;

            locale = Target;

            position = Position;

            animate = Animate;

        }


        public static void Warp(GameLocation target, Character entity, Vector2 position, bool animate = true)
        {

            if (entity.currentLocation != null)
            {

                if (animate)
                {

                    Mod.instance.iconData.AnimateQuickWarp(entity.currentLocation, entity.Position, true);

                }

                entity.currentLocation.characters.Remove(entity);

            }

            entity.ResetActives(true);

            target.characters.Add(entity);

            entity.currentLocation = target;

            entity.Position = position;

            entity.SettleOccupied();

            if (animate)
            {

                Mod.instance.iconData.AnimateQuickWarp(entity.currentLocation, entity.Position);

            }

        }

        public void RemovalSet(string From)
        {

            type = moveType.from;

            locale = From;

        }

        public void RemoveAll(Character entity)
        {

            foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
            {

                if (location.characters.Count > 0)
                {

                    if (location.characters.Contains(entity))
                    {

                        location.characters.Remove(entity);

                    }

                }

            }

        }

    }

    public class CharacterDisposition
    {
        public int Age;
        public int Manners;
        public int SocialAnxiety;
        public int Optimism;
        public Gender Gender;
        public bool datable;
        public string Birthday_Season;
        public int Birthday_Day;
        public int id;
        public int speed;
    }

}
