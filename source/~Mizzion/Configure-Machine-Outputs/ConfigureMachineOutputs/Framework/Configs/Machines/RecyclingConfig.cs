/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace ConfigureMachineOutputs.Framework.Configs.Machines
{
    internal class RecyclingConfig
    {
        public bool Enabled { get; set; } = true;

        private static string Id = "(BC)20";
        internal string QualityId { get; } = Id;
        public int InputMultiplier { get; set; } = 1;
        public bool ReplaceStoneWithOreEnabled { get; set; } = false;
        public int StoneMinOutput { get; set; } = 1;
        public int StoneMaxOutput { get; set; } = 4;
        /*        
        public int MinOutput { get; set; } = 1;
        public int MaxOutput { get; set; } = 2;
        */
        public double StoneToCopperChance { get; set; } = 0.5;
        public double StoneToIronChance { get; set; } = 0.3;
        public double StoneToGoldChance { get; set; } = 0.19;
        public double StoneToIridiumChance { get; set; } = 0.01;

        public bool ReplaceOreOutput { get; set; } = false;
        public int CopperMinOutput { get; set; } = 1;
        public int CopperMaxOutput { get; set; } = 2;
        public int IronMinOutput { get; set; } = 1;
        public int IronMaxOutput { get; set; } = 2;
        public int GoldMinOutput { get; set; } = 1;
        public int GoldMaxOutput { get; set; } = 2;
        public int IridiumMinOutput { get; set; } = 1;
        public int IridiumMaxOutput { get; set; } = 2;
        public int WoodMinOutput { get; set; } = 1;
        public int WoodMaxOutput { get; set; } = 2;
        public int RefinedQuartzMinOutput { get; set; } = 1;
        public int RefinedQuartzMaxOutput { get; set; } = 2;
        public int CoalMinOutput { get; set; } = 1;
        public int CoalMaxOutput { get; set; } = 2;
        public int ClothMinOutput { get; set; } = 1;
        public int ClothMaxOutput { get; set; } = 2;

        public int TorchMinOutput { get; set; } = 1;
        public int TorchMaxOutput { get; set; } = 2;

    }
}
