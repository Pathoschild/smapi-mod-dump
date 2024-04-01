/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Benchmarks.Strings.Benchmarks;

public partial class CharSource {
	private static class Impl {
		[StructLayout(LayoutKind.Auto)]
		internal readonly ref struct CharSource0 {
			private readonly string? String = null;
			private readonly StringBuilder? Builder = null;
			internal readonly int Length;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private CharSource0(int length) => Length = length;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource0(string s) : this(s.Length) {
				String = s;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource0(StringBuilder builder) : this(builder.Length) {
				Builder = builder;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource0(object reference) {
				if (reference is string str) {
					String = str;
					Length = str.Length;
				}
				else {
					Builder = Unsafe.As<object, StringBuilder>(ref Unsafe.AsRef(reference));
					Length = Builder.Length;
				}
			}

			internal readonly char this[int index] {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => String is not null ? String[index] : Builder![index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public readonly override string ToString() => String ?? Builder!.ToString();
		}

		[StructLayout(LayoutKind.Auto)]
		internal readonly ref struct CharSource1 {
			private readonly object Reference;
			internal readonly int Length;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private CharSource1(object reference, int length) {
				Reference = reference;
				Length = length;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource1(string s) : this(s, s.Length) {
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource1(StringBuilder builder) : this(builder, builder.Length) {
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource1(object reference) {
				Reference = reference;
				if (reference is string str) {
					Length = str.Length;
				}
				else {
					Length = Unsafe.As<object, StringBuilder>(ref Unsafe.AsRef(reference)).Length;
				}
			}

			internal readonly char this[int index] {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get {
					if (Reference is string str) {
						return str[index];
					}
					return Unsafe.As<object, StringBuilder>(ref Unsafe.AsRef(Reference))[index];
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public readonly override string ToString() => Reference.ToString()!;
		}

		[StructLayout(LayoutKind.Auto)]
		internal readonly ref struct CharSource2 {
			private readonly object Reference;
			internal readonly int Length;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private CharSource2(object reference, int length) {
				Reference = reference;
				Length = length;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource2(string s) : this(s, s.Length) {
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource2(StringBuilder builder) : this(builder, builder.Length) {
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource2(object reference) : this() {
				Reference = reference;
				if (reference is string str) {
					Length = str.Length;
				}
				else {
					Length = Unsafe.As<object, StringBuilder>(ref Unsafe.AsRef(reference)).Length;
				}
			}

			internal readonly char this[int index] {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get {
					if (Reference.GetType() == typeof(string)) {
						return Unsafe.As<object, string>(ref Unsafe.AsRef(Reference))[index];
					}
					return Unsafe.As<object, StringBuilder>(ref Unsafe.AsRef(Reference))[index];
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public readonly override string ToString() => Reference.ToString()!;
		}

		[StructLayout(LayoutKind.Auto)]
		internal readonly ref struct CharSource3 {
			private readonly object Reference;
			private unsafe readonly delegate* managed<object, int, char> Accessor;
			internal readonly int Length;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static char StringAccessor(object obj, int index) =>
				((string)obj)[index];

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static char StringBuilderAccessor(object obj, int index) =>
				((StringBuilder)obj)[index];

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private unsafe CharSource3(object reference, delegate* managed<object, int, char> accessor, int length) {
				Reference = reference;
				Accessor = accessor;
				Length = length;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal unsafe CharSource3(string s) : this(s, &StringAccessor, s.Length) {
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal unsafe CharSource3(StringBuilder builder) : this(builder, &StringBuilderAccessor, builder.Length) {
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal unsafe CharSource3(object reference) {
				Reference = reference;
				if (reference is string str) {
					Accessor = &StringAccessor;
					Length = str.Length;
				}
				else {
					Accessor = &StringBuilderAccessor;
					Length = Unsafe.As<object, StringBuilder>(ref Unsafe.AsRef(reference)).Length;
				}
			}

			internal readonly unsafe char this[int index] {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Accessor(Reference, index);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public readonly override string ToString() => Reference.ToString()!;
		}

		[StructLayout(LayoutKind.Auto)]
		internal readonly ref struct CharSource4 {
			private delegate char AccessorDelegate(object obj, int index);

			private readonly object Reference;
			private unsafe readonly AccessorDelegate Accessor;
			internal readonly int Length;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static char StringAccessor(object obj, int index) =>
				((string)obj)[index];

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static char StringBuilderAccessor(object obj, int index) =>
				((StringBuilder)obj)[index];

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private CharSource4(object reference, AccessorDelegate accessor, int length) {
				Reference = reference;
				Accessor = accessor;
				Length = length;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource4(string s) : this(s, StringAccessor, s.Length) {
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource4(StringBuilder builder) : this(builder, StringBuilderAccessor, builder.Length) {
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource4(object reference) {
				Reference = reference;
				if (reference is string str) {
					Accessor = StringAccessor;
					Length = str.Length;
				}
				else {
					Accessor = StringBuilderAccessor;
					Length = Unsafe.As<object, StringBuilder>(ref Unsafe.AsRef(reference)).Length;
				}
			}

			internal readonly unsafe char this[int index] {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Accessor(Reference, index);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public readonly override string ToString() => Reference.ToString()!;
		}

		[StructLayout(LayoutKind.Auto)]
		internal readonly ref struct CharSource5 {
			private interface InnerSource {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				internal char At(int index);

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				internal string ToString();
			}

			private interface TypedInnerSource<T> : InnerSource {
			}

			private readonly struct InnerSourceString : TypedInnerSource<string> {
				private readonly string Reference;

				internal InnerSourceString(string reference) {
					Reference = reference;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				char InnerSource.At(int index) {
					return Reference[index];
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				string InnerSource.ToString() {
					return Reference;
				}
			}

			private readonly struct InnerSourceBuilder : TypedInnerSource<StringBuilder> {
				private readonly StringBuilder Reference;

				internal InnerSourceBuilder(StringBuilder reference) {
					Reference = reference;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				char InnerSource.At(int index) {
					return Reference[index];
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				string InnerSource.ToString() {
					return Reference.ToString();
				}
			}

			private readonly InnerSource Reference;
			internal readonly int Length;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private unsafe CharSource5(InnerSource reference, int length) {
				Reference = reference;
				Length = length;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource5(string s) : this(new InnerSourceString(s), s.Length) {
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource5(StringBuilder builder) : this(new InnerSourceBuilder(builder), builder.Length) {
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource5(object reference) {
				if (reference is string str) {
					Reference = new InnerSourceString(str);
					Length = str.Length;
				}
				else {
					var builder = Unsafe.As<object, StringBuilder>(ref Unsafe.AsRef(reference));
					Reference = new InnerSourceBuilder(builder);
					Length = builder.Length;
				}
			}

			internal readonly unsafe char this[int index] {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => Reference.At(index);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public readonly override string ToString() => Reference.ToString()!;
		}

		[StructLayout(LayoutKind.Auto)]
		internal readonly ref struct CharSource6 {
			private readonly object Reference;
			internal readonly int Length;
			private readonly bool IsString;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private CharSource6(bool isString, object reference, int length) {
				IsString = isString;
				Reference = reference;
				Length = length;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource6(string s) : this(true, s, s.Length) {
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource6(StringBuilder builder) : this(false, builder, builder.Length) {
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource6(object reference) {
				Reference = reference;
				if (reference is string str) {
					IsString = true;
					Length = str.Length;
				}
				else {
					IsString = false;
					var builder = Unsafe.As<object, StringBuilder>(ref Unsafe.AsRef(reference));
					Length = builder.Length;
				}
			}

			internal readonly char this[int index] {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => IsString ?
					Unsafe.As<object, string>(ref Unsafe.AsRef(Reference))[index] : 
					Unsafe.As<object, StringBuilder>(ref Unsafe.AsRef(Reference))[index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public readonly override string ToString() => IsString ?
				Unsafe.As<object, string>(ref Unsafe.AsRef(Reference)) :
				Unsafe.As<object, StringBuilder>(ref Unsafe.AsRef(Reference)).ToString();
		}

		[StructLayout(LayoutKind.Explicit)]
		internal unsafe readonly ref struct CharSource7 {
			[FieldOffset(0)]
			private readonly string String = null!;

			[FieldOffset(0)]
			private readonly StringBuilder Builder = null!;

			[FieldOffset(8)]
			internal readonly int Length;

			[FieldOffset(12)]
			private readonly bool IsString;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private CharSource7(bool isString,int length) : this() {
				IsString = isString;
				Length = length;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource7(string s) : this(true, s.Length) {
				String = s;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource7(StringBuilder builder) : this(false, builder.Length) {
				Builder = builder;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal CharSource7(object reference) {
				if (reference is string str) {
					String = str;
					IsString = true;
					Length = str.Length;
				}
				else {
					Builder = Unsafe.As<object, StringBuilder>(ref Unsafe.AsRef(reference));
					IsString = false;
					Length = Builder.Length;
				}
			}

			internal readonly char this[int index] {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => IsString ?
					String[index] :
					Builder[index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public readonly override string ToString() => IsString ?
				String :
				Builder.ToString();
		}
	}
}
