using Microsoft.Xna.Framework;

namespace FishDex.Framework
{
	// Copy of Pathoschild.Stardew.LookupAnything.Framework.IFormattedText
	/// <summary>A snippet of formatted text.</summary>
	internal interface IFormattedText
	{
		/// <summary>The font color (or <c>null</c> for the default color).</summary>
		Color? Color { get; }

		/// <summary>The text to format.</summary>
		string Text { get; }

		/// <summary>Whether to draw bold text.</summary>
		bool Bold { get; }
	}
}