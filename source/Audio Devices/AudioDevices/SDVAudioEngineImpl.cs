/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/AudioDevices
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioDevices
{
    class SDVAudioEngineImpl : IAudioEngine
    {
        public class SDVAudioCategoryImpl : IAudioCategory
        {
            private readonly AudioCategory audioCategory;

            public SDVAudioCategoryImpl(AudioCategory audioCategory)
            {
                this.audioCategory = audioCategory;
            }

            public void SetVolume(float volume)
            {
                audioCategory.SetVolume(volume);
            }
        }

        private readonly AudioEngine audioEngine;

        public SDVAudioEngineImpl(AudioEngine audioEngine)
        {
            this.audioEngine = audioEngine;
        }

        public bool IsDisposed => audioEngine.IsDisposed;

        public AudioEngine Engine => audioEngine;

        public void Dispose()
        {
            Engine.Dispose();
        }

        public IAudioCategory GetCategory(string name)
        {
            return new SDVAudioCategoryImpl(Engine.GetCategory(name));
        }

        public void Update()
        {
            Engine.Update();
        }
    }
}
