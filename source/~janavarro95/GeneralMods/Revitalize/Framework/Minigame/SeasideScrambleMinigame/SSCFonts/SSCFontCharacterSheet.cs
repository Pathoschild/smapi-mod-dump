/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardustCore.UIUtilities.SpriteFonts.Components;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCFonts
{
    public class SSCFontCharacterSheet:StardustCore.UIUtilities.SpriteFonts.CharacterSheets.GenericCharacterSheets
    {

        public SSCFontCharacterSheet()
        {
            this.CharacterAtlus = new Dictionary<char, TexturedCharacter>();
            this.CharacterAtlus.Add('0', new TexturedCharacter('0', SeasideScramble.self.textureUtils.getTexture("SSCUI", "0"), Color.White));
            this.CharacterAtlus.Add('1', new TexturedCharacter('1', SeasideScramble.self.textureUtils.getTexture("SSCUI", "1"), Color.White));
            this.CharacterAtlus.Add('2', new TexturedCharacter('2', SeasideScramble.self.textureUtils.getTexture("SSCUI", "2"), Color.White));
            this.CharacterAtlus.Add('3', new TexturedCharacter('3', SeasideScramble.self.textureUtils.getTexture("SSCUI", "3"), Color.White));
            this.CharacterAtlus.Add('4', new TexturedCharacter('4', SeasideScramble.self.textureUtils.getTexture("SSCUI", "4"), Color.White));
            this.CharacterAtlus.Add('5', new TexturedCharacter('5', SeasideScramble.self.textureUtils.getTexture("SSCUI", "5"), Color.White));
            this.CharacterAtlus.Add('6', new TexturedCharacter('6', SeasideScramble.self.textureUtils.getTexture("SSCUI", "6"), Color.White));
            this.CharacterAtlus.Add('7', new TexturedCharacter('7', SeasideScramble.self.textureUtils.getTexture("SSCUI", "7"), Color.White));
            this.CharacterAtlus.Add('8', new TexturedCharacter('8', SeasideScramble.self.textureUtils.getTexture("SSCUI", "8"), Color.White));
            this.CharacterAtlus.Add('9', new TexturedCharacter('9', SeasideScramble.self.textureUtils.getTexture("SSCUI", "9"), Color.White));
        }

    }
}
