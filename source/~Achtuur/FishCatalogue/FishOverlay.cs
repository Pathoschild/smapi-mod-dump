/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework;
using AchtuurCore.Utility;
using FishCatalogue.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue;
internal class FishOverlay : Overlay
{
    FishHUD fishHud;
    FishHUD trapHud;
    public FishOverlay()
    {
        fishHud = new FishHUD();
        trapHud = new FishHUD();
    }
    public void EnableFishHud() => fishHud.Enable();

    public void DisableFishHud() => fishHud.Disable();

    public void ToggleFishHud() => fishHud.Toggle();

    public void EnableTrapHud() => trapHud.Enable();

    public void DisableTrapHud() => trapHud.Disable();

    public void ToggleTrapHud() => trapHud.Toggle();

    internal void DisableHuds()
    {
        DisableFishHud();
        DisableTrapHud();
    }

    // Sets enabled state for entire overlay based on huds and fishpage
    internal void SetEnabledState()
    {
        this.Enabled = fishHud.Enabled || trapHud.Enabled;
    }

    protected override void DrawOverlayToScreen(SpriteBatch spriteBatch)
    {
        string loc_id = Game1.currentLocation.Name;
        if (FishCatalogue.LocationFishData.ContainsKey(loc_id))
            DrawFishHud(spriteBatch, ModEntry.Instance.Config.HudPosition(), 1f);
        
        if (FishCatalogue.LocationFishAreas.ContainsKey(loc_id))
            DrawTrapHud(spriteBatch, ModEntry.Instance.Config.HudPosition(), 1f);
    }


    private void DrawFishHud(SpriteBatch sb, Vector2 pos, float alpha)
    {
        fishHud.Reset();
        if (!fishHud.Enabled)
            return;

        fishHud.AddAvailableFishBorder(FishCatalogue.GetCurrentlyAvailableFish());
        fishHud.Draw(sb, pos, alpha);
    }

    private void DrawTrapHud(SpriteBatch sb, Vector2 pos, float alpha)
    {
        trapHud.Reset();
        if (!trapHud.Enabled)
            return;

        trapHud.AddAvailableFishBorder(FishCatalogue.GetCurrentLocationTrappers());
        trapHud.Draw(sb, pos, alpha);
    }
}
