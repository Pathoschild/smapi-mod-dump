/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using System;
using System.Reflection;

namespace StardewValleyTodo.Helpers {
    public static class Reflect {
        public static T GetPrivate<T>(object target, string field) {
            var getter = target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);

            if (getter == null) {
                throw new ArgumentException(
                    $"There is no '{field}' in {target.GetType().Name} or it is not non-public");
            }

            return (T) getter.GetValue(target);
        }

        public static T GetPublic<T>(object target, string field) {
            var getter = target.GetType().GetField(field);

            if (getter == null) {
                throw new ArgumentException(
                    $"There is no public field '{field}' in {target.GetType().Name}");
            }

            return (T) getter.GetValue(target);
        }
    }
}
