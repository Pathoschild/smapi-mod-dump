using DailyTasksReport.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SObject = StardewValley.Object;

namespace DailyTasksReport.Tasks
{
    public class ObjectsTask : Task
    {
        private static ModConfig _config;
        private readonly ObjectsTaskId _id;
        private bool _anyObject;

        private static ObjectsTaskId _who = ObjectsTaskId.None;
        private static bool _hasLuremaster;

        private static readonly Dictionary<GameLocation, List<Vector2>> Machines =
            new Dictionary<GameLocation, List<Vector2>>();

        private static readonly Dictionary<GameLocation, List<Vector2>> CrabPots =
            new Dictionary<GameLocation, List<Vector2>>();

        internal ObjectsTask(ModConfig config, ObjectsTaskId id)
        {
            _config = config;
            _id = id;

            SettingsMenu.ReportConfigChanged += SettingsMenu_ReportConfigChanged;
            LocationEvents.ObjectsChanged += LocationEvents_ObjectsChanged;
        }

        private void SettingsMenu_ReportConfigChanged(object sender, EventArgs e)
        {
            switch (_id)
            {
                case ObjectsTaskId.UncollectedCrabpots:
                    Enabled = _config.UncollectedCrabpots;
                    break;

                case ObjectsTaskId.NotBaitedCrabpots:
                    _hasLuremaster = Game1.player.professions.Contains(11);
                    Enabled = _config.NotBaitedCrabpots && !_hasLuremaster;
                    break;

                case ObjectsTaskId.UncollectedMachines:
                    Enabled = _config.Machines.ContainsValue(true) || _config.Cask > 0;
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Object type not implemented");
            }
        }

        protected override void FirstScan()
        {
            if (_who == ObjectsTaskId.None)
                _who = _id;
            else if (_who != _id)
                return;

            foreach (var location in Game1.locations)
            {
                if (!Machines.ContainsKey(location))
                    Machines[location] = new List<Vector2>();
                if (location.waterTiles != null && !CrabPots.ContainsKey(location))
                    CrabPots[location] = new List<Vector2>();

                foreach (var @object in location.Objects.Pairs)
                    if (_config.Machines.ContainsKey(@object.Value.name) || @object.Value is Cask)
                        Machines[location].Add(@object.Key);
                    else if (@object.Value is CrabPot)
                        CrabPots[location].Add(@object.Key);

                if (!(location is BuildableGameLocation farm)) continue;

                foreach (var building in farm.buildings)
                {
                    var indoors = building.indoors.Value;
                    if (indoors == null) continue;

                    if (!Machines.ContainsKey(indoors))
                        Machines[indoors] = new List<Vector2>();

                    foreach (var @object in indoors.Objects.Pairs)
                        if (_config.Machines.ContainsKey(@object.Value.name))
                            Machines[indoors].Add(@object.Key);
                }
            }
        }

        private static void LocationEvents_ObjectsChanged(object sender, EventArgsLocationObjectsChanged e)
        {
            Vector2 pos;
            SObject obj;
            var loc = Game1.currentLocation;
            if (e.Location is MineShaft || Game1.newDay) return;

            foreach (var item in e.Added)
            {
                pos = item.Key;
                obj = item.Value;
                if ((_config.Machines.ContainsKey(obj.name) || obj is Cask) &&
                        Machines.TryGetValue(loc, out var list))
                    list.Add(pos);
                else if (obj is CrabPot && CrabPots.TryGetValue(loc, out list))
                    list.Add(pos);
            }

            foreach (var item in e.Removed)
            {
                pos = item.Key;
                obj = item.Value;
                if (Machines.TryGetValue(loc, out var list))
                    list.Remove(pos);
                if (CrabPots.TryGetValue(loc, out list))
                    list.Remove(pos);
                break;
            }
        }

        public override string GeneralInfo(out int usedLines)
        {
            usedLines = 0;
            _anyObject = false;

            if (!Enabled) return "";

            int count;
            usedLines = 1;

            switch (_id)
            {
                case ObjectsTaskId.UncollectedCrabpots:
                    count = (from pair in CrabPots
                             from pos in pair.Value
                             where (pair.Key.objects[pos] as CrabPot)?.heldObject.Value != null
                             select 1).Count();
                    if (count > 0)
                    {
                        _anyObject = true;
                        return $"Crabpots ready to collect: {count}^";
                    }
                    break;

                case ObjectsTaskId.NotBaitedCrabpots:
                    if (_hasLuremaster) break;
                    count = (from pair in CrabPots
                             from pos in pair.Value
                             where (pair.Key.objects[pos] as CrabPot)?.bait.Value == null
                             select 1).Count();
                    if (count > 0)
                    {
                        _anyObject = true;
                        return $"Crabpots not baited: {count}^";
                    }
                    break;

                case ObjectsTaskId.UncollectedMachines:
                    SObject machine;
                    count = (from pair in Machines
                             from pos in pair.Value
                             where pair.Key.objects.TryGetValue(pos, out machine) && MachineReady(machine)
                             select 1).Count();
                    if (count > 0)
                    {
                        _anyObject = true;
                        return $"Uncollected machines: {count}^";
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Object type not implemented");
            }

            usedLines = 0;
            return "";
        }

        private static bool MachineReady(SObject o)
        {
            if (o != null)
            {
                return
                    (_config.Machines.TryGetValue(o.Name, out var enabled) && enabled && o.readyForHarvest.Value) ||
                    (o is Cask cask && cask.heldObject.Value?.Quality >= _config.Cask);
            }
            return false;
        }

        public override string DetailedInfo(out int usedLines, out bool skipNextPage)
        {
            skipNextPage = false;
            usedLines = 0;

            if (!Enabled || !_anyObject) return "";

            var stringBuilder = new StringBuilder();

            switch (_id)
            {
                case ObjectsTaskId.UncollectedCrabpots:
                    stringBuilder.Append("Uncollected crabpots:^");
                    usedLines++;
                    EchoForCrabpots(ref stringBuilder, ref usedLines, c => c.heldObject.Value != null);
                    break;

                case ObjectsTaskId.NotBaitedCrabpots:
                    stringBuilder.Append("Crabpots still not baited:^");
                    usedLines++;
                    EchoForCrabpots(ref stringBuilder, ref usedLines, c => c.bait.Value == null);
                    break;

                case ObjectsTaskId.UncollectedMachines:
                    stringBuilder.Append("Machines ready to collect:^");
                    usedLines++;
                    EchoForMachines(ref stringBuilder, ref usedLines);
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Object type not implemented");
            }

            return stringBuilder.ToString();
        }

        private static void EchoForCrabpots(ref StringBuilder stringBuilder, ref int usedLines,
            Predicate<CrabPot> predicate)
        {
            foreach (var location in CrabPots)
                foreach (var position in location.Value)
                {
                    if (!(location.Key.objects[position] is CrabPot cp) || !predicate.Invoke(cp)) continue;
                    stringBuilder.Append($"{location.Key.Name} ({position.X}, {position.Y})^");
                    usedLines++;
                }
        }

        private static void EchoForMachines(ref StringBuilder stringBuilder, ref int usedLines)
        {
            foreach (var location in Machines)
                foreach (var position in location.Value)
                {
                    if (!location.Key.objects.TryGetValue(position, out var machine)) continue;
                    if (!MachineReady(machine)) continue;

                    var heldObject = machine.heldObject.Value;

                    var quality = "";
                    if (machine is Cask cask)
                    {
                        quality = heldObject.Quality == 1 ? "Silver "
                            : heldObject.Quality == 2 ? "Gold "
                            : heldObject.Quality == 4 ? "Iridium "
                            : "";
                    }
                    stringBuilder.Append(
                        $"{machine.name} with {quality}{heldObject.Name} at {location.Key.Name} ({position.X}, {position.Y})^");
                    usedLines++;
                }
        }

        public override void Draw(SpriteBatch b)
        {
            if (_who != _id) return;

            var x = Game1.viewport.X / Game1.tileSize;
            var xLimit = (Game1.viewport.X + Game1.viewport.Width) / Game1.tileSize;
            var yStart = Game1.viewport.Y / Game1.tileSize;
            var yLimit = (Game1.viewport.Y + Game1.viewport.Height) / Game1.tileSize + 1;
            for (; x <= xLimit; ++x)
                for (var y = yStart; y <= yLimit; ++y)
                {
                    if (!Game1.currentLocation.objects.TryGetValue(new Vector2(x, y), out var o)) continue;

                    var v = new Vector2(o.TileLocation.X * Game1.tileSize - Game1.viewport.X + Game1.tileSize / 8f,
                        o.TileLocation.Y * Game1.tileSize - Game1.viewport.Y - Game1.tileSize * 5 / 4f);

                    var heldObject = o.heldObject.Value;
                    switch (o)
                    {
                        case Cask cask when _config.DrawBubbleCask && heldObject?.Quality > 0 &&
                                            heldObject.Quality >= _config.Cask &&
                                            heldObject.Quality < 4:
                            DrawBubble(b, Game1.objectSpriteSheet,
                                Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                                    heldObject.ParentSheetIndex, 16, 16), v);
                            break;

                        case CrabPot cp when _config.DrawBubbleCrabpotsNotBaited && cp.bait.Value == null && !_hasLuremaster:
                            DrawBubble(b, Game1.objectSpriteSheet, new Rectangle(209, 450, 13, 13), v);
                            break;

                        default:
                            break;
                    }
                }
        }

        public override void Clear()
        {
            switch (_id)
            {
                case ObjectsTaskId.UncollectedCrabpots:
                    Enabled = _config.UncollectedCrabpots;
                    break;

                case ObjectsTaskId.NotBaitedCrabpots:
                    _hasLuremaster = Game1.player.professions.Contains(11);
                    Enabled = _config.NotBaitedCrabpots && !_hasLuremaster;
                    break;

                case ObjectsTaskId.UncollectedMachines:
                    Enabled = _config.Machines.ContainsValue(true) || _config.Cask > 0;
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Object type not implemented");
            }

            if (_who != _id) return;

            foreach (var keyValuePair in Machines)
                keyValuePair.Value.Clear();
            foreach (var keyValuePair in CrabPots)
                keyValuePair.Value.Clear();
        }
    }

    public enum ObjectsTaskId
    {
        None = -1,
        UncollectedCrabpots = 0,
        NotBaitedCrabpots = 1,
        UncollectedMachines = 2
    }
}