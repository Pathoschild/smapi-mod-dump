using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace CustomizableCartRedux
{
    public interface ICustomizableCart
    {
        event EventHandler CartProcessingComplete;
        void AddItem(Item item, int price, int quality);
    }

    public class CustomizableCartAPI : ICustomizableCart
    {
        public event EventHandler CartProcessingComplete;
        private readonly IReflectionHelper Reflector;

        public CustomizableCartAPI(IReflectionHelper Ref)
        {
            this.Reflector = Ref;
        }

        internal void InvokeCartProcessingComplete()
        {
            Log.trace("Event: CartProcessingComplete");
            if (CartProcessingComplete == null)
                return;
            Util.invokeEvent("CustomizableCartAPI.CartProcessingComplete", CartProcessingComplete.GetInvocationList(), null);
        }

        public void AddItem(Item item, int price, int quantity = 1)
        {
            Forest f = Game1.getLocationFromName("Forest") as Forest;
            bool travelingMerchantDay = f.travelingMerchantDay;
            if (travelingMerchantDay)
            {
                var travelerStock = Reflector.GetField<Dictionary<Item, int[]>>(f, "travelerStock").GetValue();
                travelerStock?.Add(item, new int[]
                {
                    price,
                    quantity
                });
            }
        }
    }
}

