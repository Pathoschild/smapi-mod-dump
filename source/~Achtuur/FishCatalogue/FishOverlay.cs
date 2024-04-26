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
using FishCatalogue.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    FishSpawnsPage fishSpawnsPage;
    public FishOverlay()
    {
        fishHud = new FishHUD();
        fishSpawnsPage = new FishSpawnsPage();
    }

    protected override void DrawOverlayToScreen(SpriteBatch spriteBatch)
    {
        //fishSpawnsPage.Draw(spriteBatch);

        string loc_id = Game1.currentLocation.Name;
        if (!FishCatalogue.LocationFishData.ContainsKey(loc_id))
            return;

        fishHud.Draw(spriteBatch, ModEntry.Instance.Config.HudPosition(), 1f);
    }
}
