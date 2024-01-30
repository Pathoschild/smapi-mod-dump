/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nickmartin1ee7/RefundStaircases
**
*************************************************/

using System.Linq;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Locations;

namespace RefundStaircases
{
    public class ModEntry : Mod
    {
        private Item? _lastKnownStaircase;
        private int _usedStaircases;
        private bool _inMines;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += (o, e) =>
            {
                if (e.OldLocation is MineShaft
                    && e.NewLocation is not MineShaft
                    && _lastKnownStaircase is not null
                    && _usedStaircases > 0)
                {
                    RefundStaircases(e);
                }
                else if (e.NewLocation is MineShaft)
                {
                    _inMines = true;
                }
            };

            helper.Events.Player.InventoryChanged += (o, e) =>
            {
                if (!_inMines)
                    return;

                var stairCaseStacksChanges = e.QuantityChanged
                    .Where(stack => stack.Item.Name == "Staircase");

                if (stairCaseStacksChanges.Any()
                    || e.Removed.Any(item => item.Name == "Staircase"))
                {
                    if (stairCaseStacksChanges.All(stack => stack.NewSize < stack.OldSize))
                    {
                        var changedStaircases = stairCaseStacksChanges
                            .Select(stack => stack.Item);

                        _lastKnownStaircase = changedStaircases.FirstOrDefault();
                        _usedStaircases += stairCaseStacksChanges.Sum(stack => stack.OldSize) - stairCaseStacksChanges.Sum(stack => stack.NewSize);
                    }

                    var removedStaircase = e.Removed.FirstOrDefault(item => item.Name == "Staircase");

                    if (removedStaircase is { Stack: > 0 })
                    {
                        _lastKnownStaircase = removedStaircase;
                        _usedStaircases++;
                    }

                }
                else if (e.Added.Any(item => item.Name == "Staircase"))
                {
                    _lastKnownStaircase = e.Added.First(item => item.Name == "Staircase");

                    if (_usedStaircases > 0)
                        _usedStaircases--;
                }
            };
        }

        private void RefundStaircases(WarpedEventArgs e)
        {
            var heldStairs = e.Player.Items.FirstOrDefault(item => item?.Name == "Staircase");

            if (heldStairs is not null)
            {
                heldStairs.Stack += _usedStaircases;
                e.Player.removeItemFromInventory(heldStairs);
                e.Player.addItemByMenuIfNecessary(heldStairs);
            }
            else if (_lastKnownStaircase is not null)
            {
                _lastKnownStaircase.Stack = _usedStaircases;
                e.Player.addItemByMenuIfNecessary(_lastKnownStaircase);
            }

            Monitor.Log($"Refunded {_usedStaircases} staircase(s).", LogLevel.Info);

            _usedStaircases = 0;
            _inMines = false;
        }
    }
}