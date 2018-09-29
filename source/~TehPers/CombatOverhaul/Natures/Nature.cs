using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Collections.Generic;
using System.IO;
using SFarmer = StardewValley.Farmer;

namespace TehPers.Stardew.CombatOverhaul.Natures {
    public abstract class Nature {
        public Texture2D ScepterTexture { get; set; }
        private static Dictionary<string, Texture2D> textureList = new Dictionary<string, Texture2D>();

        public Nature() {
            this.ScepterTexture = getTexture();
        }

        public abstract string getName();

        public abstract string getDescription();

        public abstract bool activate(GameLocation location, int x, int y, int power, SFarmer who);

        public virtual bool playWandSound() {
            return true;
        }

        protected virtual string getTextureName() {
            return "Default";
        }

        public virtual Texture2D getTexture() {
            string name = getTextureName();
            if (name != "Default" && !textureList.ContainsKey(name)) {
                string path = Path.Combine(ModEntry.INSTANCE.Helper.DirectoryPath, "resources", getTextureName() + ".png");
                textureList[name] = Texture2D.FromStream(Game1.graphics.GraphicsDevice, new FileStream(path, FileMode.Open));
            }
            return textureList[name];
        }
    }
}
