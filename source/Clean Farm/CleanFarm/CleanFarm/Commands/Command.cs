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
