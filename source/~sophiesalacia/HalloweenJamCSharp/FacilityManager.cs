/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using xTile.Tiles;

namespace HalloweenJamCSharp;

internal static class FacilityManager
{
    private static Texture2D OverheadLightTexture;
    private static Texture2D VatLightTexture;
    private static Texture2D VatGlowTexture;
    private static Texture2D VatBottomGlowTexture;
    private static Texture2D WalkwayLightTexture;

    private static readonly List<LightSource> VatLights = new();
    private static readonly List<LightSource> WalkwayLights = new();

    private static int WalkwayLightCounter = 0;
    private static int VatLightCounter = 0;

    private static Tile Tile;

    internal static bool FacilityLightsDone = false;

    internal static void InitializeVars(object sender, GameLaunchedEventArgs e)
    {
        OverheadLightTexture = Globals.ModContent.Load<Texture2D>("Assets/LightTexture.png");
        VatLightTexture = Globals.ModContent.Load<Texture2D>("Assets/VatLightTexture.png");
        VatGlowTexture = Globals.ModContent.Load<Texture2D>("Assets/VatGlowTexture.png");
        VatBottomGlowTexture = Globals.ModContent.Load<Texture2D>("Assets/VatBottomGlowTexture.png");
        WalkwayLightTexture = Globals.ModContent.Load<Texture2D>("Assets/WalkwayLightTexture.png");
    }

    internal static void DoLightsSetup()
    {
        if (WalkwayLights.Any())
            return;

        // walkway lights
        for (int x = 3; x <= 45; x += 7)
        {
            float xOffset = 0.5f;

            WalkwayLights.Add(new LightSource(1, new Vector2(x + xOffset, 11f) * 64f, 5f, new Color(255, 255, 255) * 0.35f, LightSource.LightContext.None, 0L)
            {
                lightTexture = WalkwayLightTexture
            }
            );
        }

        // vat lights
        //Game1.currentLightSources.Add(new LightSource(8, new Vector2(x, y) * 64f, 4f, new Color(0, 255, 255), LightSource.LightContext.None, 0L)
        //    {
        //        lightTexture = VatLightTexture
        //    }
        //);

        for (int xStart = 0; xStart <= 42; xStart += 7)
        {
            for (int x = xStart; x <= xStart + 5; x += 5)
            {
                for (int y = 23; y >= 1; y -= 2)
                {
                    VatLights.Add(new LightSource(8, new Vector2(x, y) * 64f, 4f, new Color(0, 255, 255), LightSource.LightContext.None, 0L)
                    {
                        lightTexture = VatLightTexture
                    }
                    );

                    VatLights.Add(new LightSource(8, new Vector2(x, y) * 64f, 4f, new Color(255, 130, 150) * 0.65f, LightSource.LightContext.None, 0L)
                    {
                        lightTexture = VatGlowTexture
                    }
                    );

                    VatLights.Add(new LightSource(8, new Vector2(x, y) * 64f, 4f, new Color(255, 70, 150) * 0.85f, LightSource.LightContext.None, 0L)
                    {
                        lightTexture = VatBottomGlowTexture
                    }
                    );
                }
            }
        }
    }

    internal static void DoFacilityLightSetup(object sender, UpdateTickedEventArgs e)
    {
        if (e.IsMultipleOf(60) && WalkwayLightCounter >= 7)
        {
            VatLightCounter++;
        }
        else if (e.IsMultipleOf(90) && WalkwayLightCounter < 7)
        {
            WalkwayLightCounter++;
        }
        else
            return;

        if (WalkwayLightCounter >= 0 && WalkwayLightCounter < WalkwayLights.Count)
        {
            Game1.playSoundPitched("cowboy_gunload", 1000);
            Game1.playSoundPitched("cameraNoise", 10);
            Game1.currentLightSources.Add(WalkwayLights[WalkwayLightCounter]);
            return;
        }

        if (VatLightCounter >= 0 && VatLightCounter < 14)
        {
            Game1.playSoundPitched("cowboy_powerup", 10);
            Game1.playSoundPitched("cowboy_powerup", 100);
            Game1.playSoundPitched("cowboy_powerup", 1000);

            foreach (var vatLight in VatLights.GetRange(3 * 12 * VatLightCounter, 3 * 12).Where((light, index) => index % 3 != 0))
            {
                Game1.currentLightSources.Add(vatLight);
            }
        }

        if (WalkwayLightCounter + VatLightCounter > WalkwayLights.Count + 14)
        {
            FacilityLightsDone = true;
            Globals.EventHelper.GameLoop.UpdateTicked -= DoFacilityLightSetup;
        }
    }

    internal static void AbortLightsIfNecessary(object sender, WarpedEventArgs e)
    {
        if (e.OldLocation.Name.Equals("Custom_sophHalloweenJam_JojaFacility"))
            Globals.EventHelper.GameLoop.UpdateTicked -= DoFacilityLightSetup;
    }

    internal static void TurnOnInitialLights()
    {
        foreach (var vatLight in VatLights.Where((light, index) => index % 3 == 0))
        {
            Game1.currentLightSources.Add(vatLight);
        }
    }

    internal static void TurnOnFacilityLights()
    {
        WalkwayLightCounter = 0;
        VatLightCounter = 0;
        FacilityLightsDone = false;
        Globals.EventHelper.GameLoop.UpdateTicked += DoFacilityLightSetup;
    }

    internal static void TurnOnComputerLight()
    {
        Game1.playSoundPitched("cowboy_gunload", 1000);
        Game1.playSoundPitched("cameraNoise", 10);
        Game1.currentLightSources.Add(new(1, new Vector2(24.5f, 32f) * 64f, 4f, new Color(255, 255, 255), LightSource.LightContext.None, 0L)
        {
            lightTexture = OverheadLightTexture
        });

        Game1.currentLightSources.Add(new(1, new Vector2(24.5f, 42f) * 64f, 4f, new Color(255, 255, 255) * 0.4f, LightSource.LightContext.None, 0L)
        {
            lightTexture = WalkwayLightTexture
        });
    }

    internal static void RestoreTiles()
    {
        Game1.currentLocation.Map.GetLayer("Back").Tiles[16, 26] = Tile;
        Game1.currentLocation.Map.GetLayer("Back").Tiles[16, 27] = Tile;
        Game1.currentLocation.Map.GetLayer("Back").Tiles[17, 26] = Tile;
        Game1.currentLocation.Map.GetLayer("Back").Tiles[17, 27] = Tile;
    }

    internal static void SaveTiles()
    {
        Tile = Game1.currentLocation.Map.GetLayer("Back").Tiles[16, 26];
    }

    internal static void KillLights()
    {
        Game1.currentLightSources.Clear();
    }
}
