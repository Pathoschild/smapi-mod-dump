/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace CropWateringBubbles
{
    public partial class ModEntry
    {
        public void updateConstantEmote() {
            GameTime time = Game1.currentGameTime;
            
            emoteInterval += time.ElapsedGameTime.Milliseconds;
            if (emoteInterval < 250f) return;
            if (currentEmoteFrame is >27 and <31) {
                currentEmoteFrame++;
            }
            else currentEmoteFrame = 28;
            emoteInterval = 0f;
        }
        public void updateEmote()
        {
            GameTime time = Game1.currentGameTime;

            emoteInterval += time.ElapsedGameTime.Milliseconds;
            if (emoteFading && emoteInterval > 20f)
            {
                emoteInterval = 0f;
                currentEmoteFrame--;
                if (currentEmoteFrame < 0)
                {
                    emoteFading = false;
                    isEmoting = false;
                }
            }
            else if (!emoteFading && emoteInterval > 20f && currentEmoteFrame <= 3)
            {
                emoteInterval = 0f;
                currentEmoteFrame++;
                if (currentEmoteFrame == 4)
                {
                    currentEmoteFrame = 28;
                }
            }
            else if (!emoteFading && emoteInterval > 250f)
            {
                emoteInterval = 0f;
                currentEmoteFrame++;
                if (currentEmoteFrame >= 28 + 4)
                {
                    emoteFading = true;
                    currentEmoteFrame = 3;
                }
            }
        }
        
        public static bool CanBecomeGiant(HoeDirt instance)
        {
            if (!Config.IncludeGiantable)
                return false;
            string indexOfHarvest = instance.crop.indexOfHarvest.Value;

            if (indexOfHarvest != "190" &&
                indexOfHarvest != "254" && 
                indexOfHarvest != "276" && 
                indexOfHarvest != "Powdermelon" && 
                indexOfHarvest != "889")
                return false;

            for(int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    if (!IsAdjacentToSame(instance, x, y))
                        return false;
                }
            }
            return true;
        }

        public static bool IsAdjacentToSame(HoeDirt instance, int v1, int v2)
        {
            return instance.Location.terrainFeatures.TryGetValue(instance.Tile + new Vector2(v1, v2), 
            out var tf) && tf is HoeDirt && (tf as HoeDirt).crop?.indexOfHarvest?.Value == instance.crop.indexOfHarvest.Value && (tf as HoeDirt).crop.currentPhase.Value >= (tf as HoeDirt).crop.phaseDays.Count - 1 && (!instance.crop.fullyGrown.Value || (tf as HoeDirt).crop.dayOfCurrentPhase.Value <= 0);
        }
    }
}