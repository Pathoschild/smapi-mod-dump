/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using System.IO.Compression;
using System.Text;
using SystemRegex = System.Text.RegularExpressions;

namespace Regex;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Declared)]
//[InliningDiagnoser(true, true)]
//[TailCallDiagnoser]
//[EtwProfiler]
//[SimpleJob(RuntimeMoniker.CoreRt50)]
public class Benchmarks {
	private const string DictionaryPath = "dictionary.zip";

	private const int RandSeed = 0x13377113;
	private const int NumPatterns = 16;

	private readonly List<string> Dictionary;
	private readonly string DictionaryCombined;

	private readonly List<SystemRegex.Regex> Patterns;
	private readonly SystemRegex.Regex[] PatternsArray;
	private readonly SystemRegex.Regex CombinedPattern;

	public Benchmarks() {
		using var dictionary = ZipFile.OpenRead(DictionaryPath);
		if (dictionary is null) {
			throw new IOException($"Could not open '{DictionaryPath}'");
		}

		var fileEntry = dictionary.Entries.First(e => e.Length != 0L);
		using var fileStream = fileEntry.Open();
		using var fileReader = new StreamReader(fileStream);

		var dictionaryList = new List<string>();
		string? line = null;
		while ((line = fileReader.ReadLine()) is not null) {
			line = line.Trim();
			if (line.Length != 0) {
				dictionaryList.Add(line);
			}
		}

		Dictionary = dictionaryList;

		DictionaryCombined = string.Join('\n', Dictionary);

		var combinedExpressionBuilder = new StringBuilder();

		var rand = new Random(RandSeed);
		Patterns = new(NumPatterns);
		for (int i = 0; i < NumPatterns; ++i) {
			var sample = Dictionary[rand.Next(Dictionary.Count)];

			sample = sample.Insert(rand.Next(sample.Length), ((char)rand.Next(97, 123)).ToString());

			var expression = $"^{sample}.*";

			Patterns.Add(new(expression, SystemRegex.RegexOptions.Compiled));

			if (combinedExpressionBuilder.Length != 0) {
				combinedExpressionBuilder.Append('|');
			}
			combinedExpressionBuilder.Append($"({expression})");
		}

		PatternsArray = Patterns.ToArray();

		CombinedPattern = new(combinedExpressionBuilder.ToString(), SystemRegex.RegexOptions.Compiled);
	}

	[Benchmark(Description = "ForLoop (List)")]
	public bool ForLoop() {
		foreach (var pattern in Patterns) {
			if (pattern.IsMatch(DictionaryCombined)) {
				return true;
			}
		}

		return false;
	}

	[Benchmark(Description = "ForLoop (Array)")]
	public bool ForLoopArray() {
		foreach (var pattern in PatternsArray) {
			if (pattern.IsMatch(DictionaryCombined)) {
				return true;
			}
		}

		return false;
	}

	[Benchmark(Description = "Combined")]
	public bool Combined() {
		if (CombinedPattern.IsMatch(DictionaryCombined)) {
			return true;
		}

		return false;
	}
}
