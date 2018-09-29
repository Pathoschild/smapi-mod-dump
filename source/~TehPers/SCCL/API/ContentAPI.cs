using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.API {
    public class ContentAPI {
        //internal static ContentAPI INSTANCE { get; } = new ContentAPI();
        internal static Dictionary<string, ContentInjector> mods = new Dictionary<string, ContentInjector>();
        private static Dictionary<string, Type> injectorDelegateTypes = new Dictionary<string, Type>();

        private ContentAPI() { }

        /**
         * <summary>Returns the injector with the given name, or a new injector if none exists</summary>
         * <param name="name">The name of the injector. This can be your mod's name.</param>
         * <returns>The injector with the given name, or a new one if needed</returns>
         **/
        public static ContentInjector GetInjector(Mod owner, string name) {
            if (!mods.ContainsKey(name)) mods[name] = new ContentInjector(owner, name);
            return mods[name];
        }

        /**
         * <summary>Returns a list of all registered injectors</summary>
         * <returns>A string[] containing the names of all injectors</returns>
         **/
        public static string[] GetAllInjectors() {
            return mods.Keys.ToArray();
        }

        /// <summary>Returns whether the specified injector has been created yet</summary>
        /// <param name="name">The name of the injector</param>
        /// <returns>True if the injector exists already</returns>
        public static bool InjectorExists(string name) {
            return mods.ContainsKey(name);
        }
    }
}
