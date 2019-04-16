using FishDex.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace FishDex.Components
{
	// From Pathoschild.Stardew.LookupAnything.Components.Sprites
	/// <summary>Simplifies access to the game's sprite sheets.</summary>
	/// <remarks>Each sprite is represented by a rectangle, which specifies the coordinates and dimensions of the image in the sprite sheet.</remarks>
	internal static class Sprites
	{
		/*********
        ** Accessors
        *********/
		/// <summary>Sprites used to draw a letter.</summary>
		public static class Letter
		{
			/// <summary>The sprite sheet containing the letter sprites.</summary>
			public static Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\letterBG");

			/// <summary>The letter background (including edges and corners).</summary>
			public static readonly Rectangle Sprite = new Rectangle(0, 0, 320, 180);
		}

		/// <summary>Sprites used to draw icons.</summary>
		public static class Icons
		{
			/// <summary>The sprite sheet containing the icon sprites.</summary>
			public static Texture2D Sheet => Game1.mouseCursors;

			/// <summary>An empty checkbox icon.</summary>
			public static readonly Rectangle EmptyCheckbox = new Rectangle(227, 425, 9, 9);

			/// <summary>A filled checkbox icon.</summary>
			public static readonly Rectangle FilledCheckbox = new Rectangle(236, 425, 9, 9);

			/// <summary>A down arrow for scrolling content.</summary>
			public static readonly Rectangle DownArrow = new Rectangle(12, 76, 40, 44);

			/// <summary>An up arrow for scrolling content.</summary>
			public static readonly Rectangle UpArrow = new Rectangle(76, 72, 40, 44);
		}

		/// <summary>A blank pixel which can be colorised and stretched to draw geometric shapes.</summary>
		public static readonly Texture2D Pixel = UIHelper.Pixel;
	}
}

