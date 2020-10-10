/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;

namespace Igorious.StardewValley.DynamicAPI.Utils
{
    public static class ExceptionHandler
    {
        public static TResult Invoke<TResult>(Func<TResult> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public static void Invoke(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }
    }
}