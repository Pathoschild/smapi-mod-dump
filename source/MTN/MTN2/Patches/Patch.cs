/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches {

    /// <summary>
    /// Enumeration declaring the type of Harmony patch.
    /// </summary>
    public enum PatchType {
        Method,
        Constructor
    }

    /// <summary>
    /// Simple class used to retain information pertaining a Harmony Patch, in addition to applying the patch.
    /// This style of patching enables cross-platform support.
    /// </summary>
    public class Patch {
        private readonly object instance;
        private readonly IModHelper helper;
        private string sdvType;
        private string original;
        private PatchType patchType;
        private bool prefix;
        private bool postfix;
        private bool transpiler;
        private Type type;
        private Type[] parameters;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="instance">The instance of MTN (Readonly).</param>
        /// <param name="sdvType">The class containing the method/constructor to be patched.</param>
        /// <param name="original">The method/constructor to be patched.</param>
        /// <param name="patchType">The type of code we are targetting. Method, or Constructor?</param>
        public Patch(IModHelper helper, string sdvType, string original, PatchType patchType, object instance) {
            this.instance = instance;
            this.helper = helper;
            this.sdvType = sdvType;
            this.original = original;
            this.patchType = patchType;
        }

        /// <summary>
        /// Set needed information for the Harmony Patch.
        /// </summary>
        /// <param name="prefix">Affix code to the beginning of the method/constructor.</param>
        /// <param name="postfix">Affix code to the end of the method/constructor</param>
        /// <param name="transpiler">Directly modify the CLR of the method (dangerousu)</param>
        /// <param name="type">The MTN class containing the patch</param>
        /// <param name="parameters">The parameters of the targetted method/constructor. Used for overloaded methods/constructors.</param>
        public void Initialize(bool prefix, bool postfix, bool transpiler, Type type, Type[] parameters = null) { 
            this.prefix = prefix;
            this.postfix = postfix;
            this.transpiler = transpiler;
            this.type = type;
            this.parameters = parameters;
        }

        /// <summary>
        /// Applies the Harmony Patch to the game code
        /// </summary>
        /// <param name="harmony">The instance (targetted application) harmony will patch up.</param>
        public void Apply(HarmonyInstance harmony) {
            ConstructorInfo Constructor = null;
            MethodInfo Original = null;
            MethodInfo Prefix = (prefix) ? helper.Reflection.GetMethod(type, "Prefix").MethodInfo : null;
            MethodInfo Transpile = (transpiler) ? helper.Reflection.GetMethod(type, "Transpiler").MethodInfo : null;
            MethodInfo Postfix = (postfix) ? helper.Reflection.GetMethod(type, "Postfix").MethodInfo : null;

            if (patchType == PatchType.Constructor) {
                Constructor = AccessTools.Constructor(getTypeSDV(sdvType), parameters);
                harmony.Patch(Constructor, (prefix) ? new HarmonyMethod(Prefix) : null,
                                        (postfix) ? new HarmonyMethod(Postfix) : null,
                                        (transpiler) ? new HarmonyMethod(Transpile) : null);
            } else {
                Original = AccessTools.Method(getTypeSDV(sdvType), original, parameters);
                harmony.Patch(Original, (prefix) ? new HarmonyMethod(Prefix) : null,
                                        (postfix) ? new HarmonyMethod(Postfix) : null,
                                        (transpiler) ? new HarmonyMethod(Transpile) : null);
            }

            return;
        }

        /// <summary>
        /// Converts the name of the class, in string form, of a class in SDV to a Type. Done to enable cross-platform support.
        /// </summary>
        /// <param name="type">Name of the SDV class</param>
        /// <returns></returns>
        private Type getTypeSDV(string type) {
            string prefix = "StardewValley.";
            Type defaultSDV = Type.GetType(prefix + type + ", Stardew Valley");

            if (defaultSDV != null) {
                return defaultSDV;
            }

            return Type.GetType(prefix + type + ", StardewValley");
        }
    }
}
