using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using System.IO;
using CustomAssetModifier.Framework.Editors;
using CustomAssetModifier.Framework;

namespace CustomAssetModifier
{

    public class CustomAssetModifier : Mod
    {
        /// <summary>
        /// Static reference to this mod's helper.
        /// </summary>
        public static IModHelper ModHelper;
        /// <summary>
        /// Static reference to this mod's monitor.
        /// </summary>
        public static IMonitor ModMonitor;

        /// <summary>
        /// Path for Mod/Content.
        /// </summary>
        public static string contentPath;
        /// <summary>
        /// Path for Mod/Content/Data
        /// </summary>
        public static string dataPath;
        /// <summary>
        /// Path for Mod/Content/Data/ObjectInformation
        /// </summary>
        public static string objectInformationPath;
        /// <summary>
        /// Path for Mod/Content/Templates/ObjectInformation
        /// </summary>
        public static string TemplatePath;


        /// <summary>
        /// Entry function for the mod.
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;

            //Just setting up a bunch of paths for the mod.
            contentPath = Path.Combine(ModHelper.DirectoryPath, "Content");
            dataPath = Path.Combine(contentPath, "Data");
            objectInformationPath = Path.Combine(dataPath, "ObjectInformation");
            TemplatePath = Path.Combine(contentPath, "Templates");

            createDirectories();
            createBlankObjectTemplate();

            //Add the ObjectInformationEditor asset editor to the list of asset editors that SMAPI uses.
            ModHelper.Content.AssetEditors.Add(new ObjectInformationEditor());
        }


        /// <summary>
        /// Creates the necessary directories for the mod.
        /// </summary>
        public void createDirectories()
        {
            //Create the Mod/Content directory.
            if (!Directory.Exists(contentPath)){
                Directory.CreateDirectory(contentPath);
            }
            //Create the Mod/Content/Data directory.
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            //Create the Mod/Content/Data/ObjectInformation directory.
            if (!Directory.Exists(objectInformationPath))
            {
                Directory.CreateDirectory(objectInformationPath);
            }
            //Create the Mod/Content/Template/ObjectInformation directory.
            if (!Directory.Exists(TemplatePath))
            {
                Directory.CreateDirectory(TemplatePath);
            }
        }

        /// <summary>
        /// Creates the blank object example for dinosaur eggs.
        /// </summary>
        public void createBlankObjectTemplate()
        {
            var ok = new AssetInformation("107","Dinosaur Egg / 720 / -300 / Arch / A giant dino egg...The entire shell is still intact!/ Mine .01 Mountain .008 / Item 1 107");
            ok.writeJson(Path.Combine(TemplatePath,"ObjectInformation",ok.id.ToString()+".json"));
        }


    }
}
