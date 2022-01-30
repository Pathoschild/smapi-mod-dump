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
using Netcode;
using StardewValley.TerrainFeatures;

namespace SDV.Shared.Abstractions
{
  public class TerrainFeaturesWrapper : ITerrainFeaturesWrapper
  {
    public TerrainFeaturesWrapper(TerrainFeature item) => GetBaseType = item;
    public TerrainFeature GetBaseType { get; }
    public IModDataDictionaryWrapper modDataForSerialization { get; set; }
    public bool NeedsUpdate { get; set; }
    public NetFields NetFields { get; }
    public Rectangle getBoundingBox(Vector2 tileLocation) => default;

    public Rectangle getRenderBounds(Vector2 tileLocation) => default;

    public void loadSprite()
    {
    }

    public bool isPassable(ICharacterWrapper c = null) => false;

    public void OnAddedToLocation(IGameLocationWrapper location, Vector2 tile)
    {
    }

    public void doCollisionAction(Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation, ICharacterWrapper who,
      IGameLocationWrapper location)
    {
    }

    public bool performUseAction(Vector2 tileLocation, IGameLocationWrapper location) => false;

    public bool performToolAction(IToolWrapper t, int damage, Vector2 tileLocation, IGameLocationWrapper location) => false;

    public bool tickUpdate(GameTime time, Vector2 tileLocation, IGameLocationWrapper location) => false;

    public void dayUpdate(IGameLocationWrapper environment, Vector2 tileLocation)
    {
    }

    public bool seasonUpdate(bool onLoad) => false;

    public bool isActionable() => false;

    public void performPlayerEntryAction(Vector2 tileLocation)
    {
    }

    public void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
    {
    }

    public bool forceDraw() => false;

    public void drawInMenu(SpriteBatch spriteBatch, Vector2 positionOnScreen, Vector2 tileLocation, float scale, float layerDepth)
    {
    }
  }
}
