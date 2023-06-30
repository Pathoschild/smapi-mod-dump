/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.Kokoro.Map;

public sealed class MapSlice<TTile> : IMap<TTile>.WithKnownSize
{
	private IMap<TTile>.WithKnownSize Wrapped { get; init; }
	private IntPoint Offset { get; init; }
	private IntPoint Size { get; init; }

	public IntRectangle Bounds
		=> new(Wrapped.Bounds.Min - Offset, Size.X, Size.Y);

	public TTile this[IntPoint point]
		=> Wrapped[point + Offset];

	public static MapSlice<TTile> Create(IMap<TTile>.WithKnownSize wrapped, IntPoint offset, IntPoint size)
		=> new(wrapped, offset, size);

	private MapSlice(IMap<TTile>.WithKnownSize wrapped, IntPoint offset, IntPoint size)
	{
		this.Wrapped = wrapped;
		this.Offset = offset;
		this.Size = size;
	}

	public sealed class Writable : IMap<TTile>.WithKnownSize, IMap<TTile>.Writable
	{
		private IMap<TTile>.WithKnownSize KnownSizeMap { get; init; }
		private IMap<TTile>.Writable WritableMap { get; init; }
		private IntPoint Offset { get; init; }
		private IntPoint Size { get; init; }

		public IntRectangle Bounds
			=> new(KnownSizeMap.Bounds.Min - Offset, Size.X, Size.Y);

		public TTile this[IntPoint point]
		{
			get => WritableMap[point + Offset];
			set => WritableMap[point + Offset] = value;
		}

		public static Writable Create<TMap>(TMap wrapped, IntPoint offset, IntPoint size)
			where TMap : IMap<TTile>.WithKnownSize, IMap<TTile>.Writable
			=> new(wrapped, wrapped, offset, size);

		private Writable(IMap<TTile>.WithKnownSize knownSizeMap, IMap<TTile>.Writable writableMap, IntPoint offset, IntPoint size)
		{
			this.KnownSizeMap = knownSizeMap;
			this.WritableMap = writableMap;
			this.Offset = offset;
			this.Size = size;
		}
	}
}