/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_SIMPLELAYOUT

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.Common.UI.SimpleLayout;

public class EmptyNode : ISimpleNode {

	public static readonly EmptyNode Instance = new();

	public Alignment Alignment => Alignment.None;

	public bool DeferSize => false;

	public void Draw(SpriteBatch batch, Vector2 position, Vector2 size, Vector2 containerSize, float alpha, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor) {

	}

	public Vector2 GetSize(SpriteFont defaultFont, Vector2 containerSize) {
		return Vector2.Zero;
	}
}

#endif
