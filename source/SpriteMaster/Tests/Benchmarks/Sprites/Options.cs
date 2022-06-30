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

namespace Benchmarks.Sprites;

public sealed class Options : BenchmarkBase.Options {
	internal static Options Default => new();


	[Option("short", "Use Short Test Set")]
	public bool Short { get; set; } = false;


	[Option("small", "Use Small Test Set")]
	public bool Small { get; set; } = false;

	public Options() { }

	protected override void Process(string[] args) {
		base.Process(args);
	}
}
