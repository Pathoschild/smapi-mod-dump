namespace CleanFarm
{
    /// <summary>A simple command object.</summary>
    internal interface ICommand
    {
        /// <summary>Executes the logic for the command.</summary>
        void Execute();
    }
}
