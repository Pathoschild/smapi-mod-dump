/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using CustomCompanions.Framework.Companions;
using CustomCompanions.Framework.Managers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCompanions.Framework.Patches
{
    internal class GameLocationPatch
    {
        private static IMonitor monitor;
        private readonly Type _gameLocation = typeof(GameLocation);

        internal GameLocationPatch(IMonitor modMonitor)
        {
            monitor = modMonitor;
        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_gameLocation, nameof(GameLocation.drawWater), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawWaterPostfix)));
        }

        private static bool DrawWaterPostfix(GameLocation __instance, SpriteBatch b)
        {
            foreach (var companion in CompanionManager.sceneryCompanions.Where(c => c.Location == __instance).SelectMany(c => c.Companions))
            {
                if (companion.model.AppearUnderwater)
                {
                    companion.DrawUnderwater(b);
                }
            }

            return true;
        }
    }
}
