/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestFramework.Framework.Menus;
using QuestFramework.Framework.Structures;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Controllers
{
    class CustomBoardController
    {
        private readonly PerScreen<List<CustomBoardTrigger>> _customBoardTriggers;
        private readonly PerScreen<CustomBoardTrigger[]> _currentLocationBoardTriggers;

        public CustomBoardController(IModEvents events)
        {
            events.Player.Warped += this.OnPlayerWarped;
            events.Display.RenderedWorld += this.OnWorldRendered;
            this._customBoardTriggers = new PerScreen<List<CustomBoardTrigger>>(() => new List<CustomBoardTrigger>());
            this._currentLocationBoardTriggers = new PerScreen<CustomBoardTrigger[]>();
        }

        private void OnWorldRendered(object sender, RenderedWorldEventArgs e)
        {
            if (Game1.eventUp || this._currentLocationBoardTriggers?.Value == null)
                return;

            foreach (var boardTrigger in this._currentLocationBoardTriggers.Value)
            {
                if (!ShouldShowIndicator(boardTrigger))
                    continue;

                float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                e.SpriteBatch.Draw(Game1.mouseCursors,
                    Game1.GlobalToLocal(
                        Game1.viewport,
                        new Vector2(
                            x: boardTrigger.Tile.X * 64 + 8 + boardTrigger.IndicatorOffset.X, 
                            y: (boardTrigger.Tile.Y * 64) + yOffset - 32 + boardTrigger.IndicatorOffset.Y)),
                    new Rectangle(395, 497, 3, 8),
                    Color.White, 0f,
                    new Vector2(1f, 4f), 4f + Math.Max(0f, 0.25f - yOffset / 16f),
                    SpriteEffects.None, 1f);
            }
        }

        private static bool ShouldShowIndicator(CustomBoardTrigger boardTrigger)
        {
            return boardTrigger.ShowIndicator
                && boardTrigger.IsUnlocked()
                && IsOfferingQuest(boardTrigger);
        }

        private static bool IsOfferingQuest(CustomBoardTrigger boardTrigger)
        {
            if (!Context.IsWorldReady)
                return false;

            switch (boardTrigger.BoardType)
            {
                case BoardType.Quests:
                    return CustomBoard.todayQuests.ContainsKey(boardTrigger.BoardName)
                        && CustomBoard.todayQuests[boardTrigger.BoardName] != null
                        && CustomBoard.todayQuests[boardTrigger.BoardName].accepted.Value == false;
                case BoardType.SpecialOrders:
                    string orderType = $"QF:{boardTrigger.BoardName}";
                    return Game1.player.team.availableSpecialOrders.Where(so => so.orderType.Value == orderType).Any()
                        && !Game1.player.team.acceptedSpecialOrderTypes.Contains(orderType) 
                        && !Game1.eventUp;
                default:
                    return false;
            }
        }

        public void RefreshBoards()
        {
            if (Context.IsSplitScreen && !Context.IsMainPlayer)
                return;

            CustomBoard.todayQuests.Clear();

            foreach (var boardTrigger in this._customBoardTriggers.Value.Where(OnlyUnlockedQuestsBoard))
            {
                CustomBoard.LoadTodayQuestsIfNecessary(boardTrigger.BoardName);
            }

            if (Context.IsMainPlayer && SDate.Now().DayOfWeek == DayOfWeek.Monday)
            {
                var order_types = this._customBoardTriggers.Value.Where(OnlyUnlockedSpecialOrdersBoard)
                    .Select(b => $"QF:{b.BoardName}")
                    .Distinct()
                    .ToArray();

                UpdateAvailableSpecialOrders(order_types);
            }
        }

        private static bool OnlyUnlockedQuestsBoard(CustomBoardTrigger boardTrigger)
        {
            return boardTrigger.BoardType == BoardType.Quests && boardTrigger.IsUnlocked();
        }

        private static bool OnlyUnlockedSpecialOrdersBoard(CustomBoardTrigger boardTrigger)
        {
            return boardTrigger.BoardType == BoardType.SpecialOrders && boardTrigger.IsUnlocked();
        }

        private void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            this._currentLocationBoardTriggers.Value = this._customBoardTriggers.Value
                .Where(t => t.LocationName == e.NewLocation.Name)
                .ToArray();
        }

        public bool CheckBoardHere(Point tile)
        {
            var boardTrigger = this._currentLocationBoardTriggers.Value?.FirstOrDefault(t => t.Tile.Equals(tile));

            if (boardTrigger != null && !Game1.eventUp && boardTrigger.IsUnlocked())
            {
                Game1.activeClickableMenu = this.CreateBoardMenu(boardTrigger);

                return true;
            }

            return false;
        }

        private IClickableMenu CreateBoardMenu(CustomBoardTrigger boardTrigger)
        {
            switch(boardTrigger.BoardType)
            {
                case BoardType.Quests:
                    return new CustomBoard(boardTrigger.BoardName, boardTrigger.Texture);
                case BoardType.SpecialOrders:
                    return new CustomOrderBoard(
                        !string.IsNullOrEmpty(boardTrigger.BoardName) ? $"QF:{boardTrigger.BoardName}" : "", 
                        boardTrigger.Texture);
                default:
                    QuestFrameworkMod.Instance.Monitor.Log($"Unknown board type `{boardTrigger.BoardName}` for board `{boardTrigger.BoardName}`.", LogLevel.Error);
                    return null;
            }
        }

        public void RegisterBoardTrigger(CustomBoardTrigger trigger)
        {
            if (QuestFrameworkMod.Instance.Status != State.LAUNCHING)
            {
                throw new InvalidOperationException($"Cannot register new board trigger when in state `{QuestFrameworkMod.Instance.Status}`.");
            }

            this._customBoardTriggers.Value.Add(trigger);
        }

        public void Reset()
        {
            this._customBoardTriggers.Value.Clear();
        }

        public static void UpdateAvailableSpecialOrders(string[] validTypes)
        {
            SpecialOrder.UpdateAvailableSpecialOrders(true);

            var order_data = Game1.content.Load<Dictionary<string, SpecialOrderData>>("Data\\SpecialOrders");
            var keys = new List<string>(order_data.Keys);

            for (int k = 0; k < keys.Count; k++)
            {
                string key = keys[k];
                bool invalid = false;
                if (!invalid && order_data[key].Repeatable != "True" && Game1.MasterPlayer.team.completedSpecialOrders.ContainsKey(key))
                {
                    invalid = true;
                }
                if (Game1.dayOfMonth >= 16 && order_data[key].Duration == "Month")
                {
                    invalid = true;
                }
                if (!invalid && !SpecialOrder.CheckTags(order_data[key].RequiredTags))
                {
                    invalid = true;
                }
                if (!invalid)
                {
                    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                    {
                        if (specialOrder.questKey.Value == key)
                        {
                            invalid = true;
                            break;
                        }
                    }
                }
                if (invalid)
                {
                    keys.RemoveAt(k);
                    k--;
                }
            }

            Random r = new Random((int)Game1.uniqueIDForThisGame + (int)((float)Game1.stats.DaysPlayed * 1.3f));
            foreach (string type_to_find in validTypes)
            {
                var typed_keys = new List<string>();
                foreach (string key3 in keys)
                {
                    if (order_data[key3].OrderType == type_to_find)
                    {
                        typed_keys.Add(key3);
                    }
                }
                
                if (type_to_find != "Qi")
                {
                    for (int j = 0; j < typed_keys.Count; j++)
                    {
                        if (Game1.player.team.completedSpecialOrders.ContainsKey(typed_keys[j]))
                        {
                            typed_keys.RemoveAt(j);
                            j--;
                        }
                    }
                }

                var all_keys = new List<string>(typed_keys);
                for (int i = 0; i < 2; i++)
                {
                    if (typed_keys.Count == 0)
                    {
                        if (all_keys.Count == 0)
                        {
                            break;
                        }
                        typed_keys = new List<string>(all_keys);
                    }

                    int index = r.Next(typed_keys.Count);
                    string key2 = typed_keys[index];
                    Game1.player.team.availableSpecialOrders.Add(SpecialOrder.GetSpecialOrder(key2, r.Next()));
                    typed_keys.Remove(key2);
                    all_keys.Remove(key2);
                }
            }
        }
    }
}
