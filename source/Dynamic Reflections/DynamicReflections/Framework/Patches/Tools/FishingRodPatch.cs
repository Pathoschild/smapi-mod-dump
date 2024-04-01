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
using StardewValley.ItemTypeDefinitions;
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

        private static bool DrawPrefix(FishingRod __instance, Farmer ___lastUser, int ___fishSize, ItemMetadata ___whichFish, SpriteBatch b)
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
                    if (___whichFish.TypeIdentifier == "(O)")
                    {
                        ParsedItemData parsedOrErrorData = ___whichFish.GetParsedOrErrorData();
                        Texture2D texture = parsedOrErrorData.GetTexture();
                        Rectangle sourceRect = parsedOrErrorData.GetSourceRect();

                        b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, ___lastUser.Position + new Vector2(0f, -56f)), sourceRect, Color.White, (___fishSize == -1 || ___whichFish.QualifiedItemId == "(O)800" || ___whichFish.QualifiedItemId == "(O)798" || ___whichFish.QualifiedItemId == "(O)149" || ___whichFish.QualifiedItemId == "(O)151") ? 0f : ((float)Math.PI * 3f / 4f), new Vector2(8f, 8f), 3f, SpriteEffects.None, (float)___lastUser.StandingPixel.Y / 10000f + 0.002f + 0.06f);
                        if (__instance.numberOfFishCaught == 2)
                        {
                            b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, ___lastUser.Position + new Vector2(-8f, -56f)), sourceRect, Color.White, (___fishSize == -1 || ___whichFish.QualifiedItemId == "(O)800" || ___whichFish.QualifiedItemId == "(O)798" || ___whichFish.QualifiedItemId == "(O)149" || ___whichFish.QualifiedItemId == "(O)151") ? 0f : ((float)Math.PI * 4f / 5f), new Vector2(8f, 8f), 3f, SpriteEffects.None, (float)___lastUser.StandingPixel.Y / 10000f + 0.002f + 0.058f);
                        }
                    }
                    else
                    {
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, ___lastUser.Position + new Vector2(-124f, -284f + yOffset) + new Vector2(44f, 68f)), new Rectangle(228, 408, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)___lastUser.StandingPixel.Y / 10000f + 0.0001f + 0.06f);
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, ___lastUser.Position + new Vector2(0f, -56f)), new Rectangle(228, 408, 16, 16), Color.White, 0f, new Vector2(8f, 8f), 3f, SpriteEffects.None, (float)___lastUser.StandingPixel.Y / 10000f + 0.002f + 0.06f);
                    }
                }

                return false;
            }

            return true;
        }
    }
}
