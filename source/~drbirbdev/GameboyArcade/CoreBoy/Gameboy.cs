/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Threading;
using CoreBoy.controller;
using CoreBoy.cpu;
using CoreBoy.gpu;
using CoreBoy.gui;
using CoreBoy.memory;
using CoreBoy.memory.cart;
using CoreBoy.serial;
using CoreBoy.sound;
using Timer = CoreBoy.timer.Timer;

namespace CoreBoy
{
    public class Gameboy : IRunnable
    {
        public static readonly int TicksPerSec = 4_194_304;

        public Mmu Mmu { get; }
        public Cpu Cpu { get; }
        public SpeedMode SpeedMode { get; }

        public bool Pause { get; set; }

        private readonly Gpu _gpu;
        private readonly Timer _timer;
        private readonly Dma _dma;
        private readonly Hdma _hdma;
        private readonly IDisplay _display;
        private readonly Sound _sound;
        private readonly SerialPort _serialPort;

        private readonly bool _gbc;

        public Gameboy(
            GameboyOptions options,
            Cartridge rom,
            IDisplay display,
            IController controller,
            ISoundOutput soundOutput,
            SerialEndpoint serialEndpoint)
        {
            this._display = display;
            this._gbc = rom.Gbc;
            this.SpeedMode = new SpeedMode();

            var interruptManager = new InterruptManager(this._gbc);

            this._timer = new Timer(interruptManager, this.SpeedMode);
            this.Mmu = new Mmu();

            var oamRam = new Ram(0xfe00, 0x00a0);

            this._dma = new Dma(this.Mmu, oamRam, this.SpeedMode);
            this._gpu = new Gpu(display, interruptManager, this._dma, oamRam, this._gbc);
            this._hdma = new Hdma(this.Mmu);
            this._sound = new Sound(soundOutput, this._gbc);
            this._serialPort = new SerialPort(interruptManager, serialEndpoint, this.SpeedMode);

            this.Mmu.AddAddressSpace(rom);
            this.Mmu.AddAddressSpace(this._gpu);
            this.Mmu.AddAddressSpace(new Joypad(interruptManager, controller));
            this.Mmu.AddAddressSpace(interruptManager);
            this.Mmu.AddAddressSpace(this._serialPort);
            this.Mmu.AddAddressSpace(this._timer);
            this.Mmu.AddAddressSpace(this._dma);
            this.Mmu.AddAddressSpace(this._sound);

            this.Mmu.AddAddressSpace(new Ram(0xc000, 0x1000));
            if (this._gbc)
            {
                this.Mmu.AddAddressSpace(this.SpeedMode);
                this.Mmu.AddAddressSpace(this._hdma);
                this.Mmu.AddAddressSpace(new GbcRam());
                this.Mmu.AddAddressSpace(new UndocumentedGbcRegisters());
            }
            else
            {
                this.Mmu.AddAddressSpace(new Ram(0xd000, 0x1000));
            }

            this.Mmu.AddAddressSpace(new Ram(0xff80, 0x7f));
            this.Mmu.AddAddressSpace(new ShadowAddressSpace(this.Mmu, 0xe000, 0xc000, 0x1e00));

            this.Cpu = new Cpu(this.Mmu, interruptManager, this._gpu, display, this.SpeedMode);

            interruptManager.DisableInterrupts(false);

            if (!options.UseBootstrap)
            {
                this.InitiliseRegisters();
            }
        }

        private void InitiliseRegisters()
        {
            var registers = this.Cpu.Registers;

            registers.SetAf(0x01b0);
            if (this._gbc)
            {
                registers.A = 0x11;
            }

            registers.SetBc(0x0013);
            registers.SetDe(0x00d8);
            registers.SetHl(0x014d);
            registers.SP = 0xfffe;
            registers.PC = 0x0100;
        }

        public void Run(CancellationToken token)
        {
            bool requestedScreenRefresh = false;
            bool lcdDisabled = false;

            while (!token.IsCancellationRequested)
            {
                if (this.Pause)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                var newMode = this.Tick();
                if (newMode.HasValue)
                {
                    this._hdma.OnGpuUpdate(newMode.Value);
                }

                if (!lcdDisabled && !this._gpu.IsLcdEnabled())
                {
                    lcdDisabled = true;
                    this._display.RequestRefresh();
                    this._hdma.OnLcdSwitch(false);
                }
                else if (newMode == Gpu.Mode.VBlank)
                {
                    requestedScreenRefresh = true;
                    this._display.RequestRefresh();
                }

                if (lcdDisabled && this._gpu.IsLcdEnabled())
                {
                    lcdDisabled = false;
                    this._display.WaitForRefresh();
                    this._hdma.OnLcdSwitch(true);
                }
                else if (requestedScreenRefresh && newMode == Gpu.Mode.OamSearch)
                {
                    requestedScreenRefresh = false;
                    this._display.WaitForRefresh();
                }
            }
        }

        public Gpu.Mode? Tick()
        {
            this._timer.Tick();
            if (this._hdma.IsTransferInProgress())
            {
                this._hdma.Tick();
            }
            else
            {
                this.Cpu.Tick();
            }

            this._dma.Tick();
            // TODO: get sound working
            // TODO: Make it async probably too, since it's sloooow
            this._sound.Tick();
            this._serialPort.Tick();
            return this._gpu.Tick();
        }
    }
}
