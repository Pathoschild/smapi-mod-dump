/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

namespace Benchmarks.Strings.Benchmarks.Sources;

public abstract class MultiStringSource<TSource0, TSource1> : StringSource
	where TSource0 : StringSource
	where TSource1 : StringSource {

	static MultiStringSource() {
		System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(TSource0).TypeHandle);
		System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(TSource1).TypeHandle);
	}
}