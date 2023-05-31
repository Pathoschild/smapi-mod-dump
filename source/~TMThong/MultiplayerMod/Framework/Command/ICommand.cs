/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerMod.Framework.Command
{
    internal interface ICommand
    {
        string Name { get; }
        string Description { get; }

        void Execute(string str, string[] args);
    }
}
