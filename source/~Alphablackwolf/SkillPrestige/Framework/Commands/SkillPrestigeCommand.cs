/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Linq;
using SkillPrestige.Logging;
using StardewModdingAPI;

namespace SkillPrestige.Framework.Commands
{
    /// <summary>Represents a command called in the SMAPI console interface.</summary>
    internal abstract class SkillPrestigeCommand
    {
        /*********
        ** Fields
        *********/
        /// <summary>The name used to call the command in the console.</summary>
        private string Name { get; }

        /// <summary>The help description for the command.</summary>
        private string Description { get; }

        /// <summary>Whether the command is used only in test mode.</summary>
        private bool TestingCommand { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Register all loaded command types.</summary>
        /// <param name="helper">The SMAPI command helper.</param>
        /// <param name="testCommands">Whether to only register testing commands.</param>
        public static void RegisterCommands(ICommandHelper helper, bool testCommands)
        {
            var commandTypes = typeof(SkillPrestigeCommand)
                .Assembly
                .GetTypesSafely()
                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(SkillPrestigeCommand)));

            foreach (var commandType in commandTypes)
            {
                var command = (SkillPrestigeCommand)Activator.CreateInstance(commandType);
                if (command != null && !(testCommands ^ command.TestingCommand))
                    command.RegisterCommand(helper);
            }
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name used to call the command in the console.</param>
        /// <param name="description">The help description for the command.</param>
        /// <param name="testingCommand">Whether the command is used only in test mode.</param>
        protected SkillPrestigeCommand(string name, string description, bool testingCommand = false)
        {
            this.Name = name;
            this.Description = description;
            this.TestingCommand = testingCommand;
        }

        /// <summary>Registers a command with the SMAPI console.</summary>
        private void RegisterCommand(ICommandHelper helper)
        {
            Logger.LogInformation($"Registering {this.Name} command...");
            helper.Add(this.Name, this.Description, (_, args) => this.Apply(args));
            Logger.LogInformation($"{this.Name} command registered.");
        }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected abstract void Apply(string[] args);
    }
}
