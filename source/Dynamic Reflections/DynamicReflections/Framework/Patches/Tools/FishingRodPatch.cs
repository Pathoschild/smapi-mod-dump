/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Object = StardewValley.Object;

namespace DynamicReflections.Framework.Patches.Tools
{
    internal class FishingRodPatch : PatchTemplate
    {
        private readonly Type _type = typeof(FishingRod);

        internal FishingRodPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_type, nameof(FishingRod.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
        }

        private static bool DrawPrefix(FishingRod __instance, Farmer ___lastUser, int ___fishSize, int ___whichFish, string ___itemCategory, SpriteBatch b)
        {
            if (DynamicReflections.isFilteringWater || DynamicReflections.isFilteringPuddles)
            {
                if (__instance.castedButBobberStillInAir)
                {
                    return true;
                }

                if (__instance.fishCaught)
                {
                    float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                    if (___itemCategory == "Object")
                    {
                        b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, ___lastUser.Position + new Vector2(0f, -56f)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ___whichFish, 16, 16), Color.White, (___fishSize == -1 || ___whichFish == 800 || ___whichFish == 798 || ___whichFish == 149 || ___whichFish == 151) ? 0f : ((float)Math.PI * 3f / 4f), new Vector2(8f, 8f), 3f, SpriteEffects.None, (float)___lastUser.getStandingY() / 10000f + 0.002f + 0.06f);
                        if (__instance.caughtDoubleFish)
                        {
                            b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, ___lastUser.Position + new Vector2(-8f, -56f)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ___whichFish, 16, 16), Color.White, (___fishSize == -1 || ___whichFish == 800 || ___whichFish == 798 || ___whichFish == 149 || ___whichFish == 151) ? 0f : ((float)Math.PI * 4f / 5f), new Vector2(8f, 8f), 3f, SpriteEffects.None, (float)___lastUser.getStandingY() / 10000f + 0.002f + 0.058f);
                        }
                    }
                    else
                    {
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, ___lastUser.Position + new Vector2(-124f, -284f + yOffset) + new Vector2(44f, 68f)), new Rectangle(228, 408, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)___lastUser.getStandingY() / 10000f + 0.0001f + 0.06f);
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, ___lastUser.Position + new Vector2(0f, -56f)), new Rectangle(228, 408, 16, 16), Color.White, 0f, new Vector2(8f, 8f), 3f, SpriteEffects.None, (float)___lastUser.getStandingY() / 10000f + 0.002f + 0.06f);
                    }
                }

                return false;
            }

            return true;
        }
    }
}
