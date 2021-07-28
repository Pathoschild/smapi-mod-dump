/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

namespace SkillPrestige.Commands
{
    /// <summary>
    /// A command that checks wehther a mod is installed
    /// </summary>
    /// // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class CheckForModCommand : SkillPrestigeCommand
    {

        public CheckForModCommand() : base("checkformod", "Checks for the existence of a mod.\n\nUsage: checkformod <uniqueId>\n- uniqueId: the mod's uniqueId as found in the manifest.") { }

        protected override bool TestingCommand => true;

        protected override void Apply(string[] args)
        {
            if (args.Length < 1)
            {
                SkillPrestigeMod.LogMonitor.Log("<uniqueid> must be specified");
                return;
            }
            var uniqueIdArgument = args[0];
            SkillPrestigeMod.LogMonitor.Log($"mod {uniqueIdArgument} {(SkillPrestigeMod.ModRegistry.IsLoaded(uniqueIdArgument) ? string.Empty : "not ")}found.");
        }
    }
}
