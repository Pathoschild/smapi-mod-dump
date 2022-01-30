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
  public interface ITerrainFeaturesWrapper : IWrappedType<TerrainFeature>
  {
    IModDataDictionaryWrapper modDataForSerialization { get; set; }
    bool NeedsUpdate { set; get; }
    NetFields NetFields { get; }
    Rectangle getBoundingBox(Vector2 tileLocation);
    Rectangle getRenderBounds(Vector2 tileLocation);
    void loadSprite();
    bool isPassable(ICharacterWrapper c = null);
    void OnAddedToLocation(IGameLocationWrapper location, Vector2 tile);

    void doCollisionAction(
      Rectangle positionOfCollider,
      int speedOfCollision,
      Vector2 tileLocation,
      ICharacterWrapper who,
      IGameLocationWrapper location);

    bool performUseAction(Vector2 tileLocation, IGameLocationWrapper location);

    bool performToolAction(
      IToolWrapper t,
      int damage,
      Vector2 tileLocation,
      IGameLocationWrapper location);

    bool tickUpdate(GameTime time, Vector2 tileLocation, IGameLocationWrapper location);
    void dayUpdate(IGameLocationWrapper environment, Vector2 tileLocation);
    bool seasonUpdate(bool onLoad);
    bool isActionable();
    void performPlayerEntryAction(Vector2 tileLocation);
    void draw(SpriteBatch spriteBatch, Vector2 tileLocation);
    bool forceDraw();

    void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 positionOnScreen,
      Vector2 tileLocation,
      float scale,
      float layerDepth);
  }
}