/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models;
using Archery.Framework.Models.Weapons;
using Archery.Framework.Objects;
using Archery.Framework.Objects.Items;
using Archery.Framework.Objects.Weapons;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace Archery.Framework.Patches.Characters
{
    internal class FarmerPatch : PatchTemplate
    {
        private readonly System.Type _object = typeof(Farmer);

        public FarmerPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Farmer.showHoldingItem), new[] { typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(ShowHoldingItemPrefix)));

            harmony.Patch(AccessTools.Method(_object, "get_ActiveObject", null), postfix: new HarmonyMethod(GetType(), nameof(IsCarringPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Farmer.Update), new[] { typeof(GameTime), typeof(GameLocation) }), postfix: new HarmonyMethod(GetType(), nameof(UpdatePostfix)));
        }

        private static bool ShowHoldingItemPrefix(Farmer __instance, Farmer who)
        {
            if (Bow.IsValid(who.mostRecentlyGrabbedItem) && InstancedObject.GetModel<BaseModel>(who.mostRecentlyGrabbedItem) is BaseModel baseModel)
            {
                var icon = baseModel.GetIcon(who);
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(baseModel.TexturePath, icon.Source, 2500f, 1, 0, who.Position + new Vector2(0f, -124f), flicker: false, flipped: false, 1f, 0f, Color.White, icon.Scale, 0f, 0f, 0f)
                {
                    motion = new Vector2(0f, -0.1f)
                });

                return false;
            }

            return true;
        }

        private static void IsCarringPostfix(Farmer __instance, ref Object __result)
        {
            if (Arrow.GetModel<AmmoModel>(__result) is AmmoModel arrowModel && arrowModel is not null)
            {
                __result = null;
            }
        }

        private static void UpdatePostfix(Farmer __instance, GameTime time, GameLocation location)
        {
            // Don't update cooldowns if the menu is currently open
            if (Game1.activeClickableMenu is not null)
            {
                return;
            }

            // Update the special attack cooldown, if applicable
            if (Bow.ActiveCooldown >= 0)
            {
                Bow.ActiveCooldown -= time.ElapsedGameTime.Milliseconds;
                if (Bow.ActiveCooldown <= 0)
                {
                    Bow.CooldownAdditiveScale = 0.5f;
                }
            }

            if (Bow.CooldownAdditiveScale >= 0)
            {
                Bow.CooldownAdditiveScale -= 0.01f;
            }
        }
    }
}
