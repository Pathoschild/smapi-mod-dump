using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Harmony;
using LeFauxMatt.CustomChores;
using LeFauxMatt.CustomChores.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using ModConfig = LeFauxMatt.HelpfulSpouses.Models.ModConfig;

namespace LeFauxMatt.HelpfulSpouses
{
    public class HelpfulSpousesMod : Mod
    {
        internal static HelpfulSpousesMod Instance { get; private set; }
        internal static ICustomChoresApi CustomChoresApi;

        /*********
        ** Fields
        *********/
        /// <summary>A list of chores and enumerable dialogue.</summary>
        private readonly IDictionary<string, ChoreData> _chores = new Dictionary<string, ChoreData>(StringComparer.OrdinalIgnoreCase);

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            // init
            Instance = this;
            Helper.ReadConfig<ModConfig>();

            // add console commands
            Helper.ConsoleCommands.Add("spouse_AddChore", "Makes spouse perform a chore.\n\nUsage: spouse_AddChore <value>\n- value: chore by name.", AddChore);

            // hook events
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Does chore and adds dialogue to your spouse when the 'spouse_addChore' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void AddChore(string command, string[] args)
        {
            // Get Spouse
            var spouse = Game1.player.getSpouse();
            if (spouse == null)
                return;

            // Get Chore
            if (!_chores.Any() && !UpdateChores())
                return;

            _chores.TryGetValue(args[0], out var chore);
            if (chore is null)
            {
                Monitor.Log($"A chore of name {args[0]} does not exist.", LogLevel.Alert);
                return;
            }

            // Check Chore
            if (!CustomChoresApi.CheckChore(chore.ChoreName))
            {
                Monitor.Log($"Cannot perform chore {args[0]}", LogLevel.Alert);
                return;
            }

            // Try Chore
            if (!CustomChoresApi.DoChore(chore.ChoreName))
            {
                Monitor.Log($"Failed to perform chore {args[0]}", LogLevel.Alert);
                return;
            }

            Monitor.Log($"Successfully performed chore {args[0]}", LogLevel.Info);

            // Add Dialogue
            var tokens = CustomChoresApi.GetChoreTokens(chore.ChoreName);
            tokens.Add("Mod", () => "HelpfulSpousesMod");
            tokens.Add("Priority", () => "");

            IList<TranslationData> dialogue = null;

            // Get Prioritized Dialogue
            for (var priority = 1; priority <= 3; ++priority)
            {
                // Dialogue by order of priority
                tokens["Priority"] = priority.ToString;
                dialogue = (
                    from translation in chore.Translations
                    where translation.Key.Equals("Dialogue", StringComparison.CurrentCultureIgnoreCase)
                          && translation.Filter(tokens)
                          && translation.HasSelector("Priority")
                    select translation).ToList();
                if (dialogue.Any())
                    break;
            }

            // Get Default Dialogue
            if (dialogue == null || !dialogue.Any())
            {
                tokens.Remove("Priority");
                dialogue = (
                    from translation in chore.Translations
                    where translation.Key.Equals("Dialogue", StringComparison.CurrentCultureIgnoreCase)
                          && translation.Filter(tokens)
                    select translation).ToList();
            }

            var dialogueText = dialogue.Shuffle().First().Tokens(tokens);
            
            if (!string.IsNullOrWhiteSpace(dialogueText))
                spouse.setNewDialogue(dialogueText, true);
            else
                Monitor.Log($"No dialogue found for chore {args[0]}", LogLevel.Warn);
        }

        /****
        ** Event handlers
        ****/
        /// <summary>The method invoked on the first update tick, once all mods are initialized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // init
            CustomChoresApi = Helper.ModRegistry.GetApi<ICustomChoresApi>("furyx639.CustomChores");

            // harmony patches to prevent default chores
            var harmony = HarmonyInstance.Create("furyx639.MarriageDutiesMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.marriageDuties)),
                prefix: new HarmonyMethod(typeof(NpcPatches), nameof(NpcPatches.MarriageDuties_Prefix))
            );
        }

        /// <summary>The method invoked when a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var r = new Random((int)Game1.stats.DaysPlayed +
                               (int)Game1.uniqueIDForThisGame / 2 +
                               (int)Game1.player.UniqueMultiplayerID);

            if (Game1.timeOfDay != 600 || !Game1.player.isMarried() || r.NextDouble() >= ModConfig.Instance.GlobalChance)
                return;

            // Get Spouse
            var spouse = Game1.player.getSpouse();
            if (spouse == null)
                return;

            // Get Spouse Chores
            ModConfig.Instance.Spouses.TryGetValue(spouse.Name, out var spouseConfig);
            if (spouseConfig == null || !spouseConfig.Any())
                return;

            // Get Spouse Hearts
            Game1.player.friendshipData.TryGetValue(spouse.Name, out var friendship);
            var spouseHearts = friendship.Points / NPC.friendshipPointsPerHeartLevel;
            if (spouseHearts < ModConfig.Instance.HeartsNeeded && ModConfig.Instance.HeartsNeeded > 0)
                return;

            // Get Chores
            var spouseChores =
                from choreConfig in spouseConfig
                where r.NextDouble() < choreConfig.Value
                select choreConfig.Key;

            // Get Randomized Chores
            if (!_chores.Any() && !UpdateChores())
                return;

            var choreList = (
                from chore in _chores
                where spouseChores.Contains(chore.Key)
                select chore.Value).Shuffle();

            // Perform Chores
            var choresDone = ModConfig.Instance.DailyLimit;
            foreach (var chore in choreList)
            {
                // Check Chore
                if (!CustomChoresApi.CheckChore(chore.ChoreName))
                    continue;
                
                // Try Chore
                if (!CustomChoresApi.DoChore(chore.ChoreName))
                    continue;

                Monitor.Log($"Successfully performed chore {chore.ChoreName}");

                // Add Dialogue
                
                var tokens = CustomChoresApi.GetChoreTokens(chore.ChoreName);
                tokens.Add("Mod", () => "HelpfulSpouses");
                tokens.Add("Priority", () => "");

                IList<TranslationData> dialogue = null;

                // Get Prioritized Dialogue
                for (var priority = 1; priority <= 3; ++priority)
                {
                    // Dialogue by order of priority
                    var currentPriority = priority;
                    tokens["Priority"] = () => currentPriority.ToString(CultureInfo.InvariantCulture);
                    dialogue = (
                        from translation in chore.Translations
                        where translation.Key.Equals("Dialogue", StringComparison.CurrentCultureIgnoreCase)
                              && translation.Filter(tokens)
                              && translation.HasSelector("Priority")
                        select translation).ToList();
                    if (dialogue.Any()) break;
                }

                // Get Default Dialogue
                if (dialogue == null || !dialogue.Any())
                {
                    tokens.Remove("Priority");
                    dialogue = (
                        from translation in chore.Translations
                        where translation.Key.Equals("Dialogue", StringComparison.CurrentCultureIgnoreCase)
                              && translation.Filter(tokens)
                        select translation).ToList();
                }

                var dialogueText = dialogue.Shuffle().First().Tokens(tokens);

                if (!string.IsNullOrWhiteSpace(dialogueText))
                    spouse.setNewDialogue(dialogueText, true);
                if (--choresDone <= 0)
                    break;
            }
        }

        private bool UpdateChores()
        {
            _chores.Clear();

            // get chores
            foreach (var choreKey in CustomChoresApi.GetChores())
            {
                _chores.Add(choreKey, CustomChoresApi.GetChore(choreKey));
            }

            return _chores.Any();
        }
    }

    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (rng == null)
                throw new ArgumentNullException(nameof(rng));
            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            var buffer = source.ToList();
            for (var i = 0; i < buffer.Count; i++)
            {
                var j = rng.Next(i, buffer.Count);
                yield return buffer[j];
                buffer[j] = buffer[i];
            }
        }
    }
}
