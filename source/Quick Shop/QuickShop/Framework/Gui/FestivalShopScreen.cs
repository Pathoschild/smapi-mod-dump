/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/QuickShop
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Components;
using StardewValley;
using Button = EnaiumToolKit.Framework.Screen.Elements.Button;

namespace QuickShop.Framework.Gui;

public class FestivalShopScreen : ScreenGui
{
    public FestivalShopScreen(List<Shop> shops) : base(ModEntry.GetInstance().Helper.Translation
        .Get("quickShop.screen.festivalShop.title"))
    {
        foreach (var shop in shops)
        {
            AddElement(new Button(ModEntry.GetInstance().GetButtonTranslation(shop.Title))
            {
                OnLeftClicked = () => { Utility.TryOpenShopMenu(shop.ShopId, shop.OwnerName); }
            });
        }

        for (var i = 1; i <= 3; i++)
        {
            var finalI = i;
            AddElement(new Button($"{ModEntry.GetInstance().GetButtonTranslation("decorationBoatShop")} {finalI}")
            {
                OnLeftClicked = () =>
                {
                    Utility.TryOpenShopMenu($"Festival_NightMarket_MagicBoat_Day{finalI}", "BlueBoat");
                }
            });
        }

        var desertFestivals = DataLoader.Shops(Game1.content)
            .Where(shop =>
                shop.Key.StartsWith("DesertFestival_")
                && shop.Value.Owners.Count == 1
                && Utility.getAllCharacters().Any(npc => npc.Name == shop.Value.Owners[0].Id));

        foreach (var festival in desertFestivals)
        {
            AddElement(new Button(
                $"{ModEntry.GetInstance().GetButtonTranslation("desertFestival")}({Utility.getAllCharacters().First(npc => npc.Name == festival.Value.Owners[0].Id).displayName})")
            {
                OnLeftClicked = () => { Utility.TryOpenShopMenu(festival.Key, festival.Value.Owners[0].Id); }
            });
        }
    }
}