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
using ImprovedResourceClumps.Framework.Configs;
using ImprovedResourceClumps.Framework.Patches;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace ImprovedResourceClumps
{
    public class ImprovedResourceClumps : Mod
    {
        private IrcConfig _config;
        private PerformToolActionPatch ptap;

        public override void Entry(IModHelper helper)
        {
            //Lets load up the config file.
            _config = helper.ReadConfig<IrcConfig>();

            //Set up Events
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.Input.ButtonPressed += ButtonPressed;
        }

        //Events
        public void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;
            if (e.IsDown(SButton.NumPad4) && _config.EnableDebugMode)
            {
                Farm loc = Game1.getFarm();
                try
                {
                    loc.resourceClumps.Add(new ResourceClump(622, 2, 2,
                        new Vector2(Game1.player.getTileX() + 2, Game1.player.getTileY())));
                    Monitor.Log("Meteor should have spawned.");
                }
                catch (Exception ex)
                {
                    Monitor.Log(ex.ToString());
                }

            }
        }
        public void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ptap = new PerformToolActionPatch(Monitor, Helper, _config);
            //Set up Harmony then we can do whatever we want.
            var harmony = HarmonyInstance.Create(Helper.ModRegistry.ModID);

            harmony.Patch(
                original: AccessTools.Method(typeof(ResourceClump), nameof(ResourceClump.performToolAction)),
                prefix: new HarmonyMethod(typeof(PerformToolActionPatch), nameof(PerformToolActionPatch.performToolAction)));
        }
    }
}
