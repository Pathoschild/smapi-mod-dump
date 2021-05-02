/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revitalize.Framework.Objects.InformationFiles;
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Revitalize.Framework.Configs
{
    public class MiningDrillConfig
    {

        public double bauxiteMineChance;
        public double tinMineChance;
        public double leadMineChance;
        public double silverMineChance;
        public double titaniumMineChance;
        public double prismaticNuggetMineChance;
        public double copperMineChance;
        public double ironMineChance;
        public double goldMineChance;
        public double iridiumMineChance;
        public double stoneMineChance;
        public double clayMineChance;
        public double sandMineChance;
        public double geodeMineChance;
        public double frozenGeodeMineChance;
        public double magmaGeodeMineChance;
        public double omniGeodeMineChance;
        //If requested put in all gems/minerals here. Otherwise hope that geodes do the trick.


        public IntRange amountOfBauxiteToMine;
        public IntRange amountOfTinToMine;
        public IntRange amountOfLeadToMine;
        public IntRange amountOfSilverToMine;
        public IntRange amountOfTitaniumToMine;
        public IntRange amountOfPrismaticNuggetsToMine;
        public IntRange amountOfCopperToMine;
        public IntRange amountOfIronToMine;
        public IntRange amountOfGoldToMine;
        public IntRange amountOfIridiumToMine;
        public IntRange amountOfStoneToMine;
        public IntRange amountOfClayToMine;
        public IntRange amountOfSandToMine;
        public IntRange amountOfGeodesToMine;
        public IntRange amountOfFrozenGeodesToMine;
        public IntRange amountOfMagmaGeodesToMine;
        public IntRange amountOfOmniGeodesToMine;

        public MiningDrillConfig()
        {
            this.bauxiteMineChance = 0.25f;
            this.tinMineChance = 0.3f;
            this.leadMineChance = 0.15f;
            this.silverMineChance = 0.10f;
            this.titaniumMineChance = 0.05f;
            this.prismaticNuggetMineChance = 0.005f;
            this.copperMineChance = 0.35f;
            this.ironMineChance = 0.20f;
            this.goldMineChance = 0.10f;
            this.iridiumMineChance = 0.005f;
            this.stoneMineChance = 1.0f;
            this.clayMineChance = 0.30f;
            this.sandMineChance = 0.20f;
            this.geodeMineChance = 0.25f;
            this.frozenGeodeMineChance = 0.15f;
            this.magmaGeodeMineChance = 0.05f;
            this.omniGeodeMineChance = 0.01f;

            this.amountOfBauxiteToMine = new IntRange(1, 3);
            this.amountOfClayToMine = new IntRange(1, 3);
            this.amountOfCopperToMine = new IntRange(1, 3);
            this.amountOfFrozenGeodesToMine = new IntRange(1, 1);
            this.amountOfGeodesToMine = new IntRange(1, 1);
            this.amountOfGoldToMine = new IntRange(1, 3);
            this.amountOfIronToMine = new IntRange(1, 3);
            this.amountOfIridiumToMine = new IntRange(1, 3);
            this.amountOfLeadToMine = new IntRange(1, 3);
            this.amountOfMagmaGeodesToMine = new IntRange(1, 1);
            this.amountOfOmniGeodesToMine = new IntRange(1, 1);
            this.amountOfPrismaticNuggetsToMine = new IntRange(1, 1);
            this.amountOfSandToMine = new IntRange(1, 2);
            this.amountOfSilverToMine = new IntRange(1, 3);
            this.amountOfStoneToMine = new IntRange(1, 5);
            this.amountOfTinToMine = new IntRange(1, 3);
            this.amountOfTitaniumToMine = new IntRange(1, 3);
        }
        

        public static MiningDrillConfig InitializeConfig()
        {
            if (File.Exists(Path.Combine(ModCore.ModHelper.DirectoryPath, "Configs", "MiningDrillMachine.json")))
                return ModCore.ModHelper.Data.ReadJsonFile<MiningDrillConfig>(Path.Combine("Configs", "MiningDrillMachine.json"));
            else
            {
                MiningDrillConfig Config = new MiningDrillConfig();
                ModCore.ModHelper.Data.WriteJsonFile<MiningDrillConfig>(Path.Combine("Configs", "MiningDrillMachine.json"), Config);
                return Config;
            }
        }
    }
}
