/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using PurrplingCore.Internal;
using StardewModdingAPI.Events;
using StardewValley.Monsters;

namespace NpcAdventure.AI.Controller
{
    internal interface IController : IUpdateable
    {
        bool IsIdle { get; }
        void Activate();
        void Deactivate();
        void SideUpdate(UpdateTickedEventArgs e);
    }
}
