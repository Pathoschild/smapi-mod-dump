/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale.Utils
{
    public enum DamageSource : int
    {
        PLAYER,
        MONSTER,
        WORLD,
        THORNS,
        STORM
    };

    internal class FarmerUtils
    {
        public static bool canBeKnockedBack = true;

        private static readonly int knockbackImmunityMilliseconds = 2500;

        private static readonly int maxHealth = 100;

        private static int axePosition = 0;

        private static readonly string[] unlockedLocations = new string[]
        {
            "OpenedSewer", "wizardJunimoNote", "ccDoorUnlock", "guildMember", "krobusUnseal", "communityUpgradeShortcuts",
            "Island_Resort", "Island_Turtle", "Island_FirstParrot", "addedParrotBoy", "Visited_Island", "Island_UpgradeHouse",
            "Island_UpgradeBridge", "CalderaTreasure"
        };

        private static readonly string[] npcsToIgnore = new string[]
        {
            "Krobus", "Dwarf", "Sandy", "Kent"
        };

        public static readonly int[] randomRoundMeleeWeapons = new int[]
        {
            0, 1, 15, 13, 5, 10, 7, 2, 60, 28, 17, 22, 18, 21, 19,
            51, 45, 23, 56, 31, 24, 27, 26, 46
        };

        public static readonly int[] randomRoundFood = new int[]
        {
            194, 196, 198, 200, 202, 203, 204, 206, 213, 224, 210,
            214, 220, 221, 223, 225, 231, 234, 236, 238, 357, 732,
            905, 729, 611, 605, 240, 239
        };

        public static readonly int[] randomRoundFoodRange = new int[2] { 1, 3 };

        public static readonly int[] randomRoundRings = new int[]
        {
            524, 810, 861, 863, 529, 533, 534, 839
        };

        public static readonly int[] randomRoundBoots = new int[]
        {
            504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514,
            515, 804, 806, 853, 854, 855, 878
        };

        public static void TakeDamage(Farmer who, DamageSource source, int damage, long? damagerID = null, string monster = "")
        {
            if (!ModEntry.BRGame.InProgress)
                damage = 0;

            if (who != Game1.player)
            {
                NetworkUtils.SendDamageToPlayer(who, source, damage, damagerID);
                return;
            }

            if (IsDying(Game1.player))
                return;

            Round round = ModEntry.BRGame.GetActiveRound();

            Game1.currentMinigame = null;

            Farmer damager = null;
            if (damagerID != null)
                damager = Game1.getFarmer((long)damagerID);

            if (damager != null && Game1.player != damager && IsOnSameTeamAs(Game1.player, damager))
                return;

            if (source != DamageSource.MONSTER)
            {
                bool oldIsEating = Game1.player.isEating;
                Game1.player.isEating = false; // Prevent invincibility
                Game1.player.takeDamage(source, damage, false, damager);
                Game1.player.isEating = oldIsEating;
            }

            bool wasInvincible = Game1.player.temporarilyInvincible;

            //Hit shake timer / Invincibility frames
            if (source != DamageSource.THORNS && !round.IsSpecialRoundType(SpecialRoundType.SLUGFEST) && !Game1.player.temporarilyInvincible)
            {
                int invincibilityDuration = 1200;
                int multiplier = Game1.player.GetEffectsOfRingMultiplier(861);
                invincibilityDuration += 1000 * multiplier;

                NetworkMessage.Send(
                    NetworkUtils.MessageTypes.TELL_PLAYER_HIT_SHAKE_TIMER,
                    NetworkMessageDestination.ALL,
                    new List<object>() { invincibilityDuration }
                );
            }

            if (Game1.player.health <= 0)
            {
                ResetStatistics();

                Game1.player.CanMove = false;
                NetworkMessage.Send(
                    NetworkUtils.MessageTypes.SEND_DEATH_ANIMATION,
                    NetworkMessageDestination.ALL,
                    new List<object>()
                );

                DelayedAction.functionAfterDelay(() =>
                {
                    Random random = new Random();

                    //Spawn their items onto the floor
                    foreach (var item in Game1.player.Items)
                    {
                        if (item != null)
                            if (!(item is Tool) || item is MeleeWeapon || item is Slingshot)
                            {
                                Debris debris = new Debris(item, Game1.player.getStandingPosition(), Game1.player.getStandingPosition() + new Vector2(64 * (float)(random.NextDouble() * 2 - 1), 64 * (float)(random.NextDouble() * 2 - 1)));
                                Game1.currentLocation.debris.Add(debris);
                            }
                    }

                    var oldLocation = Game1.player.currentLocation;
                    var oldPosition = new xTile.Dimensions.Location(
                                (int)Game1.player.Position.X - Game1.viewport.Width / 2,
                                (int)Game1.player.Position.Y - Game1.viewport.Height / 2);

                    ClearInventory();

                    SpectatorMode.EnterSpectatorMode(oldLocation, oldPosition);

                    NetworkUtils.AnnounceClientDeath(source, monster, damagerID);
                }, 1800);
            }
            else if (source == DamageSource.PLAYER && damage > 0 && canBeKnockedBack && damager != null && !wasInvincible) // Knockback
            {
                double amount = 10 + 8 * (-1 + 2 / (1 + Math.Pow(Math.E, -0.03 * damage)));
                amount = Math.Min(18, amount); //Just in case

                if (damager.leftRing.Value != null && damager.leftRing.Value.GetsEffectOfRing(529))
                    amount *= 1.5;
                if (damager.rightRing.Value != null && damager.rightRing.Value.GetsEffectOfRing(529))
                    amount *= 1.5;

                Vector2 displacement = Vector2.Subtract(Game1.player.Position, damager.Position);
                if (displacement.LengthSquared() != 0)
                {
                    displacement.Normalize();

                    displacement.Y *= -1;
                    displacement = Vector2.Multiply(displacement, (float)amount);

                    Game1.player.setTrajectory((int)displacement.X, (int)displacement.Y);
                }
            }
        }

        public static bool IsDying(Farmer who)
        {
            FarmerSprite sprite = who.FarmerSprite;
            int currentAnimaton = ModEntry.BRGame.Helper.Reflection.GetField<int>(sprite, "currentSingleAnimation").GetValue();

            return Patches.CustomDeathAnimation.customAnimations.Contains(currentAnimaton);
        }

        public static bool IsOnline(Farmer who)
        {
            if (who == Game1.player)
                return true;

            foreach (IMultiplayerPeer peer in ModEntry.BRGame.Helper.Multiplayer.GetConnectedPlayers())
            {
                if (peer.PlayerID == who.UniqueMultiplayerID)
                    return true;
            }

            return false;
        }
        public static void PlayDeathAnimation(Farmer who)
        {
            who.faceDirection(2);
            who.completelyStopAnimatingOrDoingAction();
            who.CanMove = false;

            who.FarmerSprite.animateOnce(5555, 100f, 3);
        }

        public static void AddKnockbackImmunity()
        {
            canBeKnockedBack = false;
            DelayedAction.functionAfterDelay(() =>
            {
                canBeKnockedBack = true;
            }, knockbackImmunityMilliseconds);
        }

        public static void InsertMail(string mailName = "")
        {
            if (mailName != "")
            {
                if (!Game1.player.mailReceived.Contains(mailName))
                    Game1.player.mailReceived.Add(mailName);

                return;
            }

            Game1.mailbox?.Clear();

            //Open locations
            foreach (var mailPiece in unlockedLocations)
            {
                if (!Game1.player.mailReceived.Contains(mailPiece))
                    Game1.player.mailReceived.Add(mailPiece);
            }
        }

        public static void ResetStatistics()
        {
            // Reset statistics
            Game1.player.maxHealth = maxHealth;
            Game1.player.health = maxHealth;
            Game1.player.Stamina = Game1.player.MaxStamina;

            Game1.player.ClearBuffs();

            Game1.player.swimming.Value = false;
            Game1.player.changeOutOfSwimSuit();

            Game1.player.MaxItems = 12;
            Game1.player.hasUsedDailyRevive.Value = false;
            Game1.getFarm().lastItemShipped = null;

            Game1.player.questLog.Clear();
            Game1.player.hasDarkTalisman = true; // Removes the chest in bugland
        }

        public static void ClearInventory()
        {
            // Remove items
            if (Game1.player.leftRing.Value != null)
            {
                Ring ring = Game1.player.leftRing.Value;
                ring.onUnequip(Game1.player, Game1.player.currentLocation);
            }

            if (Game1.player.rightRing.Value != null)
            {
                Ring ring = Game1.player.rightRing.Value;
                ring.onUnequip(Game1.player, Game1.player.currentLocation);
            }

            if (Game1.player.boots.Value != null)
            {
                Boots boots = Game1.player.boots.Value;
                boots.onUnequip();
            }

            Game1.player.leftRing.Set(null);
            Game1.player.rightRing.Set(null);
            Game1.player.boots.Set(null);
            Game1.player.hat.Value = null;
            Game1.player.CursorSlotItem = null;

            for (int i = 0; i < Game1.player.items.Count; i++)
            {
                if (Game1.player.items[i] is Axe)
                    axePosition = i;
                Game1.player.items[i] = null;
            }
        }

        public static void SetNPCRelations()
        {
            foreach (GameLocation gameLocation in Game1.locations)
            {
                foreach (NPC npc in gameLocation.characters.Where(x => !npcsToIgnore.Contains(x.Name)))
                {
                    Game1.player.hasPlayerTalkedToNPC(npc.Name); // Creates friendship entries if they have never met before
                    if (Game1.player.friendshipData.ContainsKey(npc.Name))
                        Game1.player.friendshipData[npc.Name].Points = 250 * 10; // Each heart is 250 points
                }
            }
        }

        public static void DismountHorse()
        {
            // Dismount
            var horse = Game1.player.mount;
            if (horse != null)
            {
                horse.dismount();
                Game1.currentLocation.characters.Remove(horse);
                Game1.player.mount = null;
            }
        }

        public static void SetupInventory()
        {
            Game1.player.items[axePosition] = new Axe() { UpgradeLevel = 3 };

            // Setup crafting recipes
            Game1.player.craftingRecipes.Clear();
            Game1.player.craftingRecipes.Add("Gate", 0);
            Game1.player.craftingRecipes.Add("Wood Fence", 0);
            Game1.player.craftingRecipes.Add("Hardwood Fence", 0);
            Game1.player.craftingRecipes.Add("Chest", 0);
        }

        public static string GetTeamName(Farmer who)
        {
            if (who.hat.Value == null)
                return "";

            int hatId = who.hat.Value.which.Value;
            if (hatId == Game.RedTeamHatId)
                return "red";
            else if (hatId == Game.GreenTeamHatId)
                return "green";
            else if (hatId == Game.BlueTeamHatId)
                return "blue";

            return "";
        }

        public static void SetTeam(string team)
        {
            if (team == "red")
                Game1.player.changeHat(Game.RedTeamHatId);
            else if (team == "green")
                Game1.player.changeHat(Game.GreenTeamHatId);
            else if (team == "blue")
                Game1.player.changeHat(Game.BlueTeamHatId);
        }

        public static bool IsOnSameTeamAs(Farmer source, Farmer other)
        {
            string sourceTeam = GetTeamName(source);
            string otherTeam = GetTeamName(other);
            return sourceTeam == otherTeam && sourceTeam != "";
        }

        public static void SetupSpecialRound()
        {
            if (ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.TRAVELLING_CART))
            {
                int weaponId = randomRoundMeleeWeapons[Game1.random.Next(randomRoundMeleeWeapons.Count())];
                MeleeWeapon weapon = new MeleeWeapon(weaponId);

                int foodId = randomRoundFood[Game1.random.Next(randomRoundFood.Count())];
                int foodAmount = Game1.random.Next(randomRoundFoodRange[0], randomRoundFoodRange[1] + 1);

                StardewValley.Object food = new StardewValley.Object(foodId, foodAmount);

                int ringId = randomRoundRings[Game1.random.Next(randomRoundRings.Count())];
                Ring ring = new Ring(ringId);

                int bootId = randomRoundBoots[Game1.random.Next(randomRoundBoots.Count())];
                Boots boots = new Boots(bootId);

                Game1.player.addItemToInventory(weapon);
                Game1.player.addItemToInventory(food);
                Game1.player.leftRing.Value = ring;
                Game1.player.boots.Value = boots;
                boots.onEquip();
            }
            else if (ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.SLINGSHOT_ONLY))
            {
                Slingshot slingshot = new Slingshot(32);

                StardewValley.Object wood = new StardewValley.Object(388, 100);
                StardewValley.Object stone = new StardewValley.Object(390, 50);
                StardewValley.Object copper = new StardewValley.Object(378, 20);

                Game1.player.addItemToInventory(slingshot);
                Game1.player.addItemToInventory(wood);
                Game1.player.addItemToInventory(stone);
                Game1.player.addItemToInventory(copper);
            }
            else if (ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.BOMBS_ONLY))
            {
                StardewValley.Object bomb = new StardewValley.Object(287, 50);
                Game1.player.addItemToInventory(bomb);
            }
        }

        public static void ResetPlayer()
        {
            InsertMail();
            ResetStatistics();
            ClearInventory();
            SetNPCRelations();
            DismountHorse();
            SetupInventory();
        }
    }
}
