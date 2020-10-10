/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using BetterJunimos.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace BetterJunimos {
    public class JunimoEditor : IAssetEditor {
        private IContentHelper Content;

        public JunimoEditor(IContentHelper Content) {
            this.Content = Content;
        }

        public bool CanEdit<T>(IAssetInfo asset) {
            if (Util.Config.FunChanges.JunimosAlwaysHaveLeafUmbrellas || Game1.isRaining) {
                return asset.AssetNameEquals(@"Characters\Junimo");
            }
            return false;
        }

        public void Edit<T>(IAssetData asset) {
            if (Util.Config.FunChanges.JunimosAlwaysHaveLeafUmbrellas || Game1.isRaining) {
                Rectangle rectangle = new Rectangle(0, 0, 128, 128);
                string umbrella = Util.Config.FunChanges.MoreColorfulLeafUmbrellas ? "JunimoUmbrellaOnly_Grayscale" : "JunimoUmbrellaOnly";
                Texture2D customTexture = Content.Load<Texture2D>("assets/" + umbrella + ".png", ContentSource.ModFolder);
                asset.AsImage().PatchImage(customTexture, rectangle, rectangle, PatchMode.Overlay);
            }
        }
    }
}
