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
using StardewValley;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;

namespace SDV.Shared.Abstractions
{
  public interface ICharacterWrapper : IWrappedType<Character>, INetObject<NetFields>
  {
    IAnimatedSpriteWrapper Sprite { get; set; }
    Vector2 Position { get; set; }
    int FacingDirection { get; set; }
    string Name { get; set; }
    bool IsEmoting { get; set; }
    int CurrentEmote { get; set; }
    float Scale { get; set; }
    int speed { get; set; }
    int Speed { get; set; }
    int addedSpeed { get; set; }
    string displayName { get; set; }
    bool willDestroyObjectsUnderfoot { get; set; }
    int CurrentEmoteIndex { get; }
    bool IsMonster { get; }
    IGameLocationWrapper currentLocation { get; set; }
    IModDataDictionaryWrapper modDataForSerialization { get; set; }
    void SetMovingUp(bool b);
    void SetMovingRight(bool b);
    void SetMovingDown(bool b);
    void SetMovingLeft(bool b);
    void setMovingInFacingDirection();
    int getFacingDirection();
    void setTrajectory(int xVelocity, int yVelocity);
    void setTrajectory(Vector2 trajectory);
    void Halt();
    void extendSourceRect(int horizontal, int vertical, bool ignoreSourceRectUpdates = true);
    bool collideWith(object o);
    void faceDirection(int direction);
    int getDirection();
    bool IsRemoteMoving();
    void tryToMoveInDirection(int direction, bool isFarmer, int damagesFarmer, bool glider);
    Vector2 GetShadowOffset();
    bool shouldCollideWithBuildingLayer(IGameLocationWrapper location);

    void MovePosition(
      GameTime time,
      Rectangle viewport,
      IGameLocationWrapper currentLocation);

    bool canPassThroughActionTiles();
    Microsoft.Xna.Framework.Rectangle nextPosition(int direction);
    Location nextPositionPoint();
    int getHorizontalMovement();
    int getVerticalMovement();
    Vector2 nextPositionVector2();
    Location nextPositionTile();
    void doEmote(int whichEmote, bool playSound, bool nextEventCommand = true);
    void doEmote(int whichEmote, bool nextEventCommand = true);
    void updateEmote(GameTime time);
    Vector2 GetGrabTile();
    Vector2 GetDropLocation();
    Vector2 GetToolLocation(Vector2 target_position, bool ignoreClick = false);
    Vector2 GetToolLocation(bool ignoreClick = false);

    int getGeneralDirectionTowards(
      Vector2 target,
      int yBias = 0,
      bool opposite = false,
      bool useTileCalculations = true);

    void faceGeneralDirection(
      Vector2 target,
      int yBias,
      bool opposite,
      bool useTileCalculations);

    void faceGeneralDirection(Vector2 target, int yBias = 0, bool opposite = false);
    void draw(SpriteBatch b);
    void draw(SpriteBatch b, float alpha = 1f);
    void draw(SpriteBatch b, int ySourceRectOffset, float alpha = 1f);
    void drawAboveAlwaysFrontLayer(SpriteBatch b);
    int GetSpriteWidthForPositioning();
    Microsoft.Xna.Framework.Rectangle GetBoundingBox();
    void stopWithoutChangingFrame();
    void collisionWithFarmerBehavior();
    int getStandingX();
    int getStandingY();
    Vector2 getStandingPosition();
    Point getStandingXY();
    Vector2 getLocalPosition(Rectangle viewport);
    bool isMoving();
    Point getTileLocationPoint();
    int getTileX();
    int getTileY();
    Vector2 getTileLocation();
    void setTileLocation(Vector2 tileLocation);
    void startGlowing(Color glowingColor, bool border, float glowRate);
    void stopGlowing();
    void jumpWithoutSound(float velocity = 8f);
    void jump();
    void jump(float jumpVelocity);
    void faceTowardFarmerForPeriod(int milliseconds, int radius, bool faceAway, IFarmerWrapper who);
    void update(GameTime time, IGameLocationWrapper location);
    void update(GameTime time, IGameLocationWrapper location, long id, bool move);
    void performBehavior(byte which);
    void checkForFootstep();
    void updateFaceTowardsFarmer(GameTime time, IGameLocationWrapper location);
    bool hasSpecialCollisionRules();
    bool isColliding(IGameLocationWrapper l, Vector2 tile);
    void animateInFacingDirection(GameTime time);
    void updateMovement(IGameLocationWrapper location, GameTime time);
    void updateGlow();
    void convertEventMotionCommandToMovement(Vector2 command);
  }
}