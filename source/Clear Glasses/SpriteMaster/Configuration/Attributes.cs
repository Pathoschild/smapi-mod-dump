/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Configuration;

internal static class Attributes {
	internal abstract class ConfigAttribute : Attribute { };

	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	internal sealed class CommentAttribute : ConfigAttribute {
		internal readonly string Message;

		internal CommentAttribute(string message) => Message = message;
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	internal sealed class IgnoreAttribute : ConfigAttribute { }

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	internal sealed class RetainAttribute : ConfigAttribute { }

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	internal sealed class OldNameAttribute : ConfigAttribute {
		internal readonly string Name;

		internal OldNameAttribute(string name) => Name = name;
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	internal sealed class OptionsAttribute : ConfigAttribute {
		[Flags]
		internal enum Flag {
			None = 0,
			FlushTextureCache = 1 << 0,
			FlushSuspendedSpriteCache = 1 << 1,
			FlushFileCache = 1 << 2,
			FlushResidentCache = 1 << 3,
			FlushTextureFileCache = 1 << 4,
			FlushMetaData = 1 << 5,
			FlushMetaDataRecache = 1 << 6,
			FlushSpriteMap = 1 << 7,
			FlushAllRenderingCaches = FlushSuspendedSpriteCache | FlushFileCache | FlushResidentCache | FlushMetaDataRecache | FlushSpriteMap,
			FlushAllInternalCaches = FlushSuspendedSpriteCache | FlushFileCache | FlushResidentCache | FlushTextureFileCache | FlushMetaData | FlushSpriteMap,
			FlushAllCaches = FlushTextureCache | FlushAllInternalCaches,
			GarbageCollect = 1 << 8,
			ResetDisplay = 1 << 9,
			RequireRestart = 1 << 10
		}

		internal readonly Flag Flags = Flag.None;

		internal OptionsAttribute(Flag flags) => Flags = flags;
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	internal abstract class LimitsAttribute : ConfigAttribute {
		internal abstract T? GetMin<T>() where T : unmanaged;
		internal abstract T? GetMax<T>() where T : unmanaged;

		internal abstract T? GetMin<T>(Type type) where T : unmanaged;
		internal abstract T? GetMax<T>(Type type) where T : unmanaged;

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		protected void ThrowIsNotAssignableException<TTo, TFrom>() =>
			throw new InvalidCastException($"{typeof(TTo)} is not assignable from {typeof(TFrom)}");

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		protected TResult ThrowIsNotAssignableException<TTo, TFrom, TResult>() =>
			throw new InvalidCastException($"{typeof(TTo)} is not assignable from {typeof(TFrom)}");
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	internal sealed class LimitsIntAttribute : LimitsAttribute {
		internal readonly long MinValue;
		internal readonly long MaxValue;

		internal override T? GetMin<T>() {
			if (typeof(T) == typeof(int)) {
				return (MinValue == int.MinValue) ? null : (T)Convert.ChangeType(MinValue, typeof(T));
			}
			else {
				return (MinValue == long.MinValue) ? null : (T)Convert.ChangeType(MinValue, typeof(T));
			}
		}

		internal override T? GetMax<T>() {
			if (typeof(T) == typeof(int)) {
				return (MaxValue == int.MaxValue) ? null : (T)Convert.ChangeType(MaxValue, typeof(T));
			}
			else {
				return (MaxValue == long.MaxValue) ? null : (T)Convert.ChangeType(MaxValue, typeof(T));
			}
		}

		internal override T? GetMin<T>(Type type) {
			if (typeof(T) == typeof(int)) {
				return (MinValue == int.MinValue) ? null : (T)Convert.ChangeType(Convert.ChangeType(MinValue, type), typeof(T));
			}
			else {
				return (MinValue == long.MinValue) ? null : (T)Convert.ChangeType(Convert.ChangeType(MinValue, type), typeof(T));
			}
		}

		internal override T? GetMax<T>(Type type) {
			if (typeof(T) == typeof(int)) {
				return (MaxValue == int.MaxValue) ? null : (T)Convert.ChangeType(Convert.ChangeType(MaxValue, type), typeof(T));
			}
			else {
				return (MaxValue == long.MaxValue) ? null : (T)Convert.ChangeType(Convert.ChangeType(MaxValue, type), typeof(T));
			}
		}

		internal LimitsIntAttribute(long min = long.MinValue, long max = long.MaxValue) {
			MinValue = min;
			MaxValue = max;
		}

		internal LimitsIntAttribute(int min = int.MinValue, int max = int.MaxValue) {
			MinValue = min;
			MaxValue = max;
		}
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	internal sealed class LimitsRealAttribute : LimitsAttribute {
		internal readonly double MinValue;
		internal readonly double MaxValue;

		internal override T? GetMin<T>() {
			if (typeof(T) == typeof(float)) {
				switch (MinValue) {
					case float.MinValue:
						return null;
					case double.MinValue:
						return null;
					default:
						return (T)Convert.ChangeType(MinValue, typeof(T));
				}
			}
			else {
				switch (MinValue) {
					case float.MinValue:
						return null;
					case double.MinValue:
						return null;
					default:
						return (T)Convert.ChangeType(MinValue, typeof(T));
				}
			}
		}

		internal override T? GetMax<T>() {
			if (typeof(T) == typeof(float)) {
				switch (MaxValue) {
					case float.MaxValue:
						return null;
					case double.MaxValue:
						return null;
					default:
						return (T)Convert.ChangeType(MaxValue, typeof(T));
				}
			}
			else {
				switch (MaxValue) {
					case float.MaxValue:
						return null;
					case double.MaxValue:
						return null;
					default:
						return (T)Convert.ChangeType(MaxValue, typeof(T));
				}
			}
		}

		internal override T? GetMin<T>(Type type) {
			if (typeof(T) == typeof(float)) {
				switch (MinValue) {
					case float.MinValue:
						return null;
					case double.MinValue:
						return null;
					default:
						return (T)Convert.ChangeType(Convert.ChangeType(Math.Clamp(MinValue, (double)float.MinValue, (double)float.MaxValue), type), typeof(T));
				}
			}
			else {
				switch (MinValue) {
					case float.MinValue:
						return null;
					case double.MinValue:
						return null;
					default:
						return (T)Convert.ChangeType(Convert.ChangeType(MinValue, type), typeof(T));
				}
			}
		}

		internal override T? GetMax<T>(Type type) {
			if (typeof(T) == typeof(float)) {
				switch (MaxValue) {
					case float.MaxValue:
						return null;
					case double.MaxValue:
						return null;
					default:
						return (T)Convert.ChangeType(Convert.ChangeType(Math.Clamp(MaxValue, (double)float.MinValue, (double)float.MaxValue), type), typeof(T));
				}
			}
			else {
				switch (MaxValue) {
					case float.MaxValue:
						return null;
					case double.MaxValue:
						return null;
					default:
						return (T)Convert.ChangeType(Convert.ChangeType(MaxValue, type), typeof(T));
				}
			}
		}

		internal LimitsRealAttribute(double min = double.MinValue, double max = double.MaxValue) {
			MinValue = min;
			MaxValue = max;
		}

		internal LimitsRealAttribute(float min = float.MinValue, float max = float.MaxValue) {
			MinValue = min;
			MaxValue = max;
		}
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	internal sealed class LimitsTimeSpanAttribute : LimitsAttribute {
		internal readonly TimeSpan MinValue;
		internal readonly TimeSpan MaxValue;

		internal override T? GetMin<T>() {
			if (typeof(T).IsAssignableFrom(typeof(TimeSpan))) {
				return (MinValue == TimeSpan.MinValue) ? null : (T)(object)MinValue;
			}

			return ThrowIsNotAssignableException<T, TimeSpan, T?>();
		}

		internal override T? GetMax<T>() {
			if (typeof(T).IsAssignableFrom(typeof(TimeSpan))) {
				return (MaxValue == TimeSpan.MaxValue) ? null : (T)(object)MaxValue;
			}

			return ThrowIsNotAssignableException<T, TimeSpan, T?>();
		}

		internal override T? GetMin<T>(Type type) {
			if (typeof(T) == typeof(TimeSpan)) {
				return (MinValue == TimeSpan.MinValue) ? null : (T)Convert.ChangeType(Convert.ChangeType(MinValue, type), typeof(T));
			}

			return ThrowIsNotAssignableException<T, TimeSpan, T?>();
		}

		internal override T? GetMax<T>(Type type) {
			if (typeof(T) == typeof(TimeSpan)) {
				return (MaxValue == TimeSpan.MaxValue) ? null : (T)Convert.ChangeType(Convert.ChangeType(MaxValue, type), typeof(T));
			}

			return ThrowIsNotAssignableException<T, TimeSpan, T?>();
		}

		internal LimitsTimeSpanAttribute(long min = long.MinValue, long max = long.MaxValue) {
			MinValue = TimeSpan.FromTicks(min);
			MaxValue = TimeSpan.FromTicks(max);
		}
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	internal sealed class AdvancedAttribute : ConfigAttribute {
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	internal class ExperimentalAttribute : ConfigAttribute {
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	internal sealed class BrokenAttribute : ExperimentalAttribute {
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	internal sealed class ChangesBehavior : ConfigAttribute {
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	internal sealed class GMCMHiddenAttribute : ConfigAttribute {
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	internal sealed class MenuNameAttribute : ConfigAttribute {
		internal readonly string Name;

		internal MenuNameAttribute(string name) {
			Name = name;
		}
	}
}
