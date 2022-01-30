/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDV.Shared.Abstractions
{
  public interface IFurnitureWrapper : IObjectWrapper, ISittableWrapper
  {
    int placementRestriction { get; }
    string description { get; }
    void OnAdded(IGameLocationWrapper loc, Vector2 tilePos);
    void setFireplace(IGameLocationWrapper location, bool playSound = true, bool broadcast = false);
    void AttemptRemoval(Action<IFurnitureWrapper> removal_action);
    bool canBeRemoved(IFarmerWrapper who);
    int GetSeatCapacity();
    void AddLightGlow(IGameLocationWrapper location);
    void RemoveLightGlow(IGameLocationWrapper location);
    void resetOnPlayerEntry(IGameLocationWrapper environment, bool dropDown = false);
    void addLights(IGameLocationWrapper environment);
    void removeLights(IGameLocationWrapper environment);
    void rotate();
    void updateRotation();
    bool isGroundFurniture();
    bool IsCloseEnoughToIFarmerWrapper(IFarmerWrapper f, int? override_tile_x = null, int? override_tile_y = null);
    int GetModifiedWallTilePosition(IGameLocationWrapper l, int tile_x, int tile_y);
    void updateDrawPosition();
    int getTilesWide();
    int getTilesHigh();

    int GetAdditionalFurniturePlacementStatus(
      IGameLocationWrapper location,
      int x,
      int y,
      IFarmerWrapper who = null);

    bool AllowPlacementOnThisTile(int tile_x, int tile_y);

    void drawAtNonTileSpot(
      SpriteBatch spriteBatch,
      Vector2 location,
      float layerDepth,
      float alpha = 1f);

    int GetAdditionalTilePropertyRadius();

    bool DoesTileHaveProperty(
      int tile_x,
      int tile_y,
      string property_name,
      string layer_name,
      ref string property_value);

    bool IntersectsForCollision(Rectangle rect);
  }
}