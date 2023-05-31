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

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class ClearNewLevelsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ClearNewLevelsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ClearNewLevelsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "clear_new_levels" };

    /// <inheritdoc />
    public override string Documentation =>
        "Clear the player's cache of new levels for the specified skills, or all vanilla skills if none are specified.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Game1.player.newLevels.Clear();
        }
        else
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (Skill.TryFromName(args[i], true, out var skill))
                {
                    Game1.player.newLevels.Set(Game1.player.newLevels
                        .Where(p => p.X != skill)
                        .ToList());
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

                    var newLevels = Reflector
                        .GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
                        .Invoke();
                    Reflector
                        .GetStaticFieldSetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
                        .Invoke(newLevels.Where(pair => pair.Key != customSkill.StringId).ToList());
                }
            }
        }
    }
}
