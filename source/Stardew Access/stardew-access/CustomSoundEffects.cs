/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;

namespace stardew_access
{
    internal class CustomSoundEffects
    {
        internal enum TYPE
        {
            Sound,
            Footstep
        }

        internal static void Initialize()
        {
            try
            {
                Dictionary<String, TYPE> soundEffects = new Dictionary<String, TYPE>();

                soundEffects.Add("drop_item", TYPE.Sound);
                soundEffects.Add("colliding", TYPE.Sound);

                soundEffects.Add("npc_top", TYPE.Footstep);
                soundEffects.Add("npc_right", TYPE.Footstep);
                soundEffects.Add("npc_left", TYPE.Footstep);
                soundEffects.Add("npc_bottom", TYPE.Footstep);

                soundEffects.Add("obj_top", TYPE.Footstep);
                soundEffects.Add("obj_right", TYPE.Footstep);
                soundEffects.Add("obj_left", TYPE.Footstep);
                soundEffects.Add("obj_bottom", TYPE.Footstep);

                soundEffects.Add("npc_mono_top", TYPE.Footstep);
                soundEffects.Add("npc_mono_right", TYPE.Footstep);
                soundEffects.Add("npc_mono_left", TYPE.Footstep);
                soundEffects.Add("npc_mono_bottom", TYPE.Footstep);

                soundEffects.Add("obj_mono_top", TYPE.Footstep);
                soundEffects.Add("obj_mono_right", TYPE.Footstep);
                soundEffects.Add("obj_mono_left", TYPE.Footstep);
                soundEffects.Add("obj_mono_bottom", TYPE.Footstep);

                for (int i = 0; i < soundEffects.Count; i++)
                {
                    KeyValuePair<String, TYPE> soundEffect = soundEffects.ElementAt(i);

                    CueDefinition cueDefinition = new CueDefinition();
                    cueDefinition.name = soundEffect.Key;

                    if (soundEffect.Value == TYPE.Sound)
                    {
                        cueDefinition.instanceLimit = 1;
                        cueDefinition.limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest;
                    }

                    SoundEffect effect;
                    string filePath = Path.Combine(MainClass.ModHelper.DirectoryPath, "sounds", $"{soundEffect.Key}.wav");
                    using (FileStream stream = new(filePath, FileMode.Open))
                    {
                        effect = SoundEffect.FromStream(stream);
                    }

                    if (soundEffect.Value == TYPE.Sound)
                        cueDefinition.SetSound(effect, Game1.audioEngine.GetCategoryIndex("Sound"), false);
                    else if (soundEffect.Value == TYPE.Footstep)
                        cueDefinition.SetSound(effect, Game1.audioEngine.GetCategoryIndex("Footsteps"), false);

                    Game1.soundBank.AddCue(cueDefinition);
                }
            }
            catch (Exception e)
            {
                MainClass.GetMonitor().Log($"Unable to initialize custom sounds:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }
    }
}
