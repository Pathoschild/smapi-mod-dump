/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.Common.UI.SimpleLayout;

/// <summary>
/// An <c>ISimpleNode</c> defines a distinct element within a simple
/// UI layout, with a size, alignment, and a method for rendering.
/// </summary>
public interface ISimpleNode {

	/// <summary>
	/// How this node should be aligned within its parent.
	/// </summary>
	Alignment Alignment { get; }

	/// <summary>
	/// When this is true, GetSize will be delayed until after all
	/// non-deferred nodes have reported their own sizes and we
	/// have an idea on how big the container will be.
	/// </summary>
	bool DeferSize { get; }

	/// <summary>
	/// Determine how large this node should be when drawn.
	/// </summary>
	/// <param name="defaultFont">The default font that should be used
	/// when drawing text.</param>
	/// <param name="containerSize">The current expected size of
	/// the container around this node, which may expand as
	/// sibling nodes report their sizes.</param>
	/// <returns>The size of this node.</returns>
	Vector2 GetSize(SpriteFont defaultFont, Vector2 containerSize);

	/// <summary>
	/// Draw the node.
	/// </summary>
	/// <param name="batch">The <see cref="SpriteBatch"/> to draw it with.</param>
	/// <param name="position">The position to draw it at.</param>
	/// <param name="size">The size to draw it within. This value was previously
	/// returned from <see cref="GetSize(SpriteFont, Vector2)"/></param>
	/// <param name="containerSize">The size of the container around this node,
	/// in case that is relevant.</param>
	/// <param name="alpha">The amount of transparency to draw with.</param>
	/// <param name="defaultFont">The default font that should be used
	/// when drawing text.</param>
	/// <param name="defaultColor">The default color that should be used
	/// when drawing text.</param>
	/// <param name="defaultShadowColor">The default shadow color that
	/// should be used when drawing text.</param>
	void Draw(SpriteBatch batch, Vector2 position, Vector2 size, Vector2 containerSize, float alpha, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor);

}
