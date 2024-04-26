/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Data
{
    internal static class SoundData
    {

        internal static void AddSounds()
        {

            /*DragonRoar: Dinosaur Roar by Mike Koenig Attribute 3.0 SoundBank.com */

            CueDefinition myCueDefinition = new CueDefinition();

            myCueDefinition.name = "DragonRoar";

            myCueDefinition.instanceLimit = 1;

            myCueDefinition.limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest;

            FileStream soundstream = new(Path.Combine(Mod.instance.Helper.DirectoryPath, "Sounds", "Roar.wav"), FileMode.Open);

            SoundEffect roarSound = SoundEffect.FromStream(soundstream);

            myCueDefinition.SetSound(roarSound, Game1.audioEngine.GetCategoryIndex("Sound"), false);

            Game1.soundBank.AddCue(myCueDefinition);

        }

    }

}
