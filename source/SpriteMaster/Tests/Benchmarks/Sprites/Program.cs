/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Benchmarks.BenchmarkBase;
using Benchmarks.Sprites.Benchmarks;
using System.Text.RegularExpressions;

namespace Benchmarks.Sprites;

public class Program : ProgramBase<Options> {
	private static Action<Regex>? GetExternalTest(Regex pattern) {
		if (pattern.IsMatch(nameof(Quality))) {
			return _ => new Quality().Test();
		}

		return null;
	}

	public static int Main(string[] args) {
		return MainBase(typeof(Program), args, GetExternalTest);
	}
}
