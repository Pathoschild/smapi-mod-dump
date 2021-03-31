/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Let-Me-Rest
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace LetMeRest.Framework.Lists
{
    public class Secrets
    {
        public static float CheckForSecrets()
        {
            Vector2 posPlayerTile = new Vector2(Game1.player.getTileX(), Game1.player.getTileY());
            StardewValley.Object actualObject = Game1.currentLocation.getObjectAtTile((int)posPlayerTile.X, (int)posPlayerTile.Y);

            if (Game1.player.hat.Value != null && actualObject != null)
            {
                string hatName = Game1.player.hat.Value.Name;

                if (actualObject.Name == "Dark Throne" && hatName == "Wizard Hat")
                {
                    return 1.4f;
                }
                if (actualObject.Name == "Green Stool" || actualObject.Name == "Blue Stool")
                {
                    if (hatName == "Propeller Hat")
                        return 1.35f;
                }
                if (actualObject.Name == "Pink Plush Seat" && hatName == "Pink Bowl")
                {
                    return 1.20f;
                }
                if (actualObject.Name == "King Chair" && hatName == "Top Hat")
                {
                    return 1.5f;
                }
                if (actualObject.Name == "Cute Chair")
                {
                    if (hatName == "Polka Bow" || hatName == "Delicate Bow" || hatName == "Butterfly Bow")
                        return 1.20f;
                }
                if (actualObject.Name == "Tropical Chair")
                {
                    if (hatName == "Living Hat" || hatName == "Totem Mask")
                        return 1.35f;
                }
                if (actualObject.Name == "Winter Chair" && hatName == "Earmuffs")
                {
                    return 1.20f;
                }
            }

            if (Game1.player.currentLocation.Name == "IslandSouthEastCave" && Game1.player.hat.Value != null)
            {
                string hatName = Game1.player.hat.Value.Name;

                if (hatName == "Pirate Hat" || hatName == "Deluxe Pirate Hat" || hatName == "Eye Patch")
                {
                    return 1.5f;
                }
            }

            if (Game1.player.currentLocation.Name == "QiNutRoom" && Game1.player.hat.Value != null)
            {
                string hatName = Game1.player.hat.Value.Name;

                if (hatName == "Qi Mask")
                {
                    return 1.65f;
                }
                else if (hatName == "???")
                {
                    return 2f;
                }
            }

            return 1;
        }
    }
}
