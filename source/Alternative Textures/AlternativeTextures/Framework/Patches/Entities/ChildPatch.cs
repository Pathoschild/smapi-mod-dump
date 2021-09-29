/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures;
using AlternativeTextures.Framework.Models;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.Entities
{
    internal class ChildPatch : PatchTemplate
    {
        private readonly Type _entity = typeof(Child);

        internal ChildPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Constructor(_entity, new[] { typeof(string), typeof(bool), typeof(bool), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(ChildPostfix)));
        }

        private static void ChildPostfix(Child __instance, string name, bool isMale, bool isDarkSkinned, Farmer parent)
        {
            var instanceName = $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(__instance)}";
            var instanceSeasonName = $"{instanceName}_{Game1.GetSeasonForLocation(__instance.currentLocation)}";

            if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceName) && AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceSeasonName))
            {
                var result = Game1.random.Next(2) > 0 ? AssignModData(__instance, instanceSeasonName, true) : AssignModData(__instance, instanceName, false);
                return;
            }
            else
            {
                if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceName))
                {
                    AssignModData(__instance, instanceName, false);
                    return;
                }

                if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceSeasonName))
                {
                    AssignModData(__instance, instanceSeasonName, true);
                    return;
                }
            }

            AssignDefaultModData(__instance, instanceSeasonName, true);
        }
    }
}
