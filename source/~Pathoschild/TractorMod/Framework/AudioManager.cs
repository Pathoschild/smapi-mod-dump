/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData;

namespace Pathoschild.Stardew.TractorMod.Framework
{
    /// <summary>Manages audio effects for the tractor.</summary>
    internal class AudioManager : IDisposable
    {
        /*********
        ** Fields
        *********/
        /// <summary>The cue ID for the tractor engine start sound.</summary>
        private readonly string StartSoundId = "Pathoschild.TractorMod/Start";

        /// <summary>The cue ID for the tractor engine rev sound.</summary>
        private readonly string RevSoundId = "Pathoschild.TractorMod/Rev";

        /// <summary>The cue ID for the tractor engine idle sound.</summary>
        private readonly string IdleSoundId = "Pathoschild.TractorMod/Idle";

        /// <summary>The cue ID for the tractor engine stop sound.</summary>
        private readonly string StopSoundId = "Pathoschild.TractorMod/Stop";

        /// <summary>The absolute path to the Tractor Mod folder.</summary>
        private readonly string DirectoryPath;

        /// <summary>Whether to play audio effects.</summary>
        private readonly Func<bool> IsActive;

        /// <summary>The sound currently being played.</summary>
        private ICue? ActiveSound;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="directoryPath">The absolute path to the Tractor Mod folder.</param>
        /// <param name="isActive">Whether to play audio effects.</param>
        public AudioManager(string directoryPath, Func<bool> isActive)
        {
            this.DirectoryPath = directoryPath;
            this.IsActive = isActive;
        }

        /// <inheritdoc cref="IContentEvents.AssetRequested"/>
        /// <param name="e">The event data.</param>
        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/AudioChanges"))
            {
                e.Edit(editor =>
                {
                    IDictionary<string, AudioCueData> data = editor.AsDictionary<string, AudioCueData>().Data;

                    this.AddCueData(data, this.StartSoundId, "start.ogg", loop: false);
                    this.AddCueData(data, this.RevSoundId, "rev.ogg", loop: true);
                    this.AddCueData(data, this.IdleSoundId, "idle.ogg", loop: true);
                    this.AddCueData(data, this.StopSoundId, "stop.ogg", loop: false);
                });
            }
        }

        /// <summary>Set whether to play idle engine effects.</summary>
        /// <param name="state">The engine state to set.</param>
        public void SetEngineState(EngineState state)
        {
            if (!this.IsActive())
                return;

            switch (state)
            {
                // start or idle
                case EngineState.Idle:
                    if (!this.IsPlaying(this.StartSoundId) && !this.IsPlaying(this.IdleSoundId))
                    {
                        if (this.ActiveSound == null || this.ActiveSound.Name == this.StopSoundId)
                            this.StartUnlessPlaying(this.StartSoundId);
                        else
                            this.StartUnlessPlaying(this.IdleSoundId);
                    }
                    break;

                // rev
                case EngineState.Rev:
                    this.StartUnlessPlaying(this.RevSoundId);
                    break;

                // stop
                case EngineState.Stop:
                    if (this.ActiveSound != null)
                        this.StartUnlessPlaying(this.StopSoundId);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown engine state '{state}'.");
            }
        }

        /// <summary>Update the audio state if needed.</summary>
        public void Update()
        {
            if (!this.IsActive())
            {
                this.StopImmediately();
                return;
            }

            if (this.ActiveSound?.Name == this.StartSoundId && this.ActiveSound.IsStopped)
                this.StartUnlessPlaying(this.IdleSoundId);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.StopImmediately();
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the given sound is currently playing.</summary>
        /// <param name="id">The audio ID.</param>
        private bool IsPlaying(string id)
        {
            return this.ActiveSound?.Name == id && this.ActiveSound.IsPlaying;
        }

        /// <summary>Start a specific audio effect.</summary>
        /// <param name="id">The audio ID to start.</param>
        private void StartUnlessPlaying(string id)
        {
            if (this.IsPlaying(id))
                return;

            this.StopImmediately();

            this.ActiveSound = Game1.soundBank.GetCue(id);
            this.ActiveSound.Play();
        }

        /// <summary>Immediately stop all tractor audio effects.</summary>
        private void StopImmediately()
        {
            if (this.ActiveSound != null)
            {
                this.ActiveSound.Stop(AudioStopOptions.Immediate);
                this.ActiveSound.Dispose();
                this.ActiveSound = null;
            }
        }

        /// <summary>Add an audio file to the game's audio data.</summary>
        /// <param name="data">The audio data to modify.</param>
        /// <param name="id">The cue ID.</param>
        /// <param name="filename">The filename within the <c>assets/audio</c> folder.</param>
        /// <param name="loop">Whether to loop the audio track.</param>
        private void AddCueData(IDictionary<string, AudioCueData> data, string id, string filename, bool loop)
        {
            string path = Path.Combine(this.DirectoryPath, "assets", "audio", filename);

            data[id] = new AudioCueData
            {
                Id = id,
                FilePaths = new(1) { path },
                Category = "Sound",
                Looped = loop,
                StreamedVorbis = true
            };
        }
    }
}
