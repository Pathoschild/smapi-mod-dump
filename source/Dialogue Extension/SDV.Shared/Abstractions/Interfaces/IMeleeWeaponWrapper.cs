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

namespace SDV.Shared.Abstractions
{
  public interface IMeleeWeaponWrapper : IToolWrapper
  {
    int getNumberOfDescriptionCategories();
    bool isScythe(int index = -1);
    int getItemLevel();
    float defaultKnockBackForThisType(int type);

    Rectangle getAreaOfEffect(
      int x,
      int y,
      int facingDirection,
      ref Vector2 tileLocation1,
      ref Vector2 tileLocation2,
      Rectangle wielderBoundingBox,
      int indexInCurrentAnimation);

    void triggerDefenseSwordFunction(IFarmerWrapper who);
    void doStabbingSwordFunction(IFarmerWrapper who);
    void triggerDaggerFunction(IFarmerWrapper who, int dagger_hits_left);
    void triggerClubFunction(IFarmerWrapper who);
    void animateSpecialMove(IFarmerWrapper who);

    void doSwipe(
      int type,
      Vector2 position,
      int facingDirection,
      float swipeSpeed,
      IFarmerWrapper f);

    void setFarmerAnimating(IFarmerWrapper who);
    void RecalculateAppliedForges(bool force = false);

    void DoDamage(
      IGameLocationWrapper location,
      int x,
      int y,
      int facingDirection,
      int power,
      IFarmerWrapper who);

    int getDrawnItemIndex();

    void drawDuringUse(
      int frameOfFarmerAnimation,
      int facingDirection,
      SpriteBatch spriteBatch,
      Vector2 playerPosition,
      IFarmerWrapper f);
    
    bool isGalaxyWeapon();
    void transform(int newIndex);
  }
}