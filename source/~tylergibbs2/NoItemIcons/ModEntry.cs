/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using System.Reflection;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace NoItemIcons
{
    public class ModEntry : Mod
    {
        public static bool IsActive { get; private set; } = false;

        public override void Entry(IModHelper helper)
        {
            Harmony harmony = new(ModManifest.UniqueID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            IsActive = false;
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            IsActive = e.NewMenu is not null;
        }

        public static void DrawWhiteIcon(SpriteBatch b, Vector2 location, float transparency, float layerDepth)
        {
            b.Draw(
                Game1.staminaRect,
                new Rectangle((int)location.X + 8, (int)location.Y + 8, 48, 48),
                Rectangle.Empty,
                Color.White * transparency,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                layerDepth
            );
        }
    }
}
