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
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using MoreConversationTopics.Integrations;
using System.Reflection;

namespace MoreConversationTopics
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        // Properties
        private ModConfig Config;

        public const string modContentPath = "Mods/vl.mct/RepeatableTopics";

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
                this.Monitor.Log(this.Helper.Translation.Get("IllFormattedConfig"), LogLevel.Warn);
            }

            // Initialize the error logger and config in all the other files via a helper function
            MCTHelperFunctions.Initialize(this.Monitor, this.Config, helper);

            // Do the Harmony things
            var harmony = new Harmony(this.ModManifest.UniqueID);
            RepeatPatcher.Apply(harmony);
            WeddingPatcher.Apply(harmony);
            BirthPatcher.Apply(harmony);
            DivorcePatcher.Apply(harmony);
            LuauPatcher.Apply(harmony);
            WorldChangePatcher.Apply(harmony);
            NightEventPatcher.Apply(harmony);
            IslandPatcher.Apply(harmony);

            // Adds a command to check current active conversation topics
            helper.ConsoleCommands.Add("vl.mct.current_CTs", "Returns a list of the current active dialogue events.", MCTHelperFunctions.console_GetCurrentCTs);

            // Adds a command to see if player has a given mail flag
            helper.ConsoleCommands.Add("vl.mct.has_flag", "Checks if the player has a mail flag.\n\nUsage: vl.mct_hasflag <flagName>\n- flagName: the possible mail flag name.", MCTHelperFunctions.console_HasMailFlag);

            // Adds a command to add a conversation topic
            helper.ConsoleCommands.Add("vl.mct.add_CT", "Adds the specified conversation topic with duration of 1 day.\n\nUsage: vl.mct_add_CT <topicName> <duration>\n- topicName: the conversation topic to add.\n- duration: duration of conversation topic to add.", MCTHelperFunctions.console_AddConversationTopic);

            // Adds a command to remove a conversation topic
            helper.ConsoleCommands.Add("vl.mct.remove_CT", "Removes the specified conversation topic.\n\nUsage: vl.mct_remove_CT <topicName>\n- topicName: the conversation topic to remove.", MCTHelperFunctions.console_RemoveConversationTopic);

            // Adds a command to check if a conversation topic is repeatable
            helper.ConsoleCommands.Add("vl.mct.is_repeatable_CT", "Checks whether the specified conversation topic is repeatable.\n\nUsage: vl.mct_remove_CT <topicName>\n- topicName: the conversation topic to check.", MCTHelperFunctions.console_IsRepeatableCT);

            // Adds a command to print all repeatable conversation topics
            helper.ConsoleCommands.Add("vl.mct.repeatable_CTs", "Returns a list of all repeatable conversation topics.", MCTHelperFunctions.console_AllRepeatableCTs);

            // Add GMCM
            helper.Events.GameLoop.GameLaunched += this.RegisterGMCM;

            // Add asset editor
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            JojaEventAssetEditor.Edit(e);
            MCTHelperFunctions.LoadRepeatTopics(e);
        }

        /// <summary>
        /// Generates the GMCM for this mod by looking at the structure of the config class.
        /// </summary>
        /// <param name="sender">Unknown, expected by SMAPI.</param>
        /// <param name="e">Arguments for event.</param>
        /// <remarks>To add a new setting, add the details to the i18n file. Currently handles: bool.</remarks>
        private void RegisterGMCM(object sender, GameLaunchedEventArgs e)
        {
            if (this.Helper.ModRegistry.Get("spacechase0.GenericModConfigMenu") is not IModInfo gmcm)
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
    }
}