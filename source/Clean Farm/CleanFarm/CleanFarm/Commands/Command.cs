/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tstaples/CleanFarm
**
*************************************************/

using System;

namespace CleanFarm
{
    /// <summary>A simple command object.</summary>
    internal class Command : ICommand
    {
        /// <summary>The action to execute.</summary>
        Action Action;

        /// <summary>Constructor.</summary>
        /// <param name="Action">The action to execute.</param>
        public Command(Action Action)
        {
            this.Action = Action;
        }

        /// <summary>Executes the logic for the command.</summary>
        public void Execute()
        {
            Action();
        }
    }
}
