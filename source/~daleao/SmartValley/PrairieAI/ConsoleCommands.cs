/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Prairie.Training;

#region using directives

using StardewModdingAPI;

using Framework;
using Framework.Events;

#endregion using directives

internal static class ConsoleCommands
{
    internal static void Register(this ICommandHelper helper)
    {
        helper.Add("neat_train",
            "Begin training the neural network.", BeginTraining);
    }

    #region command handlers

    /// <summary>Begin training the neural network.</summary>
    /// <param name="command">The console command.</param>
    /// <param name="args">The supplied arguments.</param>
    private static void BeginTraining(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        NeatExperiment.Initialize();
        new EvolutionAlgorithmUpdateEvent().Hook();
        NeatExperiment.Start();
    }

    #endregion command handlers
}