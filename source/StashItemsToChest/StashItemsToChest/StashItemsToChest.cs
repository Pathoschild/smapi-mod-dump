using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Input;

namespace StashItemsToChest
{
    public class StashItemsToChest : Mod
    {
        private KeyboardState lastKeyboardState;

        public static StashItemsToChestConfig ModConfig { get; protected set; }

        public override void Entry(IModHelper helper)
        {
            base.Entry(helper);

            ModConfig = helper.ReadConfig<StashItemsToChestConfig>();
            StardewModdingAPI.Events.GameEvents.UpdateTick += UpdateTickEvent;

            lastKeyboardState = Keyboard.GetState();
        }

        //PhthaloBlue: these blocks of codes below are from Chest Pooling mod by mralbobo
        //repo link here: https://github.com/mralbobo/stardew-chest-pooling, they are useful so I use them
        static Chest getOpenChest()
        {
            if (Game1.activeClickableMenu == null) return null;

            ItemGrabMenu itemGrabMenu = Game1.activeClickableMenu as ItemGrabMenu;
            if (itemGrabMenu != null)
            {
                if (itemGrabMenu.behaviorOnItemGrab != null && itemGrabMenu.behaviorOnItemGrab.Target is Chest)
                {
                    return itemGrabMenu.behaviorOnItemGrab.Target as Chest;
                }
            }
            else
            {
                if (Game1.activeClickableMenu.GetType().Name == "ACAMenu")
                {
                    dynamic thing = (dynamic)Game1.activeClickableMenu;
                    if (thing != null && thing.chestItems != null)
                    {
                        Chest aChest = new Chest(true);
                        aChest.items = thing.chestItems;
                        return aChest;
                    }
                }
            }
            return null;
        }


        //PhthaloBlue: these are my codes
        void UpdateTickEvent(object sender, EventArgs e)
        {
            if(!Game1.game1.IsActive)
                return;

            if (Game1.currentLocation == null)
                return;

            KeyboardState currentKeyboardState = Keyboard.GetState();
            StashUp(this.lastKeyboardState, currentKeyboardState);
            this.lastKeyboardState = currentKeyboardState;
        }

        static void StashUp(KeyboardState lastKeyboardState, KeyboardState currentKeyboardState)
        {
            // If the stash key was just released.
            if (lastKeyboardState.IsKeyDown(ModConfig.stashKey) && currentKeyboardState.IsKeyUp(ModConfig.stashKey))
            {
                Chest OpenChest = getOpenChest();
                if (OpenChest == null || OpenChest.isEmpty())
                    return;

                foreach (Item chestItem in OpenChest.items)
                {
                    foreach (Item playerItem in Game1.player.items)
                    {
                        if (playerItem == null)
                            continue;

                        if (playerItem.canStackWith(chestItem))
                        {
                            OpenChest.grabItemFromInventory(playerItem, Game1.player);
                            break;
                        }
                    }
                }
            }
        }
    }

    public class StashItemsToChestConfig
    {
        public Keys stashKey = Keys.Tab;
    }
}
