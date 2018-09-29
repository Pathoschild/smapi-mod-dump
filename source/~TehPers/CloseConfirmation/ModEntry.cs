using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace TehPers.Stardew.ShroomSpotter {

    public class ModEntry : Mod {
        public static ModEntry INSTANCE;

        public ModConfig config;

        public ModEntry() {
            INSTANCE = this;
        }

        public override void Entry(IModHelper helper) {
            this.config = helper.ReadConfig<ModConfig>();
            if (!config.ModEnabled) return;

            Form f = Control.FromHandle(Game1.game1.Window.Handle) as Form;
            f.FormClosing += GameClosing;
        }

        private void GameClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing && MessageBox.Show("Are you sure you want to quit?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }
    }
}
