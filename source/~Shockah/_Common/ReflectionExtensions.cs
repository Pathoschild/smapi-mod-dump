/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Shockah.CommonModCode
{
	public static class ReflectionExtensions
	{
		public static string GetBestName(this Type type)
			=> type.FullName ?? type.Name;

		public static bool IsBuiltInDebugConfiguration(this Assembly assembly)
			=> assembly.GetCustomAttributes(false).OfType<DebuggableAttribute>().Any(attr => attr.IsJITTrackingEnabled);
	}
}
