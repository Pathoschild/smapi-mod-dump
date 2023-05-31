/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace AchtuurCore.Patches
{
    /// <summary>
    /// Generic patcher that can be implemented by other patches. Derived classes should implement 1 method that serves as the patch.
    /// </summary>
    public abstract class GenericPatcher
    {
        /// <summary>
        /// Apply the patch the derived method implements using <see cref="Harmony.Patch"/>
        /// </summary>
        /// <param name="harmony">Harmony instance</param>
        /// <param name="monitor">Monitor instance provided by <see cref="StardewModdingAPI.IMonitor"/>, used for logging if necessary</param>
        public abstract void Patch(Harmony harmony, IMonitor monitor);

        /// <summary>
        /// Get method that will be patched as <see cref="MethodInfo"/> using <see cref="System.Reflection"/>.
        /// </summary>
        /// <typeparam name="MethodClass">Class that contains the method that will be patched</typeparam>
        /// <param name="methodname">Name of the original method that will be patched</param>
        /// <returns></returns>
        public MethodInfo getOriginalMethod<MethodClass>(string methodname)
        {
            return AccessTools.Method(typeof(MethodClass), methodname);
        }


        /// <summary>
        /// Get <see cref="HarmonyMethod"/> from this patcher' patch method
        /// </summary>
        /// <param name="methodName">Name of the method that implements the patch</param>
        /// <param name="priority">Priority of returned <see cref="HarmonyMethod"/>, use <see cref="HarmonyPriority"/> to set priorities.</param>
        /// <param name="before"><inheritdoc cref="HarmonyMethod.before"/></param>
        /// <param name="after"><inheritdoc cref="HarmonyMethod.after"/></param>
        /// <returns></returns>
        public HarmonyMethod getHarmonyMethod(string name, int? priority = null, string before = null, string after = null, bool? debug=null)
        {
            HarmonyMethod method = new HarmonyMethod(AccessTools.Method(this.GetType(), name));

            if (priority is not null)
                method.priority = (int)priority;

            if (before is not null)
                method.before = new[] { before };

            if (after is not null)
                method.after = new[] { after };

            if (debug is not null)
                method.debug = (bool)debug;

            return method;
        }
    }
}
