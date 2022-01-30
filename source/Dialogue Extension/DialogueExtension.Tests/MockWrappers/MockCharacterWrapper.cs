/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using SDV.Shared.Abstractions;
using StardewValley;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;

namespace DialogueExtension.Tests.MockWrappers
{
  public class MockCharacterWrapper : ICharacterWrapper
  {
    public MockCharacterWrapper(IEnumerable<object> args)
    {
      if (args.Count() > 0)
      {
        GetBaseType = (Character) ((IEnumerable<object>) args.First()).First();
      }
    }

    public Character GetBaseType { get; }
    public IAnimatedSpriteWrapper Sprite { get; set; }
    public Vector2 Position { get; set; }
    public int FacingDirection { get; set; }
    public string Name { get; set; }
    public bool IsEmoting { get; set; }
    public int CurrentEmote { get; set; }
    public float Scale { get; set; }
    public int speed { get; set; }
    public int Speed { get; set; }
    public int addedSpeed { get; set; }
    public string displayName { get; set; }
    public bool willDestroyObjectsUnderfoot { get; set; }
    public int CurrentEmoteIndex { get; }
    public bool IsMonster { get; }
    public IGameLocationWrapper currentLocation { get; set; }
    public IModDataDictionaryWrapper modDataForSerialization { get; set; }
    public NetFields NetFields { get; }
    public void SetMovingUp(bool b)
    {
    }

    public void SetMovingRight(bool b)
    {
    }

    public void SetMovingDown(bool b)
    {
    }

    public void SetMovingLeft(bool b)
    {
    }

    public void setMovingInFacingDirection()
    {
    }

    public int getFacingDirection() => 0;

    public void setTrajectory(int xVelocity, int yVelocity)
    {
    }

    public void setTrajectory(Vector2 trajectory)
    {
    }

    public void Halt()
    {
    }

    public void extendSourceRect(int horizontal, int vertical, bool ignoreSourceRectUpdates = true)
    {
    }

    public bool collideWith(object o) => false;

    public void faceDirection(int direction)
    {
    }

    public int getDirection() => 0;

    public bool IsRemoteMoving() => false;

    public void tryToMoveInDirection(int direction, bool isFarmer, int damagesFarmer, bool glider)
    {
    }

    public Vector2 GetShadowOffset() => default;

    public bool shouldCollideWithBuildingLayer(IGameLocationWrapper location) => false;

    public void MovePosition(GameTime time, Rectangle viewport, IGameLocationWrapper currentLocation)
    {
    }

    public bool canPassThroughActionTiles() => false;

    public Microsoft.Xna.Framework.Rectangle nextPosition(int direction) => default;

    public Location nextPositionPoint() => default;

    public int getHorizontalMovement() => 0;

    public int getVerticalMovement() => 0;

    public Vector2 nextPositionVector2() => default;

    public Location nextPositionTile() => default;

    public void doEmote(int whichEmote, bool playSound, bool nextEventCommand = true)
    {
    }

    public void doEmote(int whichEmote, bool nextEventCommand = true)
    {
    }

    public void updateEmote(GameTime time)
    {
    }

    public Vector2 GetGrabTile() => default;

    public Vector2 GetDropLocation() => default;

    public Vector2 GetToolLocation(Vector2 target_position, bool ignoreClick = false) => default;

    public Vector2 GetToolLocation(bool ignoreClick = false) => default;

    public int getGeneralDirectionTowards(Vector2 target, int yBias = 0, bool opposite = false, bool useTileCalculations = true) => 0;

    public void faceGeneralDirection(Vector2 target, int yBias, bool opposite, bool useTileCalculations)
    {
    }

    public void faceGeneralDirection(Vector2 target, int yBias = 0, bool opposite = false)
    {
    }

    public void draw(SpriteBatch b)
    {
    }

    public void draw(SpriteBatch b, float alpha = 1)
    {
    }

    public void draw(SpriteBatch b, int ySourceRectOffset, float alpha = 1)
    {
    }

    public void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
    }

    public int GetSpriteWidthForPositioning() => 0;

    public Microsoft.Xna.Framework.Rectangle GetBoundingBox() => default;

    public void stopWithoutChangingFrame()
    {
    }

    public void collisionWithFarmerBehavior()
    {
    }

    public int getStandingX() => 0;

    public int getStandingY() => 0;

    public Vector2 getStandingPosition() => default;

    public Point getStandingXY() => default;

    public Vector2 getLocalPosition(Rectangle viewport) => default;

    public bool isMoving() => false;

    public Point getTileLocationPoint() => default;

    public int getTileX() => 0;

    public int getTileY() => 0;

    public Vector2 getTileLocation() => default;

    public void setTileLocation(Vector2 tileLocation)
    {
    }

    public void startGlowing(Color glowingColor, bool border, float glowRate)
    {
    }

    public void stopGlowing()
    {
    }

    public void jumpWithoutSound(float velocity = 8)
    {
    }

    public void jump()
    {
    }

    public void jump(float jumpVelocity)
    {
    }

    public void faceTowardFarmerForPeriod(int milliseconds, int radius, bool faceAway, IFarmerWrapper who)
    {
    }

    public void update(GameTime time, IGameLocationWrapper location)
    {
    }

    public void update(GameTime time, IGameLocationWrapper location, long id, bool move)
    {
    }

    public void performBehavior(byte which)
    {
    }

    public void checkForFootstep()
    {
    }

    public void updateFaceTowardsFarmer(GameTime time, IGameLocationWrapper location)
    {
    }

    public bool hasSpecialCollisionRules() => false;

    public bool isColliding(IGameLocationWrapper l, Vector2 tile) => false;

    public void animateInFacingDirection(GameTime time)
    {
    }

    public void updateMovement(IGameLocationWrapper location, GameTime time)
    {
    }

    public void updateGlow()
    {
    }

    public void convertEventMotionCommandToMovement(Vector2 command)
    {
    }
  }
}
