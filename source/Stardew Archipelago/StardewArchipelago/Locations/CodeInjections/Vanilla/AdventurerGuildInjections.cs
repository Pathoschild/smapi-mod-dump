/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class AdventurerGuildInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago,
            LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public virtual bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
        public static bool TelephoneAdventureGuild_AddReceivedWeapons_Prefix(GameLocation __instance, string questionAndAnswer, string[] questionParams, bool __result)
        {
            try
            {
                if (questionAndAnswer != "telephone_AdventureGuild")
                {
                    return true; // run original logic
                }

                var farmer = Game1.player;

                foreach (var mail in Game1.player.mailReceived)
                {
                    if (!MailKey.TryParse(mail, out var mailKey))
                    {
                        continue;
                    }

                    var apActionName = mailKey.LetterOpenedAction;

                    if (apActionName != LetterActionsKeys.GiveMeleeWeapon)
                    {
                        continue;
                    }

                    var weaponIdStr = mailKey.ActionParameter;
                    var weaponId = int.Parse(weaponIdStr);
                    var weapon = new MeleeWeaponToRecover(weaponId);

                    if (farmer.itemsLostLastDeath.Any(x => x.Name == weapon.Name))
                    {
                        continue;
                    }

                    weapon.isLostItem = true;
                    farmer.itemsLostLastDeath.Add(weapon);
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(TelephoneAdventureGuild_AddReceivedWeapons_Prefix)}:\n{ex}",
                    LogLevel.Error);
                throw;
            }
        }


        public static bool GetAdventureShopStock_ShopBasedOnReceivedItems_Prefix(
            ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                var adventureShopStock = new Dictionary<ISalable, int[]>();
                AddWeapons(adventureShopStock);
                AddShoes(adventureShopStock);
                AddRings(adventureShopStock);
                AddSlingshots(adventureShopStock);
                AddAmmo(adventureShopStock);
                AddGilRewards(adventureShopStock);

                __result = adventureShopStock;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetAdventureShopStock_ShopBasedOnReceivedItems_Prefix)}:\n{ex}",
                    LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void AddWeapons(Dictionary<ISalable, int[]> adventureShopStock)
        {
            var weapons = new[]
            {
                WoodenBlade, IronDirk, WindSpire, Femur, SteelSmallsword, WoodClub, ElfBlade, SilverSaber, PirateSword,
                CrystalDagger, Cutlass, IronEdge, BurglarsShank, WoodMallet, Claymore, TemplarsBlade, Kudgel,
                ShadowDagger, ObsidianEdge, TemperedBroadsword, WickedKris, BoneSword, OssifiedBlade, SteelFalchion,
                TheSlammer, LavaKatana, GalaxySword, GalaxyDagger, GalaxyHammer
            };
            AddItemsToShop(adventureShopStock, weapons);
        }

        private static void AddShoes(Dictionary<ISalable, int[]> adventureShopStock)
        {
            var shoes = new[]
            {
                Sneakers, LeatherBoots, WorkBoots, TundraBoots, ThermalBoots, CombatBoots, FirewalkerBoots, DarkBoots,
                SpaceBoots, CrystalShoes
            };
            AddItemsToShop(adventureShopStock, shoes);
        }

        private static void AddRings(Dictionary<ISalable, int[]> adventureShopStock)
        {
            adventureShopStock.Add(AmethystRing.Key, AmethystRing.Value);
            adventureShopStock.Add(TopazRing.Key, TopazRing.Value);
            if (MineShaft.lowestLevelReached >= 40)
            {
                adventureShopStock.Add(AquamarineRing.Key, AquamarineRing.Value);
                adventureShopStock.Add(JadeRing.Key, JadeRing.Value);
            }

            if (MineShaft.lowestLevelReached >= 80)
            {
                adventureShopStock.Add(EmeraldRing.Key, EmeraldRing.Value);
                adventureShopStock.Add(RubyRing.Key, RubyRing.Value);
            }
        }

        private static void AddSlingshots(Dictionary<ISalable, int[]> adventureShopStock)
        {
            var slingshots = new[] { Slingshot, MasterSlingshot };
            AddItemsToShop(adventureShopStock, slingshots);
        }

        private static void AddAmmo(Dictionary<ISalable, int[]> adventureShopStock)
        {
            if (Game1.player.craftingRecipes.ContainsKey("Explosive Ammo"))
            {
                adventureShopStock.Add(ExplosiveAmmo.Key, ExplosiveAmmo.Value);
            }
        }

        private static void AddGilRewards(Dictionary<ISalable, int[]> adventureShopStock)
        {
            if (Game1.player.mailReceived.Contains("Gil_Slime Charmer Ring"))
            {
                adventureShopStock.Add(SlimeCharmerRing.Key, SlimeCharmerRing.Value);
            }

            if (Game1.player.mailReceived.Contains("Gil_Savage Ring"))
            {
                adventureShopStock.Add(SavageRing.Key, SavageRing.Value);
            }

            if (Game1.player.mailReceived.Contains("Gil_Burglar's Ring"))
            {
                adventureShopStock.Add(BurglarsRing.Key, BurglarsRing.Value);
            }

            if (Game1.player.mailReceived.Contains("Gil_Vampire Ring"))
            {
                adventureShopStock.Add(VampireRing.Key, VampireRing.Value);
            }

            if (Game1.player.mailReceived.Contains("Gil_Crabshell Ring"))
            {
                adventureShopStock.Add(CrabshellRing.Key, CrabshellRing.Value);
            }

            if (Game1.player.mailReceived.Contains("Gil_Napalm Ring"))
            {
                adventureShopStock.Add(NapalmRing.Key, NapalmRing.Value);
            }

            if (Game1.player.mailReceived.Contains("Gil_Skeleton Mask"))
            {
                adventureShopStock.Add(SkeletonMask.Key, SkeletonMask.Value);
            }

            if (Game1.player.mailReceived.Contains("Gil_Hard Hat"))
            {
                adventureShopStock.Add(HardHat.Key, HardHat.Value);
            }

            if (Game1.player.mailReceived.Contains("Gil_Arcane Hat"))
            {
                adventureShopStock.Add(ArcaneHat.Key, ArcaneHat.Value);
            }

            if (Game1.player.mailReceived.Contains("Gil_Knight's Helmet"))
            {
                adventureShopStock.Add(KnightsHelmet.Key, KnightsHelmet.Value);
            }

            if (Game1.player.mailReceived.Contains("Gil_Insect Head"))
            {
                adventureShopStock.Add(InsectHead.Key, InsectHead.Value);
            }
        }

        private static void AddItemsToShop(Dictionary<ISalable, int[]> adventureShopStock, KeyValuePair<ISalable, int[]>[] items)
        {
            foreach (var (item, price) in items)
            {
                if (_archipelago.HasReceivedItem(item.Name))
                {
                    adventureShopStock.Add(item, price);
                }
            }
        }

        private static readonly KeyValuePair<ISalable, int[]> WoodenBlade = new(new MeleeWeapon(12), new[] { 250, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> IronDirk = new(new MeleeWeapon(17), new[] { 500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> WindSpire = new(new MeleeWeapon(22), new[] { 500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> Femur = new(new MeleeWeapon(31), new[] { 500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> SteelSmallsword = new(new MeleeWeapon(11), new[] { 600, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> WoodClub = new(new MeleeWeapon(24), new[] { 600, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> ElfBlade = new(new MeleeWeapon(20), new[] { 600, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> SilverSaber = new(new MeleeWeapon(1), new[] { 750, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> PirateSword = new(new MeleeWeapon(43), new[] { 850, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> CrystalDagger = new(new MeleeWeapon(21), new[] { 1500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> Cutlass = new(new MeleeWeapon(44), new[] { 1500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> IronEdge = new(new MeleeWeapon(6), new[] { 1500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> BurglarsShank = new(new MeleeWeapon(18), new[] { 1500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> WoodMallet = new(new MeleeWeapon(27), new[] { 2000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> Claymore = new(new MeleeWeapon(10), new[] { 2000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> TemplarsBlade = new(new MeleeWeapon(7), new[] { 4000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> Kudgel = new(new MeleeWeapon(46), new[] { 4000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> ShadowDagger = new(new MeleeWeapon(19), new[] { 2000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> ObsidianEdge = new(new MeleeWeapon(8), new[] { 6000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> TemperedBroadsword = new(new MeleeWeapon(52), new[] { 6000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> WickedKris = new(new MeleeWeapon(45), new[] { 6000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> BoneSword = new(new MeleeWeapon(5), new[] { 6000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> OssifiedBlade = new(new MeleeWeapon(60), new[] { 6000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> SteelFalchion = new(new MeleeWeapon(50), new[] { 9000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> TheSlammer = new(new MeleeWeapon(28), new[] { 9000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> LavaKatana = new(new MeleeWeapon(9), new[] { 25000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> GalaxySword = new(new MeleeWeapon(4), new[] { 50000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> GalaxyDagger = new(new MeleeWeapon(23), new[] { 35000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> GalaxyHammer = new(new MeleeWeapon(29), new[] { 75000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> Sneakers = new(new Boots(504), new[] { 500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> LeatherBoots = new(new Boots(506), new[] { 500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> WorkBoots = new(new Boots(507), new[] { 500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> TundraBoots = new(new Boots(509), new[] { 750, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> ThermalBoots = new(new Boots(510), new[] { 1000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> CombatBoots = new(new Boots(508), new[] { 1250, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> FirewalkerBoots = new(new Boots(512), new[] { 2000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> DarkBoots = new(new Boots(511), new[] { 2500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> SpaceBoots = new(new Boots(514), new[] { 5000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> CrystalShoes = new(new Boots(878), new[] { 5000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> AmethystRing = new(new Ring(529), new[] { 1000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> TopazRing = new(new Ring(530), new[] { 1000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> AquamarineRing = new(new Ring(531), new[] { 2500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> JadeRing = new(new Ring(532), new[] { 2500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> EmeraldRing = new(new Ring(533), new[] { 5000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> RubyRing = new(new Ring(534), new[] { 5000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> Slingshot = new(new Slingshot(32), new[] { 500, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> MasterSlingshot = new(new Slingshot(33), new[] { 1000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> ExplosiveAmmo = new(new Object(441, int.MaxValue), new[] { 300, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> SlimeCharmerRing = new(new Ring(520), new[] { 25000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> SavageRing = new(new Ring(523), new[] { 25000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> BurglarsRing = new(new Ring(526), new[] { 20000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> VampireRing = new(new Ring(522), new[] { 15000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> CrabshellRing = new(new Ring(810), new[] { 15000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> NapalmRing = new(new Ring(811), new[] { 30000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> SkeletonMask = new(new Hat(8), new[] { 20000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> HardHat = new(new Hat(27), new[] { 20000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> ArcaneHat = new(new Hat(60), new[] { 20000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> KnightsHelmet = new(new Hat(50), new[] { 20000, int.MaxValue });
        private static readonly KeyValuePair<ISalable, int[]> InsectHead = new(new MeleeWeapon(13), new[] { 10000, int.MaxValue });
    }
}
