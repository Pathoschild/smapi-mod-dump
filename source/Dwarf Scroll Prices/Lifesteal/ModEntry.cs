/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/noriteway/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using StardewValley.Locations;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using StardewValley.Monsters;
using Microsoft.Xna.Framework;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

namespace Lifesteal
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig? Config;
        private int curAnimIndex;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            /*var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(bool), nameof(StardewValley.GameLocation.damageMonster)),
                prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.damageMonster_Prefix))
            );*/

            helper.Events.GameLoop.UpdateTicked += EveryTick;
            helper.Events.Input.ButtonPressed += OnPress;
        }
        
        private void EveryTick(object? sender, UpdateTickedEventArgs e)
        {
            //if(Game1.CurrentEvent != null) Monitor.Log(Game1.CurrentEvent.id, LogLevel.Debug);
            //IReflectedMethod busLeave = Helper.Reflection.GetMethod(BusStop.busDriveOff());
            //IReflectedMethod busLeave = Helper.Reflection.GetMethod(Game1.location);
        }

        private void OnPress(object? sender, ButtonPressedEventArgs e)
        {
            if(e.Button.IsUseToolButton())
            {
                if(Game1.player.CurrentTool is MeleeWeapon)
                {
                    this.Helper.Events.GameLoop.UpdateTicked += DetectHit;
                }
            }
        }

        private void DetectHit(object? sender, UpdateTickedEventArgs e)
        {
            //if(curAnimIndex != Game1.player.FarmerSprite.currentAnimationIndex) curAnimIndex = Game1.player.FarmerSprite.currentAnimationIndex;
            //else return;

            MeleeWeapon curWeapon = Game1.player.CurrentTool as MeleeWeapon;

            Vector2 tileLocation = Vector2.Zero;
            Vector2 tileLocation2 = Vector2.Zero;
            //Rectangle aoe = curWeapon.getAreaOfEffect((int)Game1.player.Tile.X, (int)Game1.player.Tile.Y, Game1.player.FacingDirection, ref tileLocation, ref tileLocation2, Game1.player.GetBoundingBox(), Game1.player.FarmerSprite.currentAnimationIndex);
            Vector2 toolLocation = Game1.player.GetToolLocation(ignoreClick: true);
            Rectangle aoe = curWeapon.getAreaOfEffect((int)toolLocation.X, (int)toolLocation.Y, Game1.player.FacingDirection, ref tileLocation, ref tileLocation2, Game1.player.GetBoundingBox(), Game1.player.FarmerSprite.currentAnimationIndex);

            //Game1.player.draw(Game1.spriteBatch)
            Texture2D text = new Texture2D(Game1.graphics.GraphicsDevice, aoe.Width, aoe.Height);
            //Game1.spriteBatch.Begin();
            //Game1.spriteBatch.Draw(text, toolLocation, Color.Green);
            //Game1.spriteBatch.End();

            Monitor.Log(Game1.player.FarmerSprite.currentAnimationIndex.ToString(), LogLevel.Debug);

            foreach(NPC npc in Game1.currentLocation.characters)
            {
                if(npc is Monster)
                {
                    Monster mon = npc as Monster;
                    if(mon.TakesDamageFromHitbox(aoe))
                    {
                        Monitor.Log("Monster hit!", LogLevel.Debug);
                    }

                }
            }

            if(Game1.player.FarmerSprite.currentAnimationIndex == 5) this.Helper.Events.GameLoop.UpdateTicked -= DetectHit;
        }
    }

    /*internal class ObjectPatches
    {
        private static IMonitor Monitor;

        // call this method from your Entry class
        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        // patches need to be static!
        internal static bool damageMonster_Prefix(StardewValley.Object __instance, GameLocation location, Vector2 tile, ref bool __result)
        {
            try
            {
                
                return false; // don't run original logic
            }
            catch(Exception ex)
            {
                Monitor.Log($"Failed in {nameof(CanBePlacedHere_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }*/
}
