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
using System.IO;
using System.Linq;
using System.Text;
using CoreBoy.memory.cart.battery;
using CoreBoy.memory.cart.type;

namespace CoreBoy.memory.cart
{
    public class Cartridge : IAddressSpace
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum GameboyTypeFlag
        {
            UNIVERSAL = 0x80,
            CGB = 0xc0,
            NON_CGB = 0
        }

        private readonly IAddressSpace _addressSpace;
        private int _dmgBootstrap;

        public bool Gbc { get; }
        public string Title { get; }

        public Cartridge(GameboyOptions options)
        {
            int[] rom = LoadFile(options.RomFile);
            var type = CartridgeTypeExtensions.GetById(rom[0x0147]);

            this.Title = GetTitle(rom);
            // LOG.debug("Cartridge {}, type: {}", title, type);

            var gameboyType = GetFlag(rom[0x0143]);
            int romBanks = GetRomBanks(rom[0x0148]);
            int ramBanks = GetRamBanks(rom[0x0149]);

            if (ramBanks == 0 && type.IsRam())
            {
                // LOG.warn("RAM bank is defined to 0. Overriding to 1.");
                ramBanks = 1;
            }
            // LOG.debug("ROM banks: {}, RAM banks: {}", romBanks, ramBanks);

            IBattery battery = new NullBattery();
            if (type.IsBattery() && options.IsSupportBatterySaves())
            {
                battery = options.Battery ?? new NullBattery();
            }

            if (type.IsMbc1())
            {
                this._addressSpace = new Mbc1(rom, type, battery, romBanks, ramBanks);
            }
            else if (type.IsMbc2())
            {
                this._addressSpace = new Mbc2(rom, type, battery, romBanks);
            }
            else if (type.IsMbc3())
            {
                this._addressSpace = new Mbc3(rom, type, battery, romBanks, ramBanks);
            }
            else if (type.IsMbc5())
            {
                this._addressSpace = new Mbc5(rom, type, battery, romBanks, ramBanks);
            }
            else
            {
                this._addressSpace = new Rom(rom, type, romBanks, ramBanks);
            }

            this._dmgBootstrap = options.UseBootstrap ? 0 : 1;

            if (options.ForceCgb)
            {
                this.Gbc = true;
                return;
            }

            switch (gameboyType)
            {
                case GameboyTypeFlag.NON_CGB:
                    this.Gbc = false;
                    break;
                case GameboyTypeFlag.CGB:
                    this.Gbc = true;
                    break;
                default:
                    // UNIVERSAL
                    this.Gbc = !options.ForceDmg;
                    break;
            }
        }

        private static string GetTitle(IReadOnlyList<int> rom)
        {
            var t = new StringBuilder();
            for (int i = 0x0134; i < 0x0143; i++)
            {
                char c = (char)rom[i];
                if (c == 0)
                {
                    break;
                }

                t.Append(c);
            }

            return t.ToString();
        }

        public bool Accepts(int address) => this._addressSpace.Accepts(address) || address == 0xff50;

        public void SetByte(int address, int value)
        {
            if (address == 0xff50)
            {
                this._dmgBootstrap = 1;
            }
            else
            {
                this._addressSpace.SetByte(address, value);
            }
        }


        public int GetByte(int address)
        {
            switch (this._dmgBootstrap)
            {
                case 0 when !this.Gbc && address >= 0x0000 && address < 0x0100:
                    return BootRom.GameboyClassic[address];
                case 0 when this.Gbc && address >= 0x000 && address < 0x0100:
                    return BootRom.GameboyColor[address];
                case 0 when this.Gbc && address >= 0x200 && address < 0x0900:
                    return BootRom.GameboyColor[address - 0x0100];
            }

            return address == 0xff50 ? 0xff : this._addressSpace.GetByte(address);
        }

        private static int[] LoadFile(FileSystemInfo file)
        {
            // string ext = file.Extension;
            // TODO: If file is a zip, try extract gb, gbc or rom file to play
            // Deleted original java impl

            return File.ReadAllBytes(file.FullName).Select(x => (int)x).ToArray();
        }

        private static int GetRomBanks(int id)
        {
            return id switch
            {
                0 => 2,
                1 => 4,
                2 => 8,
                3 => 16,
                4 => 32,
                5 => 64,
                6 => 128,
                7 => 256,
                0x52 => 72,
                0x53 => 80,
                0x54 => 96,
                _ => throw new ArgumentException("Unsupported ROM size: " + Integer.ToHexString(id))
            };
        }

        private static int GetRamBanks(int id)
        {
            return id switch
            {
                0 => 0,
                1 => 1,
                2 => 1,
                3 => 4,
                4 => 16,
                _ => throw new ArgumentException("Unsupported RAM size: " + Integer.ToHexString(id))
            };
        }
        public static GameboyTypeFlag GetFlag(int value)
        {
            return value switch
            {
                0x80 => GameboyTypeFlag.UNIVERSAL,
                0xc0 => GameboyTypeFlag.CGB,
                _ => GameboyTypeFlag.NON_CGB
            };
        }
    }
}
