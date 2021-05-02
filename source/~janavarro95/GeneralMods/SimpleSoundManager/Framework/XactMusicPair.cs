/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.IO;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;

namespace SimpleSoundManager
{
    public class XACTMusicPair
    {
        public WaveBank waveBank;
        public ISoundBank soundBank;

        /// <summary>Create a xwb and xsb music pack pair.</summary>
        /// <param name="helper">The mod helper from the mod that will handle loading in the file.</param>
        /// <param name="wavBankPath">A relative path to the .xwb file in the mod helper's mod directory.</param>
        /// <param name="soundBankPath">A relative path to the .xsb file in the mod helper's mod directory.</param>
        public XACTMusicPair(IModHelper helper, string wavBankPath, string soundBankPath)
        {
            wavBankPath = Path.Combine(helper.DirectoryPath, wavBankPath);
            soundBankPath = Path.Combine(helper.DirectoryPath, soundBankPath);

            this.waveBank = new WaveBank(Game1.audioEngine.Engine, wavBankPath);
            this.soundBank = new SoundBankWrapper(new SoundBank(Game1.audioEngine.Engine, soundBankPath));
        }
    }
}
