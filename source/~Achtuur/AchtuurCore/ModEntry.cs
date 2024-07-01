/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore;
using AchtuurCore.Events;
using AchtuurCore.Framework;
using AchtuurCore.Framework.Borders;
using AchtuurCore.Framework.GUI;
using AchtuurCore.Framework.Particle;
using AchtuurCore.Framework.Particle.StartBehaviour;
using AchtuurCore.Framework.Particle.UpdateBehaviour;
using AchtuurCore.Patches;
using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AchtuurCore;

internal class ModEntry : Mod
{
    internal static ModEntry Instance;
    public static EventManager EventManager;

    internal static bool DebugDraw;
    internal static List<DecayingText> DebugText;
    internal Button button;

    public override void Entry(IModHelper helper)
    {
        ModEntry.Instance = this;

        HarmonyPatcher.ApplyPatches(this,
            new WateringPatcher()
        );

        EventManager = new();
        LoadTextureAssets();

        Debug.DebugOnlyExecute(() =>
        {
            Helper.Events.Display.Rendered += OnRendered;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
        });
    }

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (e.Button == SButton.NumPad0)
            DebugDraw = !DebugDraw;

        Debug.DebugOnlyExecute(() =>
        {
            if (e.Button == SButton.R)
            {
                // simulate diamond node breaking
                int n_particles = 50;

                for (int n = 0; n < n_particles; n++)
                {
                    TrailParticle Particle = new TrailParticle(5, Color.Red, Vector2.One);
                    Particle.SetTrailColors(new List<Color> { Color.Red, Color.WhiteSmoke, Color.WhiteSmoke });

                    float r = RandomUtil.GetFloat(0, 1);
                    float g = RandomUtil.GetFloat(0, 1);
                    float b = RandomUtil.GetFloat(0.5, 1);
                    Color c = new Color(r, g, b);
                    //Particle Particle = new Particle();

                    //Particle.AddState<EdgeOfMapStartBehaviour>();
                    Particle.AddState<RandomStartBehaviour>();
                    Particle.AddState<MovementBehaviour>();
                    Particle.AddState<OrbitMovementBehaviour>();
                    //Particle.AddState<EscapeMapMovementBehaviour>();

                    // Set initial position of 
                    Particle.SetInitialPosition(e.Cursor.AbsolutePixels);
                    Particle.SetTargetFarmer(Game1.player);
                    Particle.SetSize(Vector2.One * 6);
                    //Particle.SetColor(c);

                    Particle.Start();
                }
            }
        });
    }

    private void LoadTextureAssets()
    {
        Overlay.LoadPlacementTileTexture();
        Border.LoadBorderTextureAssets();
    }

    public static void AddDebugText(string text, int lifespan=60)
    {
        DebugText.Add(new DecayingText(text, lifespan));
    }

    private void OnRendered(object sender, RenderedEventArgs e)
    {
        if (!Game1.hasLoadedGame || !DebugDraw)
            return;

        if (DebugText is null || DebugText.Count == 0)
            return;

        Vector2 basePosition = Helper.Input.GetCursorPosition().ScreenPixels + new Vector2(64);
        float y_offset = 0;
        foreach (DecayingText text in DebugText)
        {
            text.DrawToScreen(e.SpriteBatch, basePosition + new Vector2(0, y_offset));
            y_offset += Game1.dialogueFont.MeasureString(text.Text).Y + 2;
        }

        DebugText.RemoveAll(t => t.LifeSpanOver);
    }
}
