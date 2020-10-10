/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tstaples/CleanFarm
**
*************************************************/

namespace CleanFarm
{
    /// <summary>A simple command object.</summary>
    internal interface ICommand
    {
        /// <summary>Executes the logic for the command.</summary>
        void Execute();
    }
}
