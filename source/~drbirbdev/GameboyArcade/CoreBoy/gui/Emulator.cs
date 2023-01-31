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
    public class Emulator: IRunnable
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
            _runnables = new List<Thread>();
            Options = options;
        }

        public void Run(CancellationToken token)
        {
            if (!Options.RomSpecified || !Options.RomFile.Exists)
            {
                throw new ArgumentException("The ROM path doesn't exist: " + Options.RomFile);
            }

            var rom = new Cartridge(Options);
            Gameboy = CreateGameboy(rom);

            if (Options.Headless)
            {
                Gameboy.Run(token);
                return;
            }

            if (Display is IRunnable runnableDisplay)
            {
                _runnables.Add(new Thread(() => runnableDisplay.Run(token))
                {
                    Priority = ThreadPriority.AboveNormal
                });
            }

            _runnables.Add(new Thread(() => Gameboy.Run(token))
            {
                Priority = ThreadPriority.AboveNormal
            });

            _runnables.ForEach(t => t.Start());
            Active = true;
        }

        public void Stop(CancellationTokenSource source)
        {
            if (!Active)
            {
                return;
            }

            source.Cancel();
            _runnables.Clear();
        }

        public void TogglePause()
        {
            if (Gameboy != null)
                Gameboy.Pause = !Gameboy.Pause;
        }

        private Gameboy CreateGameboy(Cartridge rom)
        {
            if (Options.Headless)
            {
                return new Gameboy(Options, rom, new NullDisplay(), new NullController(), new NullSoundOutput(), new NullSerialEndpoint());
            }

            return new Gameboy(Options, rom, Display, Controller, SoundOutput, SerialEndpoint);
        }
    }
}
