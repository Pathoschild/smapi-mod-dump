/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jaksha6472/WitchTower
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;
using HarmonyLib;
using StardewValley.Network;

namespace AlvadeasWitchTower
{
    public class AlvadeasWitchTower : Mod
    {
        internal static AlvadeasWitchTower Instance { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            // Apply Harmony Patches
            var harmony = new Harmony(this.ModManifest.UniqueID);
            CustomQuests.Apply(harmony);
            EndMusicPatches.Apply(harmony);

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Player.Warped += OnWarped;
        }
        private void OnWarped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (e.NewLocation.NameOrUniqueName == "Farm" && Game1.whichFarm == 4 && Game1.currentSeason != "winter")
            {
                if (Game1.timeOfDay >= 2000 || Game1.timeOfDay <= 200)
                {
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(55, 24), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(49, 21), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(28, 29), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(21, 24), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(14, 15), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(6, 12), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(16, 7), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(26, 1), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(39, 6), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(34, 3), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(29, 4), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(22, 5), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(41, 2), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(19, 9), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(20, 14), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(7, 8), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(19, 19), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(28, 19), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(24, 17), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(26, 23), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(12, 10), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(45, 23), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(54, 17), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(57, 27), 1);
                }
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(55, 24), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(49, 21), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(28, 29), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(21, 24), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(14, 15), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(6, 12), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(16, 7), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(26, 1), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(39, 6), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(34, 3), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(29, 4), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(22, 5), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(41, 2), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(19, 9), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(20, 14), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(7, 8), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(19, 19), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(28, 19), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(24, 17), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(26, 23), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(12, 10), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(45, 23), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(54, 17), 0);
                CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(57, 27), 0);
            }

            if (e.OldLocation.NameOrUniqueName == "Farm" && Game1.whichFarm == 4)
            {
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(55, 24));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(49, 21));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(28, 29));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(21, 24));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(14, 15));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(6, 12));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(16, 7));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(26, 1));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(39, 6));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(34, 3));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(29, 4));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(22, 5));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(41, 2));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(19, 9));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(20, 14));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(7, 8));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(19, 19));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(28, 19));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(24, 17));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(26, 23));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(12, 10));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(45, 23));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(54, 17));
                CustomAmbientSounds.removeCustomSound(new Microsoft.Xna.Framework.Vector2(57, 27));
            }
        }

        private void OnUpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady && Game1.whichFarm == 4)
            {
                GameLocation location = Game1.getLocationFromName("Farm");
                if (location == Game1.player.currentLocation)
                    CustomAmbientSounds.update();
            }
        }

        private void OnSaveLoaded(object sender, EventArgs e)
        {
            // Fix the teleportation circle, if the quests are done
            if (NetWorldState.checkAnywhereForWorldStateID("walpurgis.fixedTeleportationCircle"))
                CustomQuests.fixTeleportCircle();
        }

        private void OnTimeChanged(object sender, EventArgs e)
        {
            if (Game1.player.currentLocation.NameOrUniqueName == "Farm" && Game1.whichFarm == 4 && Game1.currentSeason != "winter")
            {
                if (Game1.timeOfDay >= 2000 || Game1.timeOfDay <= 200)
                {
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(55, 24), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(49, 21), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(28, 29), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(21, 24), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(14, 15), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(6, 12), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(16, 7), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(26, 1), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(39, 6), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(34, 3), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(29, 4), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(22, 5), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(41, 2), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(19, 9), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(20, 14), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(7, 8), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(19, 19), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(28, 19), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(24, 17), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(26, 23), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(12, 10), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(45, 23), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(54, 17), 1);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(57, 27), 1);
                }
                else
                {
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(55, 24), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(49, 21), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(28, 29), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(21, 24), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(14, 15), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(6, 12), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(16, 7), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(26, 1), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(39, 6), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(34, 3), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(29, 4), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(22, 5), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(41, 2), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(19, 9), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(20, 14), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(7, 8), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(19, 19), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(28, 19), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(24, 17), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(26, 23), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(12, 10), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(45, 23), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(54, 17), 0);
                    CustomAmbientSounds.addCustomSound(new Microsoft.Xna.Framework.Vector2(57, 27), 0);
                }
            }

            // Fix the teleportation circle, if the quests are done
            if (NetWorldState.checkAnywhereForWorldStateID("walpurgis.fixedTeleportationCircle"))
                CustomQuests.fixTeleportCircle();
        }
    }
}