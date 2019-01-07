using StardewModdingAPI;
using System;
using System.Reflection;
using System.Linq;

namespace BattleRoyale
{
    class AntiCheat
    {
        public static string IllegalMod { get; private set; } = null;

        public static bool HasIllegalMods()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!IsLegal(assembly))
                {
                    IllegalMod = assembly.FullName;
                    return true;
                }

            }

            return false;
        }

        private static bool IsLegal(Assembly assembly)
        {
            try
            {
                string name = assembly.GetName().Name;
                if (name == "SplitScreen" || name == "Elevator" || name == "WindowResize" || name == "ServerBookmarker" || name == "Social Tab Patch" || name == "ServerBrowser")
                    return true;
            }
            catch (Exception) { }

            if (assembly != Assembly.GetExecutingAssembly())
                foreach (Type type in assembly.GetTypes())
                    if (type.IsSubclassOf(typeof(Mod)))
                        return false;

            return true;
        }

		internal static bool IsLegal(IMultiplayerPeerMod mod)
		{
			return new string[] { "Ilyaki.BattleRoyale", "Ilyaki.Elevator", "Ilyaki.SplitScreen", "thatnzguy.WindowResize", "Ilyaki.ServerBookmarker", "funnysnek.socialtabpatch", "Ilyaki.ServerBrowser" }.Contains(mod.ID);
		}
    }
}
