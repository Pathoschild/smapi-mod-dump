/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System.Reflection;

// ReSharper disable once CheckNamespace
namespace SkillPrestige.Framework.JsonNet.PrivateSettersContractResolvers
{
    /// <summary>Extension methods for member info for Json.Net.</summary>
    internal static class MemberInfoExtensions
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Checks to see if a property has a setter.</summary>
        /// <param name="member">The member to check.</param>
        public static bool IsPropertyWithSetter(this MemberInfo member)
        {
            PropertyInfo property = member as PropertyInfo;

            return property?.GetSetMethod(true) != null;
        }
    }
}
