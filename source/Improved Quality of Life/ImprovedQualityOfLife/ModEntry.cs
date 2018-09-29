using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;

namespace Demiacle.ImprovedQualityOfLife {
    public class ModEntry : Mod {

        public static ModData modData;
        public static ModConfig modConfig;
        public static ModEntry modEntry;
        public static IModHelper helper;
        public static string modDirectory;
        public const string saveFilePostfix = "_modData.xml";
        public static Boolean isTesting = false;
            
        public override void Entry(IModHelper helper) {
            ModEntry.helper = helper;
            ModEntry.modEntry = this;
            modData = new ModData();
            modDirectory = helper.DirectoryPath + "\\";
            
            modConfig =  helper.ReadConfig<ModConfig>();

            SaveEvents.AfterLoad += loadModDataAndInitialize;
        }

        /// <summary>
        /// Logs to the smapi console
        /// </summary>
        internal static void Log( string log ) {
            if( isTesting ) {
                System.Console.WriteLine( log );
                return;
            }

            modEntry.Monitor.Log( log );
        }

        /// <summary>
        /// Loads mod specific data from xml into the ModData class and initializes mods afterwards
        /// </summary>
        internal void loadModDataAndInitialize( object sender, EventArgs e ) {

            // Set default options
            modData.intOptions.Add( QualityOfLifeModOptions.TIME_PER_TEN_MINUTE_OPTION, 6 );

            string playerName = Game1.player.name;

            // File: \Mods\Demiacle_SVM\playerName_modData.xml
            // load file 
            if( File.Exists( modDirectory + playerName + saveFilePostfix ) ) {
                this.Monitor.Log( $"Mod data already exists for player {playerName}.... loading" );
                var loadedData = new ModData();
                Serializer.ReadFromXmlFile( out loadedData, playerName );

                // Only load options valid for this build
                foreach( var data in loadedData.boolOptions ) {
                    if( modData.boolOptions.ContainsKey( data.Key ) ) {
                        modData.boolOptions[ data.Key ] = loadedData.boolOptions[ data.Key ];
                    }
                }

                foreach( var data in loadedData.intOptions ) {
                    if( modData.intOptions.ContainsKey( data.Key ) ) {
                        modData.intOptions[ data.Key ] = loadedData.intOptions[ data.Key ];
                    }
                }
                    

            // create file and ModData
            } else {
                this.Monitor.Log( $"Mod data does not exist for player {playerName}... creating file" );
                updateModData();
            }

            initializeMods();
        }

        /// <summary>
        /// Load all mods
        /// </summary>
        private void initializeMods() {

            if( modConfig.enableAlterTenMinute ) {
                var qualityOfLifeModOptionHandler = new QualityOfLifeModOptionHandler();
                var alterTimeSpeed = new AlterTimeSpeed();
            }

            if( modConfig.enableFasterSpeedOnRoad ) {
                var speedMod = new SpeedModOnRoads();
            }

            if( modConfig.enableHorsePassThroughSingleTiles ) {
                var reduceHorseBoundingBox = new ReduceHorseBoundingBox();
            }

            if( modConfig.enableAutoOpenGate ) {
                var autoOpenGate = new AutoOpenGate();
            }

            if( modConfig.showFishBeforeCaught ) {
                var showFishBeforeCaught = new ShowFishBeforeCaught();
            }

            if( modConfig.enableSummonHorseAnywhere ) {
                var summonHorseAnywhere = new SummonHorseAnywhere();
            }

            if( modConfig.enableGrassDropsWithoutSilo ) {
                var grassDropsBeforeSilo = new GrassDropsBeforeSilo();
            }

            if( modConfig.enableQuickFishing ) {
                var quickFish = new QuickFish();
            }

            if( modConfig.enableFastForwardHourOnKeyPress ) {
                var fastForwardHour = new FastForwardHour();
            }

            if( modConfig.showToolInventory ) {
                var toolInventory = new ToolInventory();
            }

            // Broken
            //var restoreStaminaOnToolFail = new RestoreStaminaOnToolFail();
        }

        /// <summary>
        /// Overwrites the current modData to file
        /// </summary>
        internal static void updateModData() {
            helper.WriteConfig<ModConfig>( modConfig );
            Serializer.WriteToXmlFile( modData, Game1.player.name );
        }

    }
}
