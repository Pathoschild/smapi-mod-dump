using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using SObject = StardewValley.Object;


namespace DeliveryService
{
    class ModConfig
    {
        public SButton DeliverKey { get; set; } = Keys.PrintScreen.ToSButton();
        public bool WaitForWizardShop = true;
    }
}
