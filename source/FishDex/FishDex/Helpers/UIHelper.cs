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
using StardewValley;
using System;

namespace FishDex.Helpers
{
	class UIHelper
	{
		// From Pathoschild.Stardew.Common.CommonHelper
		/*********
		** Fields
		*********/
		/// <summary>A blank pixel which can be colorised and stretched to draw geometric shapes.</summary>
		private static readonly Lazy<Texture2D> LazyPixel = new Lazy<Texture2D>(() =>
		{
			Texture2D pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
			pixel.SetData(new[] { Color.White });
			return pixel;
		});

		/*********
		** Accessors
		*********/
		/// <summary>A blank pixel which can be colorised and stretched to draw geometric shapes.</summary>
		public static Texture2D Pixel => UIHelper.LazyPixel.Value;

		/****
		** Fonts
		****/
		/// <summary>Get the dimensions of a space character.</summary>
		/// <param name="font">The font to measure.</param>
		public static float GetSpaceWidth(SpriteFont font)
		{
			return font.MeasureString("A B").X - font.MeasureString("AB").X;
		}
	}
}
