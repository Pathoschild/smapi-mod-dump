/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using System.Text.RegularExpressions;

namespace StardewValleyTodo.Helpers {
    public static class ObjectKey {
        private static Regex _regex = new Regex(@"(\(.+\)\s?)?(-?\d+)");

        public static string Parse(string rawKey) {
            var id = _regex.Match(rawKey).Groups[2];

            return id.Success ? id.Value : rawKey;
        }
    }
}
