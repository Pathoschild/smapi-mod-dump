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
using System.Collections.Immutable;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slothsoft.Informant.Api;
using StardewValley.Menus;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Slothsoft.Informant.Implementation;

internal class ItemDecoratorManager : IDecoratorManager<Item> {

    private static readonly List<IDecorator<Item>> DecoratorsList = new();
    private static Rectangle? _lastToolTipCoordinates;
    
    private readonly Harmony _harmony;

    public ItemDecoratorManager(IModHelper modHelper) {
        _harmony = new Harmony(InformantMod.Instance!.ModManifest.UniqueID);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(IClickableMenu),
                nameof(IClickableMenu.drawToolTip)
            ),
            postfix: new HarmonyMethod(typeof(ItemDecoratorManager), nameof(DrawToolTip))
        );
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(IClickableMenu),
                nameof(IClickableMenu.drawTextureBox),
                new[] {
                    typeof(SpriteBatch),
                    typeof(Texture2D),
                    typeof(Rectangle),
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(Color),
                    typeof(float),
                    typeof(bool),
                    typeof(float)
                }
            ),
            postfix: new HarmonyMethod(typeof(ItemDecoratorManager), nameof(RememberToolTipCoordinates))
        );
    }

    private static void DrawToolTip(SpriteBatch b, Item? hoveredItem) {
        
        if (_lastToolTipCoordinates == null || hoveredItem == null) {
            return;
        }

        var config = InformantMod.Instance?.Config ?? new InformantConfig();
        var decorations = DecoratorsList
            .Where(g => config.DisplayIds.GetValueOrDefault(g.Id, true))
            .Where(d => d.HasDecoration(hoveredItem))
            .Select(d => d.Decorate(hoveredItem))
            .ToArray();

        if (decorations.Length == 0) {
            return;
        }
        
        var tipCoordinates = _lastToolTipCoordinates.Value;
        const int borderSize = 3 * Game1.pixelZoom;
        const int decoratorsHeight = Game1.tileSize;

        var decoratorsBox = new Rectangle(tipCoordinates.X, tipCoordinates.Y - decoratorsHeight + borderSize, tipCoordinates.Width, decoratorsHeight);
        IClickableMenu.drawTextureBox(b, Game1.menuTexture, TooltipGeneratorManager.TooltipSourceRect, decoratorsBox.X, 
            decoratorsBox.Y, decoratorsBox.Width, decoratorsBox.Height, Color.White, drawShadow: false);
        
        const int indent = 4 * Game1.pixelZoom;
        var destinationRectangle = new Rectangle(
            decoratorsBox.X + indent,
            decoratorsBox.Y + indent,
            decoratorsHeight - 2 * indent,
            decoratorsHeight - 2 * indent
        );
        
        foreach (var decoration in decorations) {
            b.Draw(decoration.Texture, destinationRectangle, null, Color.White);

            var counter = decoration.Counter;
            if (counter != null) {
                const float scale = 0.5f;
                // these x and y coordinates are the top left of the right-most number of the counter
                var x = destinationRectangle.X + destinationRectangle.Width - NumberSprite.getWidth(counter.Value % 10) + 2;
                var y = destinationRectangle.Y + destinationRectangle.Height - NumberSprite.getHeight() + 2;
                NumberSprite.draw(counter.Value, b, new Vector2(x, y), Color.White, scale, 1, 1, 0);
            }
            
            
            destinationRectangle.X += destinationRectangle.Width + Game1.pixelZoom;
        }
    }

    private static void RememberToolTipCoordinates(int x, int y, int width, int height) {
        _lastToolTipCoordinates = new Rectangle(x, y, width, height);
    }

    public IEnumerable<IDisplayable> Decorators => DecoratorsList.ToImmutableArray();

    public void Add(IDecorator<Item> decorator) {
        DecoratorsList.Add(decorator);
    }

    public void Remove(string decoratorId) {
        DecoratorsList.RemoveAll(g => g.Id == decoratorId);
    }
}