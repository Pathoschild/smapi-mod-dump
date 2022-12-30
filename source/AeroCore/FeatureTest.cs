/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Models;
using AeroCore.Particles;
using AeroCore.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace AeroCore
{
#if DEBUG
    [ModInit]
#endif
    internal class FeatureTest
    {
        private static IParticleManager particles;
        private static Emitter emitter;

        internal static void Init()
        {
            Patches.Action.ActionCursors.Add("Mailbox", 1);
            emitter = new Emitter()
            {
                Region = new(0, 0, 64, 64),
                Rate = 100
            };
            particles = ModEntry.api.CreateParticleSystem(ModEntry.helper.ModContent, "assets/MouseParticle.json", emitter, 100);
            ModEntry.helper.Events.GameLoop.UpdateTicked += Tick;
            ModEntry.helper.Events.Display.RenderedHud += Draw;
            ModEntry.helper.Events.Input.CursorMoved += MouseMoved;
            //ModEntry.helper.Events.Input.ButtonReleased += KeyReleased;
            particles.Tick(0);
        }
        private static void KeyReleased(object _, ButtonReleasedEventArgs ev)
        {
            if (ev.IsSuppressed())
                return;
        }
        private static void MouseMoved(object _, CursorMovedEventArgs ev)
        {
            emitter.Region = new(Game1.getMousePosition(), emitter.Region.Size);
            particles.Offset = ev.NewPosition.ScreenPixels / 100f;
        }
        private static void Tick(object _, UpdateTickedEventArgs ev)
        {
            particles.Tick((int)Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds);
        }
        private static void Draw(object _, RenderedHudEventArgs ev)
        {
            particles.Draw(ev.SpriteBatch);
        }
    }
}
