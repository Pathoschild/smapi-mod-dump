/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Runtime.CompilerServices;

namespace MusicMaster.Types;

internal sealed class SharedRandom : Random {
	private static readonly Random StaticRandom = new();
	private static int StaticRandomValue {
		get {
			lock (StaticRandom) {
				return StaticRandom.Next();
			}
		}
	}

	private static readonly object Lock = new();

	internal int Value => Next();
	internal double ValueDouble => NextDouble();

	internal SharedRandom() : this(StaticRandomValue) {
	}

	internal SharedRandom(int seed) : base(seed) {
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override int Next() {
		lock (Lock) {
			return base.Next();
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override int Next(int maxValue) {
		lock (Lock) {
			return base.Next(maxValue);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override int Next(int minValue, int maxValue) {
		lock (Lock) {
			return base.Next(minValue, maxValue);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void NextBytes(byte[] buffer) {
		lock (Lock) {
			base.NextBytes(buffer);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void NextBytes(Span<byte> buffer) {
		lock (Lock) {
			base.NextBytes(buffer);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override double NextDouble() {
		lock (Lock) {
			return base.NextDouble();
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	protected override double Sample() {
		lock (Lock) {
			return base.Sample();
		}
	}
}
