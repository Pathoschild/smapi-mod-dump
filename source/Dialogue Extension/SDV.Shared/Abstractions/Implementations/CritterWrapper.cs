/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;

namespace SDV.Shared.Abstractions
{
  public class CritterWrapper : ICritterWrapper
  {
    public CritterWrapper(Critter item) => GetBaseType = item;
    public Critter GetBaseType { get; }
    public Rectangle getBoundingBox(int xOffset, int yOffset) => default;

    public bool update(GameTime time, IGameLocationWrapper environment) => false;

    public void draw(SpriteBatch b)
    {
    }

    public void drawAboveFrontLayer(SpriteBatch b)
    {
    }
  }
}
