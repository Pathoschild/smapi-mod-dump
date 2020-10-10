/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rupak0577/FishDex
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FishDex
{
	// Copy of Pathoschild.Stardew.Common.SpriteInfo
	/// <summary>Represents a single sprite in a spritesheet.</summary>
	internal class SpriteInfo
	{
		/*********
        ** Accessors
        *********/
		/// <summary>The spritesheet texture.</summary>
		public Texture2D Spritesheet { get; }

		/// <summary>The area in the spritesheet containing the sprite.</summary>
		public Rectangle SourceRectangle { get; }


		/*********
        ** Public methods
        *********/
		/// <summary>Construct an instance.</summary>
		/// <param name="spritesheet">The spritesheet texture.</param>
		/// <param name="sourceRectangle">The area in the spritesheet containing the sprite.</param>
		public SpriteInfo(Texture2D spritesheet, Rectangle sourceRectangle)
		{
			this.Spritesheet = spritesheet;
			this.SourceRectangle = sourceRectangle;
		}
	}
}