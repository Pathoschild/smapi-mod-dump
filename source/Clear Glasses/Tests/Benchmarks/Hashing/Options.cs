/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Benchmarks.BenchmarkBase;

namespace Benchmarks.Hashing;

public sealed class Options : BenchmarkBase.Options {
	internal static Options Default => new();

	[Option("dictionary", "Dictionary for dictionary set")]
	public string Dictionary { get; set; } = @"D:\Stardew\SpriteMaster\Tests\Benchmarks\Hashing\bin\dictionary.zip";

	public Options() { }

	protected override void Process(string[] args) {
		base.Process(args);
	}
}
