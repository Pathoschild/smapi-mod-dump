using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Igorious.StardewValley.DynamicApi2.Extensions;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicApi2.Events
{
    public sealed class InventoryEvents
    {
        static InventoryEvents()
        {
            MenuEvents.MenuChanged += CraftedObjectChangedHandler.OnMenuChanged;
        }

        public static event Action<ObjectEventArgs> CraftedObjectChanged;

        private static class CraftedObjectChangedHandler
        {
            private static readonly Lazy<FieldInfo> CraftingPageHeldItemField = typeof(CraftingPage).GetLazyInstanceField("heldItem");
            private static readonly Lazy<FieldInfo> GameMenuPagesField = typeof(GameMenu).GetLazyInstanceField("pages");

            private static CraftingPage CraftingMenu { get; set; }
            private static Object PreviousCraftedObject { get; set; }

            public static void OnMenuChanged(object sender, EventArgsClickableMenuChanged e)
            {
                if (!(e.NewMenu is GameMenu)) return;

                CraftingMenu = e.NewMenu.GetFieldValue<List<IClickableMenu>>(GameMenuPagesField).OfType<CraftingPage>().First();
                GameEvents.UpdateTick += OnCrafted;
                MenuEvents.MenuClosed += OnCraftingMenuClosed;
            }

            private static void OnCraftingMenuClosed(object sender, EventArgsClickableMenuClosed eventArgsClickableMenuClosed)
            {
                MenuEvents.MenuClosed -= OnCraftingMenuClosed;
                GameEvents.UpdateTick -= OnCrafted;
                CraftingMenu = null;
                PreviousCraftedObject = null;
            }

            private static void OnCrafted(object sender, EventArgs e)
            {
                var heldItem = CraftingMenu.GetFieldValue<Object>(CraftingPageHeldItemField);
                if (PreviousCraftedObject == heldItem) return;

                var args = new ObjectEventArgs(heldItem);
                CraftedObjectChanged?.Invoke(args);
                if (heldItem != args.Object)
                {
                    CraftingMenu.SetFieldValue(CraftingPageHeldItemField, PreviousCraftedObject = args.Object);
                }
            }
        }
    }
}