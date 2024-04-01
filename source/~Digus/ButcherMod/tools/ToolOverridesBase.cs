/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Reflection;
using StardewValley;
using StardewValley.Tools;

namespace AnimalHusbandryMod.tools
{
    public class ToolOverridesBase
    {
        protected static void BaseToolDoFunction(Tool instance, GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            var baseMethod = typeof(Tool).GetMethod("DoFunction", BindingFlags.Public | BindingFlags.Instance);
            var functionPointer = baseMethod!.MethodHandle.GetFunctionPointer();
            var function = (Action<GameLocation, int, int, int, StardewValley.Farmer>)Activator.CreateInstance(typeof(Action<GameLocation, int, int, int, StardewValley.Farmer>), instance, functionPointer);
            function?.Invoke(location, x, y, power, who);
        }
    }
}