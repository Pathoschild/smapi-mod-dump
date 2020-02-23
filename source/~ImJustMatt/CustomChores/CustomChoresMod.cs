using System;
using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores.Framework;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LeFauxMatt.CustomChores
{
    /// <summary>The mod entry point.</summary>
    public class CustomChoresMod : Mod
    {
        public static CustomChoresMod Instance { get; private set; }

        /*********
        ** Fields
        *********/
        /// <summary>Adds new types of custom chore.</summary>
        private readonly ChoreBuilder _choreBuilder = new ChoreBuilder();

        /// <summary>Instances of custom chores created by content packs.</summary>
        private readonly IDictionary<string, IChore> _chores = new Dictionary<string, IChore>(StringComparer.OrdinalIgnoreCase);

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            // init
            Instance = this;

            // add console commands
            Helper.ConsoleCommands.Add("chores_Do", "Performs a chore.\n\nUsage: chore_Do <value>\n- value: chore by name.", DoChore);
            Helper.ConsoleCommands.Add("chores_CanDo", "Checks if a chore can be done.\n\nUsage: chore_CanDo <value>\n- value: chore by name.", CheckChore);
            Helper.ConsoleCommands.Add("chores_ListAll", "Lists all chores.\n\nUsage: chore_ListAll", ListChores);
            
            // hook events
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        public override object GetApi()
        {
            return new CustomChoresApi(_choreBuilder, _chores);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Performs a chore when the 'chore_do' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void DoChore(string command, string[] args)
        {
            // Get Chore
            _chores.TryGetValue(args[0], out var chore);
            if (chore is null)
            {
                Monitor.Log($"A chore of name {args[0]} does not exist.", LogLevel.Alert);
                return;
            }

            // Check Chore
            if (!chore.CanDoIt())
            {
                Monitor.Log($"Cannot perform chore {args[0]}", LogLevel.Alert);
                return;
            }

            // Try Chore
            if (chore.DoIt())
            {
                Monitor.Log($"Successfully performed chore {args[0]}", LogLevel.Info);
                return;
            }

            Monitor.Log($"Failed to perform chore {args[0]}", LogLevel.Alert);
        }

        /// <summary>Checks if a chore can be done when the 'chore_canDo' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void CheckChore(string command, string[] args)
        {
            // Get Chore
            _chores.TryGetValue(args[0], out var chore);
            if (chore is null)
            {
                Monitor.Log($"A chore of name {args[0]} does not exist.", LogLevel.Alert);
                return;
            }

            // Check Chore
            if (chore.CanDoIt())
            {
                Monitor.Log($"Can perform chore {args[0]}", LogLevel.Info);
                return;
            }

            Monitor.Log($"Cannot perform chore {args[0]}", LogLevel.Info);
        }

        /// <summary>Lists all chores when 'chore_listAll' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void ListChores(string command, string[] args)
        {
            foreach (var chore in _chores)
            {
                Monitor.Log($"- {chore.Key}", LogLevel.Info);
            }
        }

        /****
        ** Event handlers
        ****/
        /// <summary>The method invoked on the first update tick, once all mods are initialized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get mod apis


            // add default chore factory
            _choreBuilder.AddChoreFactory(new BaseChoreFactory());
        }

        /// <summary>The event called after a save slot is loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            _chores.Clear();
            
            // create chore instances from content packs
            foreach (var contentPack in Helper.ContentPacks.GetOwned())
            {
                if (!contentPack.HasFile("chore.json"))
                {
                    Monitor.Log($"Missing chores.json from {contentPack.Manifest.UniqueID}. Skipping.", LogLevel.Warn);
                    continue;
                }
                
                Monitor.Log($"Loading chore.json from {contentPack.Manifest.UniqueID}.");

                var translations = (
                    from translation in contentPack.Translation.GetTranslations()
                    select new TranslationData(translation)).ToList();

                var choreData = new ChoreData(
                    contentPack.Manifest.UniqueID,
                    contentPack.ReadJsonFile<IDictionary<string, object>>("chore.json"),
                    translations,
                    contentPack.HasFile("assets/image.png") ? contentPack.LoadAsset<Texture2D>("assets/image.png") : null);

                var chore = _choreBuilder.GetChore(choreData);
                if (chore == null)
                {
                    Monitor.Log($"Failed to create chore from {contentPack.Manifest.UniqueID}. Skipping.", LogLevel.Warn);
                    continue;
                }

                _chores.Add(contentPack.Manifest.UniqueID, chore);
                Monitor.Log($"Loaded chore {contentPack.Manifest.UniqueID}.", LogLevel.Debug);
            }
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