/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

namespace PointAndPlant.Framework
{
    internal class ModSickle : MeleeWeapon
    {
        /*********
        ** Properties
        *********/
        private readonly int Radius = 4;
        private readonly Vector2 Vector;


        /*********
        ** Public methods
        *********/
        public ModSickle(int spriteIndex, int radius, Vector2 vector)
            : base(spriteIndex)
        {
            this.Radius = radius;
            this.Vector = vector;
        }

        public new void DoDamage(GameLocation location, int x, int y, int facingDirection, int power, Farmer who)
        {
            this.isOnSpecial = false;

            if (this.type.Value != 2)
                this.DoFunction(location, x, y, power, who);

            string sound = "";

            List<Vector2> newvec = new List<Vector2>();

            int min = this.Radius * -1;
            int max = this.Radius;

            for (int nx = min; nx <= max; nx++)
            {
                for (int ny = min; ny <= max; ny++)
                {
                    newvec.Add(this.Vector + new Vector2(nx, ny));
                }
            }

            foreach (Vector2 key in newvec)
            {
                try
                {
                    if (location.terrainFeatures.ContainsKey(key) && location.terrainFeatures[key].performToolAction(this, 0, key, location))
                        location.terrainFeatures.Remove(key);
                    if (location.objects.ContainsKey(key) && location.objects[key].name.Contains("Weed") && location.objects[key].performToolAction(this, location))
                        location.objects.Remove(key);
                    if (location.performToolAction(this, (int)key.X, (int)key.Y))
                        break;
                }
                catch
                {
                    //StardewModdingAPI.Log.Info((object)("[Point-and-Plant] Exception: " + exception.Message));
                    //StardewModdingAPI.Log.Info((object)("[Point-and-Plant] Stack Trace: " + exception.StackTrace));
                }

            }
            if (!sound.Equals(""))
                Game1.playSound(sound);

            this.CurrentParentTileIndex = this.IndexOfMenuItemView;

            if (who == null || !who.isRidingHorse())
                return;

            who.completelyStopAnimatingOrDoingAction();
        }
    }
}
