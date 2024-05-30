/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/SDV_JojaExpress
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley.GameData.Shops;
using StardewValley.GameData;
using StardewValley;
using System.Globalization;
using StardewValley.GameData.Objects;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.Tools;

namespace JojaExpress
{
    public class LoadingManager
    {
        public static void loadAsset(object? sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("JojaExp\\string"))
            {
                e.LoadFrom(() =>
                {
                    return new Dictionary<string, string>()
                    {
                        { "display", ModEntry._Helper.Translation.Get("item.package.display") },
                        { "descrip_global", ModEntry._Helper.Translation.Get("item.package.descrip.global") },
                        { "descrip_local", ModEntry._Helper.Translation.Get("item.package.descrip.local") },
                        { "descrip_jpad", ModEntry._Helper.Translation.Get("item.jpad.descrip") },
                        { "jpad", ModEntry._Helper.Translation.Get("item.jpad") }
                    };
                }, AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Data/Shops"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, ShopData> data = asset.AsDictionary<string, ShopData>().Data;
                    data.Add("ofts.JojaExp.jojaLocal", initShop());
                    var shop = initShop();
                    QuantityModifier mdfr = new()
                    {
                        Modification = QuantityModifier.ModificationType.Multiply,
                        Id = "ofts.jojaExp.mdfr.0",
                        Amount = ModEntry.getPriceModifier()
                    };
                    ShopItemData sid0 = new()
                    {
                        ItemId = "ALL_ITEMS @requirePrice",
                        Id = "ofts.jojaExp.sid.0",
                        IgnoreShopPriceModifiers = true,
                        PriceModifiers = new() { }

                    };
                    sid0.PriceModifiers.Add(mdfr);
                    sid0.PriceModifierMode = QuantityModifier.QuantityModifierMode.Maximum;
                    shop.Items.Add(sid0);
                    data.Add("ofts.JojaExp.jojaGlobal", shop);

                    data["Joja"].Items.Add(new ShopItemData() { 
                        ItemId = "(T)ofts.jojaExp.item.jpad",
                        Id = "ofts.jojaExp.paditem",
                        IgnoreShopPriceModifiers = false,
                        PriceModifiers = new() { }
                    });
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
            {
                ModEntry._Monitor.Log("Updating Mails");
                e.Edit(asset =>
                {
                    IDictionary<string, string> mails = asset.AsDictionary<string, string>().Data;
                    string mail = ModEntry._Helper.Translation.Get("mail_format");
                    mails.Add("ofts.jojaExp.mail", mail);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                ModEntry._Monitor.Log("Adding objects");
                e.Edit(asset =>
                {
                    IDictionary<string, ObjectData> objects = asset.AsDictionary<string, ObjectData>().Data;
                    ObjectData data = new()
                    {
                        Name = "jojaExp.globalPackage",
                        DisplayName = "[LocalizedText JojaExp\\string:display]",
                        Description = "[LocalizedText JojaExp\\string:descrip_global]",
                        Type = "Basic",
                        Category = -999,
                        Price = 0,
                        Texture = "LooseSprites/Giftbox",
                        SpriteIndex = 33,
                        IsDrink = false,
                        ExcludeFromFishingCollection = true,
                        ExcludeFromRandomSale = true,
                        ExcludeFromShippingCollection = true
                    };
                    objects.Add("ofts.jojaExp.item.package.global", data);
                    ObjectData data2 = new()
                    {
                        Name = "jojaExp.localPackage",
                        DisplayName = "[LocalizedText JojaExp\\string:display]",
                        Description = "[LocalizedText JojaExp\\string:descrip_local]",
                        Type = "Basic",
                        Category = -999,
                        Price = 0,
                        Texture = "LooseSprites/Giftbox",
                        SpriteIndex = 11,
                        IsDrink = false,
                        ExcludeFromFishingCollection = true,
                        ExcludeFromRandomSale = true,
                        ExcludeFromShippingCollection = true
                    };
                    objects.Add("ofts.jojaExp.item.package.local", data2);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("JojaExp/assets/JPad"))
            {
                e.LoadFromModFile<Texture2D>("assets/Jpad", AssetLoadPriority.Low);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Tools"))
            {
                e.Edit(asset => {
                    IDictionary<string, ToolData> data = asset.AsDictionary<string, ToolData>().Data;
                    ToolData data3 = new()
                    {
                        ClassName = "GenericTool",
                        Name = "jojaExp.jpad",
                        DisplayName = "[LocalizedText JojaExp\\string:jpad]",
                        Description = "[LocalizedText JojaExp\\string:descrip_jpad]",
                        Texture = "JojaExp/assets/JPad",
                        SpriteIndex = 1,
                        SalePrice = 10000,
                        MenuSpriteIndex = 0,
                        UpgradeLevel = -1,
                        ApplyUpgradeLevelToDisplayName = false,
                        CanBeLostOnDeath = true,
                        AttachmentSlots = -1,
                        ConventionalUpgradeFrom = "",
                        UpgradeFrom = new(),
                        SetProperties = new(),
                        ModData = new(),
                        CustomFields = new()
                    };
                    data.Add("ofts.jojaExp.item.jpad", data3);
                });
            }
        }

        private static ShopData initShop()
        {
            ShopData shop = new()
            {
                Currency = 0,
                StackSizeVisibility = StackSizeVisibility.Show,
                OpenSound = "dwop",
                PurchaseSound = "purchaseClick",
                PurchaseRepeatSound = "purchaseRepeat",
                Owners = new() { initOwner() },
                SalableItemTags = new(),
                Items = new()
            };
            return shop;
        }

        private static ShopOwnerData initOwner()
        {
            ShopOwnerData owner = new();
            owner.Condition = "";
            owner.Portrait = "assets/joja";
            owner.Dialogues = new();
            owner.Id = "ofts.jojaExp.0";
            owner.Name = "AnyOrNone";
            return owner;
        }

        public static void fillShops(object? sender, AssetReadyEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Data/Shops"))
            {
                ShopData? jojaLocal = null;
                List<ShopItemData> items = new();
                HashSet<string> ids = new HashSet<string>();
                foreach (KeyValuePair<string, ShopData> shop in DataLoader.Shops(Game1.content))
                {
                    if (shop.Value.Currency != 0) continue;
                    if (shop.Key == "ofts.JojaExp.jojaLocal")
                    {
                        jojaLocal = shop.Value;
                        continue;
                    }
                    if (shop.Key == "ofts.JojaExp.jojaGlobal") continue;

                    List<QuantityModifier>? modifiers = shop.Value.PriceModifiers;
                    foreach (ShopItemData item in shop.Value.Items)
                    {
                        if (item.ItemId == null || !ItemRegistry.IsQualifiedItemId(item.ItemId)) continue;
                        if (ids.Contains(item.ItemId)) continue;
                        if (!item.IgnoreShopPriceModifiers && modifiers != null && modifiers.Count != 0)
                        {
                            if (item.PriceModifiers != null) item.PriceModifiers.AddRange(modifiers);
                            else item.PriceModifiers = modifiers;
                        }
                        try
                        {
                            var a = ItemRegistry.GetMetadata(item.ItemId).GetParsedData().DisplayName;
                            items.Add(item);
                            ids.Add(item.ItemId);
                        }catch (Exception ex)
                        {
                            ModEntry._Monitor.Log($"Failed to add {item.ItemId} to Joja Community. " +
                                $"Technical details: {ex.Message}. " +
                                $"Joja Express already handled this exception. Your game will still run normally");
                        }
                    }
                    if (modifiers != null && modifiers.Count != 0)
                    {
                        shop.Value.PriceModifiers = null;
                    }
                }
                if (jojaLocal == null) throw new Exception("failed to find jojalocal market");
                
                items.Sort((a, b) =>
                {
                    return string.Compare(ItemRegistry.GetMetadata(a.ItemId).GetParsedData().DisplayName,
                        ItemRegistry.GetMetadata(b.ItemId).GetParsedData().DisplayName,
                        new CultureInfo(ModEntry._Helper.Translation.Locale), CompareOptions.IgnoreSymbols);
                });
                jojaLocal.Items.AddRange(items);

            }
        }
    }
}
