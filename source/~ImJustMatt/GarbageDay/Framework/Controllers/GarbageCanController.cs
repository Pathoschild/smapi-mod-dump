/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using GarbageDay.Framework.Models;
using XSAutomate.Common.Extensions;
using Common.Helpers.ItemData;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace GarbageDay.Framework.Controllers
{
    internal class GarbageCanController : GarbageCanModel
    {
        private readonly GarbageDay _mod;
        private static IEnumerable<SearchableItem> _items;
        private readonly Multiplayer _multiplayer;
        private Chest _chest;
        private bool _doubleMega;
        private bool _dropQiBeans;
        private bool _garbageChecked = true;
        private bool _mega;
        private NPC _npc;

        internal GarbageCanController(GarbageDay mod)
        {
            _mod = mod;
            _multiplayer = _mod.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            _mod.Helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        internal Chest Chest => _chest ??= Location.Objects.TryGetValue(Tile, out var obj) && obj is Chest chest ? chest : null;

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is not ItemGrabMenu || !Context.IsPlayerFree) return;
            if (_npc != null)
            {
                Game1.drawDialogue(_npc);
                _npc = null;
            }

            if (Chest.items.Any() || Chest.playerChoiceColor.Value.Equals(Color.Black)) return;
            Chest.playerChoiceColor.Value = Color.DarkGray;
        }

        internal bool OpenCan()
        {
            // NPC Reaction
            _npc = null;
            var character = Utility.isThereAFarmerOrCharacterWithinDistance(new Vector2(Tile.X, Tile.Y), 7, Location);
            if (character is NPC npc && character is not Horse)
            {
                _npc = npc;
                _multiplayer.globalChatInfoMessage("TrashCan", Game1.player.Name, npc.Name);
                if (npc.Name.Equals("Linus"))
                {
                    npc.doEmote(32);
                    npc.setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Linus"), true, true);
                    Game1.player.changeFriendship(5, npc);
                    _multiplayer.globalChatInfoMessage("LinusTrashCan");
                }
                else
                    switch (npc.Age)
                    {
                        case 2:
                            npc.doEmote(28);
                            npc.setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Child"), true, true);
                            Game1.player.changeFriendship(-25, npc);
                            break;
                        case 1:
                            npc.doEmote(8);
                            npc.setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Teen"), true, true);
                            Game1.player.changeFriendship(-25, npc);
                            break;
                        default:
                            npc.doEmote(12);
                            npc.setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Adult"), true, true);
                            Game1.player.changeFriendship(-25, npc);
                            break;
                    }
            }

            if (_garbageChecked) return true;
            _garbageChecked = true;
            Game1.stats.incrementStat("trashCansChecked", 1);

            // Drop Item
            if (_dropQiBeans)
            {
                var origin = new Vector2(Tile.X + 0.5f, Tile.Y - 1) * 64f;
                Game1.createItemDebris(new Object(890, 1), origin, 2, Location, (int) origin.Y + 64);
                return false;
            }

            // Give Hat
            if (_doubleMega)
            {
                Location!.playSound("explosion");
                Chest.playerChoiceColor.Value = Color.Black; // Remove Lid
                Game1.player.addItemByMenuIfNecessary(new Hat(66));
                return false;
            }

            if (_mega)
            {
                Location!.playSound("crit");
            }

            return true;
        }

        internal void AddToLocation()
        {
            if (Chest == null && Location.Objects.ContainsKey(Tile)) return;
            var chest = new Chest(true, Tile, GarbageDay.ObjectId);
            chest.playerChoiceColor.Value = Color.DarkGray;
            chest.modData.Add("furyx639.GarbageDay", WhichCan);
            if (_mod.Config.HideFromChestsAnywhere) chest.modData.Add("Pathoschild.ChestsAnywhere/IsIgnored", "true");
            if (Chest != null)
            {
                chest.items.CopyFrom(Chest.items);
                RemoveFromLocation();
            }
            Location.Objects.Add(Tile, chest);
        }

        internal void RemoveFromLocation()
        {
            if (Chest != null) Location.Objects.Remove(Tile);
        }

        internal void DayStart(float extraLuck = 0)
        {
            if (Chest == null) return;

            // Reset State
            _garbageChecked = false;
            _dropQiBeans = false;
            Chest.playerChoiceColor.Value = Color.DarkGray;
            if (_mod.Config.HideFromChestsAnywhere) Chest.modData["Pathoschild.ChestsAnywhere/IsIgnored"] = "true";
            if (Game1.dayOfMonth % 7 == _mod.Config.GarbageDay) Chest.items.Clear();

            // Seed Random
            if (!int.TryParse(WhichCan, out var vanillaCanNumber)) vanillaCanNumber = 0;
            var garbageRandom = SeedRandom(vanillaCanNumber);

            // Mega/Double-Mega
            _mega = Game1.stats.getStat("trashCansChecked") > 20 && garbageRandom.NextDouble() < 0.01;
            _doubleMega = Game1.stats.getStat("trashCansChecked") > 20 && garbageRandom.NextDouble() < 0.002;
            if (_doubleMega || !_mega && !(garbageRandom.NextDouble() < 0.2 + Game1.player.DailyLuck + extraLuck))
                return;

            // Qi Beans
            if (Game1.random.NextDouble() <= 0.25 + extraLuck && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
            {
                _dropQiBeans = true;
                return;
            }

            // Vanilla Local Loot
            if (vanillaCanNumber >= 3 && vanillaCanNumber <= 7)
            {
                var localLoot = GetVanillaLocalLoot(garbageRandom, vanillaCanNumber, extraLuck);
                if (localLoot != -1)
                {
                    Chest.addItem(new Object(localLoot, 1));
                    Chest.playerChoiceColor.Value = RandomChestColor();
                    return;
                }
            }

            // Custom Local Loot
            if (garbageRandom.NextDouble() < 0.2 + Game1.player.DailyLuck + extraLuck)
            {
                var localItem = RandomLoot(garbageRandom);
                if (localItem != null)
                {
                    Chest.addItem(localItem.CreateItem());
                    Chest.playerChoiceColor.Value = RandomChestColor();
                    return;
                }
            }

            // Global Loot
            if (garbageRandom.NextDouble() < _mod.Config.GetRandomItemFromSeason)
            {
                var globalLoot = Utility.getRandomItemFromSeason(Game1.currentSeason, (int) (Tile.X * 653 + Tile.Y * 777), false);
                if (globalLoot != -1)
                {
                    Chest.addItem(new Object(globalLoot, 1));
                    Chest.playerChoiceColor.Value = RandomChestColor();
                    return;
                }
            }

            var globalItem = RandomLoot(garbageRandom);
            if (globalItem != null)
            {
                Chest.addItem(globalItem.CreateItem());
                Chest.playerChoiceColor.Value = RandomChestColor();
            }
        }

        private Color RandomChestColor()
        {
            if (Chest == null) return Color.DarkGray;
            var items = Chest.items.Shuffle();
            foreach (var item in items)
            {
                if (item.MatchesTagExt("color_red", true)
                    || item.MatchesTagExt("color_dark_red", true)) return Color.DarkRed;
                if (item.MatchesTagExt("color_pale_violet_red", true)) return Color.DarkViolet;
                if (item.MatchesTagExt("color_blue", true)) return Color.DarkBlue;
                if (item.MatchesTagExt("color_green", true)
                    || item.MatchesTagExt("color_dark_green", true)
                    || item.MatchesTagExt("color_jade", true)) return Color.DarkGreen;
                if (item.MatchesTagExt("color_brown", true)
                    || item.MatchesTagExt("color_dark_brown", true)) return Color.Brown;
                if (item.MatchesTagExt("color_yellow", true)
                    || item.MatchesTagExt("color_dark_yellow", true)) return Color.Yellow;
                if (item.MatchesTagExt("color_aquamarine", true)) return Color.Aquamarine;
                if (item.MatchesTagExt("color_purple", true)
                    || item.MatchesTagExt("color_dark_purple", true)) return Color.Purple;
                if (item.MatchesTagExt("color_cyan", true)) return Color.DarkCyan;
                if (item.MatchesTagExt("color_pink", true)) return Color.Pink;
                if (item.MatchesTagExt("color_orange", true)) return Color.DarkOrange;
            }

            return Color.Gray;
        }

        private SearchableItem RandomLoot(Random randomizer)
        {
            var lootTable = Game1.content.Load<Dictionary<string, double>>($"Mods/GarbageDay/Loot/{WhichCan}");
            if (!string.IsNullOrWhiteSpace(MapLoot))
            {
                lootTable = lootTable.Concat(Game1.content.Load<Dictionary<string, double>>($"Mods/GarbageDay/Loot/{MapLoot}"))
                    .ToDictionary(loot => loot.Key, loot => loot.Value);
            }

            if (!lootTable.Any())
                return null;

            var totalWeight = lootTable.Values.Sum();
            var targetIndex = randomizer.NextDouble() * totalWeight;
            double currentIndex = 0;
            foreach (var lootItem in lootTable)
            {
                currentIndex += lootItem.Value;
                if (currentIndex < targetIndex)
                    continue;
                return (_items ??= new ItemRepository().GetAll())
                    .Where(entry => entry.Item.MatchesTagExt(lootItem.Key))
                    .Shuffle()
                    .FirstOrDefault();
            }

            return null;
        }

        private static int GetVanillaLocalLoot(Random garbageRandom, int whichCan, float luck = 1f)
        {
            var item = -1;
            switch (whichCan)
            {
                case 3 when garbageRandom.NextDouble() < 0.2 + Game1.player.DailyLuck + luck:
                    return garbageRandom.NextDouble() < 0.05 ? 749 : 535;
                case 4 when garbageRandom.NextDouble() < 0.2 + Game1.player.DailyLuck + luck:
                    return 378 + garbageRandom.Next(3) * 2;
                case 5 when garbageRandom.NextDouble() < 0.2 + Game1.player.DailyLuck + luck && Game1.dishOfTheDay != null:
                    return Game1.dishOfTheDay.ParentSheetIndex != 217 ? Game1.dishOfTheDay.ParentSheetIndex : 216;
                case 6 when garbageRandom.NextDouble() < 0.2 + Game1.player.DailyLuck + luck:
                    return 223;
                case 7 when garbageRandom.NextDouble() < 0.2 + luck:
                    if (!Utility.HasAnyPlayerSeenEvent(191393)) item = 167;
                    if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater")
                        && !Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja"))
                    {
                        item = !(garbageRandom.NextDouble() < 0.25 + luck) ? 270 : 809;
                    }

                    break;
            }

            return item;
        }

        private static Random SeedRandom(int whichCan)
        {
            var garbageRandom = new Random((int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + 777 + whichCan * 77);
            var prewarm = garbageRandom.Next(0, 100);
            for (var k = 0; k < prewarm; k++)
            {
                garbageRandom.NextDouble();
            }

            prewarm = garbageRandom.Next(0, 100);
            for (var j = 0; j < prewarm; j++)
            {
                garbageRandom.NextDouble();
            }

            return garbageRandom;
        }
    }
}