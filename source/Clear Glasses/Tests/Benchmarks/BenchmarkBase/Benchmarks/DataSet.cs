/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using Microsoft.Toolkit.HighPerformance;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Benchmarks.BenchmarkBase.Benchmarks;

[PublicAPI]
public interface IDataSet<T> {
	public T Data { get; }

	private static uint SubIndex(int length) {
		if (length == 0) {
			return 0u;
		}

		//return (uint)BitOperations.Log2((uint)length) + 1u;
		return (uint)BitOperations.Log2((uint)length * 2) + 1u;
	}

	public uint Index =>
		Data switch {
			string str => SubIndex(str.Length),
			Array arr => SubIndex(arr.Length),
			null => uint.MaxValue,
			_ => (uint)Data.GetHashCode()
		};
}

[PublicAPI]
public unsafe interface IDataSetPtr<T> : IDataSet<T[]> where T : unmanaged {
	public T* DataPtr { get; }
}

[PublicAPI]
public interface IDataSetArray<T> : IDataSet<T[]> where T : unmanaged {
	public ReadOnlySpan<T> Span => Data;

	protected static T[] MakeArray(Random random, long length, bool pinned) {
		var data = GC.AllocateUninitializedArray<T>((int)length, pinned: pinned);
		var span = data.AsSpan().AsBytes();
		random.NextBytes(span);

		return data;
	}
}

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct DataSetArrayFixed<T> : IDataSetArray<T>, IDataSetPtr<T> where T : unmanaged {
	private readonly IDataSetArray<T> Base => this;

	public readonly T[] Data { get; }
	public readonly unsafe T* DataPtr { get; }

	public DataSetArrayFixed(Random random, long length, bool validate = false) :
		this(IDataSetArray<T>.MakeArray(random, length, pinned: true), validate) {
	}

	public DataSetArrayFixed(T[] data, bool validate = false) {
		Data = data;
		unsafe {
			DataPtr = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(data));
		}

		if (validate) {
			//Validate(this);
		}
	}

	public override readonly string ToString() => $"({Base.Index:D2}) {Data.Length}";
}

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct DataSetArrayUnfixed<T> : IDataSetArray<T> where T : unmanaged {
	private readonly IDataSetArray<T> Base => this;

	public readonly T[] Data { get; }

	public DataSetArrayUnfixed(Random random, long length, bool validate = false) :
		this(IDataSetArray<T>.MakeArray(random, length, pinned: false), validate) {
	}

	public DataSetArrayUnfixed(T[] data, bool validate = false) {
		Data = data;

		if (validate) {
			//Validate(this);
		}
	}

	public override readonly string ToString() => $"({Base.Index:D2}) {Data.Length}";
}

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct DataSet<T> : IDataSet<T> where T : notnull {
	private readonly IDataSet<T> Base => this;

	public readonly T Data { get; }

	private readonly string Name {
		get {
			if (Data is string str) {
				return str.Length.ToString();
			}

			return Data.ToString() ?? "unknown";
		}
	}

	public DataSet(T data) {
		Data = data;
	}

	public override readonly string ToString() => $"({Base.Index:D2}) {Name}";
}
