/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BleakCodex/SimpleSounds
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;
using System.IO;

namespace SimpleSounds
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                Content? data = contentPack.ReadJsonFile<Content>("content.json");

                if (data is null)
                {
                    this.Monitor.Log("No content.json found, skipping...", LogLevel.Error);
                    continue;
                }

                if (data.Sounds is null)
                {
                    this.Monitor.Log("Sounds is a require parameter, skipping...", LogLevel.Error);
                    continue;
                }

                if (data.Sounds is null)
                {
                    this.Monitor.Log("Sounds is a require parameter, skipping...", LogLevel.Error);
                    continue;
                }

                for (int i = 0; i < data.Sounds.Count; i++)
                {
                    Sound sound = data.Sounds[i];

                    if (sound.Name is null)
                    {
                        this.Monitor.Log($"Sounds[{i}] is missing required attribute Name", LogLevel.Error);
                        continue;
                    }
                    if (sound.FromFile is null)
                    {
                        this.Monitor.Log($"Sounds[{i}] is missing required attribute FromFile", LogLevel.Error);
                        continue;
                    }

                    CueDefinition myCueDefinition = new CueDefinition();
                    myCueDefinition.name = sound.Name;
                    if (sound.InstanceLimit > 0)
                    {
                        myCueDefinition.instanceLimit = sound.InstanceLimit;
                        myCueDefinition.limitBehavior = sound.LimitBehavior;
                    }
                    SoundEffect audio;
                    string filePathCombined = Path.Combine(contentPack.DirectoryPath, sound.FromFile);
                    using (FileStream stream = new FileStream(filePathCombined, FileMode.Open))
                    {
                        audio = SoundEffect.FromStream(stream);
                    }
                    myCueDefinition.SetSound(audio, Game1.audioEngine.GetCategoryIndex("Sound"), sound.Loop);
                    Game1.soundBank.AddCue(myCueDefinition);

                    this.Monitor.Log($"Added new sound {sound.Name} to soundbank", LogLevel.Error);
                }
                    
            }
        }
    }
}
