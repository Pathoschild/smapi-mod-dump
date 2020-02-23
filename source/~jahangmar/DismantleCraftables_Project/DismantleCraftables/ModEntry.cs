//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

using System;

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace DismantleCraftables
{
    public class ModEntry: Mod
    {
        private DismantleCraftablesConfig config;

        private delegate void Action();

        //private Dictionary<string, string> craftingRecipes;

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<DismantleCraftablesConfig>();
            if (!(config.loss >= 0))
            {
                Monitor.Log("value of 'loss' in config.json must be greater than 0%. Stopping to load mod.", LogLevel.Error);
                return;
            }

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            //craftingRecipes = Helper.Content.Load<Dictionary<string, string>>("Data//CraftingRecipes", ContentSource.GameContent);
        }

        void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady && e.Button.Equals(config.dismantle_key) && Game1.player.CurrentItem != null && Game1.activeClickableMenu == null)
            {
                Item item = Game1.player.CurrentItem;
                Monitor.Log($"Current Item is {item.Name} with stack size {item.Stack}" + ((item is StardewValley.Object) ? " and it is an object" : "" ), LogLevel.Trace);

                if (CraftingRecipe.craftingRecipes.TryGetValue(item.Name, out string raw))
                {
                    Game1.activeClickableMenu = new DismantleDialogueBox(Helper, Monitor, item, TryDismantling);
                }
                else
                {
                    Game1.showRedMessage(Helper.Translation.Get("msg.wrongitem"));
                }
            }
        }

        void TryDismantling()
        {
            Item item = Game1.player.CurrentItem;
            if (item != null && item is StardewValley.Object obj)
            {
                Monitor.Log("Trying to dismantle " + item.Name, LogLevel.Trace);
                if (CraftingRecipe.craftingRecipes.TryGetValue(item.Name, out string raw))
                {
                    string[] rawArray = (raw.Split('/'))[0].Split(' ');
                    int[] ingrediences = new int[rawArray.Length / 2];
                    int[] amounts = new int[rawArray.Length / 2];
                    for (int i = 0; i < rawArray.Length; i += 2)
                        ingrediences[i / 2] = Convert.ToInt32(rawArray[i]);
                    for (int i = 1; i < rawArray.Length; i += 2)
                        amounts[i / 2] = Convert.ToInt32(rawArray[i]);

                    string[] ingredienceNames = new string[ingrediences.Length];
                    List<StardewValley.Object> resources = new List<StardewValley.Object>();
                    for (int i = 0; i < ingrediences.Length; i++)
                    {
                        int amount = amounts[i] * item.Stack;
                        resources.Add(new StardewValley.Object(ingrediences[i], Convert.ToInt32(amount - amount * ((float) config.loss / (float) 100))));
                    }
                    Game1.player.removeItemFromInventory(Game1.player.CurrentItem);
                    foreach (StardewValley.Object resource in resources)
                    {
                        //Quality fertilzer needs generic fish item that cannot be obtained
                        if (resource.parentSheetIndex != -4)
                        {
                            if (Game1.player.couldInventoryAcceptThisObject(resource.ParentSheetIndex, resource.Stack))
                            {
                                Game1.player.addItemToInventory(resource);
                                Game1.addHUDMessage(new HUDMessage("", resource.Stack, true, Color.Black, resource));
                            }
                            else
                            {
                                Game1.createItemDebris(resource, Game1.player.getStandingPosition(), Game1.player.FacingDirection, null, -1);
                            }
                        }
                    }

                }
                else
                    Monitor.Log("No recipe found", LogLevel.Trace);
            }
        }


        class DismantleDialogueBox : DialogueBox
        {
            private IModHelper Helper;
            private IMonitor Monitor;
            private Action Action;

            public DismantleDialogueBox(IModHelper helper, IMonitor monitor, Item item, Action action)
                : base(helper.Translation.Get(item.Stack > 1 ? "dia.question" : "dia.question_single", new { item_Stack = item.Stack, item_DisplayName = item.DisplayName }), new List<Response>()
            {
                new Response("yes", helper.Translation.Get("dia.yes")),
                new Response("no", helper.Translation.Get("dia.no"))
            })
            {
                Helper = helper;
                Monitor = monitor;
                Action = action;
            }

            public override void receiveLeftClick(int x, int y, bool playSound = true)
            {
                int sel = Helper.Reflection.GetField<int>(this, "selectedResponse").GetValue();
                if (sel == 0)
                {
                    Action();
                    playSound = false;
                    Game1.playSound("barrelBreak");
                }
                base.receiveLeftClick(x, y, playSound);
            }
        }


    }


}
