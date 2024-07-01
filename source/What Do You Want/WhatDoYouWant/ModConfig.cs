/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/WhatDoYouWant
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace WhatDoYouWant
{
    public sealed class ModConfig
    {
        public KeybindList WhatDoYouWantKeypress = KeybindList.Parse("F2");

        // TODO Community Center

        // TODO Golden Walnuts

        // Full Shipment

        public string ShippingSortOrder = Shipping.SortOrder_Category;

        // Gourmet Chef

        public string CookingSortOrder = Cooking.SortOrder_KnownRecipesFirst;

        // Craft Master

        public string CraftingSortOrder = Crafting.SortOrder_KnownRecipesFirst;

        // Master Angler

        public string FishingSortOrder = Fishing.SortOrder_SeasonsSpringFirst;

        // A Complete Collection

        public string MuseumSortOrder = Museum.SortOrder_Type;

        // TODO Stardrops

        // Polyculture

        public string PolycultureSortOrder = Polyculture.SortOrder_SeasonsSpringFirst;
    }
}
