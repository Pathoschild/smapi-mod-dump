/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/MoreConversationTopics
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using MoreConversationTopics.Integrations;
using System.Reflection;

namespace MoreConversationTopics
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        // Properties
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Read in config file and create if needed
            try
            {
                this.Config = this.Helper.ReadConfig<ModConfig>();
            }
            catch (Exception)
            {
                this.Config = new ModConfig();
                this.Monitor.Log(this.Helper.Translation.Get("IllFormatedConfig"), LogLevel.Warn);
            }

            // Initialize the error logger in WeddingPatcher
            WeddingPatcher.Initialize(this.Monitor, this.Config);
            LuauPatcher.Initialize(this.Monitor, this.Config);
            BirthPatcher.Initialize(this.Monitor, this.Config);
            DivorcePatcher.Initialize(this.Monitor, this.Config);
            RepeatPatcher.Initialize(this.Monitor, this.Config);
            WorldChangePatcher.Initialize(this.Monitor, this.Config);

            // Do the Harmony things
            var harmony = new Harmony(this.ModManifest.UniqueID);
            WeddingPatcher.Apply(harmony);
            LuauPatcher.Apply(harmony);
            BirthPatcher.Apply(harmony);
            DivorcePatcher.Apply(harmony);
            RepeatPatcher.Apply(harmony);
            WorldChangePatcher.Apply(harmony);

            // Adds a command to check current active conversation topics
            helper.ConsoleCommands.Add("current_conversation_topics", "Returns a list of the current active dialogue events", (str, strs) =>
            {
                if (!Context.IsWorldReady)
                    return;

                // Add a test event to see if it's working
                //if (!Game1.player.activeDialogueEvents.ContainsKey("testDialogueEvent"))
                //{
                //    Game1.player.activeDialogueEvents.Add("testDialogueEvent", 1);
                //}

                Monitor.Log(string.Join(", ", Game1.player.activeDialogueEvents.Keys),LogLevel.Debug);
            });

            // Adds a command to see if player has a given mail flag
            helper.ConsoleCommands.Add("player_hasmailflag", "Checks if the player has a mail flag.\n\nUsage: player_hasmailflag <flagName>\n- flagName: the possible mail flag name.", this.HasMailFlag);

            // Adds a command to add a conversation topic
            helper.ConsoleCommands.Add("add_conversation_topic", "Adds the specified conversation topic with duration of 1 day.\n\nUsage: add_conversation_topic <flagName>\n- flagName: the conversation topic to add.", this.AddConversationTopic);

            // Adds a command to remove a conversation topic
            helper.ConsoleCommands.Add("remove_conversation_topic", "Removes the specified conversation topic.\n\nUsage: remove_conversation_topic <flagName>\n- flagName: the conversation topic to remove.", this.RemoveConversationTopic);

            // Add GMCM
            helper.Events.GameLoop.GameLaunched += this.RegisterGMCM;
        }

        /// <summary>
        /// Generates the GMCM for this mod by looking at the structure of the config class.
        /// </summary>
        /// <param name="sender">Unknown, expected by SMAPI.</param>
        /// <param name="e">Arguments for event.</param>
        /// <remarks>To add a new setting, add the details to the i18n file. Currently handles: bool.</remarks>
        private void RegisterGMCM(object sender, GameLaunchedEventArgs e)
        {
            IModInfo gmcm = this.Helper.ModRegistry.Get("spacechase0.GenericModConfigMenu");
            if (gmcm is null)
            {
                this.Monitor.Log(this.Helper.Translation.Get("GmcmNotFound"), LogLevel.Debug);
                return;
            }
            if (gmcm.Manifest.Version.IsOlderThan("1.6.0"))
            {
                this.Monitor.Log(this.Helper.Translation.Get("GmcmVersionMessage", new { version = "1.6.0", currentversion = gmcm.Manifest.Version.ToString() }), LogLevel.Info);
                return;
            }
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config));

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("mod-description"));

            foreach (PropertyInfo property in typeof(ModConfig).GetProperties())
            {
                MethodInfo getter = property.GetGetMethod();
                MethodInfo setter = property.GetSetMethod();
                if (getter is null || setter is null)
                {
                    this.Monitor.Log("Config appears to have a mis-formed option?");
                    continue;
                }
                if (property.PropertyType.Equals(typeof(int)))
                {
                    var getterDelegate = (Func<ModConfig, int>)Delegate.CreateDelegate(typeof(Func<ModConfig, int>), getter);
                    var setterDelegate = (Action<ModConfig, int>)Delegate.CreateDelegate(typeof(Action<ModConfig, int>), setter);
                    configMenu.AddNumberOption(
                        mod: this.ModManifest,
                        getValue: () => getterDelegate(this.Config),
                        setValue: (int value) => setterDelegate(this.Config, value),
                        name: () => this.Helper.Translation.Get($"{property.Name}.title"),
                        tooltip: () => this.Helper.Translation.Get($"{property.Name}.description"),
                        min: 1,
                        max: 14,
                        interval: 1);
                }
                else
                {
                    this.Monitor.Log($"{property.Name} unaccounted for.", LogLevel.Warn);
                }
            }
        }

        // Helper function to check if a string is on the list of repeatable CTs added by this mod
        public static Boolean isRepeatableCTAddedByMod(string topic)
        {
            string[] modRepeatableConversationTopics = new string[] { "wedding", "luauBest", "luauShorts", "luauPoisoned", "divorce", "babyBoy", "babyGirl" };
            foreach (string s in modRepeatableConversationTopics)
            {
                if (s == topic)
                {
                    return true;
                }
            }
            return false;
        }

        // Checks mail flags for console command
        private void HasMailFlag(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            try
            {
                if (Game1.player.mailReceived.Contains(args[0]))
                {
                    this.Monitor.Log($"Yes, you have this mail flag", LogLevel.Debug);
                }
                else
                {
                    this.Monitor.Log($"No, you don't have this mail flag", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Bad or missing argument with exception: {ex}", LogLevel.Error);
            }
        }

        // Add conversation topic for console command
        private void AddConversationTopic(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            try
            {
                Game1.player.activeDialogueEvents.Add(args[0], 1);
                this.Monitor.Log($"Added conversation topic", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Bad or missing argument with exception: {ex}", LogLevel.Error);
            }
        }

        // Remove conversation topic for console command
        private void RemoveConversationTopic(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            try
            {
                Game1.player.activeDialogueEvents.Remove(args[0]);
                this.Monitor.Log($"Removed conversation topic", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Bad or missing argument with exception: {ex}", LogLevel.Error);
            }
        }
    }
}