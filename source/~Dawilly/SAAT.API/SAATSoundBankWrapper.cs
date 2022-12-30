/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/SAAT
**
*************************************************/

using System;
using System.Runtime.CompilerServices;

using Microsoft.Xna.Framework.Audio;

using StardewModdingAPI;

using StardewValley;

[assembly: InternalsVisibleTo("SAAT.Mod")]

namespace SAAT.API
{
    /// <summary>
    /// Implementation of Stardew Valley's <see cref="ISoundBank"/> interface. Allowing for additional 
    /// functionality in regards to <see cref="Cue"/> management.
    /// </summary>
    internal class SAATSoundBankWrapper : ISoundBank
    {
        private readonly ICue defaultCue;
        private readonly IMonitor monitor;
        private readonly ISoundBank sdvSoundBankWrapper;
        private readonly CueDefinition defaultDefinition;

        private bool disposed;

        /// <summary>Gets a value indicating if the sound bank is in use.</summary>
        public bool IsInUse => this.sdvSoundBankWrapper.IsInUse;

        /// <summary>Gets a value indicating if the sound bank has been disposed of.</summary>
        public bool IsDisposed => this.disposed;

        internal ISoundBank VanillaSoundBank => this.sdvSoundBankWrapper;

        /// <summary>
        /// Create a new instance of the <see cref="SAATSoundBankWrapper"/> class, an implementation of <see cref="ISoundBank"/>.
        /// </summary>
        /// <param name="defaultCue">The cue implementation to default to on failure to retrieve/load cues.</param>
        /// <param name="monitor">Implementation of SMAPI's monitor and logging system.</param>
        /// <param name="wrapper">The game code's implementation of <see cref="ISoundBank"/> currently in use.</param>
        internal SAATSoundBankWrapper(CueDefinition defaultCue, IMonitor monitor, ISoundBank wrapper)
        {
            this.defaultDefinition = defaultCue;
            wrapper.AddCue(defaultCue);
            this.defaultCue = wrapper.GetCue(this.defaultDefinition.name);

            this.sdvSoundBankWrapper = wrapper;
            this.monitor = monitor;
        }

        /// <summary>
        /// Finalizer for the <see cref="SAATSoundBankWrapper"/> class.
        /// </summary>
        ~SAATSoundBankWrapper()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Add a <see cref="CueDefinition"/> to the sound bank, allowing for cue creation of the audio track.
        /// </summary>
        /// <param name="cueDefinition">The instance to add.</param>
        public void AddCue(CueDefinition cueDefinition)
        {
            this.sdvSoundBankWrapper.AddCue(cueDefinition);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get a cue by its corresponding name.
        /// </summary>
        /// <param name="name">The name of the cue.</param>
        /// <returns>The <see cref="ICue"/> instance with a matching name.</returns>
        /// <remarks>Returns a silent cue on failure.</remarks>
        public ICue GetCue(string name)
        {
            try
            {
                return this.sdvSoundBankWrapper.GetCue(name);
            }
            catch (ArgumentNullException noCall)
            {
                this.ErrorNullOrEmtpyName(noCall, nameof(GetCue));
            }
            catch (ArgumentException badCall)
            {
                this.ErrorNotFound(badCall, nameof(Cue), name);
            }

            return this.defaultCue;
        }

        /// <summary>
        /// Gets the underlying cue definition of a cue.
        /// </summary>
        /// <param name="name">The name of the cue definition.</param>
        /// <returns>The cue definition instance.</returns>
        public CueDefinition GetCueDefinition(string name)
        {
            try
            {
                return this.sdvSoundBankWrapper.GetCueDefinition(name);
            }
            catch (ArgumentNullException noCall)
            {
                this.ErrorNullOrEmtpyName(noCall, nameof(GetCueDefinition));
            }
            catch (ArgumentException badCall)
            {
                this.ErrorNotFound(badCall, nameof(CueDefinition), name);
            }

            return this.defaultDefinition;
        }

        /// <summary>
        /// plays a cue.
        /// </summary>
		/// <param name="name">The name of the cue to play.</param>
        public void PlayCue(string name)
        {
            try
            {
                this.sdvSoundBankWrapper.PlayCue(name);
                return;
            }
            catch (ArgumentNullException noCall)
            {
                this.ErrorNullOrEmtpyName(noCall, nameof(PlayCue));
            }
            catch (ArgumentException badCall)
            {
                this.ErrorNotFound(badCall, nameof(Cue), name);
            }

            this.defaultCue.Play();
        }

        /// <summary>
        /// Plays a cue. The static, 3d positional information is not implemented.
        /// </summary>
		/// <param name="name">The name of the cue to play.</param>
		/// <param name="listener">The listener state.</param>
		/// <param name="emitter">The cue emitter state.</param>
        public void PlayCue(string name, AudioListener listener, AudioEmitter emitter)
        {
            try
            {
                this.sdvSoundBankWrapper.PlayCue(name, listener, emitter);
                return;
            }
            catch (ArgumentNullException noCall)
            {
                this.ErrorNullOrEmtpyName(noCall, nameof(PlayCue));
            }
            catch (ArgumentException badCall)
            {
                this.ErrorNotFound(badCall, nameof(Cue), name);
            }

            //this.defaultCue.Prepare();
            //this.defaultCue.Apply3D(listener, emitter);
            this.defaultCue.Play();
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            // Dispose managed resources.
            if (disposing)
            {
                // This is managed. Way underneath it (WaveBank -> SoundEffects) there is unmanaged.
                this.sdvSoundBankWrapper.Dispose();
            }

            // Dispose unmanaged resources.

            this.disposed = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ErrorNullOrEmtpyName(ArgumentNullException exception, string methodName)
        {
            this.monitor.Log($"A mod called {methodName} with an empty name. See logs for details.", LogLevel.Warn);
            this.monitor.Log($"Caller: {exception.Source}\n Stack Trace: {exception.StackTrace}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ErrorNotFound(ArgumentException exception, string type, string name)
        {
            this.monitor.Log($"Could not find a {type} with the name \"{name}\". See logs for details.", LogLevel.Warn);
            this.monitor.Log($"Caller: {exception.Source}\n Stack Trace: {exception.StackTrace}");
        }
    }
}
