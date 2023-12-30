/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using DeepWoodsMod.Stuff;
using DeepWoodsMod.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Linq;
using xTile.Dimensions;

namespace DeepWoodsMod
{
    public class DeepWoodsMaxHouse : GameLocation
    {
        private NetVector2 internalMaxHutLocation = new NetVector2();

        private static PerScreen<Vector2> maxHutLocation = new();

        public static Vector2 MaxHutLocation
        {
            get
            {
                return maxHutLocation.Value;
            }
            set
            {
                maxHutLocation.Value = value;
                if (Game1.getLocationFromName("DeepWoodsMaxHouse") is DeepWoodsMaxHouse deepWoodsMaxHouse)
                {
                    deepWoodsMaxHouse.internalMaxHutLocation.Value = value;
                    deepWoodsMaxHouse.updateWarps();
                }
            }
        }

        public DeepWoodsMaxHouse()
            : base("Maps/DeepWoodsMaxHouse", "DeepWoodsMaxHouse")
        {
            InitNetFields();

            if (Game1.IsMasterGame)
            {
                this.largeTerrainFeatures.Add(new DeepWoodsMaxHousePuzzle(new Vector2(10, 6)));
            }

            updateWarps();
        }

        public override void updateWarps()
        {
            this.warps.Clear();
            this.warps.Add(new Warp(19, 25, "DeepWoods", (int)internalMaxHutLocation.X + 2, (int)internalMaxHutLocation.Y + 6, false));
        }

        private void InitNetFields()
        {
            NetFields.AddField(internalMaxHutLocation);
        }

        public override bool isCollidingPosition(Microsoft.Xna.Framework.Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
        {
            foreach (var largeTerrainFeature in largeTerrainFeatures)
            {
                if (largeTerrainFeature.getBoundingBox().Intersects(position))
                {
                    if (largeTerrainFeature is DeepWoodsMaxHousePuzzle puzzle)
                    {
                        return !puzzle.isPassable();
                    }
                }
            }

            return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, projectile, ignoreCharacterRequirement);
        }

        public override bool performAction(string action, Farmer who, Location tileLocation)
        {
            if (action != null && who.IsLocalPlayer)
            {
                string[] array = action.Split(' ');
                switch (array[0])
                {
                    case "DeepWoodsMaxShop":
                        openDeepWoodsMaxShop();
                        return true;
                    case "DeepWoodsMaxQuests":
                        openDeepWoodsMaxQuests();
                        return true;
                    case "DeepWoodsMaxStuff":
                        openDeepWoodsMaxStuff();
                        return true;
                    case "DeepWoodsMaxBooks":
                        openDeepWoodsMaxBooks();
                        return true;
                }
            }

            return base.performAction(action, who, tileLocation);
        }

        public override void performTouchAction(string fullActionString, Vector2 playerStandingPosition)
        {
            if (fullActionString != null)
            {
                string[] array = fullActionString.Split(' ');
                if (array.Length == 2 && array[0] == "DeepWoodsMaxPuzzle")
                {
                    doDeepWoodsMaxPuzzle(array[1], playerStandingPosition);
                    return;
                }
            }

            base.performTouchAction(fullActionString, playerStandingPosition);
        }

        private void doDeepWoodsMaxPuzzle(string tileId, Vector2 tilePos)
        {
             this.largeTerrainFeatures.Where(l => l is DeepWoodsMaxHousePuzzle).Select(l => l as DeepWoodsMaxHousePuzzle).FirstOrDefault()?.doPuzzle(tileId);
        }

        private void openDeepWoodsMaxStuff()
        {
            DeepWoodsQuestMenu.OpenQuestMenu(I18N.StuffMessage, new Response[2]
            {
                new Response("SearchThroughStuff", I18N.StuffAnswerSearch).SetHotKey(Keys.Y),
                new Response("No", I18N.StuffAnswerNevermind).SetHotKey(Keys.Escape)
            });
        }

        private void openDeepWoodsMaxBooks()
        {
            var bookText = I18N.BookTexts.Get(new Random().Next(1, I18N.BookTexts.textIDs.Length));
            DeepWoodsQuestMenu.OpenQuestMenu(bookText, new Response[1]
            {
                new Response("No", I18N.CloseBook).SetHotKey(Keys.Escape)
            });
        }

        private void openDeepWoodsMaxQuests()
        {
            // TODO: Display quest board
            DeepWoodsQuestMenu.OpenQuestMenu(I18N.QuestsEmptyMessage, new Response[1]
            {
                new Response("No", I18N.MessageBoxClose).SetHotKey(Keys.Escape)
            });
        }

        private void openDeepWoodsMaxShop()
        {
            // TODO: Add a shop
            DeepWoodsQuestMenu.OpenQuestMenu(I18N.ShopEmptyMessage, new Response[1]
            {
                new Response("No", I18N.MessageBoxClose).SetHotKey(Keys.Escape)
            });
        }
    }
}
