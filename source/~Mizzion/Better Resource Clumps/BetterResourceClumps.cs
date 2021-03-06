/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Better_Resource_Clumps
{
    public class BetterResourceClumps : Mod
    {
        private ResourcePrefix pre;
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.Input.ButtonPressed += ButtonPressed;
        }

        public void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;
            if (e.IsDown(SButton.NumPad4))
            {
                Farm loc = Game1.getFarm();
                try
                {
                    loc.resourceClumps.Add(new ResourceClump(622, 2, 2,
                        new Vector2(Game1.player.getTileX() + 2, Game1.player.getTileY())));
                    Monitor.Log("Meteor should spawn");
                }
                catch (Exception ex)
                {
                    Monitor.Log(ex.ToString());
                }
                
            }
        }
        public void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            pre = new ResourcePrefix(Monitor, Helper);
            //Set up Harmony then we can do whatever we want.
            var harmony = HarmonyInstance.Create(Helper.ModRegistry.ModID);

            harmony.Patch(
                original: AccessTools.Method(typeof(ResourceClump), nameof(ResourceClump.performToolAction)),
                prefix: new HarmonyMethod(typeof(ResourcePrefix), nameof(ResourcePrefix.performToolAction)));
        }
    }
}
