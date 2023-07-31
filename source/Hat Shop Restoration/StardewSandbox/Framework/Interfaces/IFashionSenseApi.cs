/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/StardewSandbox
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using static StardewValley.FarmerSprite;

namespace HatShopRestoration.Framework.Interfaces
{
    public interface IFashionSenseApi
    {
        public enum Type
        {
            Unknown,
            Hair,
            Accessory,
            [Obsolete("No longer maintained. Use Accessory instead.")]
            AccessorySecondary,
            [Obsolete("No longer maintained. Use Accessory instead.")]
            AccessoryTertiary,
            Hat,
            Shirt,
            Pants,
            Sleeves,
            Shoes,
            Player
        }

        KeyValuePair<bool, string> SetAppearanceLockState(Type appearanceType, string targetPackId, string targetAppearanceName, bool isLocked, IManifest callerManifest);
    }
}