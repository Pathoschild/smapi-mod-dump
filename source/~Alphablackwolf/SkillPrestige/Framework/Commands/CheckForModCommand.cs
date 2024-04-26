/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

namespace SkillPrestige.Framework.Commands
{
    /// <summary>A command that checks whether a mod is installed.</summary>
    // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class CheckForModCommand : SkillPrestigeCommand
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public CheckForModCommand()
            : base("checkformod", "Checks for the existence of a mod.\n\nUsage: checkformod <uniqueId>\n- uniqueId: the mod's uniqueId as found in the manifest.", testingCommand: true) { }

        /*********
        ** Protected methods
        *********/
        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            if (args.Length < 1)
            {
                ModEntry.LogMonitor.Log("<uniqueid> must be specified");
                return;
            }
            string uniqueIdArgument = args[0];
            ModEntry.LogMonitor.Log($"mod {uniqueIdArgument} {(ModEntry.ModRegistry.IsLoaded(uniqueIdArgument) ? string.Empty : "not ")}found.");
        }
    }
}
