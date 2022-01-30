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
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class MonsterWrapper : NPCWrapper, IMonsterWrapper
  {
    public MonsterWrapper(NPC npc) : base(npc)
    {
    }

    public int DamageToFarmer { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Slipperiness { get; set; }
    public int ExperienceGained { get; set; }
    public IFarmerWrapper Player { get; }
    public bool focusedOnFarmers { get; set; }
    public bool wildernessFarmMonster { get; set; }
    public void onDealContactDamage(IFarmerWrapper who)
    {
    }

    public IEnumerable<IItemWrapper> getExtraDropItems()
    {
      yield break;
    }

    public void drawAboveAllLayers(SpriteBatch b)
    {
    }

    public bool isInvincible() => false;

    public void setInvincibleCountdown(int time)
    {
    }

    public IDebrisWrapper ModifyMonsterLoot(IDebrisWrapper debris) => null;

    public int GetBaseDifficultyLevel() => 0;

    public void BuffForAdditionalDifficulty(int additional_difficulty)
    {
    }

    public void InitializeForLocation(IGameLocationWrapper location)
    {
    }

    public void shedChunks(int number)
    {
    }

    public void shedChunks(int number, float scale)
    {
    }

    public void deathAnimation()
    {
    }

    public void parried(int damage, IFarmerWrapper who)
    {
    }

    public int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, IFarmerWrapper who) => 0;

    public int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, string hitSound) => 0;

    public void behaviorAtGameTick(GameTime time)
    {
    }

    public bool passThroughCharacters() => false;

    public bool ShouldActuallyMoveAwayFromPlayer() => false;

    public void noMovementProgressNearPlayerBehavior()
    {
    }

    public void defaultMovementBehavior(GameTime time)
    {
    }

    public bool TakesDamageFromHitbox(Rectangle area_of_effect) => false;

    public bool OverlapsFarmerForDamage(IFarmerWrapper who) => false;
  }
}
