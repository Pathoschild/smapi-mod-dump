using NpcAdventure.Internal;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NpcAdventure.Utils
{
    internal static partial class DialogueHelper
    {
        private static bool GetDialogueString(Dictionary<string, string> dialogues, string key, out string text)
        {
            var keys = from _key in dialogues.Keys
                       where _key.StartsWith(key + "~") || _key.StartsWith(key + "$")
                       select _key;

            if (keys.Count() > 0)
            {
                int i = Game1.random.Next(0, keys.Count() + 1);

                Console.WriteLine($"{i} of {keys.Count()}");

                if (i < keys.Count() && dialogues.TryGetValue(keys.ElementAt(i), out text))
                    return true;
            }

            if (dialogues.TryGetValue(key, out text))
                return true;

            text = key;

            return false;
        }

        public static bool GetDialogueString(NPC n, string key, out string text)
        {
            if (GetDialogueString(n.Dialogue, key, out text))
                return true;

            text = $"{n.Name}.{text}";

            return false;
        }

        public static string GetDialogueString(NPC n, string key)
        {
            GetDialogueString(n, key, out string text);
            return text;
        }

        public static bool GetVariousDialogueString(NPC n, string key, out string text)
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
                if (GetDialogueString(n, k, out text))
                    return true;

            text = key;
            return false;
        }

        public static bool GetVariousDialogueString(NPC n, string key, GameLocation l, out string text)
        {
            return GetVariousDialogueString(n, $"{key}_{l.Name}", out text);
        }

        public static bool GetBubbleString(Dictionary<string, string> bubbles, NPC n, string type, out string text)
        {
            if (GetDialogueString(bubbles, $"{type}_{n.Name}", out text))
            {
                text = string.Format(text, Game1.player?.Name, n.Name);

                return true;
            }

            return false;
        }

        public static bool GetBubbleString(Dictionary<string, string> bubbles, NPC n, GameLocation l, out string text)
        {
            return GetBubbleString(bubbles, n, l.Name, out text);
        }

        public static Dialogue GenerateDialogue(NPC n, string key, bool returnsNull = true)
        {
            if (GetVariousDialogueString(n, key, out string text) || !returnsNull)
                return new CompanionDialogue(text, n) { Tag = $"{n.Name}_{key}" };

            return null;
        }

        public static Dialogue GenerateDialogue(NPC n, GameLocation l, string key, bool returnsNull = true)
        {
            if (GetVariousDialogueString(n, key, l, out string text) || !returnsNull)
            {
                return new CompanionDialogue(text, n) { Tag = $"{n.Name}_{key}_{l.Name}" };
            }

            return null;
        }

        public static Dialogue GenerateStaticDialogue(NPC n, string key)
        {
            if (GetDialogueString(n, key, out string text))
            {
                return new CompanionDialogue(text, n) { Tag = $"{n.Name}_{key}" };
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

        public static void DrawDialogue(Dialogue dialogue)
        {
            NPC speaker = dialogue.speaker;

            speaker.CurrentDialogue.Push(dialogue);
            Game1.drawDialogue(speaker);
        }
    }
}
