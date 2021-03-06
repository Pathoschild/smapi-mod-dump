/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace DeluxeGrabberRedux
{
    public class ModApi : IDeluxeGrabberReduxApi
    {
        private readonly Mod Mod;

        private Func<SObject, Vector2, GameLocation, KeyValuePair<SObject, int>> _getMushroomHarvest;
        private Func<SObject, Vector2, GameLocation, KeyValuePair<SObject, int>> _getBerryBushHarvest;
        public Func<SObject, Vector2, GameLocation, KeyValuePair<SObject, int>> GetMushroomHarvest
        {
            get
            {
                return _getMushroomHarvest;
            }
            set
            {
                if (_getMushroomHarvest != null && _getMushroomHarvest != value)
                {
                    Mod.Monitor.LogOnce("GetMushroomHarvest override is being set more than once. This usually means that multiple mods are conflicting when attempting to integrate with DeluxeGrabberRedux.", LogLevel.Error);
                }
                _getMushroomHarvest = value;
            }
        }

        public Func<SObject, Vector2, GameLocation, KeyValuePair<SObject, int>> GetBerryBushHarvest
        {
            get
            {
                return _getBerryBushHarvest;
            }
            set
            {
                if (_getBerryBushHarvest != null && _getBerryBushHarvest != value)
                {
                    Mod.Monitor.LogOnce("GetBerryBushHarvest override is being set more than once. This usually means that multiple mods are conflicting when attempting to integrate with DeluxeGrabberRedux.", LogLevel.Error);
                }
                _getBerryBushHarvest = value;
            }
        }

        public ModApi(Mod mod)
        {
            Mod = mod;
        }
    }
}
