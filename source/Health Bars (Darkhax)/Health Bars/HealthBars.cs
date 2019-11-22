using System;
using StardewValley;
using System.Reflection;
using StardewValley.Monsters;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;

namespace HealthBars {

    public class HealthBars : Mod {

        FieldInfo RockCrabShellGone = typeof(RockCrab).GetField("shellGone", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo BatSeenPlayer = typeof(Bat).GetField("seenPlayer", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo GolemSeenPlayer = typeof(RockGolem).GetField("seenPlayer", BindingFlags.Instance | BindingFlags.NonPublic);

        public Texture2D WhiteTexture { get; protected set; }

        public override void Entry(IModHelper helper) {

            GraphicsEvents.OnPostRenderEvent += this.OnPostRender;

            // Create a generic 1x1 white pixel texture. Used to render the health bar.
            this.WhiteTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.WhiteTexture.SetData(new Color[] { Color.White });
        }

        public void OnPostRender(object sender, EventArgs e) {

            if (Game1.currentLocation != null && Game1.activeClickableMenu == null && Game1.CurrentEvent == null) {

                foreach (NPC npc in Game1.currentLocation.getCharacters()) {

                    if (npc is Monster monster) {

                        if (this.CanSeeMonster(monster)) {

                            int localX = monster.GetBoundingBox().Center.X - Game1.viewport.X;
                            int localY = monster.GetBoundingBox().Y - Game1.viewport.Y - 20;

                            localX -= 25;

                            float healthPercent = (float)monster.Health / monster.MaxHealth;
                            int width = (int)(50f * healthPercent);



                            Rectangle rect = new Rectangle(localX, localY, width, 10);
                            Game1.spriteBatch.Draw(WhiteTexture, rect, getColorFromHealth(healthPercent));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a monster is a valid candidate for something the player could know about.
        /// </summary>
        /// <param name="monster">The instance of the monster.</param>
        /// <returns>Whether or not the monster is visible.</returns>
        private bool CanSeeMonster(Monster monster) {

            // Hide rock crabs that have a shell and are not moving.
            if (monster is RockCrab crab) {

                return ((NetBool)this.RockCrabShellGone.GetValue(crab)).Value || crab.isMoving();
            }

            // Hide bats that have not been woken by the player.
            else if (monster is Bat bat) {

                return ((NetBool)this.BatSeenPlayer.GetValue(bat)).Value;
            }

            // Hide rock golems that have not woken up.
            else if (monster is RockGolem golem) {

                return ((NetBool)this.GolemSeenPlayer.GetValue(golem)).Value;
            }

            // Hide generally invisible mobs.
            return !monster.IsInvisible;
        }

        /// <summary>
        /// Gets a color based on the health % of an entity. 100% is green, 50% is yellow, and 0% is red.
        /// </summary>
        /// <param name="health">The amount of health the entity has.</param>
        /// <returns></returns>
        private Color getColorFromHealth(float health) {

            if (health >= 0.5f) {

                return new Color(1f - 1f * (health - 0.5f) / 0.5f, 1f, 0f);
            }

            return new Color(1f, 1f * health / 0.5f, 0f);
        }
    }
}
