/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/namelessto/EnchantedGalaxyWeapons
**
*************************************************/

using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Constants;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace EnchantedGalaxyWeapons
{
    internal class CustomBreakableObject : Object
    {
        /*********
        ** Properties
        *********/

        private readonly List<int> StepsLevel = new() { 20, 40, 60, 80, 100, 120 };


        [XmlElement("debris")]
        private readonly int debris;

        [XmlElement("health")]
        private new int health;

        [XmlElement("hitSound")]
        private readonly string hitSound = "woodWhack";

        [XmlElement("breakSound")]
        private readonly string breakSound = "barrelBreak";

        [XmlElement("breakDebrisSource")]
        private readonly NetRectangle breakDebrisSource = new();

        [XmlElement("breakDebrisSource2")]
        private readonly NetRectangle breakDebrisSource2 = new();

        /// <summary>
        /// Constructors
        /// </summary>
        public CustomBreakableObject() { }
        public CustomBreakableObject(Vector2 tile, string itemId, int health = 3, int debrisType = 12, string hitSound = "woodWhack", string breakSound = "barrelBreak")
    : base(tile, itemId)
        {
            this.health = health;
            this.debris = debrisType;
            this.hitSound = hitSound;
            this.breakSound = breakSound;
            this.breakDebrisSource.Value = new Rectangle(598, 1275, 13, 4);
            this.breakDebrisSource2.Value = new Rectangle(611, 1275, 10, 4);
        }

        /// <summary>
        /// Pretty much the same as vanilla code but return CustomBreakableObject
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="mine"></param>
        /// <returns></returns>
        public static CustomBreakableObject GetBarrelForMines(Vector2 tile, MineShaft mine)
        {
            int mineArea = mine.getMineArea();
            string itemId = ((mine.GetAdditionalDifficulty() > 0) ? (((mineArea == 0 || mineArea == 10) && !mine.isDarkArea()) ? "262" : "118") : (mineArea switch
            {
                40 => "120",
                80 => "122",
                121 => "124",
                _ => "118",
            }));
            CustomBreakableObject barrel = new(tile, itemId);
            if (Game1.random.NextBool())
            {
                barrel.showNextIndex.Value = true;
            }
            return barrel;
        }

        /// <summary>
        /// Same as vanilla code but without shakeTimer and calls to ReleaseContents in this class
        /// </summary>
        /// <param name="t"></param>
        public override bool performToolAction(Tool t)
        {
            GameLocation location = this.Location;
            if (location == null)
            {
                return false;
            }
            if (t != null && t.isHeavyHitter())
            {
                this.health--;
                if (t is MeleeWeapon weapon && weapon.type.Value == 2)
                {
                    this.health--;
                }
                if (this.health <= 0)
                {
                    if (this.breakSound != null)
                    {
                        base.playNearbySoundAll(this.breakSound);
                    }
                    this.ReleaseContents(t.getLastFarmerToUse());

                    location.objects.Remove(base.TileLocation);
                    int numDebris = Game1.random.Next(4, 12);
                    Color chipColor = this.GetChipColor();
                    for (int i = 0; i < numDebris; i++)
                    {
                        Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", Game1.random.NextBool() ? this.breakDebrisSource.Value : this.breakDebrisSource2.Value, 999f, 1, 0, base.TileLocation * 64f + new Vector2(32f, 32f), flicker: false, Game1.random.NextBool(), (base.TileLocation.Y * 64f + 32f) / 10000f, 0.01f, chipColor, 4f, 0f, (float)Game1.random.Next(-5, 6) * (float)Math.PI / 8f, (float)Game1.random.Next(-5, 6) * (float)Math.PI / 64f)
                        {
                            motion = new Vector2((float)Game1.random.Next(-30, 31) / 10f, Game1.random.Next(-10, -7)),
                            acceleration = new Vector2(0f, 0.3f)
                        });
                    }
                }
                else if (this.hitSound != null)
                {
                    base.playNearbySoundAll(this.hitSound);
                    Color? debrisColor = ((base.ItemId == "120") ? new Color?(Color.White) : null);
                    Game1.createRadialDebris(location, this.debris, (int)base.TileLocation.X, (int)base.TileLocation.Y, Game1.random.Next(4, 7), resource: false, -1, item: false, debrisColor);
                }
            }
            return false;
        }

        /// <summary>
        /// Same as vanilla code but calls to ReleaseContents in this class
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool onExplosion(Farmer who)
        {
            who ??= Game1.player;
            GameLocation location = this.Location;
            if (location == null)
            {
                return true;
            }
            this.ReleaseContents(who);
            int numDebris = Game1.random.Next(4, 12);
            Color chipColor = this.GetChipColor();
            for (int i = 0; i < numDebris; i++)
            {
                Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", Game1.random.NextBool() ? this.breakDebrisSource.Value : this.breakDebrisSource2.Value, 999f, 1, 0, base.TileLocation * 64f + new Vector2(32f, 32f), flicker: false, Game1.random.NextBool(), (base.TileLocation.Y * 64f + 32f) / 10000f, 0.01f, chipColor, 4f, 0f, (float)Game1.random.Next(-5, 6) * (float)Math.PI / 8f, (float)Game1.random.Next(-5, 6) * (float)Math.PI / 64f)
                {
                    motion = new Vector2((float)Game1.random.Next(-30, 31) / 10f, Game1.random.Next(-10, -7)),
                    acceleration = new Vector2(0f, 0.3f)
                });
            }
            return true;
        }

        /// <summary>
        /// Change the drop of the barrels to be for the galaxy and infinity weapons.
        /// Allows also to drop Qi beans, Qi gems and special items like books.
        /// Better chance to have a drop with higher mineLevel
        /// </summary>
        /// <param name="who"></param>
        public void ReleaseContents(Farmer who)
        {
            GameLocation location = this.Location;
            if (location == null)
            {
                return;
            }

            if (who == null)
            {
                return;
            }
            Random r = Utility.CreateRandom(base.TileLocation.X, (double)base.TileLocation.Y * 10000.0, Game1.stats.DaysPlayed, (location as MineShaft)?.mineLevel ?? 0);

            int x = (int)base.TileLocation.X;
            int y = (int)base.TileLocation.Y;
            int mineLevel = -1;
            int difficultyLevel = 0;

            if (location is MineShaft mine)
            {
                mineLevel = mine.mineLevel;
                if (mine.isContainerPlatform(x, y))
                {
                    mine.updateMineLevelData(0, -1);
                }
                difficultyLevel = mine.GetAdditionalDifficulty();

                if (mine.mineLevel > 120 && !mine.isSideBranch())
                {
                    int floor = mine.mineLevel - 121;
                    if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0)
                    {
                        float chance = (float)(floor + Game1.player.team.calicoEggSkullCavernRating.Value * 2) * 0.003f;
                        if (chance > 0.33f)
                        {
                            chance = 0.33f;
                        }
                        if (r.NextBool(chance))
                        {
                            Game1.createMultipleObjectDebris("CalicoEgg", x, y, r.Next(1, 4), who.UniqueMultiplayerID, location);
                        }
                    }
                }
            }

            if (r.NextDouble() <= 0.05 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
            {
                Game1.createMultipleObjectDebris("(O)890", x, y, r.Next(1, 3), who.UniqueMultiplayerID, location);
            }
            if (Utility.tryRollMysteryBox(0.0081 + Game1.player.team.AverageDailyLuck() / 15.0, r))
            {
                Game1.createItemDebris(ItemRegistry.Create((Game1.player.stats.Get(StatKeys.Mastery(2)) != 0) ? "(O)GoldenMysteryBox" : "(O)MysteryBox"), new Vector2(x, y) * 64f + new Vector2(32f), -1, location);
            }
            double maxSucceedChance = ModEntry.Config.BaseSpawnChance;

            Utility.trySpawnRareObject(who, new Vector2(x, y) * 64f, location, 1.5, 1.0, -1, r);

            if (difficultyLevel > 0)
            {
                maxSucceedChance += 0.1;
                if (!(r.NextDouble() < 0.15))
                {
                    if (r.NextDouble() < 0.008)
                    {
                        Game1.createMultipleObjectDebris("(O)858", x, y, 1, location);
                    }
                }
            }
            if (!ModEntry.Config.HaveGlobalChance)
            {
                foreach (var level in StepsLevel)
                {
                    if (mineLevel >= level)
                    {
                        maxSucceedChance += ModEntry.Config.IncreaseSpawnChanceStep;
                    }
                }
            }

            double dropChance = r.NextDouble();
            if (dropChance <= maxSucceedChance)
            {
                MeleeWeapon weapon = GenerateWeapon(r);
                Game1.createItemDebris(weapon, new Vector2(x, y) * 64f + new Vector2(32f), r.Next(4), Game1.currentLocation);
            }
            ModEntry.MaxSpawnForDay--;

            if (ModEntry.MaxSpawnForDay == 0)
            {
                HUDMessage message = HUDMessage.ForCornerTextbox(ModEntry.ModInstance.Helper.Translation.Get("game.aura-left"));
                Game1.addHUDMessage(message);
            }
            return;
        }

        /// <summary>
        /// Create a weapon for weaponsIDS list and try to give Innate Enchantment
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private MeleeWeapon GenerateWeapon(Random r)
        {
            List<string> weaponsIDs = getWeaponsToChoose();
            List<BaseEnchantment> enchantments;
            int itemID = 0;
            if (ModEntry.Config.SkipInfinityCheck || ModEntry.UnlockedInfinity)
            {
                itemID = r.Next(weaponsIDs.Count);
            }
            else if (!ModEntry.Config.SkipInfinityCheck && !ModEntry.UnlockedInfinity)
            {
                itemID = r.Next(weaponsIDs.Count / 2);
            }

            MeleeWeapon weapon = new(weaponsIDs[itemID]);
            if (ModEntry.Config.KeepVanilla)
            {
                weapon = (MeleeWeapon)MeleeWeapon.attemptAddRandomInnateEnchantment(weapon, r, force: ModEntry.Config.ForceInnateEnchantment);
            }
            else
            {
                enchantments = getInnateEnchentments();
                weapon = (MeleeWeapon)attemptAddInnateEnchantment(weapon, r, enchantments, force: ModEntry.Config.ForceInnateEnchantment);
            }

            if (r.NextDouble() <= ModEntry.Config.ChanceForEnchantment || ModEntry.Config.ForceHaveEnchantment)
            {
                enchantments = getEnchentments();
                if (enchantments.Count > 0)
                {
                    int enchantmentIndex = r.Next(enchantments.Count);
                    weapon.AddEnchantment(enchantments[enchantmentIndex]);
                }
            }
            return weapon;
        }

        private List<string> getWeaponsToChoose()
        {
            List<string> weaponIDS = new();
            if (ModEntry.Config.AllowGalSword)
            {
                weaponIDS.Add("4");
            }
            if (ModEntry.Config.AllowGalDagger)
            {
                weaponIDS.Add("23");
            }
            if (ModEntry.Config.AllowGalHammer)
            {
                weaponIDS.Add("29");
            }
            if (ModEntry.Config.AllowInfSword)
            {
                weaponIDS.Add("62");
            }
            if (ModEntry.Config.AllowInfHammer)
            {
                weaponIDS.Add("63");
            }
            if (ModEntry.Config.AllowInfDagger)
            {
                weaponIDS.Add("64");
            }

            return weaponIDS;
        }

        private List<BaseEnchantment> getEnchentments()
        {
            List<BaseEnchantment> enchantments = new();
            if (ModEntry.Config.AllowArtful)
            {
                enchantments.Add(new ArtfulEnchantment());
            }
            if (ModEntry.Config.AllowBugKiller)
            {
                enchantments.Add(new BugKillerEnchantment());
            }
            if (ModEntry.Config.AllowCrusader)
            {
                enchantments.Add(new CrusaderEnchantment());
            }
            if (ModEntry.Config.AllowHaymaker)
            {
                enchantments.Add(new HaymakerEnchantment());
            }
            if (ModEntry.Config.AllowVampiric)
            {
                enchantments.Add(new VampiricEnchantment());
            }

            return enchantments;
        }

        private List<BaseEnchantment> getInnateEnchentments()
        {
            List<BaseEnchantment> enchantments = new();
            if (ModEntry.Config.AllowDefense)
            {
                enchantments.Add(new DefenseEnchantment());
            }
            if (ModEntry.Config.AllowWeight)
            {
                enchantments.Add(new LightweightEnchantment());
            }
            if (ModEntry.Config.AllowSlimeGatherer)
            {
                enchantments.Add(new SlimeGathererEnchantment());
            }
            if (ModEntry.Config.AllowSlimeSlayer)
            {
                enchantments.Add(new SlimeSlayerEnchantment());
            }
            if (ModEntry.Config.AllowCritPow)
            {
                enchantments.Add(new CritPowerEnchantment());
            }
            if (ModEntry.Config.AllowCritChance)
            {
                enchantments.Add(new CritEnchantment());
            }
            if (ModEntry.Config.AllowAttack)
            {
                enchantments.Add(new AttackEnchantment());
            }
            if (ModEntry.Config.AllowSpeed)
            {
                enchantments.Add(new WeaponSpeedEnchantment());
            }

            return enchantments;
        }

        public static Item attemptAddInnateEnchantment(Item item, Random r, List<BaseEnchantment> enchantsToReroll, bool force = false)
        {
            if (enchantsToReroll.Count <= 0)
            {
                return item;
            }

            if (r == null)
            {
                r = Game1.random;
            }

            if ((r.NextDouble() < ModEntry.Config.ChanceForInnate || force) && item is MeleeWeapon meleeWeapon)
            {
                int max = Math.Min(enchantsToReroll.Count, ModEntry.Config.MaxInnateEnchantments);
                int min = Math.Min(max, ModEntry.Config.MinInnateEnchantments);
                int enchantmentsToSelect = r.Next(min, max);
                int itemLevel = meleeWeapon.getItemLevel();
                int n = enchantsToReroll.Count;

                for (int j = n - 1; j > 0; j--)
                {
                    int k = r.Next(j + 1);
                    BaseEnchantment value = enchantsToReroll[k];
                    enchantsToReroll[k] = enchantsToReroll[j];
                    enchantsToReroll[j] = value;
                }

                for (int i = 0; i < enchantmentsToSelect; i++)
                {
                    switch (enchantsToReroll[i])
                    {
                        case DefenseEnchantment:
                            meleeWeapon.AddEnchantment(new DefenseEnchantment
                            {
                                Level = Math.Max(1, Math.Min(2, r.Next(itemLevel + 1) / 2 + 1))
                            });
                            break;
                        case LightweightEnchantment:
                            meleeWeapon.AddEnchantment(new LightweightEnchantment
                            {
                                Level = r.Next(1, 6)
                            });
                            break;
                        case SlimeGathererEnchantment:
                            meleeWeapon.AddEnchantment(new SlimeGathererEnchantment());
                            break;
                        case AttackEnchantment:
                            meleeWeapon.AddEnchantment(new AttackEnchantment
                            {
                                Level = Math.Max(1, Math.Min(5, r.Next(itemLevel + 1) / 2 + 1))
                            });
                            break;
                        case CritEnchantment:
                            meleeWeapon.AddEnchantment(new CritEnchantment
                            {
                                Level = Math.Max(1, Math.Min(3, r.Next(itemLevel) / 3))
                            });
                            break;
                        case WeaponSpeedEnchantment:
                            meleeWeapon.AddEnchantment(new WeaponSpeedEnchantment
                            {
                                Level = Math.Max(1, Math.Min(Math.Max(1, 4 - meleeWeapon.speed.Value), r.Next(itemLevel)))
                            });
                            break;
                        case SlimeSlayerEnchantment:
                            meleeWeapon.AddEnchantment(new SlimeSlayerEnchantment());
                            break;
                        case CritPowerEnchantment:
                            meleeWeapon.AddEnchantment(new CritPowerEnchantment
                            {
                                Level = Math.Max(1, Math.Min(3, r.Next(itemLevel) / 3))
                            });
                            break;
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// Same as vanilla
        /// </summary>
        /// <returns></returns>
        public Color GetChipColor()
        {
            return base.ItemId switch
            {
                "120" => Color.White,
                "122" => new Color(109, 122, 80),
                "174" => new Color(107, 76, 83),
                _ => new Color(130, 80, 30),
            };
        }
    }
}
