/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;

namespace ItemResearchSpawner
{
    /// <summary>Get translations from the mod's <c>i18n</c> folder.</summary>
    /// <remarks>This is auto-generated from the <c>i18n/default.json</c> file when the T4 template is saved.</remarks>
    [GeneratedCode("TextTemplatingFileGenerator", "1.0.0")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Deliberately named for consistency and to match translation conventions.")]
    internal static class I18n
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod's translation helper.</summary>
        private static ITranslationHelper Translations;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="translations">The mod's translation helper.</param>
        public static void Init(ITranslationHelper translations)
        {
            I18n.Translations = translations;
        }

        /// <summary>Get a translation equivalent to "Name".</summary>
        public static string Sort_ByName()
        {
            return I18n.GetByKey("sort.by-name");
        }

        /// <summary>Get a translation equivalent to "Category".</summary>
        public static string Sort_ByCategory()
        {
            return I18n.GetByKey("sort.by-category");
        }

        /// <summary>Get a translation equivalent to "ID".</summary>
        public static string Sort_ById()
        {
            return I18n.GetByKey("sort.by-id");
        }

        /// <summary>Get a translation equivalent to "All".</summary>
        public static string Category_All()
        {
            return I18n.GetByKey("category.all");
        }

        /// <summary>Get a translation equivalent to "Ammo".</summary>
        public static string Category_Ammo()
        {
            return I18n.GetByKey("category.ammo");
        }

        /// <summary>Get a translation equivalent to "Animal products".</summary>
        public static string Category_AnimalProducts()
        {
            return I18n.GetByKey("category.animal-products");
        }

        /// <summary>Get a translation equivalent to "Artifacts".</summary>
        public static string Category_Artifacts()
        {
            return I18n.GetByKey("category.artifacts");
        }

        /// <summary>Get a translation equivalent to "Artisan".</summary>
        public static string Category_Artisan()
        {
            return I18n.GetByKey("category.artisan");
        }

        /// <summary>Get a translation equivalent to "Bigcraftable".</summary>
        public static string Category_BigCraftable()
        {
            return I18n.GetByKey("category.big-craftable");
        }

        /// <summary>Get a translation equivalent to "Boots".</summary>
        public static string Category_Boots()
        {
            return I18n.GetByKey("category.boots");
        }

        /// <summary>Get a translation equivalent to "Clothes".</summary>
        public static string Category_Clothes()
        {
            return I18n.GetByKey("category.clothes");
        }

        /// <summary>Get a translation equivalent to "Craftable".</summary>
        public static string Category_Craftable()
        {
            return I18n.GetByKey("category.craftable");
        }

        /// <summary>Get a translation equivalent to "Crops".</summary>
        public static string Category_Crops()
        {
            return I18n.GetByKey("category.crops");
        }

        /// <summary>Get a translation equivalent to "Decoration".</summary>
        public static string Category_Dacor()
        {
            return I18n.GetByKey("category.dacor");
        }

        /// <summary>Get a translation equivalent to "Fish".</summary>
        public static string Category_Fish()
        {
            return I18n.GetByKey("category.fish");
        }

        /// <summary>Get a translation equivalent to "Flowers".</summary>
        public static string Category_Flowers()
        {
            return I18n.GetByKey("category.flowers");
        }

        /// <summary>Get a translation equivalent to "Forage plants".</summary>
        public static string Category_Forage()
        {
            return I18n.GetByKey("category.forage");
        }

        /// <summary>Get a translation equivalent to "Furniture".</summary>
        public static string Category_Furniture()
        {
            return I18n.GetByKey("category.furniture");
        }

        /// <summary>Get a translation equivalent to "Geodes".</summary>
        public static string Category_Geodes()
        {
            return I18n.GetByKey("category.geodes");
        }

        /// <summary>Get a translation equivalent to "Hats".</summary>
        public static string Category_Hats()
        {
            return I18n.GetByKey("category.hats");
        }

        /// <summary>Get a translation equivalent to "Legendary fish".</summary>
        public static string Category_LegendaryFish()
        {
            return I18n.GetByKey("category.legendary-fish");
        }

        /// <summary>Get a translation equivalent to "Minerals".</summary>
        public static string Category_Minerals()
        {
            return I18n.GetByKey("category.minerals");
        }

        /// <summary>Get a translation equivalent to "Monster loot".</summary>
        public static string Category_MonsterLoot()
        {
            return I18n.GetByKey("category.monster-loot");
        }

        /// <summary>Get a translation equivalent to "Noncraftable".</summary>
        public static string Category_NonCraftable()
        {
            return I18n.GetByKey("category.non-craftable");
        }

        /// <summary>Get a translation equivalent to "Rare minerals".</summary>
        public static string Category_RareMinerals()
        {
            return I18n.GetByKey("category.rare-minerals");
        }

        /// <summary>Get a translation equivalent to "Resources".</summary>
        public static string Category_Resources()
        {
            return I18n.GetByKey("category.resources");
        }

        /// <summary>Get a translation equivalent to "Rings".</summary>
        public static string Category_Rings()
        {
            return I18n.GetByKey("category.rings");
        }

        /// <summary>Get a translation equivalent to "Saplings".</summary>
        public static string Category_Saplings()
        {
            return I18n.GetByKey("category.saplings");
        }

        /// <summary>Get a translation equivalent to "Seeds".</summary>
        public static string Category_Seeds()
        {
            return I18n.GetByKey("category.seeds");
        }

        /// <summary>Get a translation equivalent to "Tools".</summary>
        public static string Category_Tools()
        {
            return I18n.GetByKey("category.tools");
        }

        /// <summary>Get a translation equivalent to "Tree fruits".</summary>
        public static string Category_TreeFruits()
        {
            return I18n.GetByKey("category.tree-fruits");
        }

        /// <summary>Get a translation equivalent to "Unique crops".</summary>
        public static string Category_UniqueCrops()
        {
            return I18n.GetByKey("category.unique-crops");
        }

        /// <summary>Get a translation equivalent to "Usable".</summary>
        public static string Category_Usable()
        {
            return I18n.GetByKey("category.usable");
        }

        /// <summary>Get a translation equivalent to "Weapons".</summary>
        public static string Category_Weapons()
        {
            return I18n.GetByKey("category.weapons");
        }

        /// <summary>Get a translation equivalent to "Cooking".</summary>
        public static string Category_Cooking()
        {
            return I18n.GetByKey("category.cooking");
        }

        /// <summary>Get a translation equivalent to "Misc".</summary>
        public static string Category_Misc()
        {
            return I18n.GetByKey("category.misc");
        }
        
        /// <summary>Get a translation by its key.</summary>
        /// <param name="key">The translation key.</param>
        /// <param name="tokens">An object containing token key/value pairs. This can be an anonymous object (like <c>new { value = 42, name = "Cranberries" }</c>), a dictionary, or a class instance.</param>
        public static Translation GetByKey(string key, object tokens = null)
        {
            if (I18n.Translations == null)
                throw new InvalidOperationException($"You must call {nameof(I18n)}.{nameof(I18n.Init)} from the mod's entry method before reading translations.");
            return I18n.Translations.Get(key, tokens);
        }
    }
}
