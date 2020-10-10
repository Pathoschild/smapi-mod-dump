/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AxesOfEvil/SV_DeliveryService
**
*************************************************/

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
