/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

namespace NeverToxic.StardewMods.LittleHelpersCore.Framework.Commands
{
    internal abstract class BaseCommand : ICommand
    {
        public abstract bool CanExecute(int tile);

        public abstract void Execute(int tile);

        public void Handle(int tile)
        {
            if (this.CanExecute(tile))
                this.Execute(tile);
        }
    }
}
