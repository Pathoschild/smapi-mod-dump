/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using NpcAdventure.Loader;
using NpcAdventure.StateMachine;
using NpcAdventure.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NpcAdventure.Dialogues
{
    /// <summary>
    /// Provides a companion dialogues for assigned NPC.
    /// Also provides static helper tools for work with static dialogues (like speech bubbles)
    /// </summary>
    public partial class DialogueProvider
    {
        public const char FLAG_RANDOM = '~';
        public const char FLAG_CHANCE = '^';
        private readonly IContentLoader contentLoader;
        private readonly string sourcePrefix;
        private readonly SortedDictionary<string, List<string>> keyLookupCache;
        private readonly VariousKeyGenerator keygen;
        private Dictionary<string, string> dialogueCache;
        private readonly CompanionStateMachine csm;

        /// <summary>
        /// Create an instance of dialogue provider with assigned NPC and content loader
        /// </summary>
        /// <param name="csm">Companion State Machine</param>
        /// <param name="contentLoader">Content asset loader</param>
        /// <param name="sourcePrefix">Asset source file prefix</param>
        internal DialogueProvider(CompanionStateMachine csm, IContentLoader contentLoader, string sourcePrefix = "Dialogue/")
        {
            this.csm = csm;
            this.contentLoader = contentLoader;
            this.sourcePrefix = sourcePrefix;
            this.dialogueCache = new Dictionary<string, string>();
            this.keyLookupCache = new SortedDictionary<string, List<string>>();
            this.keygen = new VariousKeyGenerator();
        }

        public NPC Npc { get => this.csm.Companion; }

        /// <summary>
        /// Setup dialogue provider for new day.
        /// Change dialogue key generator settings, clear all provider's caches,
        /// (re)load dialogue strings from content source and assign them to NPC's dialogue registry
        /// </summary>
        public void SetupForNewDay()
        {
            if (this.Npc == null)
                return;

            Farmer f = Game1.player;

            this.keygen.Date = SDate.Now();
            this.keygen.Weather = Helper.GetCurrentWeatherName();
            this.keygen.IsNight = Game1.isDarkOut();
            this.keygen.FriendshipHeartLevel = f.getFriendshipHeartLevelForNPC(this.Npc.Name);
            this.keygen.FriendshipStatus = FetchFriendshipStatus(f, this.Npc);

            this.keyLookupCache.Clear();
            this.dialogueCache.Clear();
            this.keygen.PossibleKeys.Clear();
            this.keygen.GenerateVariousKeys();
            this.LoadDialogues();
        }

        /// <summary>
        /// (Re)load dialogues from source to NPC's dialogue registry
        /// </summary>
        public void LoadDialogues()
        {
            this.dialogueCache = new Dictionary<string, string>().Concat(
                this.contentLoader.LoadStrings($"{this.sourcePrefix}{this.Npc.Name}"))
                .ToDictionary(d => d.Key, d => d.Value);

            SetupCompanionDialogues(this.Npc, this.dialogueCache);
        }

        /// <summary>
        /// Refresh dialogues to NPC dialogue registry 
        /// if the key missing in this registry but exists in content source.
        /// </summary>
        /// <param name="key">Probably missing key</param>
        private void ReloadIfLost(string key)
        {
            if (this.dialogueCache.Count <= 0 || this.dialogueCache.ContainsKey(key) && !this.Npc.Dialogue.ContainsKey(key))
            {
                // Reaload and refresh NPC dialogues only when 
                // the key is missing in NPC dialogue registry but in pre-loaded cache this key exists.
                this.LoadDialogues();
            }
        }

        /// <summary>
        /// Returns a raw dialogue string as key-value pair from specified dialogue registry
        /// </summary>
        /// <param name="dialogues">A dialogue registry</param>
        /// <param name="key">For which key we want to fetch a dialogue?</param>
        /// <param name="rawDialogue">Fetched dialogue. Value part is <paramref name="key"/> if dialogue not exists for this key</param>
        /// <returns>True if raw dialogue found and returned in <paramref name="rawDialogue"/></returns>
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

        /// <summary>
        /// Returns a raw dialogue string as key-value pair from NPC's dialogue registry.
        /// If dialogue for requested key was lost from NPC's registry, refresh it and try again.
        /// </summary>
        /// <param name="key">For which key we want to fetch a dialogue?</param>
        /// <param name="rawDialogue">Fetched dialogue. Value part is <paramref name="key"/> if dialogue not exists for this key</param>
        /// <param name="retryReload">Reload dialogues to NPC's regisry if lost them?</param>
        /// <returns>True if raw dialogue found and returned in <paramref name="rawDialogue"/></returns>
        public bool GetRawDialogue(string key, out KeyValuePair<string, string> rawDialogue, bool retryReload = true)
        {
            if (GetRawDialogue(this.Npc.Dialogue, key, out rawDialogue))
                return true;

            if (retryReload)
            {
                // Dialogue not found? Companion dialogue list probably erased, reload it...
                this.ReloadIfLost(key);

                // ...and try again to fetch dialogue
                if (GetRawDialogue(this.Npc.Dialogue, key, out rawDialogue))
                    return true;
            }

            // Dialogue still can't be fetch? So we mark this dialogue as undefined and return dialogue key path as text
            rawDialogue = new KeyValuePair<string, string>(key, $"{this.Npc.Name}.{key}");

            return false;
        }

        /// <summary>
        /// Returns a dialogue text for NPC as string.
        /// Can returns spouse dialogue, if farmer are married with NPC and this dialogue is defined
        /// 
        /// Lookup dialogue key patterns: {key}_Spouse, {key}_Dating {key}
        /// </summary>
        /// <param name="f">Farmer</param>
        /// <param name="key">Dialogue key</param>
        /// <returns>A dialogue text</returns>
        public string GetFriendSpecificDialogueText(Farmer f, string key)
        {
            if (Helper.IsSpouseMarriedToFarmer(this.Npc, f) && this.GetRawDialogue($"{key}_Spouse", out KeyValuePair<string, string> rawSpousedDialogue))
            {
                return rawSpousedDialogue.Value;
            }

            if (f.friendshipData.TryGetValue(this.Npc.Name, out Friendship friendship)
                && friendship.IsDating()
                && this.GetRawDialogue($"{key}_Dating", out KeyValuePair<string, string> rawDatingDialogue))
            {
                return rawDatingDialogue.Value;
            }

            this.GetRawDialogue(key, out KeyValuePair<string, string> rawDialogue);
            return rawDialogue.Value;
        }

        /// <summary>
        /// Get a variated dialogue by environment state, concreted day or month, friendship atatus and more.
        /// </summary>
        /// <param name="key">Key of requested kind of dialogue</param>
        /// <param name="rawDialogue">The requested dialogue. If the dialogue not exists, contains a <paramref name="key"/> as dialogue text.</param>
        /// <returns>True if requested kind of dialogue exists.</returns>
        public bool GetVariableRawDialogue(string key, out KeyValuePair<string, string> rawDialogue)
        {
            // Try to find a relevant dialogue
            if (!this.TryFetchVariableDialogue(key, out rawDialogue))
            {
                // No dialogue found? Companion dialogues are probably lost, reload them
                this.ReloadIfLost(key);

                // And try dialogue fetch again. 
                // Returns false when dialogue really not exists and as text will be returned dialogue key path
                return this.TryFetchVariableDialogue(key, out rawDialogue);
            }

            return true;
        }

        private bool TryFetchVariableDialogue(string baseKey, out KeyValuePair<string, string> rawDialogue)
        {
            if (this.CheckGeneratedKeysIntegrity())
            {
                // Clear key lookup cache, pre-generated possible keys (they are outdated now)
                // and regenerate new relevant possible keys
                this.keyLookupCache.Clear();
                this.keygen.PossibleKeys.Clear();
                this.keygen.GenerateVariousKeys();
            }

            if (!this.keyLookupCache.ContainsKey(baseKey))
            {
                // Filter only existing dialogues possible keys and save them to cache under baseKey
                var filteredPossibleKeys = from key in this.keygen.PossibleKeys
                                           where this.dialogueCache.ContainsKey(baseKey + key)
                                           select key;

                this.keyLookupCache.Add(baseKey, filteredPossibleKeys.ToList());
            }


            foreach (string k in this.keyLookupCache[baseKey])
                if (this.GetRawDialogue(baseKey + k, out rawDialogue, false))
                    return true;
            rawDialogue = new KeyValuePair<string, string>(baseKey, $"{this.Npc.Name}.{baseKey}");

            return false;
        }

        private bool CheckGeneratedKeysIntegrity()
        {
            Farmer f = Game1.player;
            var isNight = Game1.isDarkOut();
            FriendshipStatus friendshipStatus = FetchFriendshipStatus(f, this.Npc);
            var heartLevel = f.getFriendshipHeartLevelForNPC(this.Npc.Name);
            bool invalidated = false;

            if (this.keygen.IsNight != isNight || this.keygen.FriendshipStatus != friendshipStatus || this.keygen.FriendshipHeartLevel != heartLevel)
            {
                this.keygen.IsNight = isNight;
                this.keygen.FriendshipHeartLevel = heartLevel;
                this.keygen.FriendshipStatus = friendshipStatus;
                invalidated = true;
            }

            if (!invalidated && this.keygen.PossibleKeys.Count <= 0)
            {
                invalidated = true;
            }

            return invalidated;
        }

        private static FriendshipStatus FetchFriendshipStatus(Farmer f, NPC n)
        {
            if (f.friendshipData.TryGetValue(n.Name, out Friendship friendship))
            {
                return friendship.Status;
            }
            
            return FriendshipStatus.Friendly;
        }

        /// <summary>
        /// Get a variated dialogue for a game location by environment state, 
        /// concreted day or month, friendship atatus and more.
        /// </summary>
        /// <param name="key">Key of requested kind of dialogue</param>
        /// <param name="l">For which location?</param>
        /// <param name="rawDialogue">
        ///     The requested dialogue. 
        ///     If the dialogue not exists, contains a <paramref name="key"/> and location name as dialogue text.
        /// </param>
        /// <returns>True if requested kind of dialogue exists.</returns>
        public bool GetVariableRawDialogue(string key, GameLocation l, out KeyValuePair<string, string> rawDialogue)
        {
            return this.GetVariableRawDialogue($"{key}_{l.Name}", out rawDialogue);
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
        internal static bool GetAmbientBubbleString(Dictionary<string, string> bubbles, NPC n, GameLocation l, out string text)
        {
            return GetBubbleString(bubbles, n, $"ambient_{l.Name}", out text);
        }

        /// <summary>
        /// Generate a variable dialogue
        /// </summary>
        /// <param name="n"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dialogue GenerateDialogue(string key)
        {
            if (this.GetVariableRawDialogue(key, out KeyValuePair<string, string> rawDilogue))
                return CreateDialogueFromRaw(this.Npc, rawDilogue);

            return null;
        }

        /// <summary>
        /// Generate a variable dialogue for location
        /// </summary>
        /// <param name="n"></param>
        /// <param name="l"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dialogue GenerateDialogue(GameLocation l, string key)
        {
            if (this.GetVariableRawDialogue(key, l, out KeyValuePair<string, string> rawDialogue))
            {
                return CreateDialogueFromRaw(this.Npc, rawDialogue);
            }

            return null;
        }

        /// <summary>
        /// Generate pure static dialogue
        /// </summary>
        /// <param name="n"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dialogue GenerateStaticDialogue(string key)
        {
            if (this.GetRawDialogue(key, out KeyValuePair<string, string> rawDialogue))
            {
                return CreateDialogueFromRaw(this.Npc, rawDialogue);
            }

            return null;
        }

        /// <summary>
        /// Generate pure static dialogue for a location
        /// </summary>
        /// <param name="n"></param>
        /// <param name="l"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dialogue GenerateStaticDialogue(GameLocation l, string key)
        {
            return this.GenerateStaticDialogue($"{key}_{l.Name}");
        }

        /// <summary>
        /// Setup dialogues from specific registry to companion NPC's dialogue registry.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="dialogues"></param>
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
