/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.Kokoro;

public sealed class StructRef<T> where T : struct
{
	public T Value { get; set; }

	public StructRef(T initialValue)
	{
		this.Value = initialValue;
	}

	public static implicit operator StructRef<T>(T value)
		=> new(value);

	public static implicit operator T(StructRef<T> value)
		=> value.Value;
}

public sealed class NullableStructRef<T> where T : struct
{
	public T? Value { get; set; }

	public NullableStructRef(T? initialValue = null)
	{
		this.Value = initialValue;
	}

	public static implicit operator NullableStructRef<T>(T? value)
		=> new(value);

	public static implicit operator T?(NullableStructRef<T> value)
		=> value.Value;
}