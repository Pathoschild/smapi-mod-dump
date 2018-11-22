using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;

namespace CapitalistSplitMoney
{
    internal class MainBinListener
    {
        public static void OnShipItem(Item item, Farmer player)
        {
            if (Game1.IsMasterGame)
            {
				BinListener.Unregister();

                Console.WriteLine($"Host local bin add {item.Name} x{item.Stack}");

                if (!ModEntry.OldItems.ContainsKey(item))//Can be called twice by the game.
                    ModEntry.OldItems.Add(item, player.UniqueMultiplayerID);
            }
        }

        public static void OnRemoveItem(Item item)
        {
            if (Game1.IsMasterGame)
            {
				BinListener.Unregister();

				bool removed = ModEntry.OldItems.Remove(item);
                Console.WriteLine($"Host local remove {item.Name} x{item.Stack}");
            }
        }
    }

    #region Listeners
    class BinListener1 : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(ShippingBin), "shipItem", new Type[] { typeof(Item), typeof(Farmer) });

        public static bool Prefix(Item i, Farmer who)
        {
            //Console.WriteLine($"ShippingBin.shipItem(item, farmer) called. item_null={i == null}, who_null={who == null}");
            
            if (i != null && who != null)
                MainBinListener.OnShipItem(i, who);

            return true;
        }
    }

    class BinListener2 : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Farm), "shipItem", new Type[] { typeof(Item), typeof(Farmer) });

        public static bool Prefix(Item i, Farmer who)
        {
            //Console.WriteLine($"Farm.shipItem(item, farmer) called. item_null={i == null}, who_null={who == null}");

            if (i != null && who != null)
                MainBinListener.OnShipItem(i, who);

            return true;
        }
    }

    class BinListener3 : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(ShippingBin), "leftClicked", new Type[] { });

        public static bool Prefix(Farm ___farm, ShippingBin __instance)
        {
            if (___farm != null && Game1.player.ActiveObject != null && Game1.player.ActiveObject.canBeShipped() && Vector2.Distance(Game1.player.getTileLocation(), new Vector2((float)(int)__instance.tileX.Value + 0.5f, (float)(int)__instance.tileY.Value + 0.5f)) <= 2.25f)
            {
                //Console.WriteLine("ShippingBin.leftClicked");
                MainBinListener.OnShipItem(Game1.player.ActiveObject, Game1.player);
            }

            return true;
        }
    }
    
    class BinListener4 : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Farm), "leftClick", new Type[] { typeof(int), typeof(int), typeof(Farmer) });

        public static bool Prefix(int x, int y, Farmer who)
        {
            if (who.ActiveObject != null && x / 64 >= 71 && x / 64 <= 72 && y / 64 >= 13 && y / 64 <= 14 && who.ActiveObject.canBeShipped() && Vector2.Distance(who.getTileLocation(), new Vector2(71.5f, 14f)) <= 2f)
            {
                //Console.WriteLine("Farm.leftClick");
                MainBinListener.OnShipItem(who.ActiveObject, who);
            }

            return true;
        }
    }

    //public override void receiveLeftClick(int x, int y, bool playSound = true)
    //sub on prefix, unsub on postfix
    class BinListener5 : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(ItemGrabMenu), "receiveLeftClick", new Type[] { typeof(int), typeof(int), typeof(bool) });

        public static bool Prefix(bool ___shippingBin)
        {
            if (___shippingBin)
            {
                Game1.getFarm().shippingBin.OnValueAdded += OnAdd;
                Game1.getFarm().shippingBin.OnValueRemoved += OnRemove;
            }

            return true;
        }

        private static void OnAdd(Item item)
        {
            MainBinListener.OnShipItem(item, Game1.player);
        }

        private static void OnRemove(Item item)
        {
            MainBinListener.OnRemoveItem(item);
        }

        public static void Postfix(bool ___shippingBin)
        {
            if (___shippingBin)
            {
                Game1.getFarm().shippingBin.OnValueAdded -= OnAdd;
                Game1.getFarm().shippingBin.OnValueRemoved -= OnRemove;
            }
        }
    }
    #endregion
}
