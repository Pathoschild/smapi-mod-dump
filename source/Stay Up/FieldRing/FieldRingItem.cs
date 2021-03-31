/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/su226/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace Su226.FieldRing {
  class FieldRingItem : Ring, ISaveElement {
    public static Texture2D ItemTexture;

    public FieldRingItem() {
      Category = -96;
      Name = "Su226.FieldRing";
      displayName = M.Helper.Translation.Get("item.field_ring.name");
      description = M.Helper.Translation.Get("item.field_ring.description");
      uniqueID.Value = new Random().Next();
      parentSheetIndex.Value = indexInTileSheet.Value = M.Config.Index;
    }

    public Dictionary<string, string> getAdditionalSaveData() {
      return null;
    }

    public object getReplacement() {
      Ring ring = new Ring();
      ring.uniqueID.Value = this.uniqueID.Value;
      return ring;
    }

    public void rebuild(Dictionary<string, string> additionalSaveData, object replacement) {
      uniqueID.Value = (replacement as Ring).uniqueID.Value;
    }

    public override void update(GameTime time, GameLocation environment, Farmer who) {
      for (int i = environment.characters.Count - 1; i >= 0; i--) {
        Character character = environment.characters[i];
        Point characterPos = character.GetBoundingBox().Center;
        Point playerPos = who.GetBoundingBox().Center;
        Vector2 distance = new Vector2(characterPos.X - playerPos.X, characterPos.Y - playerPos.Y);
        if (character is Monster monster && distance.Length() <= M.Config.Range) {
          if (monster.Slipperiness != -1) {
            distance.Normalize();
            distance *= M.Config.Knockback;
            monster.xVelocity = distance.X;
            monster.yVelocity = -distance.Y;
          }
          int damage;
          if (M.Config.PercentageDamage) {
            damage = monster.MaxHealth * M.Config.Damage / 100;
          } else {
            damage = M.Config.Damage;
          }
          if (!M.Config.IgnoreResilience) {
            damage = Math.Max(1, damage - monster.resilience.Value);
          }
          if (!M.Config.IgnoreMissChance && Game1.random.NextDouble() < monster.missChance) {
            damage = 0;
          }
          if (M.Config.IgnoreCooldown || !monster.isInvincible()) {
            monster.setInvincibleCountdown(225);
            DamageMonster(monster, damage, who);
          }
        }
      }
    }

    private void DamageMonster(Monster monster, int damage, Farmer who) {
      GameLocation location = monster.currentLocation;
      Point center = monster.GetBoundingBox().Center;
      if (damage == 0) {
        location.debris.Add(new Debris("Miss", 1, center.ToVector2(), Color.LightGray, 1, 0));
        return;
      }
      damage = Math.Min(damage, monster.Health);
      monster.Health -= damage;
      location.playSound("hitEnemy");
      location.removeDamageDebris(monster);
      location.debris.Add(new Debris(damage, new Vector2(center.X + 16, center.Y), new Color(255, 130, 0), 1f, monster));
      if (monster.Health == 0) {
        monster.deathAnimation();
        location.monsterDrop(monster, center.X, center.Y, who);
        location.characters.Remove(monster);
        who.leftRing.Value?.onMonsterSlay(monster, location, who);
        who.rightRing.Value?.onMonsterSlay(monster, location, who);
        if (!location.isFarm) {
          who.gainExperience(4, monster.ExperienceGained);
          who.checkForQuestComplete(null, 1, 1, null, monster.Name, 4);
          if (Game1.player.team.specialOrders != null) {
            foreach (SpecialOrder order in Game1.player.team.specialOrders) {
              order.onMonsterSlain?.Invoke(Game1.player, monster);
            }
          }
          if (!(monster is GreenSlime) || (monster as GreenSlime).firstGeneration) {
            Game1.stats.monsterKilled(monster.Name);
          }
        }
        if (monster.isHardModeMonster) {
          Game1.stats.incrementStat("hardModeMonstersKilled", 1);
        }
        Game1.stats.MonstersKilled++;
      }
    }

    public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow) {
      spriteBatch.Draw(ItemTexture, location + new Vector2(Game1.tileSize) / 2 * scaleSize, null, color * transparency, 0.0f, new Vector2(8) * scaleSize, Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
    }
  }
}
