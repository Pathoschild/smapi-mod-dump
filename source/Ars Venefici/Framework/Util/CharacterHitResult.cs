/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Util
{
    public class CharacterHitResult : HitResult
    {
        private Character character;

        public CharacterHitResult(Character character) : base(character.getStandingPosition(), 0)
        {
            this.character = character;
        }

        public CharacterHitResult(Character character, Vector2 location) : base(location, 0)
        {            
            this.character = character;
        }

        public Character GetCharacter()
        {
            return character;
        }

        public override HitResultType GetHitResultType()
        {
            return HitResultType.CHARACTER;
        }
    }
}
