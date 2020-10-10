/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using Entoarox.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;

namespace StardewHack.PressurePlate
{
    public class ModEntry : Mod
    {
        public  static Texture2D plate;

        public override void Entry(IModHelper helper) {
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            plate = helper.Content.Load<Texture2D>("switch.png");
        }

        void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e) {
            Game1.player.addItemToInventory(new PressurePlate());
        }
    }

    public class PressurePlate : StardewValley.Object, ICustomItem {
        int release_delay = 0;
    
        public PressurePlate() { 
            name = "Pressure Plate";
        }
        public PressurePlate(Vector2 pos) : this() {
            tileLocation.Value = pos;
        }
        
        public override string getDescription() {
            return "Automatically opens gates when stepped upon.";
        }
    
        public override void draw (SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            //Color c = finished.Value ? successColor : (isOn.Value ? onColor : offColor).Value;
            Color c = Color.White;
            int offset = release_delay == 0 ? 0 : 4;
            spriteBatch.Draw (ModEntry.plate, Game1.GlobalToLocal (Game1.viewport, new Vector2 (x * 64, y * 64 + offset)), new Rectangle (0, 0, 16, 16), c, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
        }
        
        public override void drawWhenHeld (SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            spriteBatch.Draw (ModEntry.plate, objectPosition, new Rectangle (0, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max (0f, (float)(f.getStandingY () + 3) / 10000f));
        }
        
        public override void drawInMenu (SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            bool flag = Stack > 1;
            spriteBatch.Draw (ModEntry.plate, location + new Vector2 (32f, 32f), new Rectangle (0, 0, 16, 16), color * transparency, 0f, new Vector2 (8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
            if (flag) {
                Utility.drawTinyDigits (stack, spriteBatch, location + new Vector2 ((float)(64 - Utility.getWidthOfTinyDigitString (stack, 3f * scaleSize)) + 3f * scaleSize, 64f - 18f * scaleSize + 2f), 3f * scaleSize, 1f, color);
            }
        }
        
        public override bool canBePlacedHere (GameLocation l, Vector2 tile) {
            if (l.Objects.ContainsKey(tile)) return false;
            if (l.terrainFeatures.ContainsKey(tile) && !l.terrainFeatures[tile].isPassable()) return false;
            return true; 
        }
        
        public override bool isActionable (Farmer who) {
            return false;
        }
        
        public override bool placementAction (GameLocation location, int x, int y, Farmer who = null)
        {
            Vector2 vector = new Vector2 (x / 64, y / 64);
            location.objects.Add (vector, new PressurePlate(vector) );
            return true;
        }
        
        public override bool isPlaceable () {
            return true;
        }

        public override bool isPassable () {
            return true;
        }

        
        public override void updateWhenCurrentLocation (GameTime time, GameLocation environment)
        {
            var farmer = Game1.player;
            if (farmer.getTileLocation ().Equals (tileLocation)) {
                if (release_delay == 0) {
                    Game1.playSound ("boulderCrack");
                }
                release_delay = 10;
                return;
            } 
            if (release_delay > 0) {
                release_delay--;
                if (release_delay==0) {
                    farmer.currentLocation.playSound("boulderCrack", StardewValley.Network.NetAudio.SoundContext.Default);
                }
            }
        }
        
        public override bool performToolAction (Tool t, GameLocation location)
        {
            if (!(t is StardewValley.Tools.MeleeWeapon) && t.isHeavyHitter ()) {
                var who = Game1.player;
                location.playSound ("hammer", StardewValley.Network.NetAudio.SoundContext.Default);
                Vector2 origin = who.GetToolLocation (false);
                Vector2 destination = who.GetBoundingBox().Center.ToVector2();
                location.debris.Add (new Debris(this, origin, destination));
                location.Objects.Remove(TileLocation);
            }
            return false;
        }
        
        public override void performRemoveAction (Vector2 tileLocation, GameLocation environment) {}
        public override void dropItem (GameLocation location, Vector2 origin, Vector2 destination) {}
        public override int sellToStorePrice (long specificPlayerID = -1L) { return 0; }
    }
}

