using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableTravelingCart
{
    public static class Util
    {
        public static void invokeEvent(string name, IEnumerable<Delegate> handlers, object sender)
        {
            var args = new EventArgs();
            foreach (EventHandler handler in handlers.Cast<EventHandler>())
            {
                try
                {
                    handler.Invoke(sender, args);
                }
                catch (Exception e)
                {
                    CustomizableCartRedux.Logger.Log($"Exception while handling event {name}:\n{e}", LogLevel.Trace);
                }
            }
        }

        public static void invokeEvent<T>(string name, IEnumerable<Delegate> handlers, object sender, T args)
        {
            foreach (EventHandler<T> handler in handlers.Cast<EventHandler<T>>())
            {
                try
                {
                    handler.Invoke(sender, args);
                }
                catch (Exception e)
                {
                    CustomizableCartRedux.Logger.Log($"Exception while handling event {name}:\n{e}", LogLevel.Trace);
                }
            }
        }
    }
}
