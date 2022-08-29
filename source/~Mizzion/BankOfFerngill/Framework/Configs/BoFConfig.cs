/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace BankOfFerngill.Framework.Configs
{
    public class BoFConfig
    {
        public int BaseBankingInterest { get; set; } = 1; //1% interest rate.

        public bool EnableRandomEvents { get; set; } = false;//Random events, like money taken or given randomly.

        public bool EnableVaultRoomDeskActivation { get; set; } = false;
        public LoanConfig LoanSettings { get; set; } = new();

        public HarderModeConfig HardModSettings { get; set; } = new();
    }
}