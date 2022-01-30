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
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class MeleeWeaponWrapper : ToolWrapper, IMeleeWeaponWrapper
  {
    public MeleeWeaponWrapper(Item item) : base(item)
    {
    }

    public int getNumberOfDescriptionCategories() => 0;

    public bool isScythe(int index = -1) => false;

    public int getItemLevel() => 0;

    public float defaultKnockBackForThisType(int type) => 0;

    public Rectangle getAreaOfEffect(int x, int y, int facingDirection, ref Vector2 tileLocation1, ref Vector2 tileLocation2,
      Rectangle wielderBoundingBox, int indexInCurrentAnimation) =>
      default;

    public void triggerDefenseSwordFunction(IFarmerWrapper who)
    {
    }

    public void doStabbingSwordFunction(IFarmerWrapper who)
    {
    }

    public void triggerDaggerFunction(IFarmerWrapper who, int dagger_hits_left)
    {
    }

    public void triggerClubFunction(IFarmerWrapper who)
    {
    }

    public void animateSpecialMove(IFarmerWrapper who)
    {
    }

    public void doSwipe(int type, Vector2 position, int facingDirection, float swipeSpeed, IFarmerWrapper f)
    {
    }

    public void setFarmerAnimating(IFarmerWrapper who)
    {
    }

    public void RecalculateAppliedForges(bool force = false)
    {
    }

    public void DoDamage(IGameLocationWrapper location, int x, int y, int facingDirection, int power, IFarmerWrapper who)
    {
    }

    public int getDrawnItemIndex() => 0;

    public void drawDuringUse(int frameOfFarmerAnimation, int facingDirection, SpriteBatch spriteBatch, Vector2 playerPosition,
      IFarmerWrapper f)
    {
    }

    public bool isGalaxyWeapon() => false;

    public void transform(int newIndex)
    {
    }
  }
}
