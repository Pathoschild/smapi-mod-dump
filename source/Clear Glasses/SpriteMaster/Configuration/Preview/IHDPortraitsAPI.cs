/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace SpriteMaster.Configuration.Preview;

// https://github.com/tlitookilakin/HDPortraits/blob/415ede4348c79161afa78db9f843165d05da3cb1/HDPortraits/IHDPortraitsAPI.cs
public interface IHDPortraitsAPI {
	/// <summary>
	/// The name of the current override portrait to use
	/// </summary>
	public string OverrideName { set; get; }
	/// <summary>
	/// Draw NPC Portrait over region
	/// </summary>
	/// <param name="b">The spritebatch to draw with</param>
	/// <param name="npc">NPC</param>
	/// <param name="index">Portrait index</param>
	/// <param name="region">The region of the screen to draw to</param>
	/// <param name="color">Tint</param>
	/// <param name="reset">Whether or not to reset animations this tick</param>
	public void DrawPortrait(SpriteBatch b, NPC npc, int index, XRectangle region, XColor? color = null, bool reset = false);
	/// <summary>
	/// Draw NPC Portrait at position with default size
	/// </summary>
	/// <param name="b">The spritebatch to draw with</param>
	/// <param name="npc">NPC</param>
	/// <param name="index">Portrait index</param>
	/// <param name="position">Position to draw at</param>
	/// <param name="color">Tint</param>
	/// <param name="reset">Whether or not to reset animations this tick</param>
	public void DrawPortrait(SpriteBatch b, NPC npc, int index, XNA.Point position, XColor? color = null, bool reset = false);
	/// <summary>
	/// Retrieves the texture and texture region to use for a portrait
	/// </summary>
	/// <param name="npc">NPC</param>
	/// <param name="index">Portrait index</param>
	/// <param name="elapsed">Time since last call (for animation)</param>
	/// <param name="reset">Whether or not to reset animations this tick</param>
	/// <returns>The source region &amp; the texture to use</returns>
	public (XRectangle, Texture2D) GetTextureAndRegion(NPC npc, int index, int elapsed = -1, bool reset = false);
	[Obsolete("Directly invalidate the necessary asset instead. It will be automatically reloaded.")]
	public void ReloadData();
	/// <summary>
	/// Draw NPC or custom portrait over region
	/// </summary>
	/// <param name="b">The spritebatch to draw with</param>
	/// <param name="name">Override name</param>
	/// <param name="suffix">Context suffix, or null</param>
	/// <param name="index">Portrait index</param>
	/// <param name="region">The region of the screen to draw to</param>
	/// <param name="color">Tint</param>
	/// <param name="reset">Whether or not to reset animations this tick</param>
	public void DrawPortrait(SpriteBatch b, string name, string? suffix, int index, XRectangle region, XColor? color = null, bool reset = false);

	/// <summary>
	/// Draw NPC or custom portrait with default size
	/// </summary>
	/// <param name="b">The spritebatch to draw with</param>
	/// <param name="name">Override name</param>
	/// <param name="suffix">Context suffix, or null</param>
	/// <param name="index">Portrait index</param>
	/// <param name="position">The position on the screen to draw at</param>
	/// <param name="color">Tint</param>
	/// <param name="reset">Whether or not to reset animations this tick</param>
	public void DrawPortrait(SpriteBatch b, string name, string suffix, int index, XNA.Point position, XColor? color = null, bool reset = false);
	/// <summary>
	/// Retrieves the texture and texture region to use for a portrait
	/// </summary>
	/// <param name="name">Override name</param>
	/// <param name="suffix">Context suffix, or null</param>
	/// <param name="index">Portrait index</param>
	/// <param name="elapsed">Time since last call (for animation)</param>
	/// <param name="reset">Whether or not to reset animations this tick</param>
	/// <returns>The source region &amp; the texture to use</returns>
	public (XRectangle, Texture2D) GetTextureAndRegion(string name, string? suffix, int index, int elapsed = -1, bool reset = false);
	/// <summary>
	/// Draw NPC Portrait over region, or current override texture
	/// </summary>
	/// <param name="b">The spritebatch to draw with</param>
	/// <param name="npc">NPC</param>
	/// <param name="index">Portrait index</param>
	/// <param name="region">The region of the screen to draw to</param>
	/// <param name="color">Tint</param>
	/// <param name="reset">Whether or not to reset animations this tick</param>
	public void DrawPortraitOrOverride(SpriteBatch b, NPC npc, int index, XRectangle region, XColor? color = null, bool reset = false);
	/// <summary>
	/// Draw NPC Portrait at position with default size, or current override texture
	/// </summary>
	/// <param name="b">The spritebatch to draw with</param>
	/// <param name="npc">NPC</param>
	/// <param name="index">Portrait index</param>
	/// <param name="position">Position to draw at</param>
	/// <param name="color">Tint</param>
	/// <param name="reset">Whether or not to reset animations this tick</param>
	public void DrawPortraitOrOverride(SpriteBatch b, NPC npc, int index, XNA.Point position, XColor? color = null, bool reset = false);
}
