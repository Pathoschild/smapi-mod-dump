using NpcAdventure.Internal;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace NpcAdventure.Utils
{
    public static partial class DialogueHelper
    {
        public const char FLAG_RANDOM = '~';
        public const char FLAG_CHANCE = '^';

        public static bool GetRawDialogue(Dictionary<string, string> dialogues, string key, out KeyValuePair<string, string> rawDialogue)
        {
            var keys = from _key in dialogues.Keys
                       where _key.StartsWith(key + FLAG_RANDOM) || _key.StartsWith(key + FLAG_CHANCE)
                       select _key;
            var randKeys = keys.Where((k) => k.Contains(FLAG_RANDOM)).ToList();
            var chanceKeys = keys.Where((k) => k.Contains(FLAG_CHANCE)).ToList();

            if (chanceKeys.Count > 0)
            {
                // Chance conditioned dialogue
                foreach (string k in chanceKeys)
                {
                    var s = k.Split(FLAG_CHANCE);
                    float chance = float.Parse(s[1]) / 100;
                    if (Game1.random.NextDouble() <= chance && dialogues.TryGetValue(k, out string chancedText))
                    {
                        rawDialogue = new KeyValuePair<string, string>(k, chancedText);
                        return true;
                    }
                }
            }

            if (randKeys.Count > 0)
            {
                // Randomized dialogue
                int i = Game1.random.Next(0, randKeys.Count() + 1);

                if (i < randKeys.Count() && dialogues.TryGetValue(randKeys[i], out string randomText))
                {
                    rawDialogue = new KeyValuePair<string, string>(randKeys[i], randomText);
                    return true;
                }
            }

            if (dialogues.TryGetValue(key, out string text))
            {
                // Standard dialogue
                rawDialogue = new KeyValuePair<string, string>(key, text);
                return true;
            }

            rawDialogue = new KeyValuePair<string, string>(key, key);

            return false;
        }

        public static bool GetRawDialogue(NPC n, string key, out KeyValuePair<string, string> rawDialogue)
        {
            if (GetRawDialogue(n.Dialogue, key, out rawDialogue))
                return true;

            rawDialogue = new KeyValuePair<string, string>(key, $"{n.Name}.{key}");

            return false;
        }

        /// <summary>
        /// Returns a dialogue text for NPC as string.
        /// Can returns spouse dialogue, if famer are married with NPC and this dialogue is defined
        /// 
        /// Lookup dialogue key patterns: {key}_Spouse, {key}
        /// </summary>
        /// <param name="n"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSpecificDialogueText(NPC n, Farmer f, string key)
        {
            if (Helper.IsSpouseMarriedToFarmer(n, f) && GetRawDialogue(n, $"{key}_Spouse", out KeyValuePair<string, string> rawSpousedDialogue))
                return rawSpousedDialogue.Value;

            GetRawDialogue(n, key, out KeyValuePair<string, string> rawDialogue);
            return rawDialogue.Value;
        }

        public static bool GetVariableRawDialogue(NPC n, string key, out KeyValuePair<string, string> rawDialogue)
        {
            Farmer f = Game1.player;
            VariousKeyGenerator keygen = new VariousKeyGenerator()
            {
                Date = SDate.Now(),
                IsNight = Game1.isDarkOut(),
                IsMarried = Helper.IsSpouseMarriedToFarmer(n, f),
                FriendshipHeartLevel = f.getFriendshipHeartLevelForNPC(n.Name),
                Weather = Helper.GetCurrentWeatherName(),
            };

            // Generate possible dialogue keys
            keygen.GenerateVariousKeys(key);

            // Try to find a relevant dialogue
            foreach (string k in keygen.PossibleKeys)
                if (GetRawDialogue(n, k, out rawDialogue))
                    return true;

            rawDialogue = new KeyValuePair<string, string>(key, $"{n.Name}.{key}");

            return false;
        }

        public static bool GetVariableRawDialogue(NPC n, string key, GameLocation l, out KeyValuePair<string, string> rawDialogue)
        {
            return GetVariableRawDialogue(n, $"{key}_{l.Name}", out rawDialogue);
        }

        /// <summary>
        /// Returns a specific speech bubble for an NPC.
        /// 
        /// Definition pattern: `<type>_<npc>`
        /// </summary>
        /// <param name="bubbles"></param>
        /// <param name="n"></param>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static bool GetBubbleString(Dictionary<string, string> bubbles, NPC n, string type, out string text)
        {
            bool fullyfill = GetRawDialogue(bubbles, $"{type}_{n.Name}", out KeyValuePair<string, string> rawDialogue);

            text = string.Format(rawDialogue.Value, Game1.player?.Name, n.Name);
            
            return fullyfill;
        }

        /// <summary>
        /// Returns a location speech bubble for an NPC. This bubble definition must be prefixed with `ambient_`.
        /// 
        /// Whole definition pattern: `ambient_<location>_<npc>`
        /// </summary>
        /// <param name="bubbles"></param>
        /// <param name="n"></param>
        /// <param name="l"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static bool GetBubbleString(Dictionary<string, string> bubbles, NPC n, GameLocation l, out string text)
        {
            return GetBubbleString(bubbles, n, $"ambient_{l.Name}", out text);
        }

        public static Dialogue GenerateDialogue(NPC n, string key)
        {
            if (GetVariableRawDialogue(n, key, out KeyValuePair<string, string> rawDilogue))
                return CreateDialogueFromRaw(n, rawDilogue);

            return null;
        }

        public static Dialogue GenerateDialogue(NPC n, GameLocation l, string key)
        {
            if (GetVariableRawDialogue(n, key, l, out KeyValuePair<string, string> rawDialogue))
            {
                return CreateDialogueFromRaw(n, rawDialogue);
            }

            return null;
        }

        public static Dialogue GenerateStaticDialogue(NPC n, string key)
        {
            if (GetRawDialogue(n, key, out KeyValuePair<string, string> rawDialogue))
            {
                return CreateDialogueFromRaw(n, rawDialogue);
            }

            return null;
        }

        public static Dialogue GenerateStaticDialogue(NPC n, GameLocation l, string key)
        {
            return GenerateStaticDialogue(n, $"{key}_{l.Name}");
        }

        public static void SetupCompanionDialogues(NPC n, Dictionary<string, string> dialogues)
        {
            foreach (var pair in dialogues)
                n.Dialogue[pair.Key] = pair.Value;
        }

        private static Dialogue CreateDialogueFromRaw(NPC n, KeyValuePair<string, string> rawDialogue)
        {
            var dialogue = CompanionDialogue.Create(rawDialogue.Value, n, rawDialogue.Key);

            if (rawDialogue.Key.Contains(FLAG_RANDOM))
                dialogue.SpecialAttributes.Add("randomized");

            if (rawDialogue.Key.Contains(FLAG_CHANCE))
                dialogue.SpecialAttributes.Add("possibly");

            return dialogue;
        }

        internal static void DrawDialogue(Dialogue dialogue)
        {
            NPC speaker = dialogue.speaker;

            speaker.CurrentDialogue.Push(dialogue);
            Game1.drawDialogue(speaker);
        }

        public static void RemoveDialogueFromStack(NPC n, Dialogue dialogue)
        {
            Stack<Dialogue> temp = new Stack<Dialogue>(n.CurrentDialogue.Count);

            while (n.CurrentDialogue.Count > 0)
            {
                Dialogue d = n.CurrentDialogue.Pop();

                if (!d.Equals(dialogue))
                    temp.Push(d);
            }

            while (temp.Count > 0)
                n.CurrentDialogue.Push(temp.Pop());
        }
    }
}
