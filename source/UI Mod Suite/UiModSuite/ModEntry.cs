using UiModSuite.UiMods;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using UiModSuite.Options;

namespace UiModSuite {
    public class ModEntry : Mod {

        public static ModData modData;
        public static ModEntry modEntry;
        public static ModConfig modConfig;
        public static IModHelper helper;
        public static string modDirectory;
        public const string saveFilePostfix = "_modData.xml";
        public static Boolean isTesting = false;
            
        public override void Entry(IModHelper helper) {
            ModEntry.helper = helper;
            ModEntry.modEntry = this;
            modData = new ModData();
            modDirectory = helper.DirectoryPath + @"\\";
            modConfig = helper.ReadConfig<ModConfig>();

            // Loads the correct settings on character load
            SaveEvents.AfterLoad += loadModData;
           
            // Skip Intro
            MenuEvents.MenuChanged += SkipIntro.onMenuChange;
        }

        internal static void Log( string log ) {
            if( isTesting ) {
                Console.WriteLine( log );
                return;
            }

            modEntry.Monitor.Log( log );
        }

        /// <summary>
        /// Loads mod specific data
        /// </summary>
        internal void loadModData( object sender, EventArgs e ) {

            string playerName = Game1.player.name;

            // File: \Mods\Demiacle_SVM\playerName_modData.xml
            // load file 
            if( File.Exists( modDirectory + playerName + saveFilePostfix ) ) {
                this.Monitor.Log( $"Mod data already exists for player {playerName}.... loading" );
                var loadedData = new ModData();
                Serializer.ReadFromXmlFile( out loadedData, playerName );

                // Only load options valid for this build
                foreach( var data in loadedData.boolSettings ) {
                    modData.boolSettings[ data.Key ] = loadedData.boolSettings[ data.Key ];
                }

                // Only load options valid for this build
                foreach( var data in loadedData.intSettings ) {
                    modData.intSettings[ data.Key ] = loadedData.intSettings[ data.Key ];
                }

                // Only load options valid for this build
                foreach( var data in loadedData.stringSettings ) {
                    modData.stringSettings[ data.Key ] = loadedData.stringSettings[ data.Key ];
                }

                // Always load character location data
                // Beware this may need a check later
                modData.locationOfTownsfolkOptions = loadedData.locationOfTownsfolkOptions;
                    


            // create file and ModData
            } else {
                this.Monitor.Log( $"Mod data does not exist for player {playerName}... creating file" );
                updateModData();
            }

            // Initialize mods
            var optionPageHandler = new ModOptionsPageHandler();

        }

        /// <summary>
        /// Overwrites the current modData to file
        /// </summary>
        internal static void updateModData() {
            Serializer.WriteToXmlFile( modData, Game1.player.name );
        }

    }
}
