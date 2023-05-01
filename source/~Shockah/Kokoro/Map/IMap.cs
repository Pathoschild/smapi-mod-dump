/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.Kokoro.Map
{
	public interface IMap<TTile>
	{
		TTile this[IntPoint point] { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Nested in another interface")]
		public interface WithKnownSize : IMap<TTile>
		{
			IntRectangle Bounds { get; }
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Nested in another interface")]
		public interface Writable : IMap<TTile>
		{
			new TTile this[IntPoint point] { get; set; }
		}
	}
}