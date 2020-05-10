using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using static DecraftingModCS.Utils;

namespace DecraftingModCS
{
    public class Config
    {
        public SButton[] Buttons = new SButton[] { SButton.Space };
    }

    public class ModEntry : Mod
    {
        public List<CraftableItem> Items;
        public Config Config;

        public override void Entry(IModHelper helper)
        {
            Monitor.Log("D E B U G", LogLevel.Debug);
            helper.Events.Input.ButtonPressed += OnPress;
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            Config = helper.ReadConfig<Config>();
            Items = new List<CraftableItem>();
        }

        private void SaveLoaded(object Sender, SaveLoadedEventArgs args)
        {
            Items = new List<CraftableItem>();

            foreach (KeyValuePair<string, string> kvpLoop in CraftingRecipe.craftingRecipes)
            {
                CraftingRecipe recipe = new CraftingRecipe(kvpLoop.Key, false);
                Item item = recipe.createItem();
                CraftableItem Item = new CraftableItem(item, recipe);
                string[] Ingradients = kvpLoop.Value.Split('/')[0].Split(' ');

                Monitor.Log("Recipe gives: " + Item.Item.Stack + "x " + Item.Item.DisplayName, LogLevel.Debug);
                               
                for (int i = 0; i < Ingradients.Length - 1; i += 2)
                {
                    Item I = ItemFromID(int.Parse(Ingradients[i]), int.Parse(Ingradients[i + 1]));
                    Item.Ingradients.Add(ItemFromID(int.Parse(Ingradients[i]), int.Parse(Ingradients[i + 1])));
                    Monitor.Log(I.Stack + "x " + I.DisplayName, LogLevel.Debug);
                }

                Monitor.Log("-----------------------------------------------------------", LogLevel.Debug);
                
                Items.Add(Item);
            }
        }

        private void OnPress(object Sender, ButtonPressedEventArgs args)
        {
            if (Context.IsWorldReady)
            {
                foreach (SButton btn in Config.Buttons)
                {
                    if (!Helper.Input.IsDown(btn)) return;
                }

                Item ToDecraft = Game1.player.CurrentItem;

                foreach (CraftableItem CItem in Items)
                {
                    if (CItem.Equal(ToDecraft))
                    {
                        Monitor.Log("Decrafting " + ToDecraft.DisplayName + " (id " + ToDecraft.ParentSheetIndex + ").", LogLevel.Debug);

                        Game1.player.removeItemFromInventory(ToDecraft);

                        foreach (Item ing in CItem.Ingradients)
                        {
                            Game1.player.addItemToInventory(ItemFromID(ing.ParentSheetIndex, ing.Stack * ToDecraft.Stack / CItem.Item.Stack));
                        }
                        
                        return;
                    }
                }
            }
        }
    }
}
