/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using System;
using System.Reflection;

namespace MusicMaster.Extensions;

internal static class AssemblyExt {
	internal static Assembly? GetAssembly(string assemblyName) =>
		AppDomain.CurrentDomain.GetAssemblies().SingleOrDefaultF(assembly => assembly.GetName().Name == assemblyName);

	internal static Assembly GetRequiredAssembly(string assemblyName) =>
		GetAssembly(assemblyName) ?? ThrowHelper.ThrowNullReferenceException<Assembly>($"Could not find required assembly '{assemblyName}'");
}
