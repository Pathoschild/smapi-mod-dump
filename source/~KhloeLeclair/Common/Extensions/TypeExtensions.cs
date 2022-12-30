/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leclair.Stardew.Common.Extensions;

internal static class TypeExtensions {

	// The following two methods are implemented to duplicate the logic used
	// internally by Pintail. Pintail is licensed under the MIT license:
	// https://github.com/Nanoray-pl/Pintail/blob/master/LICENSE

	internal static IEnumerable<Type> GetInterfacesRecursively(this Type type, bool includingSelf) {
		return type.GetInterfacesRecursivelyInternal(includingSelf, new HashSet<Type>());
	}

	private static IEnumerable<Type> GetInterfacesRecursivelyInternal(this Type type, bool includingSelf, HashSet<Type> visited) {
		if (visited.Add(type)) {
			if (includingSelf && type.IsInterface)
				yield return type;
			foreach (Type interfaceType in type.GetInterfaces()) {
				yield return interfaceType;
				foreach (Type recursiveType in type.GetInterfacesRecursivelyInternal(false, visited))
					yield return recursiveType;
			}
		}
	}

}
