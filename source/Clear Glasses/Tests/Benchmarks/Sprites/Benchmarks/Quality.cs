/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Attributes;
using LinqFasterer;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions.Reflection;
using SpriteMaster.Types;
using System.Collections.Concurrent;
using System.Reflection;

namespace Benchmarks.Sprites.Benchmarks;

public sealed class Quality {
	private readonly BlockCompress BlockCompressInstance = new();

	private sealed class Result {
		internal long MinDifference = long.MaxValue;
		internal long MaxDifference = 0L;
		internal long MaxPossibleDifference = 0L;
		internal long TotalDifference = 0L;
		internal double MeanDifference = -1.0;
		internal readonly List<long> Differences = new();
	}

	private Result ProcessTest(MethodInfo method, Textures8.SpriteData data) {
		var tempDataSet = new Textures8.SpriteDataSet(new[] {data});

		{
			var blockData = new Color8[4 * 4];
			for (int i = 0; i < (4 * 4); ++i) {
				blockData[i] = new Color8(0x44, 0x55, 0x66, 0x77);
			}

			var blockDataBytes = blockData.AsSpan().Cast<Color8, byte>().ToArray();

			var blockSpriteData = new Textures8.SpriteData("block", new(blockDataBytes), (4, 4));

			var blockDataSet = new Textures8.SpriteDataSet(new[] {blockSpriteData});

			method.Invoke(BlockCompressInstance, new object[] { blockDataSet });
		}

		method.Invoke(BlockCompressInstance, new object[] { tempDataSet });

		byte[] compressedData = (tempDataSet.Data[0].TempReference as IEnumerable<byte>)!.ToArray();

		Span<byte> uncompressedData;
		try {
			uncompressedData =
				SpriteMaster.Resample.TextureDecode.DecodeBlockCompressedTexture(SurfaceFormat.Dxt5, data.Size, compressedData);
		}
		catch (InvalidOperationException) {
			uncompressedData =
				SpriteMaster.Resample.TextureDecode.DecodeBlockCompressedTexture(SurfaceFormat.Dxt1a, data.Size, compressedData);
		}

		if (uncompressedData.Length != data.Span.Length) {
			throw new InvalidOperationException("Data Length Mismatch");
		}

		var result = new Result();

		var referenceData = data.Span.Cast<byte, Color8>();
		var testData = uncompressedData.Cast<byte, Color8>();

		long maxPossibleDifference = 0L;

		for (int i = 0; i < referenceData.Length; ++i) {
			var reference = referenceData[i];
			var test = testData[i];

			if (reference == test) {
				result.Differences.Add(0L);
				continue;
			}

			//uint diff = SpriteMaster.Colors.ColorHelpers.RedmeanDifference(reference, test, linear: false, alpha: true);
			//uint maxDiff = SpriteMaster.Colors.ColorHelpers.RedmeanDifference(Color8.Zero, new Color8(255, 255, 255, 255), linear: false, alpha: true);

			int diffA = Math.Abs(reference.A.Value - test.A.Value);
			int diffR = Math.Abs(reference.R.Value - test.R.Value);
			int diffG = Math.Abs(reference.G.Value - test.G.Value);
			int diffB = Math.Abs(reference.B.Value - test.B.Value);

			maxPossibleDifference += byte.MaxValue + byte.MaxValue + byte.MaxValue + byte.MaxValue;

			long sumDifference = diffA + diffR + diffG + diffB;

			result.Differences.Add(sumDifference);
			result.TotalDifference += sumDifference;
			result.MinDifference = Math.Min(result.MinDifference, sumDifference);
			result.MaxDifference = Math.Max(result.MaxDifference, sumDifference);
		}

		result.MaxPossibleDifference += maxPossibleDifference;

		return result;
	}

	public void Test() {
		var methods = typeof(BlockCompress).GetMethods().WhereF(method => method.HasAttribute<BenchmarkAttribute>());

		var runners = Program.CurrentOptions.Runners.Select(Options.CreatePattern).ToArray();

		if (runners.Length != 0) {
			methods = methods.Where(method => runners.Any(runner => runner.IsMatch(method.Name))).ToArray();
		}

		ConcurrentDictionary<MethodInfo, ConcurrentBag<Result>> results = new();

		foreach (var dataSet in Textures8.DefaultDataSetsStatic) {
			foreach (var data in dataSet.Data) {
				foreach (var method in methods) {
					var resultList = results.GetOrAdd(method, _ => new());

					var result = ProcessTest(method, data);
					resultList.Add(result);
				}
			}
		}

		// Accumulate Resultss
		ConcurrentDictionary<MethodInfo, Result> accumulatedResults = new();

		foreach (var (method, resultBag) in results) {
			var result = accumulatedResults.GetOrAdd(method, _ => new());
			foreach (var subResult in resultBag) {
				result.Differences.AddRange(subResult.Differences);
				result.MinDifference = Math.Min(result.MinDifference, subResult.MinDifference);
				result.MaxDifference = Math.Max(result.MaxDifference, subResult.MaxDifference);
				result.TotalDifference += subResult.TotalDifference;
				result.MaxPossibleDifference += subResult.MaxPossibleDifference;
			}

			result.MeanDifference = (double)result.TotalDifference / result.Differences.Count;
		}

		var accumulatedResultList = accumulatedResults.OrderBy(kv => (double)kv.Value.TotalDifference / kv.Value.MaxPossibleDifference);

		var objectResults = accumulatedResultList.Select(kv => {
			return new object[] {
				kv.Key.Name,
				kv.Value.TotalDifference,
				kv.Value.MeanDifference.ToString("N2"),
				kv.Value.MaxDifference,
				((1.0 - ((double)kv.Value.TotalDifference / kv.Value.MaxPossibleDifference)) * 100.0).ToString("N2") + " %"
			};
		});

		var titles = new string[] {"Method", "Total Diffs", "Mean Diffs", "Max Diffs", "Quality"};

		PrintTable(titles, objectResults);
	}

	private static void PrintTable(string[] titles, IEnumerable<object[]> data) {
		var dataArray = data.ToArray();

		int[] maxLengths = new int[titles.Length];

		for (int i = 0; i < maxLengths.Length; ++i) {
			maxLengths[i] = Math.Max(titles[i].Length, dataArray.Max(v => v[i].ToString()!.Length));
		}

		Console.WriteLine(string.Join(" | ", titles.SelectF((title, index) => title.PadLeft(maxLengths[index] + 2))));
		foreach (var values in dataArray) {
			Console.WriteLine(string.Join(" | ", values.SelectF((value, index) => value.ToString()!.PadLeft(maxLengths[index] + 2))));
		}
	}
}
