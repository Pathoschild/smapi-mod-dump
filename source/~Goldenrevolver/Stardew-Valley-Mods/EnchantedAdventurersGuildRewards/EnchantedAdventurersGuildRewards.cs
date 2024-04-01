/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace EnchantedAdventurersGuildRewards
{
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;
    using StardewValley.Enchantments;
    using StardewValley.GameData.Weapons;
    using StardewValley.Tools;
    using System;
    using System.Collections.Generic;

    public class EnchantedAdventurersGuildRewards : Mod
    {
        public EnchantedAdventurersGuildRewardsConfig Config { get; set; }

        public static IManifest Manifest { get; set; }

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<EnchantedAdventurersGuildRewardsConfig>();
            Manifest = this.ModManifest;

            EnchantedAdventurersGuildRewardsConfig.VerifyConfigValues(Config, this);

            helper.Events.GameLoop.GameLaunched += delegate
            {
                EnchantedAdventurersGuildRewardsConfig.SetUpModConfigMenu(Config, this);
            };

            Helper.Events.GameLoop.DayStarted += delegate
            {
                AddOrDeleteRecipes();
            };

            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.Player.InventoryChanged += EnchantOnAddedToInventory;

            Patcher.PatchAll(this);
        }

        public const string darkSwordUQID = "2";
        public const string holyBladeUQID = "3";
        public const string templarsBladeUQID = "7";
        public const string insectHeadUQID = "13";
        public const string boneSwordUQID = "5";

        public const string skeletonMaskUQID = "8";
        public const string hardHatUQID = "27";
        public const string knightsHelmUQID = "50";
        public const string squiresHelmUQID = "51";

        public const string vampireRingQUID = "522";
        public const string arcaneHatQUID = "60";

        public static readonly string holyBladeRecipeKey = $"{Manifest?.UniqueID}/holyBladeRecipe";
        public static readonly string darkSwordRecipeKey = $"{Manifest?.UniqueID}/darkSwordRecipe";

        public static readonly string holyBladeCorruptionKey = $"{Manifest?.UniqueID}/holyBladeCorruption";
        public static readonly string darkSwordCorruptionKey = $"{Manifest?.UniqueID}/darkSwordCorruption";

        internal void EnchantOnAddedToInventory(object sender, InventoryChangedEventArgs e)
        {
            if (e.Player == null)
            {
                return;
            }

            foreach (var item in e.Added)
            {
                if (item is MeleeWeapon meleeWeapon)
                {
                    MaybeAddMissingEnchantments(Config, meleeWeapon);
                }
                else if (Config.EnableDebugSwordRecipes)
                {
                    FixBrokenDebugWeapon(item, e.Player);
                }
            }
        }

        internal static void MaybeAddMissingEnchantments(EnchantedAdventurersGuildRewardsConfig config, MeleeWeapon meleeWeapon)
        {
            if (meleeWeapon.enchantments.Count > 0 || meleeWeapon.previousEnchantments.Count > 0)
            {
                return;
            }

            switch (meleeWeapon.QualifiedItemId)
            {
                case $"(W){insectHeadUQID}":
                    if (config.EnableBugKillerInsectHead)
                    {
                        meleeWeapon.AddEnchantment(new BugKillerEnchantment());
                    }
                    break;

                case $"(W){darkSwordUQID}":
                    meleeWeapon.AddEnchantment(new VampiricEnchantment());
                    break;

                case $"(W){holyBladeUQID}":
                    meleeWeapon.AddEnchantment(new CrusaderEnchantment());
                    break;

                    //case $"(W){boneSwordUQID}":
                    //    meleeWeapon.AddEnchantment(new ArtfulEnchantment());
                    //    break;
            }
        }

        internal static void FixBrokenDebugWeapon(Item item, Farmer who)
        {
            if (item.ItemId is $"(W){darkSwordUQID}" or $"(W){holyBladeUQID}")
            {
                who.removeItemFromInventory(item);
                var sword = ItemRegistry.Create(item.ItemId) as MeleeWeapon;

                sword.AddEnchantment((item.ItemId == $"(W){holyBladeUQID}") ? new CrusaderEnchantment() : new VampiricEnchantment());
                who.addItemToInventory(sword);
            }
        }

        internal void AddOrDeleteRecipes()
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                bool hasHolyBladeRecipe = farmer.craftingRecipes.ContainsKey(holyBladeRecipeKey);
                bool hasDarkSwordRecipe = farmer.craftingRecipes.ContainsKey(darkSwordRecipeKey);

                if (Config.EnableDebugSwordRecipes)
                {
                    if (!hasHolyBladeRecipe)
                    {
                        farmer.craftingRecipes.Add(holyBladeRecipeKey, 0);
                    }

                    if (!hasDarkSwordRecipe)
                    {
                        farmer.craftingRecipes.Add(darkSwordRecipeKey, 0);
                    }
                }
                else
                {
                    if (hasHolyBladeRecipe)
                    {
                        farmer.craftingRecipes.Remove(holyBladeRecipeKey);
                    }

                    if (hasDarkSwordRecipe)
                    {
                        farmer.craftingRecipes.Remove(darkSwordRecipeKey);
                    }
                }
            }
        }

        private static string ApplyDefenseTooltip(string origTooltip, int defense)
        {
            var fields = origTooltip.Split('/');
            fields[1] += $"\n{Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", defense)}";
            return string.Join("/", fields);
        }

        internal void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/hats"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                    if (Config.SomeHatsGrantDefense)
                    {
                        data[hardHatUQID] = ApplyDefenseTooltip(data[hardHatUQID], 1);
                        data[squiresHelmUQID] = ApplyDefenseTooltip(data[squiresHelmUQID], 1);
                        data[knightsHelmUQID] = ApplyDefenseTooltip(data[knightsHelmUQID], 2);
                    }

                    if (Config.SkeletonMaskBoneThrowSynergy)
                    {
                        var fields = data[skeletonMaskUQID].Split('/');

                        var translatedBoneSwordName = Game1.content.LoadString("Strings\\Weapons:BoneSword_Name");

                        fields[1] += $"\n{Helper.Translation.Get("SkeletonMask_BonusDescription", new { boneSwordName = translatedBoneSwordName })}";

                        data[skeletonMaskUQID] = string.Join("/", fields);
                    }
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Weapons"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, WeaponData> data = asset.AsDictionary<string, WeaponData>().Data;

                    if (Config.RevertInsectHeadBuff)
                    {
                        data[insectHeadUQID].MinDamage = 10;
                        data[insectHeadUQID].MaxDamage = 20;
                    }
                });
            }

            // intentionally only english
            if (e.Name.IsEquivalentTo("Strings/Weapons"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                    var recipeChanges = new Dictionary<string, Tuple<string, string>>();

                    if (Config.RenameEnglishDarkSwordToDarkBlade)
                    {
                        data["DarkSword_Name"] = "Dark Blade";
                        //data["InfinityDagger_Name"] = "Infinity Razor";
                    }
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                    var recipeChanges = new Dictionary<string, Tuple<string, string>>();

                    if (Config.EnableDebugSwordRecipes)
                    {
                        // add display name, because this will turn into an error item
                        data[holyBladeRecipeKey] = $"(W){templarsBladeUQID} 1 (H)60 1/Home/(W){holyBladeUQID}/false/null/[LocalizedText Strings\\Weapons:DarkSword_Name]";
                        data[darkSwordRecipeKey] = $"(W){templarsBladeUQID} 1 (O)522 1/Home/(W){darkSwordUQID}/false/null/[LocalizedText Strings\\Weapons:HolyBlade_Name]";
                    }
                });
            }
        }

        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }
    }

    /// <summary>
    /// Extension methods for IGameContentHelper.
    /// </summary>
    public static class GameContentHelperExtensions
    {
        /// <summary>
        /// Invalidates both an asset and the locale-specific version of an asset.
        /// </summary>
        /// <param name="helper">The game content helper.</param>
        /// <param name="assetName">The (string) asset to invalidate.</param>
        /// <returns>if something was invalidated.</returns>
        public static bool InvalidateCacheAndLocalized(this IGameContentHelper helper, string assetName)
            => helper.InvalidateCache(assetName)
                | (helper.CurrentLocaleConstant != LocalizedContentManager.LanguageCode.en && helper.InvalidateCache(assetName + "." + helper.CurrentLocale));
    }
}