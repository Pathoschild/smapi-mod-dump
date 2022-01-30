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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDV.Shared.Abstractions
{
  public interface IMonsterWrapper : INPCWrapper
  {
    int DamageToFarmer { get; set; }
    int Health { get; set; }
    int MaxHealth { get; set; }
    int Slipperiness { get; set; }
    int ExperienceGained { get; set; }
    IFarmerWrapper Player { get; }
    bool focusedOnFarmers { get; set; }
    bool wildernessFarmMonster { get; set; }
    void onDealContactDamage(IFarmerWrapper who);
    IEnumerable<IItemWrapper> getExtraDropItems();
    void drawAboveAllLayers(SpriteBatch b);
    bool isInvincible();
    void setInvincibleCountdown(int time);
    IDebrisWrapper ModifyMonsterLoot(IDebrisWrapper debris);
    int GetBaseDifficultyLevel();
    void BuffForAdditionalDifficulty(int additional_difficulty);
    void InitializeForLocation(IGameLocationWrapper location);
    void shedChunks(int number);
    void shedChunks(int number, float scale);
    void deathAnimation();
    void parried(int damage, IFarmerWrapper who);

    int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      IFarmerWrapper who);

    int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      string hitSound);
    
    void behaviorAtGameTick(GameTime time);
    bool passThroughCharacters();
    bool ShouldActuallyMoveAwayFromPlayer();
    void noMovementProgressNearPlayerBehavior();
    void defaultMovementBehavior(GameTime time);
    bool TakesDamageFromHitbox(Rectangle area_of_effect);
    bool OverlapsFarmerForDamage(IFarmerWrapper who);
  }
}