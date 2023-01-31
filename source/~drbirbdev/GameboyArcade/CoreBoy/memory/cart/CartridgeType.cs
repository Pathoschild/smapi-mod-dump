/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace CoreBoy.memory.cart
{
    public enum CartridgeType
    {
        ROM = 0x00,
        ROM_MBC1 = 0x01,
        ROM_MBC1_RAM = 0x02,
        ROM_MBC1_RAM_BATTERY = 0x03,
        ROM_MBC2 = 0x05,
        ROM_MBC2_BATTERY = 0x06,
        ROM_RAM = 0x08,
        ROM_RAM_BATTERY = 0x09,
        ROM_MMM01 = 0x0b,
        ROM_MMM01_SRAM = 0x0c,
        ROM_MMM01_SRAM_BATTERY = 0x0d,
        ROM_MBC3_TIMER_BATTERY = 0x0f,
        ROM_MBC3_TIMER_RAM_BATTERY = 0x10,
        ROM_MBC3 = 0x11,
        ROM_MBC3_RAM = 0x12,
        ROM_MBC3_RAM_BATTERY = 0x13,
        ROM_MBC5 = 0x19,
        ROM_MBC5_RAM = 0x1a,
        ROM_MBC5_RAM_BATTERY = 0x01b,
        ROM_MBC5_RUMBLE = 0x1c,
        ROM_MBC5_RUMBLE_SRAM = 0x1d,
        ROM_MBC5_RUMBLE_SRAM_BATTERY = 0x1e
    }
}