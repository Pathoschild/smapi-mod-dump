/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BetterJunimos {
    public static class JunimoEditor {
        internal static void OnAssetRequested(object sender, AssetRequestedEventArgs e) { 
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Junimo")) {
                e.Edit(asset => {
                    var editor = asset.AsImage();
                    
                    if (BetterJunimos.Config.FunChanges.JunimosAlwaysHaveLeafUmbrellas || Game1.isRaining) {
                        var rectangle = new Rectangle(0, 0, 128, 128);
                        var umbrella = BetterJunimos.Config.FunChanges.MoreColorfulLeafUmbrellas ? "JunimoUmbrellaOnly_Grayscale" : "JunimoUmbrellaOnly";
                        var customTexture = BetterJunimos.SHelper.ModContent.Load<Texture2D>($"assets/{umbrella}.png");
                        editor.PatchImage(customTexture, rectangle, rectangle, PatchMode.Overlay);
                        return;
                    }
                    if (Game1.IsWinter) {
                        var rectangle = new Rectangle(0, 0, 128, 128);
                        var customTexture = BetterJunimos.SHelper.ModContent.Load<Texture2D>($"assets/JunimoBeanie.png");
                        editor.PatchImage(customTexture, rectangle, rectangle, PatchMode.Overlay);
                    }
                });
            }
        }
    }
}
