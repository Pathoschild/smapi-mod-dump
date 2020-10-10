/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using System;
namespace SilentOak.Patching.Extensions
{
    public static class VersionExtensions
    {
        public static bool Match(this Version version, string versionExpression)
        {
            int[] versionComponents = { version.Major, version.Minor, version.Build, version.Revision };
            string[] versionExpressionComponents = versionExpression.Split('.');
            if (versionExpressionComponents.Length > 4)
            {
                throw new FormatException($"{versionExpression} is not a valid version expression.");
            }

            for (int i = 0; i < versionExpressionComponents.Length; i++)
            {
                if (versionExpressionComponents[i] == "*")
                {
                    return true;
                }

                bool parsed = int.TryParse(versionExpressionComponents[i], out int component);
                if (!parsed)
                {
                    throw new FormatException($"{versionExpression} is not a valid version expression.");
                }

                if (component != versionComponents[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
