/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slothsoft.Informant.Api;
using StardewValley.Menus;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Slothsoft.Informant.Implementation;

internal class SellPriceDisplayable : IDisplayable {
    
    private static string DisplayableId => "sell-price";
    private static readonly Rectangle CoinSourceBounds = new(5, 69, 6, 6);
    
    private record MoneyToDisplay(int One, int? Stack);

    private static Vector2? _lastCurrencyCoordinates;
    private static bool _myCall;

    private readonly IModHelper _modHelper;
    private readonly Harmony _harmony;

    public SellPriceDisplayable(IModHelper modHelper, string? uniqueId = null) {
        _modHelper = modHelper;
        _harmony = new Harmony(uniqueId ?? InformantMod.Instance!.ModManifest.UniqueID);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(IClickableMenu),
                nameof(IClickableMenu.drawToolTip)
            ),
            prefix: new HarmonyMethod(typeof(SellPriceDisplayable), nameof(ManipulateMoneyValue)),
            postfix: new HarmonyMethod(typeof(SellPriceDisplayable), nameof(DrawAdditionalMoneyValues))
        );
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(SpriteBatch),
                nameof(SpriteBatch.Draw),
                new[] {
                    typeof(Texture2D),
                    typeof(Vector2),
                    typeof(Rectangle?),
                    typeof(Color),
                    typeof(float),
                    typeof(Vector2),
                    typeof(Vector2),
                    typeof(SpriteEffects),
                    typeof(float),
                }
            ),
            prefix: new HarmonyMethod(typeof(SellPriceDisplayable), nameof(RememberCurrencyCoordinates))
        );
    }

    public string Id => DisplayableId;
    public string DisplayName => _modHelper.Translation.Get("SellPriceDisplayable");
    public string Description => _modHelper.Translation.Get("SellPriceDisplayable.Description");

    // __state => https://harmony.pardeike.net/articles/patching-injections.html
    // "Patches can use an argument called __state to store information in the prefix method that can be accessed again in the postfix method."

    private static void ManipulateMoneyValue(Item? hoveredItem, ref int moneyAmountToShowAtBottom, out MoneyToDisplay? __state) {
        __state = null;

        var config = InformantMod.Instance?.Config ?? new InformantConfig();
        if (!config.DisplayIds.GetValueOrDefault(DisplayableId, true)) {
            return; // this "decorator" is deactivated
        }
           
        if (hoveredItem != null && moneyAmountToShowAtBottom < 0) {
            moneyAmountToShowAtBottom = CalculateSellPrice(hoveredItem) ?? -1;

            if (moneyAmountToShowAtBottom >= 0) {
                int? stackMoney = hoveredItem.Stack > 1 ? hoveredItem.Stack * moneyAmountToShowAtBottom : null;
                __state = new MoneyToDisplay(moneyAmountToShowAtBottom, stackMoney);
            }
        }
    }

    private static int? CalculateSellPrice(Item item) {
        if (item is not SObject obj) {
            return null; // we only sell SObjects
        }

        var price = Utility.getSellToStorePriceOfItem(item, countStack: false);
        return price >= 0 || obj.canBeShipped() ? price : null;
    }

    private static void DrawAdditionalMoneyValues(SpriteBatch b, Item? hoveredItem, MoneyToDisplay? __state) {
        if (_lastCurrencyCoordinates == null || hoveredItem == null || __state == null || __state.Stack == null) {
            return;
        }

        _myCall = true;
        
        // Draws red coins on the positon we calculated the "real" coins are, to see if we are on the spot
        // b.Draw(Game1.debrisSpriteSheet, _lastCurrencyCoordinates.Value, CoinSourceBounds, Color.Crimson, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);

        var font = Game1.smallFont;
        var moneyCoordinatesX = _lastCurrencyCoordinates.Value.X + 1.5f * CoinSourceBounds.Width * Game1.pixelZoom;
        var moneyCoordinatesY = _lastCurrencyCoordinates.Value.Y;
        var moneyCoordinates = new Vector2(moneyCoordinatesX, moneyCoordinatesY);

        b.DrawString(font, __state.Stack.ToString(), moneyCoordinates + new Vector2(2f, 2f), Game1.textShadowColor);
        b.DrawString(font, __state.Stack.ToString(), moneyCoordinates + new Vector2(0.0f, 2f), Game1.textShadowColor);
        b.DrawString(font, __state.Stack.ToString(), moneyCoordinates + new Vector2(2f, 0.0f), Game1.textShadowColor);
        b.DrawString(font, __state.Stack.ToString(), moneyCoordinates, Game1.textColor);

        moneyCoordinates.X += font.MeasureString(__state.Stack.ToString()).X + 0.5f * CoinSourceBounds.Width * Game1.pixelZoom;
        
        b.Draw(Game1.debrisSpriteSheet, moneyCoordinates - new Vector2(2f, 2f),
            CoinSourceBounds, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
        b.Draw(Game1.debrisSpriteSheet, moneyCoordinates + new Vector2(2f, 2f),
            CoinSourceBounds, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
        
        _myCall = false;
    }

    private static void RememberCurrencyCoordinates(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects,  float layerDepth) {
        if (_myCall) return; // we don't need to remember our own calls
        
        if (texture == Game1.debrisSpriteSheet) {
            // CoinSourceBounds is the direct rectangle over the coint icon, but it seems sometimes a bigger rectangle is drawn
            // to have a bit of an empty space to the left or on the top or ....
            if (sourceRectangle != null && sourceRectangle.Value.Contains(CoinSourceBounds))
            {
                _lastCurrencyCoordinates = new Vector2 {
                    // sourceRectangle = 0, 64, 16, 16
                    // actualCoinPosition = 5, 69, 6, 6 
                    // origin = 8, 8 (later scaled by scale = 4,4)
                    //
                    // I don't know how I'm getting from the above input to the -12 necessary for alligning the coin
                    X = position.X - 3 * Game1.pixelZoom, 
                    Y = position.Y - 3 * Game1.pixelZoom, 
                };
            }
        }
    }
}