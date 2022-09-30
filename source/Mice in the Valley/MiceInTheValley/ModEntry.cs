/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/MiceInTheValley
**
*************************************************/

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

using MiceInTheValley.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MiceInTheValley {
    public class ModEntry : Mod {
        // Squeak.
        private SoundEffect sound_;
        private ModConfig config_;

        public override void Entry(IModHelper helper) {
            config_ = helper.ReadConfig<ModConfig>();
            helper.Events.Player.Warped += OnWarped;
            // Register the asset loader.
            helper.Events.Content.AssetRequested += OnAssetRequested;

            // Load the sound.
            string soundFilePath = Path.Combine(helper.DirectoryPath, "assets/mouse.wav");
            this.Monitor.Log($"Sound file: {soundFilePath}");

            using (Stream fs = new FileStream(soundFilePath, FileMode.Open)) {
                sound_ = SoundEffect.FromStream(fs);
            }
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e) {
            if (e.NameWithoutLocale.IsEquivalentTo("mouse")
             || e.NameWithoutLocale.IsEquivalentTo("mouse_white")
             || e.NameWithoutLocale.IsEquivalentTo("mouse_tiger")) {
                this.Monitor.Log($"Load asset {e.NameWithoutLocale}");

                e.LoadFromModFile<Texture2D>($"assets/{e.NameWithoutLocale}.png", AssetLoadPriority.Exclusive);
            }
        }

        // Mice are added when warping to a new location.
        private void OnWarped(object sender, WarpedEventArgs e) {
            // Add at most 10 mice per location.
            for (int i = 0; i < 10; ++i) {
                addMice(e.NewLocation, 0.5);
            }
        }

        // Taken from GameLocation.addBunnies() .
        private void addMice(GameLocation location, double chance) {
            if (location is Desert || !(Game1.random.NextDouble() < chance) || location.largeTerrainFeatures == null) {
                return;
            }

            int num = 0;
            Vector2 position;
            Vector2 direction = RandomDirection();
            while (true) {
                if (num >= 3) {
                    // No suitable terrain feature found, give up.
                    return;
                }

                // Pick a random terrain feature and check its usability.
                int index = Game1.random.Next(location.largeTerrainFeatures.Count);
                if (location.largeTerrainFeatures.Count > 0 && location.largeTerrainFeatures[index] is Bush) {
                    position = location.largeTerrainFeatures[index].tilePosition;
                    int num2 = Game1.random.Next(5, 12);
                    bool doIt = true;
                    for (int i = 0; i < num2; i++) {
                        position -= direction;
                        Rectangle rectangle = new Rectangle((int) position.X * 64, (int) position.Y * 64, 64, 64);
                        bool placeable = location.isTileLocationTotallyClearAndPlaceable(position);
                        if (!location.largeTerrainFeatures[index].getBoundingBox().Intersects(rectangle) && !placeable) {
                            doIt = false;

                            break;
                        }
                    }
                    if (doIt) {
                        break;
                    }
                }
                num++;
            }

            int speed = Game1.random.Next(10);
            Mouse.Species species = Mouse.Species.mouse;
            // White mice are rare.
            if (Game1.random.NextDouble() < 0.05) {
                species = Mouse.Species.mouse_white;
            }
            // Tiger mice live only on Ginger Island and are rare, too.
            else if (Game1.random.NextDouble() < 0.2 && location.Name.StartsWith("Island")) {
                species = Mouse.Species.mouse_tiger;
            }
            var mouse = new Mouse(this.Monitor, position, direction, speed, species, sound_, config_);
            // Add to critters (no reflection necessary to access the list)
            location.addCritter(mouse);

            Monitor.Log($"Added mouse in {location.Name} at {position}");
        }

        // A direction is represented by a signed unit vector in X or Y direction.
        private static Vector2 RandomDirection() {
            Vector2 direction = Game1.random.NextDouble() < 0.5 ? Vector2.UnitY : Vector2.UnitX;
            bool flip = Game1.random.NextDouble() < 0.5;
            if (flip) {
                direction *= -1f;
            }

            return direction;
        }
    }
}
