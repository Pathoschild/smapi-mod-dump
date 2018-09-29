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