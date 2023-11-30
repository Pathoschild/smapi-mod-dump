/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Threading;
using CoreBoy.controller;
using CoreBoy.gpu;
using CoreBoy.memory.cart;
using CoreBoy.serial;
using CoreBoy.sound;

namespace CoreBoy.gui
{
    public class Emulator : IRunnable
    {
        public Gameboy Gameboy { get; set; }
        public IDisplay Display { get; set; } = new BitmapDisplay();
        public IController Controller { get; set; } = new NullController();
        public ISoundOutput SoundOutput { get; set; } = new NullSoundOutput();
        public SerialEndpoint SerialEndpoint { get; set; } = new NullSerialEndpoint();
        public GameboyOptions Options { get; set; }
        public bool Active { get; set; }

        private readonly List<Thread> _runnables;

        public Emulator(GameboyOptions options)
        {
            this._runnables = new List<Thread>();
            this.Options = options;
        }

        public void Run(CancellationToken token)
        {
            if (!this.Options.RomSpecified || !this.Options.RomFile.Exists)
            {
                throw new ArgumentException("The ROM path doesn't exist: " + this.Options.RomFile);
            }

            var rom = new Cartridge(this.Options);
            this.Gameboy = this.CreateGameboy(rom);

            if (this.Options.Headless)
            {
                this.Gameboy.Run(token);
                return;
            }

            if (this.Display is IRunnable runnableDisplay)
            {
                this._runnables.Add(new Thread(() => runnableDisplay.Run(token))
                {
                    Priority = ThreadPriority.AboveNormal
                });
            }

            this._runnables.Add(new Thread(() => this.Gameboy.Run(token))
            {
                Priority = ThreadPriority.AboveNormal
            });

            this._runnables.ForEach(t => t.Start());
            this.Active = true;
        }

        public void Stop(CancellationTokenSource source)
        {
            if (!this.Active)
            {
                return;
            }

            source.Cancel();
            this._runnables.Clear();
        }

        public void TogglePause()
        {
            if (this.Gameboy != null)
                this.Gameboy.Pause = !this.Gameboy.Pause;
        }

        private Gameboy CreateGameboy(Cartridge rom)
        {
            if (this.Options.Headless)
            {
                return new Gameboy(this.Options, rom, new NullDisplay(), new NullController(), new NullSoundOutput(), new NullSerialEndpoint());
            }

            return new Gameboy(this.Options, rom, this.Display, this.Controller, this.SoundOutput, this.SerialEndpoint);
        }
    }
}
