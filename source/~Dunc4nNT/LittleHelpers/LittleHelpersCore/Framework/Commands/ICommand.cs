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
    internal interface ICommand
    {
        bool CanExecute(int tile);

        void Handle(int tile);

        void Execute(int tile);
    }
}
