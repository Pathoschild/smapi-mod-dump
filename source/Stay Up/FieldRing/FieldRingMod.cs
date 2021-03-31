/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/su226/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Su226.FieldRing {
  class M {
    public static IModHelper Helper;
    public static IMonitor Monitor;
    public static Config Config;
  }
  class FieldRingMod : Mod {
    private Texture2D EffectTexture;

    public override void Entry(IModHelper helper) {
      M.Helper = Helper;
      M.Monitor = Monitor;
      M.Config = helper.ReadConfig<Config>();
      helper.Events.Display.RenderedWorld += OnRenderedWorld;
      FieldRingItem.ItemTexture = helper.Content.Load<Texture2D>("assets/field_ring.png");
      EffectTexture = helper.Content.Load<Texture2D>("assets/field_effect.png");
      helper.ConsoleCommands.Add("fieldring_add", "Add a field ring to player's inventory.", FieldRingAdd);
    }
    
    private void FieldRingAdd(string name, string[] arguments) {
      Game1.player.addItemToInventory(new FieldRingItem());
    }

    private void OnRenderedWorld(object o, RenderedWorldEventArgs e) {
      foreach (Farmer f in Game1.currentLocation.farmers) {
        if (f.isWearingRing(M.Config.Index)) {
          Point center = f.GetBoundingBox().Center;
          e.SpriteBatch.Draw(EffectTexture, new Rectangle(center.X - M.Config.Range - Game1.viewport.X, center.Y - M.Config.Range - Game1.viewport.Y, M.Config.Range * 2, M.Config.Range * 2), Color.White);
        }
      }
    }
  }
}
