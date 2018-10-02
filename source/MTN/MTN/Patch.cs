using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Network;

namespace MTN
{
    /// <summary>
    /// Simple class used to retain information about Harmony Patches, as well as apply them.
    /// Harmony Patches are done this way to enable cross-platform support.
    /// </summary>
    public class Patch
    {
        public int infoType = 0;
        public string SDVType;
        public string original;
        public bool hasPrefix = false;
        public bool hasPostfix = false;
        public bool hasTranspiler = false;
        public Type type;
        public Type[] constructorType;

        /// <summary>
        /// The Constructor, sets all the needed information for the Harmony Patch
        /// </summary>
        /// <param name="SDVType">The class that said method belongs to, in string version. Converted internally for cross-platform support.</param>
        /// <param name="original">The name of the method we are targetting</param>
        /// <param name="prefix">Set to true if we are appending code before calling the original method.</param>
        /// <param name="postfix">Set to true if we are appending code after calling the original method.</param>
        /// <param name="transpiler">Set to true if we are directly modifying the CIL of the method (Can be dangerous)</param>
        /// <param name="patchClass">The class containing the patches</param>
        /// <param name="infoType">1 for Constructor, 0 for method. Zero by default.</param>
        /// <param name="constructorParameters">The parameters of a constructor, can also be used for otherloaded methods</param>
        public Patch(string SDVType, string original, bool prefix, bool postfix, bool transpiler, Type patchClass, int infoType = 0, Type[] Parameters = null)
        {
            this.infoType = infoType;
            this.SDVType = SDVType;
            this.original = original;
            hasPrefix = prefix;
            hasPostfix = postfix;
            hasTranspiler = transpiler;
            type = patchClass;
            constructorType = Parameters;
        }

        /// <summary>
        /// Applies the Harmony Patch to the game code.
        /// </summary>
        /// <param name="h">The instance (targetted application) harmony will patch up.</param>
        public void Apply(HarmonyInstance h)
        {
            ConstructorInfo con;
            MethodInfo orig;
            MethodInfo prefix;
            MethodInfo postfix;
            MethodInfo transpiler;

            prefix = (hasPrefix) ? Memory.instance.Helper.Reflection.GetMethod(type, "Prefix").MethodInfo : null;
            postfix = (hasPostfix) ? Memory.instance.Helper.Reflection.GetMethod(type, "Postfix").MethodInfo : null;
            transpiler = (hasTranspiler) ? Memory.instance.Helper.Reflection.GetMethod(type, "Transpiler").MethodInfo : null;

            switch (infoType) {
                case 0:
                    orig = AccessTools.Method(getTypeSDV(SDVType), original, constructorType);
                    h.Patch(orig, (prefix != null) ? new HarmonyMethod(prefix) : null, (postfix != null) ? new HarmonyMethod(postfix) : null, (transpiler != null) ? new HarmonyMethod(transpiler) : null); ;
                    break;
                case 1:
                    con = AccessTools.Constructor(getTypeSDV(SDVType), constructorType);
                    h.Patch(con, (prefix != null) ? new HarmonyMethod(prefix) : null, (postfix != null) ? new HarmonyMethod(postfix) : null, (transpiler != null) ? new HarmonyMethod(transpiler) : null);
                    break;
            }
            
            return;
        }

        /// <summary>
        /// Converts the name of the class, in string form, of a class in SDV to a Type. Done to enable cross-platform support.
        /// </summary>
        /// <param name="type">The SDV class, in string form</param>
        /// <returns></returns>
        private static Type getTypeSDV(string type)
        {
            string prefix = "StardewValley.";
            Type defaulSDV = Type.GetType(prefix + type + ", Stardew Valley");

            if (defaulSDV != null)
                return defaulSDV;
            else
                return Type.GetType(prefix + type + ", StardewValley");

        }
    }
}
