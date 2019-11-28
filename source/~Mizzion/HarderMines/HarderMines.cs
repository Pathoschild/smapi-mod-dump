using System;
using System.Collections.Generic;
using System.Linq;
using HarderMines.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;

namespace HarderMines
{
    public class HarderMines : Mod
    {

        //Fields
        private static Random _rnd;
        private Dictionary<int, List<Treasure>> _bossInventory;
        private Dictionary<int, int> _bossInventoryState;
        private Dictionary<Monster, Action> _bossHpEvents;
        private List<Boss> _monsterSpawner;


        /// <summary>
        /// Method that gets called first in the mod.
        /// </summary>
        /// <param name="helper">Helper class</param>
        public override void Entry(IModHelper helper)
        {
            //Set up the fields so they wont return NRE
            _bossInventory = new Dictionary<int, List<Treasure>>();
            _bossInventoryState = new Dictionary<int, int>();
            _bossHpEvents = new Dictionary<Monster, Action>();
            _monsterSpawner = new List<Boss>();

            //Events
            var events = helper.Events;
            //GameLoop Events
            events.GameLoop.UpdateTicked += OnUpdateTicked;
            events.GameLoop.Saved += OnSaved;
            events.GameLoop.SaveLoaded += OnSaveLoaded;
            //Player Events
            events.Player.Warped += OnWarped;
            events.Player.InventoryChanged += OnInventoryChanged;
        }


        //Private Methods

        //Event Methods
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Game1.inMine)
                return;

            List<Monster> monsters = new List<Monster>();

            foreach (var m in _bossHpEvents)
            {
                List<Boss> bosses = new List<Boss>();

                foreach (Boss bs in _monsterSpawner)
                {
                    if (m.Key.Health <= bs.TriggerHp)
                    {
                        for (int i = 0; i < bs.Count; i++)
                        {
                            int monX = (int)m.Key.getTileLocation().X;//getTileX();
                            int monY = (int) m.Key.getTileLocation().Y;//getTileY();

                            for (int x = monX - 3; x < monX + 3; x++)//Unreachable code.....need to fix
                            {
                                for (int y = monY - 3; y < monY + 3; y++)
                                {
                                    if (Game1.mine.isTileClearForMineObjects(new Vector2(x, y)))
                                    {
                                        Monitor.Log("Tile was clear, trying to spawn monster.", LogLevel.Trace);
                                        Game1.mine.tryToAddMonster(new GreenSlime(new Vector2(x, y), Color.Green), x, y);
                                        break;
                                    }
                                }
                            }
                            bosses.Add(bs);
                        }
                    }
                }

                foreach (var bs in bosses)
                    _monsterSpawner.Remove(bs);
                if (m.Key.Health <= 0)
                {
                    m.Value();
                    monsters.Add(m.Key);
                }

            }
            foreach(var boss in monsters)
                _bossHpEvents.Remove(boss);
        }

        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (!Game1.inMine || Game1.currentLocation.Name.Contains("UndergroundMine"))
                return;

            int mineLevel = Game1.mine.mineLevel;
            if (Game1.mine.objects.Any() && mineLevel == 10 && _bossInventoryState[0] == 1)
            {
                _bossInventoryState.Remove(0);
                List<Treasure> treasureList = new List<Treasure>();
                foreach (var mo in Game1.mine.objects.Pairs.ToList())
                {
                    Item mineItem = mo.Value;
                    treasureList.Add(new Treasure(mineItem.ParentSheetIndex, mineItem.Stack));
                }
                _bossInventory.Add(0, treasureList);
            }
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation is MineShaft && MultipleOf(Game1.mine.mineLevel, 10))
                CreateLevel();
        }

        private void OnSaved(object sender, SavedEventArgs e)
        {
            //We either read or set up _data.
            SaveData data = Helper.Data.ReadSaveData<SaveData>("HarderMines") ?? new SaveData();

            //Now we set the data
            for (int i = 0; i < 12; i++)
            {
                string s = $"{i}|{_bossInventoryState[i]}";
                foreach (var ti in _bossInventory[i])
                {
                    if (ti.Id != -1)
                        s += $"|{ti.Id}|{ti.Count}";
                }
                data.Treasures.Add(s);
            }

            //Now we write to it.
            Helper.Data.WriteSaveData("HarderMines", data);
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            SaveData data = Helper.Data.ReadSaveData<SaveData>("HarderMines") ?? new SaveData();

            


            List<string> dataOut = data.Treasures;

            foreach (var s in dataOut)
            {
                Monitor.Log($"Let's just output this for now.\n{s}"); 
            }
            //Set up the treasure :P
            for (int i = 0; i < 12; i++)
            {
                if (!_bossInventoryState.ContainsKey(i))
                    InitiateTreasure(i);
            }
        }
        //End of Event Methods 

        //Methods that need to be ran by code
        private void InitiateTreasure(int n)
        {
            _bossInventoryState.Add(n, 0);
            List<Treasure> tiList = new List<Treasure>();

            if (n == 0)
            {
                tiList.Add(new Treasure(334, 5));
                tiList.Add(new Treasure(459, 1));
                tiList.Add(new Treasure(506, 1));
            }
            else
                tiList.Add(new Treasure(350, 3));
            _bossInventory.Add(n, tiList);
        }

        private void CreateLevel()
        {
            if (!Game1.inMine || Game1.currentLocation.Name.Contains("UndergroundMine"))
                return;

            int height = Game1.mine.Map.DisplayHeight;
            int width = Game1.mine.Map.DisplayWidth;
            bool triggered = false;

            for (int x = 1; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    if (Game1.mine.isTileClearForMineObjects(new Vector2(x, y)))
                    {
                        BigSlime bg = new BigSlime(new Vector2(x, y), 0){Health = 400, Speed = 6};

                        bg.jitteriness.Value = 100.0f;
                        bg.ExperienceGained = 40;
                        bg.DamageToFarmer = 12;
                        bg.c.Value = new Color(0.0f, 50f, 0.0f);
                        bg.glowingColor = new Color(byte.MaxValue, 0.0f, 0.0f);
                        bg.glowRate = 20.0f;
                        bg.isGlowing = true;
                        bg.glowingTransparency = 0.0f;
                        Game1.mine.tryToAddMonster(bg, x, y);

                        triggered = true;
                        _bossHpEvents.Add(bg, BossDied);
                        _monsterSpawner.Add(new Boss(new GreenSlime(), 2, 300));
                        _monsterSpawner.Add(new Boss(new GreenSlime(), 4, 200));
                        _monsterSpawner.Add(new Boss(new GreenSlime(), 6, 100));
                        _monsterSpawner.Add(new Boss(new GreenSlime(), 16, 20));
                    }
                    if(triggered)
                        break;
                }
                if (triggered)
                    break;
            }

            var objects = Game1.currentLocation.objects.Pairs;
            if (objects.Any())
            {
                var pair = objects.FirstOrDefault();
                Game1.mine.removeObject(pair.Key, false);
                Monitor.Log("HarderMines removed game chest.");
            }

            if (_bossInventoryState[0] != 1 || _bossInventory[0].Count <= 0)
                return;

            Chest chest = new Chest("interactive", new Vector2(9f, 9f), Game1.mine);

            foreach (Treasure t in _bossInventory[0])
            {
                switch (t.Id)
                {
                    case -1:
                        continue;
                    case 506:
                        chest.addItem(new Boots(506));
                        continue;
                    default:
                        chest.addItem(new StardewValley.Object(Vector2.Zero, t.Id, t.Count));
                        continue;
                }
            }
            Game1.mine.objects.Add(new Vector2(9f, 9f), chest);
            Monitor.Log("Added chest.");
            _bossInventoryState[0] = 1;
        }

        private void BossDied()
        {
            if (_bossInventoryState[0] == 0)
            {
                Chest chest = new Chest("interactive", new Vector2(9f, 9f), Game1.mine);

                foreach (var ti in _bossInventory[0])
                {
                    if (ti.Id == 506)
                        chest.addItem(new Boots(506));
                    else
                        chest.addItem(new StardewValley.Object(Vector2.Zero, ti.Id, ti.Count));
                }
                Game1.mine.objects.Add(new Vector2(9f,9f), chest);
                _bossInventoryState[0] = 1;
            }
            Monitor.Log("Boss has died.", LogLevel.Trace);
            Game1.playSound("powerup");
        }
        //Personal Methods to make life easier
        public bool MultipleOf(int in1, int in2)
        {
            return (in1 % in2) == 0;
        }
        //Struct

        /// <summary>
        /// Struct for Monsters. Should probably rename it.... but oh well
        /// </summary>
        private struct Boss
        {
            public readonly int Count;
            public readonly int TriggerHp;

            public Boss(Monster m, int count, int triggerHp)
            {
                Count = count;
                TriggerHp = triggerHp;
            }
        }
    }
}
