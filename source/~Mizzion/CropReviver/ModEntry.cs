/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace CropReviver
{
    public class ModEntry : Mod
    {
        private bool undeadCrop;
        public override void Entry(IModHelper helper)
        {
            //Our Entry Method

            helper.Events.GameLoop.DayStarted += OnDayStart;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.IsDown(SButton.F7))
            {
                undeadCrop = !undeadCrop;
                Game1.showGlobalMessage($"UndeadCrop is now {undeadCrop}");
            }

            if (e.IsDown(SButton.F8))
            {
                if (!Context.IsPlayerFree)
                    return;
                foreach (var tf in Game1.getFarm().terrainFeatures.Pairs.ToList())
                {
                    if (tf.Value is not HoeDirt dirt || dirt.crop is null)
                        return;
                   
                    var c = dirt.crop;

                    if (!c.dead.Value && undeadCrop)
                    {
                        c.growCompletely();
                        //c.fullyGrown.Value = true;
                        c.dead.Value = true;
                    }
                    
                }
            }
        }

        private void OnDayStart(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;
            
            //Lets search terrain features
            foreach (var tf in Game1.getFarm().terrainFeatures.Pairs.ToList())
            {
                if (tf.Value is not HoeDirt dirt)
                    return;
                if (dirt.crop != null)
                {
                    var c = dirt.crop;

                    if (c.dead.Value && undeadCrop)
                    {
                        c.dead.Value = false;
                        c.growCompletely();
                    }
                }
            }
        }
    }
}