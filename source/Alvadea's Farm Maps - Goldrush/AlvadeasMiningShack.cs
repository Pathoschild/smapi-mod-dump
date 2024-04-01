/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jaksha6472/MiningShack
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;
using HarmonyLib;
using StardewValley.Network;

namespace AlvadeasMiningShack
{
    public class AlvadeasMiningShack : Mod
    {
        internal static AlvadeasMiningShack Instance { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            // Apply Harmony Patches
            var harmony = new Harmony(this.ModManifest.UniqueID);
            FarmPatches.Apply(harmony);

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        // Code from https://stardewvalleywiki.com/Modding:Maps
        // Load the Shack Interior as new map
        private void OnSaveLoaded(object sender, EventArgs e)
        {
            // Fix the Shack, if it's already repaired
            if (NetWorldState.checkAnywhereForWorldStateID("miningShackRepaired"))
                FarmPatches.fixShack();
        }

        private void OnTimeChanged(object sender, EventArgs e)
        {
            // Fix the Shack, if it's already repaired
            if (NetWorldState.checkAnywhereForWorldStateID("miningShackRepaired"))
                FarmPatches.fixShack();
        }

        private void OnWarped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (e.NewLocation.NameOrUniqueName == "Farm" && Game1.whichFarm == 3)
            {
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(16, 14), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(18, 16), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(18, 19), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(17, 23), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(14, 23), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(13, 19), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(13, 15), 0);
            }
            if (e.OldLocation.NameOrUniqueName == "Farm" && Game1.whichFarm == 3)
            {
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(16, 14));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(18, 16));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(18, 19));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(17, 23));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(14, 23));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(13, 19));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(13, 15));
            }
        }

        private void OnUpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady && Game1.whichFarm == 3)
            {
                GameLocation location = Game1.getLocationFromName("Farm");
                if (location == Game1.player.currentLocation)
                    CustomAmbientSounds.update();
            }
        }

        private void OnReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(16, 14));
            CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(18, 16));
            CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(18, 19));
            CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(17, 23));
            CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(14, 23));
            CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(13, 19));
            CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(13, 15));
        }
    }
}