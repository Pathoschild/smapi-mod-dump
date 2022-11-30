/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewRoguelike.Enchantments;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StardewRoguelike
{
    /// <summary>
    /// All types of curses. The enum value is the unique
    /// buff ID to be used in-game.
    /// </summary>
    #pragma warning disable format
    internal enum CurseType
    {
        GlassCannon           = 89000,
        DamageOverTime        = 89001,
        PlayerKnockback       = 89002,
        BrittleCrown          = 89003,
        MoreEnemiesLessHealth = 89004,
        //GestureOfTheDrowned   = 89005,
        BombsAroundPlayer     = 89006,
        DoubleSpeedEveryone   = 89007,
        HealOverTime          = 89008,
        BuffsMorePotent       = 89009,
        BootsBetterImmunity   = 89010,
        BootsBetterDefense    = 89011,
        MoreCritsLessDamage   = 89012,
        CheaperMerchant       = 89013,
        MonsterBuffs          = 89014
    }
    #pragma warning restore format

    internal class Curse : Buff
    {
        private static string GetCurseDisplayName(CurseType curseType)
        {
            return curseType switch
            {
                CurseType.GlassCannon => I18n.CurseTypeName_GlassCannon(),
                CurseType.DamageOverTime => I18n.CurseTypeName_DamageOverTime(),
                CurseType.PlayerKnockback => I18n.CurseTypeName_PlayerKnockback(),
                CurseType.BrittleCrown => I18n.CurseTypeName_BrittleCrown(),
                CurseType.MoreEnemiesLessHealth => I18n.CurseTypeName_MoreEnemiesLessHealth(),
                //CurseType.GestureOfTheDrowned => I18n.CurseTypeName_GestureOfTheDrowned()
                CurseType.BombsAroundPlayer => I18n.CurseTypeName_BombsAroundPlayer(),
                CurseType.DoubleSpeedEveryone => I18n.CurseTypeName_DoubleSpeedEveryone(),
                CurseType.HealOverTime => I18n.CurseTypeName_HealOverTime(),
                CurseType.BuffsMorePotent => I18n.CurseTypeName_BuffsMorePotent(),
                CurseType.BootsBetterImmunity => I18n.CurseTypeName_BootsBetterImmunity(),
                CurseType.BootsBetterDefense => I18n.CurseTypeName_BootsBetterDefense(),
                CurseType.MoreCritsLessDamage => I18n.CurseTypeName_MoreCritsLessDamage(),
                CurseType.CheaperMerchant => I18n.CurseTypeName_CheaperMerchant(),
                CurseType.MonsterBuffs => I18n.CurseTypeName_MonsterBuffs(),
                _ => throw new NotImplementedException(),
            };
        }

        private static string GetCurseDescription(CurseType curseType)
        {
            return curseType switch {
                CurseType.GlassCannon => I18n.CurseTypeDesc_GlassCannon(),
                CurseType.DamageOverTime => I18n.CurseTypeDesc_DamageOverTime(),
                CurseType.PlayerKnockback => I18n.CurseTypeDesc_PlayerKnockback(),
                CurseType.BrittleCrown => I18n.CurseTypeDesc_BrittleCrown(),
                CurseType.MoreEnemiesLessHealth => I18n.CurseTypeDesc_MoreEnemiesLessHealth(),
                //CurseType.GestureOfTheDrowned => I18n.CurseTypeDesc_GestureOfTheDrowned()
                CurseType.BombsAroundPlayer => I18n.CurseTypeDesc_BombsAroundPlayer(),
                CurseType.DoubleSpeedEveryone => I18n.CurseTypeDesc_DoubleSpeedEveryone(),
                CurseType.HealOverTime => I18n.CurseTypeDesc_HealOverTime(),
                CurseType.BuffsMorePotent => I18n.CurseTypeDesc_BuffsMorePotent(),
                CurseType.BootsBetterImmunity => I18n.CurseTypeDesc_BootsBetterImmunity(),
                CurseType.BootsBetterDefense => I18n.CurseTypeDesc_BootsBetterDefense(),
                CurseType.MoreCritsLessDamage => I18n.CurseTypeDesc_MoreCritsLessDamage(),
                CurseType.CheaperMerchant => I18n.CurseTypeDesc_CheaperMerchant(),
                CurseType.MonsterBuffs => I18n.CurseTypeDesc_MonsterBuffs(),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// The description of the Curse instance.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// The buff icon to use when displaying curses.
        /// </summary>
        public static readonly int IconId = 18;

        /// <summary>
        /// The duration (in milliseconds) of buffs that are made
        /// potent via the <see cref="CurseType.BuffsMorePotent"/> curse.
        /// </summary>
        public static readonly int PotentBuffDuration = 1000 * 60 * 5;

        /// <summary>
        /// The amount of game ticks to wait before spawning bombs when
        /// the player has the <see cref="CurseType.BombsAroundPlayer"/> curse.
        /// </summary>
        private static int TicksToSpawnBombs = 60 * 10;

        /// <summary>
        /// The amount of damage over time to deal when the player has the
        /// <see cref="CurseType.DamageOverTime"/> curse.
        /// </summary>
        public static int DOTDamageToTick = 0;

        /// <summary>
        /// The amount of heal over time to perform when the player has the
        /// <see cref="CurseType.HealOverTime"/> curse.
        /// </summary>
        public static int HOTHealToTick = 0;

        public Curse(CurseType curseType) : base(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, minutesDuration: 1, "Roguelike", "Roguelike")
        {
            Description = $"{GetCurseDisplayName(curseType)}\n{GetCurseDescription(curseType)}";

            if (curseType == CurseType.DoubleSpeedEveryone)
                buffAttributes[9] = 1;

            which = (int)curseType;

            // Give the curse the maximum duration.
            millisecondsDuration = int.MaxValue;
        }

        /// <summary>
        /// Spawns three cherry bombs within 3 tiles of the local player.
        /// </summary>
        public static void SpawnBombsAroundPlayer()
        {
            if (Game1.player.currentLocation is not MineShaft)
                return;

            MineShaft mine = (MineShaft)Game1.player.currentLocation;

            SObject bomb = new(286, 1);
            int bombsToSpawn = 3;
            int spawnPadding = 3;
            while (bombsToSpawn > 0)
            {
                Vector2 playerPosition = Game1.player.getTileLocation();
                Vector2 tileToSpawn = new(
                    playerPosition.X + Game1.random.Next(-spawnPadding, spawnPadding),
                    playerPosition.Y + Game1.random.Next(-spawnPadding, spawnPadding)
                );
                mine.isTileClearForMineObjects(tileToSpawn);
                tileToSpawn *= 64f;

                bomb.placementAction(mine, (int)tileToSpawn.X, (int)tileToSpawn.Y, Game1.player);
                bombsToSpawn--;
            }
        }

        /// <summary>
        /// Adjusts a passed monster based on the curses
        /// that are active in the game.
        /// </summary>
        /// <param name="monster">The monster to adjust.</param>
        public static void AdjustMonster(ref Monster monster)
        {
            if (AnyFarmerHasCurse(CurseType.MoreEnemiesLessHealth))
            {
                monster.MaxHealth = (int)Math.Round(monster.MaxHealth * .75f);
                monster.Health = monster.MaxHealth;
            }
            if (AnyFarmerHasCurse(CurseType.DoubleSpeedEveryone))
                monster.Speed *= 2;
        }

        public static void UpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            bool isMonsterArea = Game1.player.currentLocation is MineShaft mine && (bool)mine.GetType().GetProperty("isMonsterArea", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(mine);
            if (isMonsterArea && HasCurse(CurseType.BombsAroundPlayer) && Game1.shouldTimePass())
            {
                if (TicksToSpawnBombs > 0)
                {
                    TicksToSpawnBombs--;

                    if (TicksToSpawnBombs == 0)
                    {
                        SpawnBombsAroundPlayer();
                        TicksToSpawnBombs = Game1.random.Next(60 * 9, 60 * 12);
                    }
                }
            }

            //if (HasCurse(CurseType.GestureOfTheDrowned))
            //{
            //    if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is MeleeWeapon weapon && Game1.player.CanMove && !Game1.player.canOnlyWalk && !Game1.eventUp && !Game1.player.onBridge.Value)
            //        weapon.animateSpecialMove(Game1.player);
            //}

            if (!e.IsOneSecond)
                return;

            if (HasCurse(CurseType.DamageOverTime) && DOTDamageToTick > 0 && Game1.shouldTimePass())
            {
                int toRemove = Math.Max(3, DOTDamageToTick / 3);
                Game1.player.health -= toRemove;
                DOTDamageToTick = Math.Max(DOTDamageToTick - toRemove, 0);

                if (Game1.player.get_FarmerActiveHatQuest() is not null)
                    Game1.player.get_FarmerActiveHatQuest()!.DamageTaken += toRemove;

                if (Game1.player.health <= 0 && Game1.player.GetEffectsOfRingMultiplier(863) > 0 && !Game1.player.hasUsedDailyRevive.Value)
                {
                    Game1.player.startGlowing(new Color(255, 255, 0), border: false, 0.25f);
                    DelayedAction.functionAfterDelay(delegate
                    {
                        Game1.player.stopGlowing();
                    }, 500);
                    Game1.playSound("yoba");
                    for (int i = 0; i < 13; i++)
                    {
                        float xPos = Game1.random.Next(-32, 33);
                        Game1.player.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(114, 46, 2, 2), 200f, 5, 1, new Vector2(xPos + 32f, -96f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
                        {
                            attachedCharacter = Game1.player,
                            positionFollowsAttachedCharacter = true,
                            motion = new Vector2(xPos / 32f, -3f),
                            delayBeforeAnimationStart = i * 50,
                            alphaFade = 0.001f,
                            acceleration = new Vector2(0f, 0.1f)
                        });
                    }
                    Game1.player.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(157, 280, 28, 19), 2000f, 1, 1, new Vector2(-20f, -16f), flicker: false, flipped: false, 1E-06f, 0f, Color.White, 4f, 0f, 0f, 0f)
                    {
                        attachedCharacter = Game1.player,
                        positionFollowsAttachedCharacter = true,
                        alpha = 0.1f,
                        alphaFade = -0.01f,
                        alphaFadeFade = -0.00025f
                    });
                    Game1.player.health = (int)Math.Min(Game1.player.maxHealth, Game1.player.maxHealth * 0.5f + Game1.player.GetEffectsOfRingMultiplier(863));
                    Game1.player.hasUsedDailyRevive.Value = true;
                    DOTDamageToTick = 0;
                }
            }

            if (HasCurse(CurseType.HealOverTime) && HOTHealToTick > 0 && Game1.shouldTimePass())
            {
                int toAdd = Math.Max(3, HOTHealToTick / 3);
                Game1.player.health = Math.Min(Game1.player.health + toAdd, Game1.player.maxHealth);
                HOTHealToTick = Math.Max(0, HOTHealToTick - toAdd);

                if (Game1.player.get_FarmerActiveHatQuest() is not null)
                    Game1.player.get_FarmerActiveHatQuest()!.HealthHealed += toAdd;
            }
        }

        // Only works for Game1.player
        public static bool HasCurse(CurseType curseType)
        {
            return Game1.buffsDisplay.otherBuffs.Any(b => b.which == (int)curseType);
        }

        public static bool HasAnyCurse()
        {
            return Game1.player.get_FarmerCurses().Count > 0;
        }

        public static bool HasAllCurses()
        {
            return Game1.player.get_FarmerCurses().Count == Enum.GetValues<CurseType>().Length;
        }

        public static bool AnyFarmerHasCurse(CurseType curseType)
        {
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                if (farmer.get_FarmerCurses().Contains((int)curseType))
                    return true;
            }

            return false;
        }

        public static CurseType? GetRandomUniqueCurse(Random random = null)
        {
            random ??= Game1.random;

            List<CurseType> curseTypes = Enum.GetValues<CurseType>().ToList();
            curseTypes = curseTypes.Where(c => !HasCurse(c)).ToList();

            if (curseTypes.Count == 0)
                return null;

            return curseTypes[random.Next(curseTypes.Count)];
        }

        public static void RemoveRandomCurse()
        {
            int totalCurses = Game1.player.get_FarmerCurses().Count;
            if (totalCurses == 0)
                return;

            CurseType randomCurse = (CurseType)Game1.player.get_FarmerCurses().ElementAt(Game1.random.Next(totalCurses));
            RemoveCurse(randomCurse);
        }

        public static bool AddCurse(CurseType curseType)
        {
            if (HasCurse(curseType))
                return true;

            switch (curseType)
            {
                case CurseType.BootsBetterDefense:
                case CurseType.BootsBetterImmunity:
                    if (Game1.player.boots.Value != null)
                    {
                        Game1.player.boots.Value.reloadData();
                        Game1.player.boots.Value.onUnequip();
                    }
                    break;
                default:
                    break;
            }

            Curse curse = new(curseType);
            Game1.player.get_FarmerCurses().Add((int)curseType);
            bool result = Game1.buffsDisplay.addOtherBuff(curse);

            switch (curseType)
            {
                case CurseType.GlassCannon:
                    Game1.player.enchantments.Add(new GlassCannonEnchantment());
                    break;
                case CurseType.DamageOverTime:
                    DOTDamageToTick = 0;
                    break;
                case CurseType.HealOverTime:
                    HOTHealToTick = 0;
                    break;
                case CurseType.BootsBetterDefense:
                case CurseType.BootsBetterImmunity:
                    if (Game1.player.boots.Value != null)
                    {
                        Game1.player.boots.Value.reloadData();
                        Game1.player.boots.Value.onEquip();
                    }
                    break;
                case CurseType.BuffsMorePotent:
                    if (Game1.buffsDisplay.food is not null)
                    {
                        Game1.player.removeBuffAttributes(Game1.buffsDisplay.food.buffAttributes);
                        Game1.buffsDisplay.food.millisecondsDuration = PotentBuffDuration;
                        for (int i = 0; i < Game1.buffsDisplay.food.buffAttributes.Length; i++)
                        {
                            if (Game1.buffsDisplay.food.buffAttributes[i] > 0)
                                Game1.buffsDisplay.food.buffAttributes[i] += 1;
                        }
                        Game1.player.addBuffAttributes(Game1.buffsDisplay.food.buffAttributes);
                    }
                    if (Game1.buffsDisplay.drink is not null)
                    {
                        Game1.player.removeBuffAttributes(Game1.buffsDisplay.drink.buffAttributes);
                        Game1.buffsDisplay.drink.millisecondsDuration = PotentBuffDuration;
                        for (int i = 0; i < Game1.buffsDisplay.drink.buffAttributes.Length; i++)
                        {
                            if (Game1.buffsDisplay.drink.buffAttributes[i] > 0)
                                Game1.buffsDisplay.drink.buffAttributes[i] += 1;
                        }
                        Game1.player.addBuffAttributes(Game1.buffsDisplay.drink.buffAttributes);
                    }
                    Game1.buffsDisplay.syncIcons();
                    break;
                default:
                    break;
            }

            return result;
        }

        public static bool RemoveCurse(CurseType curseType)
        {
            if (!HasCurse(curseType))
                return true;

            switch (curseType)
            {
                case CurseType.GlassCannon:
                    GlassCannonEnchantment toRemove = null;
                    foreach (BaseEnchantment enchantment in Game1.player.enchantments)
                    {
                        if (enchantment is GlassCannonEnchantment glassCannon)
                        {
                            toRemove = glassCannon;
                            break;
                        }
                    }
                    if (toRemove is not null)
                        Game1.player.enchantments.Remove(toRemove);
                    break;
                case CurseType.DamageOverTime:
                    DOTDamageToTick = 0;
                    break;
                case CurseType.HealOverTime:
                    HOTHealToTick = 0;
                    break;
                case CurseType.BootsBetterDefense:
                case CurseType.BootsBetterImmunity:
                    if (Game1.player.boots.Value != null)
                    {
                        Game1.player.boots.Value.onUnequip();
                    }
                    break;
                default:
                    break;
            }

            Game1.player.get_FarmerCurses().Remove((int)curseType);
            bool result = Game1.buffsDisplay.removeOtherBuff((int)curseType);

            switch (curseType)
            {
                case CurseType.BootsBetterDefense:
                case CurseType.BootsBetterImmunity:
                    if (Game1.player.boots.Value != null)
                    {
                        Game1.player.boots.Value.reloadData();
                        Game1.player.boots.Value.onEquip();
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        public static void RemoveAllCurses()
        {
            foreach (CurseType curseType in Enum.GetValues(typeof(CurseType)))
                RemoveCurse(curseType);
        }
    }
}
