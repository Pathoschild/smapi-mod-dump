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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Object = StardewValley.Object;

namespace SDV.Shared.Abstractions
{
  public class FurnitureWrapper : ObjectWrapper, IFurnitureWrapper
  {
    public FurnitureWrapper(Object item) : base(item)
    {
    }

    public bool IsSittingHere(Farmer who) => false;

    public bool HasSittingFarmers() => false;

    public void RemoveSittingFarmer(Farmer farmer)
    {
    }

    public int GetSittingFarmerCount() => 0;

    public List<Vector2> GetSeatPositions(bool ignore_offsets = false) => null;

    public Vector2? GetSittingPosition(Farmer who, bool ignore_offsets = false) => null;

    public Vector2? AddSittingFarmer(Farmer who) => null;

    public int GetSittingDirection() => 0;

    public Rectangle GetSeatBounds() => default;

    public bool IsSeatHere(GameLocation location) => false;

    public bool IsSittingHere(IFarmerWrapper who) => false;

    public void RemoveSittingFarmer(IFarmerWrapper farmer)
    {
    }

    public Vector2? GetSittingPosition(IFarmerWrapper who, bool ignore_offsets = false) => null;

    public Vector2? AddSittingFarmer(IFarmerWrapper who) => null;

    public bool IsSeatHere(IGameLocationWrapper location) => false;

    public int placementRestriction { get; }
    public string description { get; }
    public void OnAdded(IGameLocationWrapper loc, Vector2 tilePos)
    {
    }

    public void setFireplace(IGameLocationWrapper location, bool playSound = true, bool broadcast = false)
    {
    }

    public void AttemptRemoval(Action<IFurnitureWrapper> removal_action)
    {
    }

    public bool canBeRemoved(IFarmerWrapper who) => false;

    public int GetSeatCapacity() => 0;

    public void AddLightGlow(IGameLocationWrapper location)
    {
    }

    public void RemoveLightGlow(IGameLocationWrapper location)
    {
    }

    public void resetOnPlayerEntry(IGameLocationWrapper environment, bool dropDown = false)
    {
    }

    public void addLights(IGameLocationWrapper environment)
    {
    }

    public void removeLights(IGameLocationWrapper environment)
    {
    }

    public void rotate()
    {
    }

    public void updateRotation()
    {
    }

    public bool isGroundFurniture() => false;

    public bool IsCloseEnoughToIFarmerWrapper(IFarmerWrapper f, int? override_tile_x = null, int? override_tile_y = null) => false;

    public int GetModifiedWallTilePosition(IGameLocationWrapper l, int tile_x, int tile_y) => 0;

    public void updateDrawPosition()
    {
    }

    public int getTilesWide() => 0;

    public int getTilesHigh() => 0;

    public int GetAdditionalFurniturePlacementStatus(IGameLocationWrapper location, int x, int y, IFarmerWrapper who = null) => 0;

    public bool AllowPlacementOnThisTile(int tile_x, int tile_y) => false;

    public void drawAtNonTileSpot(SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha = 1)
    {
    }

    public int GetAdditionalTilePropertyRadius() => 0;

    public bool DoesTileHaveProperty(int tile_x, int tile_y, string property_name, string layer_name, ref string property_value) => false;

    public bool IntersectsForCollision(Rectangle rect) => false;
  }
}