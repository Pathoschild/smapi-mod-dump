/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewDruid.Character;
using StardewValley.Locations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewDruid.Character.CharacterHandle;
using StardewValley.Minigames;

namespace StardewDruid.Journal
{
    public class LoreData
    {

        public static string RequestLore(CharacterHandle.characters character)
        {

            switch (character)
            {
                default:
                case characters.Effigy:

                    return "(lore) Tell me more about the Circle of Druids.";

                case characters.Jester:

                    return "(lore) What is the mission of the Jester of Fate?";

                case characters.Revenant:

                    return "(lore) After so many years, you must have a lot to talk about.";

                case characters.Shadowtin:

                    return "(lore) Do you have time to talk about the Undervalley?";

            }

        }

        public static string CharacterLore(CharacterHandle.characters character)
        {

            switch (character)
            {

                default:
                case characters.Effigy:

                    return "The Effigy: Our traditions are etched into the bedrock of the valley.";

                case characters.Jester:

                    return "The Jester of Fate: I enjoy answering questions. One of my dearest sisters was a sphinx.";

                case characters.Revenant:

                    return "The Revenant: The bats are chatty. So I never want for something to talk to. Or yell at.";

                case characters.Shadowtin:

                    return "Shadowtin Bear: Certainly. As long as the discussion relates to treasure. Or Dragons.";

            }

        }

        public enum stories
        {
            
            Weald_Effigy,

            Mists_Effigy,
            
            Effigy_self_1,
            Effigy_Jester_1,
            Effigy_Shadowtin_1,

            Stars_Effigy,
            Stars_Revenant,

            Revenant_self_1,

            Jester_Effigy_1,
            Jester_self_1,
            Jester_Shadowtin_1,

            Fates_Effigy,
            Fates_Jester,
            
            Shadowtin_Effigy_1,
            Shadowtin_Jester_1,
            Shadowtin_self_1,
            
            Ether_Effigy,
            Ether_Jester,
            Ether_Shadowtin,

        }

        public static Dictionary<LoreData.stories,Lorestory> LoreList()
        {

            Dictionary<LoreData.stories, Lorestory> storylist = new();

            // ===========================================
            // Weald

            storylist[stories.Weald_Effigy] = new()
            {
                story = stories.Weald_Effigy,
                character = characters.Effigy,
                question = "What role do the Two Kings play?",
                answer = "In times past, the King of Oaks and the King of Holly would war upon the Equinox. " +
                                "Their warring would conclude for the sake of new life in Spring. When need arose, they lent their strength to a conflict from which neither could fully recover. " +
                                "They rest now, dormant. May those awake guard the change of seasons.",

            };

            // ===========================================
            // Mists

            storylist[stories.Mists_Effigy] = new()
            {
                story = stories.Mists_Effigy,
                character = characters.Effigy,
                question = "Who is the Lady Beyond the Shore?",
                answer = "The Lady of the Isle of Mists is as beautiful and distant as the sunset on the Gem Sea. She was once a courtier of the Two Kings, from a time before the war. " +
                                "The first farmer was closest to her in counsel and in conviction. She helped establish the circle and remained here a shortwhile before she was called to the Isle. " +
                                "(The Effigy's eyes flicker a brilliant turqoise). " +
                                "There is a feeling that comes with my memories of that time, a feeling I cannot describe.",

            };

            // ===========================================
            // Effigy

            storylist[stories.Effigy_self_1] = new()
            {
                story = stories.Effigy_self_1,
                character = characters.Effigy,
                question = "I want to know more about the First Farmer",
                answer = "The first farmer was blessed by the elderborn, the monarchs of the valley, to cultivate and protect this special land. " +
                                "He used this blessing to construct me, and showed me how I could preserve his techniques for a future successor. " +
                                "Though my friend is long gone, I remain, because the power of the elders remain. For now.",

            };

            storylist[stories.Effigy_Jester_1] = new()
            {
                story = stories.Effigy_Jester_1,
                character = characters.Jester,
                question = "Do you think the Effigy could learn the mysteries of the fates?",
                answer = "As powerful as he is, I think he's set in his ways. Set in the past. Well at least I think that's why he doesn't like my tricks. " +
                            "He talks about the first of the valley farmers all the time. They must have have been good friends. Has he asked you to build a pyre yet? (Jester gives a mischievous smirk)",

            };

            storylist[stories.Effigy_Shadowtin_1] = new()
            {
                story = stories.Effigy_Shadowtin_1,
                character = characters.Shadowtin,
                question = "From your perspective, the Effigy must seem a strange mystical artifact",
                answer = "Of all the constructs embued with the power of the elderborn, I've never heard of one so loyal to his former master. " +
                "I've done my own assessment of the quality of his make. The clothes and head-dress are cheap garbage. And threadbare. " +
                "I suspect a large cat has been kneading them, as the back is scratched and covered in fur. " +
                "You'll probably need to replace them at some point. Or burn them. The real value in the Effigy is a fashioned inner core that is saturated with elder power. " +
                "It's the heart, and the brain. A treasure from the elder age.",

            };


            // ===========================================
            // Stars

            storylist[stories.Stars_Effigy] = new()
            {
                story = stories.Stars_Effigy,
                character = characters.Effigy,
                question = "Do the Stars have names?",
                answer = "The Stars have no names that can be uttered by earthly dwellers. " +
                                "They exist in the Great Expanse of the celestial realm, and care not for the life of our world, though their light sustains much of it. " +
                                "The Stars that have shone on you belong to a broken constellation of Sisters, who all mourn for one who fell to our realm. " +
                                "In fact, the circle of druids afforded a worldly name to the missing star-born. (Effigy's flaming eyes flicker). I have been forbidden to share it.",

            };

            storylist[stories.Stars_Revenant] = new()
            {
                story = stories.Stars_Revenant,
                character = characters.Revenant,
                question = "Tell me how you ended up like this",
                answer = "Well now. It's hard to believe, seeing as they are so pretty and we're so, well, plain, but a star came here once. " +
                        "For a time, as the star graced our humble realm, the kings stopped warring over the seasons. They gardened palaces, wrote ballads, made wine, played music. " +
                        "Still, the world turns, the seasons must change, and the old war resumed. But the stakes had changed. The ancient ones, the dragons, wanted the star for themselves. " +
                        "What came next was fire, death, and misery everywhere. I believed in the mission of the Star Guardians, so I trained as a holy warrior, and was accepted into their order. " +
                        "I survived much of the fighting, but many fell to the wrath of the heavens, so much so that it was hard to know whether our cause was just or folly. " +
                        "I think for all those lost, Yoba wept, because the Fates were sent to make it right. The Star-born, however, had already vanished. " +
                        "The Reaper of Fate tracked me here, and when he found me, he accused me of circumventing justice. He cursed me, never to die, never to rest, and never to leave. " +
                        "So I remain here, a feeble skeleton of a man, while the reaper is still out there, still looking for the star that fell so long ago."

            };

            // ===========================================
            // Revenant

            storylist[stories.Revenant_self_1] = new()
            {
                story = stories.Revenant_self_1,
                character = characters.Revenant,
                question = "So, giant bats. Friends of yours?",
                answer = "Heh heh. It's the sacred water that gives them vitality, and fills their heads with all the whispers of the mists. " +
                        "After a few generations they started to speak, shrieking and flapping about, yapping on about the Voice beyond the Shore. " +
                        "But there's only so much a creature can do before it betrays it's nature. " +
                        "So they hang in the dark. They wait. They sing when it rains."

            };

            // ===========================================
            // Fates

            storylist[stories.Fates_Jester] = new()
            {
                story = stories.Fates_Jester,
                character = characters.Jester,
                question = "Tell me more about your kin, the Fates",
                answer = "Every Fate has a special role we're given by Fortumei, the greatest of us, priestess of Yoba. " +
                "Some of us are fairies, and care for the fates of plants and little things. " +
                "For my contribution, well, I've had some pretty cool moments... (Jester is pensive as his voice trails off)",

            };

            // ===========================================
            // Jester

            storylist[stories.Jester_Effigy_1] = new()
            {
                story = stories.Jester_Effigy_1,
                character = characters.Effigy,
                question = "What do you know of Jester and the Fates?",
                answer = "The Fates weave the cords of destiny into the great tapestry that is the story of the world. " +
                            "It is said that they each serve a special purpose known only to Yoba, and so they often appear to work by mystery and happenchance, by whim even. " +
                            "(Effigy motions ever so slightly in the direction of Jester) They should not be underestimated or trifled with. " +
                            "No matter how whimsical they appear, their adherance to the decrees of their high priestess is absolute.",

            };

            storylist[stories.Jester_self_1] = new()
            {
                story = stories.Jester_self_1,
                character = characters.Jester,
                question = "How goes your search for the fallen one?",
                answer = "I'm as lost as when I started. But, I have found out something about myself, something embarrassing, even disturbing, and yet, I must accept it. I like to hide in boxes.",

            };

            storylist[stories.Jester_Shadowtin_1] = new()
            {
                story = stories.Jester_Shadowtin_1,
                character = characters.Shadowtin,
                question = "Jester has no care for treasures.",
                answer = "He claims to be a powerful agent of destiny, but he's as blind as a newborn kitten, and naive about the horrors that await him on his quest into the undervalley. " +
                "I doubt he's been sent by Yoba. I doubt Yoba still cares about any of us."

            };

            // ===========================================
            // Shadowtin

            storylist[stories.Shadowtin_Effigy_1] = new()
            {
                story = stories.Shadowtin_Effigy_1,
                character = characters.Effigy,
                question = "Our circle now has it's own treasure hunter",
                answer = "All manner of otherfolk traded and befriended with the first farmer, but he always had the most trouble with the shadowfolk. " +
                "It's difficult to see their intentions. It's difficult to see them in any lack of light."

            };

            storylist[stories.Shadowtin_Jester_1] = new()
            {
                story = stories.Shadowtin_Jester_1,
                character = characters.Jester,
                question = "Shadowtin doesn't believe in luck. Or chance. Or fortune.",
                answer = "I think I get what he wants, I mean, trinkets and shiny things are great. But they aren't everything. He said he'd help me uncover the secret of the Fallen Star, but he doesn't care about my sacred mission. " +
                "Still, I think he has a part to play for Yoba in our great purpose. (Jester grins) You can just beat him up again if he tries to double-cross us.",

            };


            storylist[stories.Shadowtin_self_1] = new()
            {
                story = stories.Shadowtin_self_1,
                character = characters.Shadowtin,
                question = "How did you and the other shadowfolk come into the service of Lord Deep?",
                answer = "The folklore of shadows is enscribed on the outer surface of the great vessel. " +
                "The narrative starts with Lord Deep, before the first of my forefolk is mentioned, and the stories suggest we have always been subservient to him. But I believe those first enscriptions have been tampered with. " +
                "I know now, from research, that the vessel is dragon-forged. Perhaps we served an ancient one, perhaps Lord Deep rewrote our history. I hope my travels and the treasures we uncover yield answers.",

            };


            // ===========================================
            // Ether

            storylist[stories.Ether_Effigy] = new()
            {
                story = stories.Ether_Effigy,
                character = characters.Effigy,
                question = "Who were the Masters of the Ether?",
                answer = "I know very little of Dragonkind and their ilk. They were the first servants of Yoba, and perhaps they disappointed their creator. " +
                "Their bones have become the foundation of the other world, their potent life essence has become the streams of ether that flow through the planes."

            };

            storylist[stories.Ether_Jester] = new()
            {
                story = stories.Ether_Jester,
                character = characters.Jester,
                question = "Where are the Dragons?",
                answer = "We're talking about creatures that could reforge the world itself, Farmer. They don't like our kind, otherfolk or humanfolk. " +
                "Actually I'm... (Jester's hairs raise across it's backside) kind of scared of them. I'm glad you have my back!",

            };

            storylist[stories.Ether_Shadowtin] = new()
            {
                story = stories.Ether_Shadowtin,
                character = characters.Shadowtin,
                question = "So what dragon treasures have you found?",
                answer = "I've found this cloak, and this carnyx, and this bear mask. The Dragons would demand the best tributes, from the greatest artisans, with the finest materials available. " +
                "All Shadowfolk prize such treasures, and it's a very competitive society, so I have to carry mine with me at all times.",

            };

            //I believe that, in his quest for Justice, he cursed all his followers to become spectres. Perhaps to guard his secrets, or worse, to harbour the essence of their souls.

            return storylist;

        }

    }

    public class Lorestory
    {

        public LoreData.stories story;

        public CharacterHandle.characters character;

        public string question;

        public string answer;

    }

}
