/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using ConfigureMachineOutputs.Framework.Configs;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace ConfigureMachineOutputs.Framework.Patches
{
    internal class CheckForActionPatch
    {
        private static CmoConfig _config;
        private static IMonitor _monitor;

        //Variables that will be used by Machines that are enabled.
        private static Random _rnd;
        private static int _minOut;
        private static int _maxOut;
        private static int _output;

        public CheckForActionPatch(IMonitor monitor, CmoConfig config)
        {
            _config = config;
            _monitor = monitor;
        }

        //Gets the crystalarium time needed. Ripped from SDV source
        private static int GetMinutesForCrystalarium(int whichGem)
        {
            switch (whichGem)
            {
                case 60:
                    return 3000;
                case 62:
                    return 2240;
                case 64:
                    return 3000;
                case 66:
                    return 1360;
                case 68:
                    return 1120;
                case 70:
                    return 2400;
                case 72:
                    return 7200;
                case 80:
                    return 420;
                case 82:
                    return 1300;
                case 84:
                    return 1120;
                case 86:
                    return 800;
                default:
                    return 5000;
            }
        }
        

        public static void Postfix(SObject __instance, Farmer who, ref bool justCheckingForActivity)
        {
            if(!justCheckingForActivity)
                _monitor.Log("Postfixing SObject:checkForAction", LogLevel.Trace);


            SObject machine = __instance;

            //set up the random
            _rnd = new Random();
            if (machine.Name.Equals("Bee House") && !justCheckingForActivity)
            {
                machine.heldObject.Value.Stack = _config.Machines.BeeHouse.CustomBeeHouseEnabled
                    ? _rnd.Next(_config.Machines.BeeHouse.BeeHouseMinOutput,
                        _config.Machines.BeeHouse.BeeHouseMaxOutput)
                    : 1;
            }
            if (machine.Name.Equals("Bee House") && Game1.currentSeason.Equals("winter") && !justCheckingForActivity)
            {
                machine.heldObject.Value.Stack = _config.Machines.BeeHouse.CustomBeeHouseEnabled
                    ? _rnd.Next(_config.Machines.BeeHouse.BeeHouseMinOutput,
                        _config.Machines.BeeHouse.BeeHouseMaxOutput)
                    : 1;
            }
            if (machine.Name.Equals("Crystalarium") && !justCheckingForActivity && machine.heldObject.Value != null)
            {
                machine.heldObject.Value.Stack = _config.Machines.Crystalarium.CustomCrystalariumEnabled 
                    ? _rnd.Next(_config.Machines.Crystalarium.CrystalMinOutput, _config.Machines.Crystalarium.CystalMaxOutput) 
                    : 1;
            }
            if (machine.Name.Equals("Tapper") && !justCheckingForActivity)
            {
                _minOut = _config.Machines.Tapper.CustomTapperEnabled ? _config.Machines.Tapper.TapperMinOutput : 1;
                _maxOut = _config.Machines.Tapper.CustomTapperEnabled ? _config.Machines.Tapper.TapperMaxOutput : 1;
                _output = _rnd.Next(_minOut, _maxOut);
                machine.heldObject.Value.Stack = _output;
            }
            if (machine.Name.Equals("Worm Bin") && !justCheckingForActivity)
            {
                _minOut = _config.Machines.WormBin.CustomWormBinEnabled ? _config.Machines.WormBin.WormBinMinOutput : 1;
                _maxOut = _config.Machines.WormBin.CustomWormBinEnabled ? _config.Machines.WormBin.WormBinMaxOutput : 6;
                _output = _rnd.Next(_minOut, _maxOut);
                machine.heldObject.Value.Stack = _output;
            }
            if (machine.Name.Equals("Lightning Rod") && !justCheckingForActivity && machine.heldObject.Value != null)
            {
                _minOut = _config.Machines.LightningRod.CustomLightningRodEnabled
                    ? _config.Machines.LightningRod.LightningRodMinOutput
                    : 1;
                _maxOut = _config.Machines.LightningRod.CustomLightningRodEnabled
                    ? _config.Machines.LightningRod.LightningRodMaxOutput
                    : 1;
                _output = _rnd.Next(_minOut, _maxOut);
                machine.heldObject.Value.Stack = _output;
            }
            if (machine.Name.Equals("Solar Panel") && !justCheckingForActivity && machine.heldObject.Value != null)
            {
                _minOut = _config.Machines.SolarPanel.CustomSolarPanelEnabled ? _config.Machines.SolarPanel.SolarPanelMinOutput
                    : 1;
                _maxOut = _config.Machines.SolarPanel.CustomSolarPanelEnabled
                    ? _config.Machines.SolarPanel.SolarPanelMaxOutput
                    : 1;
                _output = _rnd.Next(_minOut, _maxOut);
                machine.heldObject.Value.Stack = _output;
            }

            if (machine.Name.Equals("Wood Chipper") && !justCheckingForActivity && machine.heldObject.Value != null)
            {
                _minOut = _config.Machines.WoodChipper.CustomWoodChipperEnabled ? _config.Machines.WoodChipper.WoodChipperMinOutput
                    : 1;
                _maxOut = _config.Machines.WoodChipper.CustomWoodChipperEnabled
                    ? _config.Machines.WoodChipper.WoodChipperMaxOutput
                    : 1;
                _output = _rnd.Next(_minOut, _maxOut);
                machine.heldObject.Value.Stack = _output;
            }
            /*
            if (machine.Name.Equals("Coffee Maker") && !justCheckingForActivity && machine.heldObject.Value != null)
            {
                _minOut = _config.Machines.CoffeeMaker.CustomCoffeeMakerEnabled ? _config.Machines.CoffeeMaker.CoffeeMakerMinOutput
                    : 1;
                _maxOut = _config.Machines.CoffeeMaker.CustomCoffeeMakerEnabled
                    ? _config.Machines.CoffeeMaker.CoffeeMakerMaxOutput
                    : 1;
                _output = _rnd.Next(_minOut, _maxOut);
                machine.heldObject.Value.Stack = _output;
            }*/

            if (machine.Name.Equals("Heavy Tapper") && !justCheckingForActivity && machine.heldObject.Value != null)
            {
                _minOut = _config.Machines.HeavyTapper.CustomHeavyTapperEnabled ? _config.Machines.HeavyTapper.HeavyTapperMinOutput
                    : 1;
                _maxOut = _config.Machines.HeavyTapper.CustomHeavyTapperEnabled
                    ? _config.Machines.HeavyTapper.HeavyTapperMaxOutput
                    : 1;
                _output = _rnd.Next(_minOut, _maxOut);
                machine.heldObject.Value.Stack = _output;
            }
        }
    }
}
