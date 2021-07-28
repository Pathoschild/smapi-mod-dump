/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using PlatoTK.Content;
using PlatoTK.Patching;
using PlatoTK.Network;
using PlatoTK.UI;
using StardewValley;
using System;
using PlatoTK.Events;
using PlatoTK.Lua;
using PlatoTK.Utils;
using PlatoTK.Presets;

namespace PlatoTK
{
    public interface IPlatoHelper
    {
        ISharedDataHelper SharedData { get; }
        IHarmonyHelper Harmony { get; }
        IContentHelper Content { get; }
        ILuaHelper Lua { get; }

        IPresetHelper Presets { get; }

        IPlatoEventsHelper Events { get; }

        IUIHelper UI { get; }

        IBasicUtils Utilities { get; }
        StardewModdingAPI.IModHelper ModHelper { get; }
        DelayedAction SetDelayedAction(int delay, Action action);

        void SetDelayedUpdateAction(int delay, Action action);
        void SetTickDelayedUpdateAction(int delay, Action action);
        bool CheckConditions(string conditions, object caller);

        void AddConditionsProvider(IConditionsProvider provider);
    }
}
