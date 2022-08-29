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
using Benchmarks.BenchmarkBase.Benchmarks;
using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Extensions.Reflection;
using System.Numerics;
using System.Reflection;

namespace Benchmarks.Hashing.Benchmarks;

public partial class Buffers {
	private const int RandSeed = 0x13377113;
	private static readonly long MinSize = Program.CurrentOptions.Min;
	private static readonly long MaxSize = Program.CurrentOptions.Max;

	private static void Validate<T>(in DataSetArrayFixed<T> dataSet) where T : unmanaged {
		var data = dataSet.Data;
		var span = data.AsSpan().AsBytes();

		var referenceHash = SpriteMaster.Hashing.Algorithms.XxHash3.Hash64(span);

		void CheckHash(ulong hash, string name) {
			if (referenceHash != hash) {
				Console.Error.WriteLine($"Hashes Not Equal ({data.Length}) : {referenceHash} != {hash} [{name}]");
			}
		}

		var tempInstance = new Buffers();

		foreach (var method in typeof(Buffers).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)) {
			if (!method.HasAttribute<BenchmarkAttribute>()) {
				continue;
			}

			if (Program.CurrentOptions.Runners.Count != 0) {
				if (!Program.CurrentOptions.Runners.Contains(method.Name)) {
					continue;
				}
			}
			else {
				if (!method.Name.StartsWith("xxHash3", StringComparison.InvariantCultureIgnoreCase) || method.Name.StartsWith("xxHash32", StringComparison.InvariantCultureIgnoreCase)) {
					continue;
				}
			}

			var initMethod = typeof(Buffer).GetMethod(method.Name, BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);
			initMethod?.Invoke(tempInstance, null);

			// public ulong xxHash3Experimental(DataSet<byte> dataSet) {
#pragma warning disable CS0618
			var hash = method.Invoke<ulong>(tempInstance, (object)dataSet);
#pragma warning restore CS0618

			Console.Out.WriteLine($"Validating {method.Name} {data.Length}");
			CheckHash(hash, method.Name);
		}
	}

	static Buffers() {
		var random = new Random(RandSeed);

		DataSetArrayFixed<byte> MakeDataSet(long length) =>
			new(random, length);

		long start = MinSize;

		bool makeEmpty = start == 0;

		int requiredSize = (makeEmpty ? 1 : 0) + (BitOperations.Log2((uint)MaxSize) - BitOperations.Log2((uint)MinSize)) + 1;

		var dataSets = new List<DataSetArrayFixed<byte>>(requiredSize);

		if (makeEmpty) {
			var dataSet = MakeDataSet(start);

			dataSets.Add(dataSet);

			start = 1;
		}

		if (MaxSize == MinSize) {
			if (MaxSize != 0) {
				var dataSet = MakeDataSet(MaxSize);
				dataSets.Add(dataSet);
			}
		}
		else {
			for (long i = start; i <= MaxSize; i *= 2) {
				//var halfIndex = (i >> 1) + (i >> 2);
				//if (halfIndex != 0 && halfIndex != i) {
				//	dataSets.Add(MakeDataSet(halfIndex));
				//}

				dataSets.Add(MakeDataSet(i));
			}
		}

		if (Program.CurrentOptions.DoValidate) {
			Console.Out.WriteLine("Performing Benchmark Validation");

			foreach (var dataSet in dataSets) {
				Validate(in dataSet);
			}
		}

		DefaultDataSetsStatic.AddRange(dataSets);
	}

	private static void SetBoolField(Type type, string name, bool value) {
		var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
		if (field is null) {
			throw new NullReferenceException(name);
		}

		field.SetValue(null, value);
	}
}
