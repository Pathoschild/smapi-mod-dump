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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CoreBoy.cpu.op;
using CoreBoy.cpu.opcode;
using CoreBoy.gpu;

namespace CoreBoy.cpu
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum State
    {
        OPCODE,
        EXT_OPCODE,
        OPERAND,
        RUNNING,
        IRQ_READ_IF,
        IRQ_READ_IE,
        IRQ_PUSH_1,
        IRQ_PUSH_2,
        IRQ_JUMP,
        STOPPED,
        HALTED
    }

    public class Cpu
    {
        public Registers Registers { get; set; }
        public Opcode CurrentOpcode { get; private set; }
        public State State { get; private set; } = State.OPCODE;

        private readonly IAddressSpace _addressSpace;
        private readonly InterruptManager _interruptManager;
        private readonly Gpu _gpu;
        private readonly IDisplay _display;
        private readonly SpeedMode _speedMode;

        private int _opcode1;
        private int _opcode2;
        private readonly int[] _operand = new int[2];
        private List<Op> _ops;
        private int _operandIndex;
        private int _opIndex;


        private int _opContext;
        private int _interruptFlag;
        private int _interruptEnabled;

        private InterruptManager.InterruptType _requestedIrq;

        private int _clockCycle;
        private bool _haltBugMode;
        private readonly Opcodes _opcodes;

        public Cpu(IAddressSpace addressSpace, InterruptManager interruptManager, Gpu gpu, IDisplay display, SpeedMode speedMode)
        {
            this._opcodes = new Opcodes();
            this.Registers = new Registers();
            this._addressSpace = addressSpace;
            this._interruptManager = interruptManager;
            this._gpu = gpu;
            this._display = display;
            this._speedMode = speedMode;
        }

        public void Tick()
        {
            this._clockCycle++;
            int speed = this._speedMode.GetSpeedMode();

            if (this._clockCycle >= (4 / speed))
            {
                this._clockCycle = 0;
            }
            else
            {
                return;
            }

            if (this.State == State.OPCODE || this.State == State.HALTED || this.State == State.STOPPED)
            {
                if (this._interruptManager.IsIme() && this._interruptManager.IsInterruptRequested())
                {
                    if (this.State == State.STOPPED)
                    {
                        this._display.Enabled = true;
                    }

                    this.State = State.IRQ_READ_IF;
                }
            }

            switch (this.State)
            {
                case State.IRQ_READ_IF:
                case State.IRQ_READ_IE:
                case State.IRQ_PUSH_1:
                case State.IRQ_PUSH_2:
                case State.IRQ_JUMP:
                    this.HandleInterrupt();
                    return;
                case State.HALTED when this._interruptManager.IsInterruptRequested():
                    this.State = State.OPCODE;
                    break;
            }

            if (this.State == State.HALTED || this.State == State.STOPPED)
            {
                return;
            }

            bool accessedMemory = false;

            while (true)
            {
                int pc = this.Registers.PC;
                switch (this.State)
                {
                    case State.OPCODE:
                        this.ClearState();
                        this._opcode1 = this._addressSpace.GetByte(pc);
                        accessedMemory = true;
                        if (this._opcode1 == 0xcb)
                        {
                            this.State = State.EXT_OPCODE;
                        }
                        else if (this._opcode1 == 0x10)
                        {
                            this.CurrentOpcode = this._opcodes.Commands[this._opcode1];
                            this.State = State.EXT_OPCODE;
                        }
                        else
                        {
                            this.State = State.OPERAND;
                            this.CurrentOpcode = this._opcodes.Commands[this._opcode1];
                            if (this.CurrentOpcode == null)
                            {
                                throw new InvalidOperationException($"No command for 0x{this._opcode1:X2}");
                            }
                        }

                        if (!this._haltBugMode)
                        {
                            this.Registers.IncrementPc();
                        }
                        else
                        {
                            this._haltBugMode = false;
                        }

                        break;

                    case State.EXT_OPCODE:
                        if (accessedMemory)
                        {
                            return;
                        }

                        accessedMemory = true;
                        this._opcode2 = this._addressSpace.GetByte(pc);
                        this.CurrentOpcode ??= this._opcodes.ExtCommands[this._opcode2];

                        if (this.CurrentOpcode == null)
                        {
                            throw new InvalidOperationException($"No command for {this._opcode2:X}cb 0x{this._opcode2:X2}");
                        }

                        this.State = State.OPERAND;
                        this.Registers.IncrementPc();
                        break;

                    case State.OPERAND:
                        while (this._operandIndex < this.CurrentOpcode.Length)
                        {
                            if (accessedMemory)
                            {
                                return;
                            }

                            accessedMemory = true;
                            this._operand[this._operandIndex++] = this._addressSpace.GetByte(pc);
                            this.Registers.IncrementPc();
                        }

                        this._ops = this.CurrentOpcode.Ops.ToList();
                        this.State = State.RUNNING;
                        break;

                    case State.RUNNING:
                        if (this._opcode1 == 0x10)
                        {
                            if (this._speedMode.OnStop())
                            {
                                this.State = State.OPCODE;
                            }
                            else
                            {
                                this.State = State.STOPPED;
                                this._display.Enabled = false;
                            }

                            return;
                        }
                        else if (this._opcode1 == 0x76)
                        {
                            if (this._interruptManager.IsHaltBug())
                            {
                                this.State = State.OPCODE;
                                this._haltBugMode = true;
                                return;
                            }
                            else
                            {
                                this.State = State.HALTED;
                                return;
                            }
                        }

                        if (this._opIndex < this._ops.Count)
                        {
                            var op = this._ops[this._opIndex];
                            bool opAccessesMemory = op.ReadsMemory() || op.WritesMemory();
                            if (accessedMemory && opAccessesMemory)
                            {
                                return;
                            }

                            this._opIndex++;

                            var corruptionType = op.CausesOemBug(this.Registers, this._opContext);
                            if (corruptionType != null)
                            {
                                this.HandleSpriteBug(corruptionType.Value);
                            }

                            this._opContext = op.Execute(this.Registers, this._addressSpace, this._operand, this._opContext);
                            op.SwitchInterrupts(this._interruptManager);

                            if (!op.Proceed(this.Registers))
                            {
                                this._opIndex = this._ops.Count;
                                break;
                            }

                            if (op.ForceFinishCycle())
                            {
                                return;
                            }

                            if (opAccessesMemory)
                            {
                                accessedMemory = true;
                            }
                        }

                        if (this._opIndex >= this._ops.Count)
                        {
                            this.State = State.OPCODE;
                            this._operandIndex = 0;
                            this._interruptManager.OnInstructionFinished();
                            return;
                        }

                        break;

                    case State.HALTED:
                    case State.STOPPED:
                        return;
                }
            }
        }

        private void HandleInterrupt()
        {
            switch (this.State)
            {
                case State.IRQ_READ_IF:
                    this._interruptFlag = this._addressSpace.GetByte(0xff0f);
                    this.State = State.IRQ_READ_IE;
                    break;

                case State.IRQ_READ_IE:
                    this._interruptEnabled = this._addressSpace.GetByte(0xffff);
                    this._requestedIrq = null;
                    foreach (var irq in InterruptManager.InterruptType.Values())
                    {
                        if ((this._interruptFlag & this._interruptEnabled & (1 << irq.Ordinal)) != 0)
                        {
                            this._requestedIrq = irq;
                            break;
                        }
                    }

                    if (this._requestedIrq == null)
                    {
                        this.State = State.OPCODE;
                    }
                    else
                    {
                        this.State = State.IRQ_PUSH_1;
                        this._interruptManager.ClearInterrupt(this._requestedIrq);
                        this._interruptManager.DisableInterrupts(false);
                    }

                    break;

                case State.IRQ_PUSH_1:
                    this.Registers.DecrementSp();
                    this._addressSpace.SetByte(this.Registers.SP, (this.Registers.PC & 0xff00) >> 8);
                    this.State = State.IRQ_PUSH_2;
                    break;

                case State.IRQ_PUSH_2:
                    this.Registers.DecrementSp();
                    this._addressSpace.SetByte(this.Registers.SP, this.Registers.PC & 0x00ff);
                    this.State = State.IRQ_JUMP;
                    break;

                case State.IRQ_JUMP:
                    this.Registers.PC = this._requestedIrq.Handler;
                    this._requestedIrq = null;
                    this.State = State.OPCODE;
                    break;
            }
        }

        private void HandleSpriteBug(SpriteBug.CorruptionType type)
        {
            if (!this._gpu.GetLcdc().IsLcdEnabled())
            {
                return;
            }

            int stat = this._addressSpace.GetByte(GpuRegister.Stat.Address);
            if ((stat & 0b11) == (int)Gpu.Mode.OamSearch && this._gpu.GetTicksInLine() < 79)
            {
                SpriteBug.CorruptOam(this._addressSpace, type, this._gpu.GetTicksInLine());
            }
        }

        public void ClearState()
        {
            this._opcode1 = 0;
            this._opcode2 = 0;
            this.CurrentOpcode = null;
            this._ops = null;

            this._operand[0] = 0x00;
            this._operand[1] = 0x00;
            this._operandIndex = 0;

            this._opIndex = 0;
            this._opContext = 0;

            this._interruptFlag = 0;
            this._interruptEnabled = 0;
            this._requestedIrq = null;
        }
    }
}
