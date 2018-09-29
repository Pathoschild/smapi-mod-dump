using System;
using System.Collections.Generic;
using System.Reflection;
using Igorious.StardewValley.DynamicApi2.Extensions;
using Igorious.StardewValley.DynamicApi2.Services;
using Igorious.StardewValley.DynamicApi2.Utils;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicApi2.Compatibility
{
    internal sealed class CjbItemSpawnerCompatibilityLayout
    {
        private const string CjbItemSpawnerID = "CJBItemSpawner";

        private static readonly Lazy<CjbItemSpawnerCompatibilityLayout> Lazy = new Lazy<CjbItemSpawnerCompatibilityLayout>(() => new CjbItemSpawnerCompatibilityLayout());
        public static CjbItemSpawnerCompatibilityLayout Instance => Lazy.Value;
        private CjbItemSpawnerCompatibilityLayout() { }

        private static FieldInfo _itemListField;
        private static MethodInfo _loadInventoryMethod;
        private IClickableMenu CurrentMenu { get; set; }

        public void Initialize()
        {
            if (!Smapi.GetModRegistry().IsLoaded(CjbItemSpawnerID)) return;
            MenuEvents.MenuChanged += OnMenuChanged;
        }

        private void OnMenuChanged(object s, EventArgsClickableMenuChanged e)
        {
            if (e.NewMenu?.GetType().FullName != "CJBItemSpawner.ItemMenu") return;

            Log.Trace("Overriding CJB Item Spawner menu items...");

            CurrentMenu = e.NewMenu;
            var itemListField = _itemListField ?? (_itemListField = CurrentMenu.GetType().GetInstanceField("itemList"));
            var itemList = CurrentMenu.GetFieldValue<List<Item>>(itemListField);
            for (var i = 0; i < itemList.Count; ++i)
            {
                var item = itemList[i] as Object;
                if (item == null) continue;
                itemList[i] = Wrapper.Instance.Wrap(item);
            }

            var loadInventoryMethod = _loadInventoryMethod ?? (_loadInventoryMethod = CurrentMenu.GetType().GetInstanceMethod("loadInventory"));
            loadInventoryMethod.Invoke(CurrentMenu, null);

            Log.Trace("Overrided CJB Item Spawner menu items.");
        }
    }
}