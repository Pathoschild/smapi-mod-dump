/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Reflection;

namespace SpriteMaster.Extensions;

static class DelegateExt {
	internal static T CreateDelegate<T>(this MethodInfo method) where T : Delegate => (T)Delegate.CreateDelegate(typeof(T), method);
}
