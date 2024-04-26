/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SymaLoernn/Stardew_HatsOnPetsPlus
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HatsOnPetsPlus.HOPPHelperFunctions;

namespace HatsOnPetsPlus
{
    internal class PetData
    {
        public PetData() { }

        public PetData(ExternalSpriteModData[] externalSprites) {
            foreach (ExternalSpriteModData externalSprite in externalSprites)
            {
                SpriteData internalSprite = new SpriteData(externalSprite.HatOffsetX, externalSprite.HatOffsetY, externalSprite.Direction, externalSprite.Scale, externalSprite.DoNotDrawHat);
                addSprite(externalSprite.SpriteId, externalSprite.Flipped, internalSprite);
                
                if (externalSprite.Default.HasValue && externalSprite.Default.Value)
                {
                    defaultSprite = internalSprite;
                }
            }
        }

        public void addSprite(int spriteId, bool? flipped, SpriteData sprite)
        {
            sprites[new Tuple<int, bool>(spriteId, flipped.HasValue && flipped.Value)] = sprite;
        }


        // The key is construted as a Tuple<int, bool> :
        // The first part (int) is the sprite ID (0 for the top left sprite on the sprite sheet, then its numbered left to right and top to bottom)
        // The second part of the key (bool) is the "flipped" value of the sprite : true is flipped, false isn't 
        internal Dictionary<Tuple<int, bool>, SpriteData> sprites = new Dictionary<Tuple<int, bool>, SpriteData>();

        internal SpriteData? defaultSprite;
    }
}
