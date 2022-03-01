/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes
{
    class CustomOverlays
    {

        List<Tuple<SparklingText, Vector2>> sparklingTexts = new List<Tuple<SparklingText, Vector2>>();

        public CustomOverlays(IModHelper modHelper)
        {
            modHelper.Events.Display.RenderedActiveMenu += Display_RenderedActiveMenu;
            modHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        public void AddSparklingText(SparklingText text, Vector2 position)
        {
            sparklingTexts.Add(Tuple.Create(text, position));
        }

        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            for(int i = sparklingTexts.Count - 1; i >= 0; i--)
            {
                if (sparklingTexts[i].Item1.update(Game1.currentGameTime))
                {
                    sparklingTexts.RemoveAt(i);
                }
            }
        }

        private void Display_RenderedActiveMenu(object sender, StardewModdingAPI.Events.RenderedActiveMenuEventArgs e)
        {
            foreach(var text in sparklingTexts)
            {
                text.Item1.draw(e.SpriteBatch, text.Item2);
            }
        }
    }
}
