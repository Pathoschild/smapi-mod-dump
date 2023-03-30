/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using PlatoUI.Content;
using PlatoUI.UI;
using StardewValley;
using System;

namespace PlatoUI
{
    public interface IPlatoUIHelper
    {  
        IUIHelper UI { get; }
        IContentHelper Content { get; }

        StardewModdingAPI.IModHelper ModHelper { get; }
        DelayedAction SetDelayedAction(int delay, Action action);
        void SetDelayedUpdateAction(int delay, Action action);
        void SetTickDelayedUpdateAction(int delay, Action action);
    }
}
