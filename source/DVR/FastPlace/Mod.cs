using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Linq;

namespace FastPlace
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.InventoryChanged += InventoryChanged;
            helper.Events.World.ObjectListChanged += ObjectChanged;
        }

        private StardewValley.Object lastPlaced;

        private void ObjectChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (e.IsCurrentLocation && e.Removed.Count() == 0 && e.Added.Count() == 1)
            {
                var item = e.Added.Single().Value;
                if (item.bigCraftable.Value && item.isPlaceable())
                {
                    lastPlaced = item;
                }
            }
        }

        private void InventoryChanged(object sender, InventoryChangedEventArgs e)
        {

            if (e.IsLocalPlayer && e.Removed.Count() == 1 && e.Added.Count() == 0 && e.QuantityChanged.Count() == 0)
            {
                var item = e.Removed.Single() as StardewValley.Object;
                if (item == null || item.ParentSheetIndex != lastPlaced?.ParentSheetIndex)
                {
                    return;
                }
                if (e.Player.hasItemInInventoryNamed(item.Name))
                {
                    foreach (var it in e.Player.Items.Select((x, i) => new { Value = x, Index = i }))
                    {
                        var idx = it.Index;
                        var itm = it.Value;
                        if (itm?.Name == item.Name) {
                            e.Player.Items[e.Player.CurrentToolIndex] = itm;
                            e.Player.Items[idx] = null; 
                            break;
                        }
                    }       
                }
            }
            
        }
    }
}
