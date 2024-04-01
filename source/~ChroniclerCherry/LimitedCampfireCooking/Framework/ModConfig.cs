/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

namespace LimitedCampfireCooking.Framework;

class ModConfig
{
    public bool EnableAllCookingRecipies { get; set; } = false;
    public string[] Recipes { get; set; } = {
        "Fried Egg",
        "Baked Fish",
        "Parsnip Soup",
        "Vegetable Stew",
        "Bean Hotpot",
        "Glazed Yams",
        "Carp Surprise",
        "Fish Taco",
        "Tom Kha Soup",
        "Trout Soup",
        "Pumpkin Soup",
        "Algae Soup",
        "Pale Broth",
        "Roasted Hazelnuts",
        "Chowder",
        "Lobster Bisque",
        "Fish Stew"
    };
}
