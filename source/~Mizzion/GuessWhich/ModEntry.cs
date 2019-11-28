using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace GuessWhich
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            

            //helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += ButtonPressed;

            //Set up command to empty the chests out :P
            helper.ConsoleCommands.Add("guess_clear", "Destroys Chests.\n\nUsage: guess_clear <value>\n- value: Either 'gift' or 'empty'.\nWill destroy the associated chest.", this.DoClear);
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (e.IsDown(SButton.N))
            {
                Spawn(1);
            }
        }

        private void Spawn(int itemId)
        {
            foreach (var loc in Game1.locations)
            {
                for (int xTile = 0; xTile < loc.Map.Layers[0].LayerWidth; ++xTile)
                {
                    for (int yTile = 0; yTile < loc.Map.Layers[0].LayerHeight; ++yTile)
                    {
                        string prop = loc.doesTileHaveProperty((int)xTile, (int)yTile, "Diggable", "Back");
                        if (prop != null || prop == null)
                        {
                            if (loc.isTileLocationTotallyClearAndPlaceable(xTile, yTile))
                            {
                                Cask c = new Cask(new Vector2(xTile, yTile));
                                
                                    loc.objects.Add(new Vector2(xTile, yTile), c);
                                
                            }
                        }
                    }
                }
            }
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            int chest1s = 0;
            int chest2s = 0;
            Random rnd = new Random();
            
            //Clear the chests first 
            DoClear();

            foreach (var loc in Game1.locations)
            {
                for (int xTile = 0; xTile < loc.Map.Layers[0].LayerWidth; ++xTile)
                {
                    for (int yTile = 0; yTile < loc.Map.Layers[0].LayerHeight; ++yTile)
                    {
                        string prop = loc.doesTileHaveProperty((int) xTile, (int) yTile, "Diggable", "Back");
                        if (prop != null)
                        {
                            if (loc.isTileLocationTotallyClearAndPlaceable(xTile, yTile))
                            {
                                if (rnd.Next(1, 500) <= 20)
                                {
                                    var obj = Game1.objectInformation;
                                    var objList = obj.ToList()[rnd.Next(obj.Count)];
                                    Chest c = new Chest(0,
                                        new List<Item>() {new SObject(objList.Key, rnd.Next(1, 15), false, -1, 0)},
                                        new Vector2(xTile, yTile), true, 1)
                                    {
                                        Tint = new Color(rnd.Next(255), rnd.Next(255), rnd.Next(255))
                                    };
                                    loc.objects.Add(new Vector2(xTile, yTile), c);
                                   chest2s++;
                                }
                                else
                                {
                                    Chest c = new Chest(true);
                                    loc.objects.Add(new Vector2(xTile, yTile), c);
                                   chest1s++;
                                }
                            }
                        }
                    }
                }
            }
            Game1.showGlobalMessage($"Totals:\nEmpty Chests: {chest1s}\nGift Chest:{chest2s}\n \nGood Luck!");
        }

        private void DoClear(string command, string[] args)
        {
            string which = args[0].ToLower();
            int clearCount = 0;
            switch (which)
            {
                case "empty"://Works, needs to be cleaned up though
                    
                    foreach (var loc in Game1.locations)
                    {
                        var obj = loc.objects;
                        foreach (var o in obj.Pairs.Where(ob => ob.Value is Chest chest && chest.items.Count == 0)
                            .ToList())
                        {
                            loc.objects.Remove(new Vector2(o.Key.X, o.Key.Y));
                            clearCount++;
                        }
                    }
                    Monitor.Log($"Cleared {clearCount} empty chests.");
                    break;
                case "gift":
                    foreach (var loc in Game1.locations)
                    {
                        var obj = loc.objects;
                        foreach (var o in obj.Pairs.Where(ob => ob.Value is Chest chest && chest.giftbox.Value)
                            .ToList())
                        {
                            loc.objects.Remove(new Vector2(o.Key.X, o.Key.Y));
                            clearCount++;
                        }
                            
                    }

                    Monitor.Log($"Cleared {clearCount} gift chests.");
                    break;
                default:
                    Monitor.Log($"Command value was incorrect. It needs to be guess_clear gift or guess_clear empty");
                    break;
            }
        }

        private void DoClear()
        {
            int clearCount = 0;
            //Clear up Chests
            foreach (var loc in Game1.locations)
            {
                var obj = loc.objects;
                foreach (var o in obj.Pairs.Where(ob => ob.Value is Chest chest && chest.items.Count == 0)
                    .ToList())
                {
                    loc.objects.Remove(new Vector2(o.Key.X, o.Key.Y));
                    clearCount++;
                }
            }
            Monitor.Log($"Cleared {clearCount} empty chests.");
            clearCount = 0;
            //Now we do the gifts
            foreach (var loc in Game1.locations)
            {
                var obj = loc.objects;
                foreach (var o in obj.Pairs.Where(ob => ob.Value is Chest chest && chest.giftbox.Value)
                    .ToList())
                {
                    loc.objects.Remove(new Vector2(o.Key.X, o.Key.Y));
                    clearCount++;
                }

            }

            Monitor.Log($"Cleared {clearCount} gift chests.");
        }
    }
}
