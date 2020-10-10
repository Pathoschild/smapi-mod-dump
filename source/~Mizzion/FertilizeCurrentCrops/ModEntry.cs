/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace FertilizeCurrentCrops
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            //helper.Events.Input.ButtonPressed += OnButtonPressed;

        }

        /*
         * Private Void Events
         */
        /// <summary>
        /// Event that gets triggered when a button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !e.IsDown(SButton.MouseLeft))
            return;

            //Make sure the player has a fertilizer selected and that they are left clicking
            if (e.IsDown(SButton.MouseLeft))
            {
                GameLocation loc = Game1.player.currentLocation;
                ICursorPosition curs = Helper.Input.GetCursorPosition();
                if (loc is null)
                    return;
                //Go through the locations object. Then we look for hoedirt
                foreach (var obj in loc.objects.Pairs)
                {
                    if (loc.terrainFeatures.TryGetValue(curs.GrabTile, out TerrainFeature terrainFeature) &&
                        terrainFeature is HoeDirt hoeDirt)
                    {
                        if (hoeDirt.crop != null && Game1.player.ActiveObject.Category == -19 && hoeDirt.fertilizer.Value == 0)
                        {
                            hoeDirt.fertilizer.Value = Game1.player.ActiveObject.ParentSheetIndex;
                            Game1.player.reduceActiveItemByOne();
                        }
                        else
                            Monitor.Log($"Couldn't fertilize. Fertilizer: {hoeDirt.fertilizer.Value}");
                            

                    }
                }
            }
        }
    }
}
