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
using System.Linq;

namespace CoreBoy.memory
{
    public class MemoryRegisters : IAddressSpace
    {
        private readonly Dictionary<int, IRegister> _registers;
        private readonly Dictionary<int, int> _values = new Dictionary<int, int>();
        private readonly RegisterType[] _allowsWrite = { RegisterType.W, RegisterType.RW };
        private readonly RegisterType[] _allowsRead = { RegisterType.R, RegisterType.RW };

        public MemoryRegisters(params IRegister[] registers)
        {
            var map = new Dictionary<int, IRegister>();
            foreach (var r in registers)
            {
                if (map.ContainsKey(r.Address))
                {
                    throw new ArgumentException($"Two registers with the same address: {r.Address}");
                }

                map.Add(r.Address, r);
                this._values.Add(r.Address, 0);
            }

            this._registers = map;
        }

        private MemoryRegisters(MemoryRegisters original)
        {
            this._registers = original._registers;
            this._values = new Dictionary<int, int>(original._values);
        }

        public int Get(IRegister reg)
        {
            return this._registers.ContainsKey(reg.Address)
                ? this._values[reg.Address]
                : throw new ArgumentException("Not valid register: " + reg);
        }

        public void Put(IRegister reg, int value)
        {
            this._values[reg.Address] = this._registers.ContainsKey(reg.Address)
                ? value
                : throw new ArgumentException("Not valid register: " + reg);
        }

        public MemoryRegisters Freeze() => new MemoryRegisters(this);

        public int PreIncrement(IRegister reg)
        {
            if (!this._registers.ContainsKey(reg.Address))
            {
                throw new ArgumentException("Not valid register: " + reg);
            }

            var value = this._values[reg.Address] + 1;
            this._values[reg.Address] = value;
            return value;
        }

        public bool Accepts(int address) => this._registers.ContainsKey(address);

        public void SetByte(int address, int value)
        {
            var regType = this._registers[address].Type;
            if (this._allowsWrite.Contains(regType))
            {
                this._values[address] = value;
            }
        }

        public int GetByte(int address)
        {
            var regType = this._registers[address].Type;
            return this._allowsRead.Contains(regType) ? this._values[address] : 0xff;
        }
    }
}

