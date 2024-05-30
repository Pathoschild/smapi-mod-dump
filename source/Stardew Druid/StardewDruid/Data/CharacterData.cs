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
using StardewDruid.Dialogue;
using StardewDruid.Journal;
using StardewDruid.Location;
using StardewModdingAPI;
using StardewValley;
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
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using xTile;
using xTile.Dimensions;

namespace StardewDruid.Data
{
    public static class CharacterData
    {

        public enum locations
        {
            home,
            farm
        }

        public enum characters
        {   
            none,
            disembodied,
            Effigy,
            energies,
            waves,
            herbalism,
            Cuchulan,
            Morrigan,
        }

        public enum subjects
        {
            approach,
            quests,
            lore,
            adventure,
            nevermind,
            attune,
        }

        public static Vector2 CharacterStart(locations location)
        {

            switch (location)
            {
                case locations.home:

                    return WarpData.WarpStart(LocationData.druid_grove_name);

                case locations.farm:

                    return WarpData.WarpStart("Farm");

            }

            return Vector2.Zero;

        }

        public static string CharacterLocation(locations location)
        {

            switch (location)
            {

                case locations.home:

                    return LocationData.druid_grove_name;

                case locations.farm:

                    return "Farm";

            }

            return null;

        }

        public static void CharacterWarp(StardewDruid.Character.Character entity, locations destination)
        {
            
            if (entity.currentLocation != null)
            {
                entity.currentLocation.characters.Remove(entity);

            }

            entity.Position = CharacterStart(destination);

            string destiny = CharacterLocation(destination);

            if (Mod.instance.locations.ContainsKey(destiny))
            {

                Mod.instance.locations[destiny].characters.Add(entity);

                return;

            }

            entity.currentLocation = Game1.getLocationFromName(CharacterLocation(destination));

            entity.currentLocation.characters.Add(entity);

        }

        public static void CharacterRemove(characters character)
        {

            if(Mod.instance.characters.ContainsKey(character))
            {

                StardewDruid.Character.Character entity = Mod.instance.characters[character];

                if (entity.currentLocation != null)
                {
                    entity.currentLocation.characters.Remove(entity);

                }

                Mod.instance.characters.Remove(character);

            }

        }

        public static void CharacterLoad(characters character, StardewDruid.Character.Character.mode mode)
        {

            if (!Context.IsMainPlayer)
            {

                return;

            }

            if (Mod.instance.characters.ContainsKey(character))
            {

                Mod.instance.characters[character].SwitchToMode(mode, Game1.player);

                return;

            }

            switch (character)
            {

                case characters.Effigy:


                    Mod.instance.characters[characters.Effigy] = new Effigy(characters.Effigy);

                    Mod.instance.dialogue[characters.Effigy] = new(characters.Effigy);

                    Mod.instance.characters[characters.Effigy].SwitchToMode(mode, Game1.player);

                    break;
                

            }

        }

        public static void CharacterFind(characters character)
        {

            foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
            {

                if (location.characters.Count > 0)
                {
                    for (int index = location.characters.Count - 1; index >= 0; --index)
                    {
                        NPC npc = location.characters[index];

                        if (npc is StardewDruid.Character.Character entity)
                        {

                            if (entity.characterType == character)
                            {
                                
                                Mod.instance.characters[character] = entity;

                            }

                        }

                    }

                }

            }

            if(Context.IsMainPlayer)
            {

                CharacterLoad(character, Character.Character.mode.home);

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

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", character.ToString()+".png"));

            }

        }

        public static Texture2D CharacterPortrait(characters character)
        {

            switch (character)
            {
                /*case characters.jester:
                case characters.shadowtin:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", CharacterNames()[character] + "Portrait.png"));*/

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
            /*
            if (characterName == "Jester")
            {

                disposition.id += 1;
                disposition.Birthday_Season = "fall";

            }

            if (characterName == "Shadowtin")
            {

                disposition.id += 2;
                disposition.Birthday_Season = "winter";

            }*/

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

                            return "The Effigy: Greetings, successor";

                        case characters.energies:

                            return "Energies of the Weald: Squire (echo) squire squire squire...";

                        case characters.waves:

                            return "Murmurs of the Waves: Yes, friend";

                        case characters.herbalism:

                            return "A mortar and pestle used for crafting herbal remedies";
                    }

                    break;

                case subjects.quests:

                    switch (character)
                    {

                        case characters.Effigy:

                            return "(quests) What can I do for the circle?";

                    }

                    break;

                case subjects.lore:

                    switch (character)
                    {

                        case characters.Effigy:

                            return "(lore) I want to learn more about the weald.";

                        case characters.energies:

                            if(Mod.instance.save.reliquary.ContainsKey(IconData.relics.minister_mitre.ToString()))
                            {

                                return "(relics) I think I've found an artifact from the time of the circle's founding";

                            }

                            return null;

                        case characters.waves:

                            int avalant = Mod.instance.relicsData.ProgressFishQuest();

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

                                return "(adventure) I have a task for you";

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
                        case characters.waves:

                            Cast.Rite.rites compare = Cast.Rite.rites.weald;

                            if(character == characters.waves)
                            {

                                compare = Cast.Rite.rites.mists;

                            }

                            int toolIndex = Mod.instance.AttuneableWeapon();
                            
                            if(toolIndex == -1 || toolIndex == 999)
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

                            return "(attune) I want to dedicate this " + Game1.player.CurrentTool.Name;

                        case characters.herbalism:

                            if (Mod.instance.save.herbalism.ContainsKey(Journal.HerbalData.herbals.ligna))
                            {

                                return "(herbalism) Replenish potions";

                            }

                            return "The bowl beckons. You reach out, tentatively, and touch its rim with your fingertip.";

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

                    switch (character)
                    {

                        case characters.Effigy:

                            generate.intro = "The Effigy: Our traditions are etched into the bedrock of the valley.";

                            generate.responses.Add("What role do the Two Kings play?");

                            generate.answers.Add("In times past, the King of Oaks and the King of Holly would war upon the Equinox. " +
                                "Their warring would conclude for the sake of new life in Spring. When need arose, they lent their strength to a conflict from which neither could fully recover. " +
                                "They rest now, dormant. May those awake guard the change of seasons.");

                            generate.responses.Add("I want to know more about the First Farmer");

                            generate.answers.Add("The first farmer was blessed by the elderborn, the monarchs of the valley, to cultivate and protect this special land. " +
                                "He used this blessing to construct me, and showed me how I could preserve his techniques for a future successor. " +
                                "Though my friend is long gone, I remain, because the power of the elders remain. For now.");

                            if (Mod.instance.questHandle.IsComplete("swordMists"))
                            {
                                
                                generate.responses.Add("Who is the Lady Beyond the Shore?");

                                generate.answers.Add("The Lady of the Isle of Mists is as beautiful and distant as the sunset on the Gem Sea. She was once a courtier of the Two Kings, from a time before the war. " +
                                    "The first farmer was closest to her in counsel and in conviction. She helped establish the circle and remained here a shortwhile before she was called to the Isle. " +
                                    "(The Effigy's eyes flicker a brilliant turqoise). " +
                                    "There is a feeling that comes with my memories of that time, a feeling I cannot describe.");

                            }

                            return generate;

                        case characters.energies:

                            generate.intro = "Rustling in the woodland: It is one of the vestments of the king's bishop. " +
                                "There were other vestments that belonged to the Kings' minister, all now in the possession of the circles enemies." +
                                "To gather all four pieces would be a boon to the Weald.";

                            return generate;

                        case characters.waves:

                            int avalant = Mod.instance.relicsData.ProgressFishQuest();

                            if(avalant == -1)
                            {

                                return null;

                            }

                            if (avalant == 6)
                            {

                                generate.intro = "Murmurs of the Waves: Indeed. The finned faithful will be very pleased... to be rid of the ruinous device! " +
                                    "You see, it points towards the broken forgehall of the drowned city, deep within the abyssal trench, where mortals cannot tread. It is useless to you.";

                                generate.responses.Add("The parts appear to be quite sea-resistant. Perhaps I can use them to repair the community center fishtank.");

                                generate.answers.Add("Fish... Tank? We weren't aware the little creatures had armed themselves with war machines. " +
                                    "We will warn the guardians of the depths to prepare for a fish uprising! But do as you will with the Avalant.");

                                if (!Game1.MasterPlayer.mailReceived.Contains("JojaMember") && !(Game1.getLocationFromName("CommunityCenter") as CommunityCenter).areasComplete[2])
                                {
                                    Mod.instance.questHandle.AssignQuest("relicsMists");
                                }

                                Mod.instance.relicsData.ProgressFishQuest(true);

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

                                    if (Mod.instance.characters[CharacterData.characters.Effigy].modeActive != Character.Character.mode.track)
                                    {

                                        generate.responses.Add("(follow) Come explore the valley with me.");

                                        generate.answers.Add("1");

                                    }

                                    if (Mod.instance.characters[CharacterData.characters.Effigy].modeActive != Character.Character.mode.roam)
                                    {

                                        generate.responses.Add("(work) My farm would benefit from your gentle stewardship.");

                                        generate.answers.Add("2");

                                    }

                                    if (Mod.instance.characters[CharacterData.characters.Effigy].modeActive != Character.Character.mode.home 
                                        && Mod.instance.characters[CharacterData.characters.Effigy].currentLocation.Name != CharacterData.CharacterLocation(locations.home))
                                    {

                                        generate.responses.Add("(rest) Thank you for everything you do for the circle.");

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

                                    return null;

                                case 1: // Follow player

                                    switch (character)
                                    {

                                        case characters.Effigy:

                                            generate.intro = "I will see how you exercise the authority of the sleeping kings.";
                                            
                                            break;

                                    }

                                    Mod.instance.characters[character].SwitchToMode(Character.Character.mode.track, Game1.player);

                                    return generate;

                                case 2: // Work on farm

                                    switch (character)
                                    {

                                        case characters.Effigy:

                                            generate.intro = "I will take my place amongst the posts and furrows of my old master's home.";

                                            break;

                                    }

                                    Mod.instance.characters[character].SwitchToMode(Character.Character.mode.roam, Game1.player);

                                    return generate;

                                case 3: // Return to grove

                                    switch (character)
                                    {

                                        case characters.Effigy:

                                            generate.intro = "I will return to where I may hear the rumbling energies of the valley's leylines.";

                                            break;

                                    }

                                    Mod.instance.characters[character].SwitchToMode(Character.Character.mode.home, Game1.player);

                                    return generate;

                                case 10:

                                    new Wand().DoFunction(Game1.player.currentLocation, 0, 0, 0, Game1.player);

                                    return null;

                            }

                            break;
                    }

                    break;

                case subjects.attune:

                    int toolIndex = Mod.instance.AttuneableWeapon();

                    switch (character)
                    {

                        case characters.energies:

                            if (toolIndex == -1 || toolIndex == 999)
                            {

                                return null;

                            }

                            Dictionary<int, Rite.rites> comparison = SpawnData.WeaponAttunement(true);


                            if (comparison.ContainsKey(toolIndex))
                            {

                                if (comparison[toolIndex] == Cast.Rite.rites.weald)
                                {


                                    generate.intro = "Sighs of the Earth: The " + Game1.player.CurrentTool.Name + " was crafted from materials blessed by the Lords of the Weald. It will serve your purposes as squire just fine, but its allegiance will always be to the Two Kings.";

                                    return generate;

                                }
                                else
                                {

                                    return null;

                                }

                            }

                            if (Mod.instance.save.attunement.ContainsKey(toolIndex))
                            {

                                if (Mod.instance.save.attunement[toolIndex] == Cast.Rite.rites.weald)
                                {

                                    Mod.instance.iconData.ImpactIndicator(Game1.player.currentLocation, Game1.player.Position, IconData.impacts.nature, 6f, new());

                                    Game1.player.currentLocation.playSound("yoba");

                                    Mod.instance.DetuneWeapon();

                                    generate.intro = "Sighs of the Earth: This " + Game1.player.CurrentTool.Name + " will no longer serve the Two Kings";

                                    return generate;

                                }

                                return null;

                            }

                            Mod.instance.iconData.ImpactIndicator(Game1.player.currentLocation, Game1.player.Position, IconData.impacts.nature, 6f, new());

                            Game1.player.currentLocation.playSound("yoba");

                            Mod.instance.AttuneWeapon(Cast.Rite.rites.weald);

                            generate.intro = "Sighs of the Earth: This " + Game1.player.CurrentTool.Name + " will serve the Two Kings";

                            return generate;

                        case characters.waves:

                            if (toolIndex == -1 || toolIndex == 999)
                            {

                                return null;

                            }

                            Dictionary<int, Rite.rites> comparisonMists = SpawnData.WeaponAttunement(true);

                            if (comparisonMists.ContainsKey(toolIndex))
                            {

                                if (comparisonMists[toolIndex] == Cast.Rite.rites.mists)
                                {

                                    generate.intro = "Murmurs of the Waves: The " + Game1.player.CurrentTool.Name + " is from a time before the Lady, before the mists swirled around the isle, before the city it was forged in was lost to the storm.";

                                    return generate;

                                }
                                else
                                {

                                    return null;

                                }

                            }

                            if (Mod.instance.save.attunement.ContainsKey(toolIndex))
                            {

                                if (Mod.instance.save.attunement[toolIndex] == Cast.Rite.rites.mists)
                                {

                                    Mod.instance.iconData.ImpactIndicator(Game1.player.currentLocation, Game1.player.Position, IconData.impacts.splash, 6f, new());

                                    Game1.player.currentLocation.playSound("yoba");

                                    Mod.instance.DetuneWeapon();

                                    generate.intro = "Murmurs of the Waves: This " + Game1.player.CurrentTool.Name + " will no longer serve the Lady Beyond the Shore";

                                    return generate;

                                }

                                return null;

                            }

                            Mod.instance.iconData.ImpactIndicator(Game1.player.currentLocation, Game1.player.Position, IconData.impacts.splash, 6f, new());

                            Game1.player.currentLocation.playSound("yoba");

                            Mod.instance.AttuneWeapon(Cast.Rite.rites.mists);

                            generate.intro = "Murmurs of the Waves: This " + Game1.player.CurrentTool.Name + " will serve the Lady Beyond the Shore";

                            return generate;

                        case characters.herbalism:

                            if (Mod.instance.save.herbalism.ContainsKey(Journal.HerbalData.herbals.ligna))
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
                                        "Even stranger, you know the craft of the druids that stood before this bowl in ages past. " +
                                        "With a bit of amateur effort, three stoppered flasks, each with a small amount of herbal remedy, dangle from your belt. " +
                                        "The bowl is pleased. (Herbalism journal unlocked)";

                                    Mod.instance.save.herbalism[Journal.HerbalData.herbals.ligna] = 3;

                                    Mod.instance.save.herbalism[Journal.HerbalData.herbals.impes] = 3;

                                    Mod.instance.save.herbalism[Journal.HerbalData.herbals.celeri] = 3;


                                    break;

                            }

                            return generate;

                    }

                    break;

            }

            return null;

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
