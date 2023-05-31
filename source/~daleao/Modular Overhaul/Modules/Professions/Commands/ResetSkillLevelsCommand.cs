/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using System.Linq;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;

#endregion using directives

[UsedImplicitly]
internal sealed class ResetSkillLevelsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ResetSkillLevelsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ResetSkillLevelsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "reset_levels", "reset_skills", "reset" };

    /// <inheritdoc />
    public override string Documentation =>
        "Reset the level of the specified skills, or all skills if none are specified. Does not remove professions.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Skill.List.ForEach(s => s.Reset());
            SCSkill.Loaded.ForEach(s => s.Value.Reset());
        }
        else
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (Skill.TryFromName(args[i], true, out var skill))
                {
                    skill.Reset();
                }
                else
                {
                    var customSkill = SCSkill.Loaded.Values.FirstOrDefault(s =>
                        string.Equals(s.DisplayName, args[i], StringComparison.CurrentCultureIgnoreCase));
                    if (customSkill is null)
                    {
                        Log.W($"Ignoring unknown skill {args[i]}.");
                        continue;
                    }

                    customSkill.Reset();
                }
            }
        }
    }
}
