/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace RingOverhaul
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;
    using StardewValley.GameData.Objects;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RingOverhaul : Mod
    {
        public Texture2D ExplorerRingTexture { get; set; }

        public Texture2D BerserkerRingTexture { get; set; }

        public Texture2D PaladinRingTexture { get; set; }

        private bool ShouldApplyCustomSprites
        {
            get
            {
                return Config.CraftableGemRings && Config.CraftableGemRingsCustomSprites
                    && Config.CraftableGemRingsMetalBar != 1 && !Helper.ModRegistry.IsLoaded("BBR.BetterRings");
            }
        }

        public const string IridiumBandNonQualifiedID = "527";
        public const string JukeBoxRingNonQualifiedID = "528";

        internal RingOverhaulConfig Config;

        // TODO double check 1.6 mini jukebox changes, especially Game1.player calls vs who calls

        // TODO make rings of the same category type not equippable at the same time (care for compatibility with equip more rings)

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<RingOverhaulConfig>();

            RingOverhaulConfig.VerifyConfigValues(this, Config);

            string path = Helper.ModRegistry.IsLoaded("BBR.BetterRings") ? "assets/betterRings" : "assets";

            ExplorerRingTexture = Helper.ModContent.Load<Texture2D>($"{path}/explorer_ring.png");
            BerserkerRingTexture = Helper.ModContent.Load<Texture2D>($"{path}/berserker_ring.png");
            PaladinRingTexture = Helper.ModContent.Load<Texture2D>($"{path}/paladin_ring.png");

            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            Helper.Events.Content.AssetRequested += OnAssetRequested;

            Helper.Events.GameLoop.GameLaunched += delegate { RingOverhaulConfig.SetUpModConfigMenu(Config, this); };

            Patcher.PatchAll(this);
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Config.JukeboxRingEnabled)
            {
                return;
            }

            foreach (var farmer in Game1.getAllFarmers())
            {
                if (farmer.craftingRecipes.ContainsKey("Mini-Jukebox")
                    && !farmer.craftingRecipes.ContainsKey("Jukebox Ring"))
                {
                    farmer.craftingRecipes.Add("Jukebox Ring", 0);
                }
            }
        }

        /// <summary>
        /// Small helper method to log to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        /// <summary>
        /// Small helper method to log an error to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        /// <param name="e">an optional error message to log additionally</param>
        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, ObjectData> data = asset.AsDictionary<string, ObjectData>().Data;

                    if (Config.IridiumBandChangesEnabled)
                    {
                        var entry = data[IridiumBandNonQualifiedID];
                        entry.Description = Helper.Translation.Get("IridiumBandTooltip");
                        data[IridiumBandNonQualifiedID] = entry;
                    }

                    if (Config.JukeboxRingEnabled)
                    {
                        var entry = data[JukeBoxRingNonQualifiedID];
                        //entry.Description = "[LocalizedText Strings\\Objects:JukeboxRing_Description]";
                        entry.Description = "[LocalizedText Strings\\BigCraftables:MiniJukebox_Description]";
                        data[JukeBoxRingNonQualifiedID] = entry;
                    }
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Maps/springobjects") && ShouldApplyCustomSprites)
            {
                e.Edit((asset) =>
                {
                    var editor = asset.AsImage();

                    string path = Config.CraftableGemRingsMetalBar switch
                    {
                        2 => "assets/gem_rings_iron.png",
                        3 => "assets/gem_rings_gold.png",
                        _ => "assets/gem_rings_progressive.png",
                    };

                    Texture2D sourceImage = Helper.ModContent.Load<Texture2D>(path);
                    editor.PatchImage(sourceImage, targetArea: new Rectangle(16, 352, 96, 16));
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                    var recipeChanges = new Dictionary<string, Tuple<string, string>>();

                    if (Config.JukeboxRingEnabled)
                    {
                        data["Jukebox Ring"] = "336 1 787 1 464 1/Home/528/false/null/";
                    }

                    if (!Config.OldIridiumBandRecipe && Config.IridiumBandChangesEnabled)
                    {
                        recipeChanges["Iridium Band"] = new Tuple<string, string>("337 2 529 1 530 1 531 1 532 1 533 1 534 1", "Combat 9");
                    }

                    if (!Config.OldGlowStoneRingRecipe)
                    {
                        recipeChanges["Glowstone Ring"] = new Tuple<string, string>("517 1 519 1", "Mining 4");
                    }

                    if (Config.MinorRingCraftingChanges)
                    {
                        recipeChanges["Sturdy Ring"] = new Tuple<string, string>("334 2 86 5 338 5", "Combat 1");
                        recipeChanges["Warrior Ring"] = new Tuple<string, string>("335 5 382 25 84 10", "Combat 4");
                    }

                    foreach (var item in recipeChanges)
                    {
                        var entry = data[item.Key];
                        var fields = entry.Split('/');
                        fields[0] = item.Value.Item1;
                        fields[4] = item.Value.Item2;
                        data[item.Key] = string.Join("/", fields);
                    }

                    if (!Config.OldGlowStoneRingRecipe)
                    {
                        data["Glow Ring"] = "516 1 768 5/Home/517/false/Mining 4/";
                        data["Magnet Ring"] = "518 1 769 5/Home/519/false/Mining 4/";
                    }

                    if (Config.CraftableGemRings)
                    {
                        var dict = new Dictionary<string, int> { { "Amethyst Ring", 66 }, { "Topaz Ring", 68 }, { "Aquamarine Ring", 62 }, { "Jade Ring", 70 }, { "Emerald Ring", 60 }, { "Ruby Ring", 64 } }.ToList();

                        int itemId = 529;
                        int combatLevel = 2;
                        int oreBar = Config.CraftableGemRingsMetalBar == 2 ? 335 : Config.CraftableGemRingsMetalBar == 3 ? 336 : 334;

                        for (int i = 0; i < dict.Count; i++)
                        {
                            data[dict[i].Key] = $"{dict[i].Value} 1 {oreBar} 1/Home/{itemId}/false/Combat {combatLevel}/";

                            if (Config.CraftableGemRingsMetalBar == 0 && i is 1 or 3)
                            {
                                oreBar++;
                            }

                            if (Config.CraftableGemRingsUnlockLevels != 0)
                            {
                                combatLevel += i == 2 ? 2 : 1;
                            }
                            else if (i is 1 or 3)
                            {
                                combatLevel += 2;
                            }

                            itemId++;
                        }
                    }
                });
            }
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