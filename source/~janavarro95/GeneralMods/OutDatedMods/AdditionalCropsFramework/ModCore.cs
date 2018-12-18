using AdditionalCropsFramework.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardustCore.Objects.Tools;
using StardustCore.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AdditionalCropsFramework
{ 

    /*
    Todo: + Reuse Seedbag code from revitalize in this mod. (Done!)
          + Make a way to interact with the modded seedbags (look at revitalize's code again) (Done! Fixed with planter boxes)
          ! Serialize and deserialize the modded crop objects and planter boxes and seed bags that exist in the game. Maybe have a list to keep track of all of them or just iterate through everything in the world?
          + Make it so I can plant the modded crops. (DONE! Fixed in planterboxes)
          ! Make sure crops grow overnight.
          ! Add way for crops to be watered and to ensure that there is a graphical update when the crop is being watered.
          ! Add way to harvest crop from planterbox without removing planterbox.
          ! Fix invisible planterbox so that it does get removed when planting seeds on tillable soil and keep that soil as HoeDirt instead of reverting to normal. This can also be used to just make the dirt look wet.
          ! Add in Multiple layers to the Planter Boxes: SoilLayer->CropLayer->BoxLayer is the draw order. Mainly for aesthetics. Box on top, dry dirt below, and wet dirt below that.
         * * *  */
    public class ModCore : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static readonly List<ModularCropObject> SpringWildCrops = new List<ModularCropObject>();
        public static readonly List<ModularCropObject> SummerWildCrops = new List<ModularCropObject>();
        public static readonly List<ModularCropObject> FallWildCrops = new List<ModularCropObject>();
        public static readonly List<ModularCropObject> WinterWildCrops = new List<ModularCropObject>();
        public static Framework.Config ModConfig;

        private List<Item> shippingList;


       public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = this.Monitor;
            StardewModdingAPI.Events.SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            StardewModdingAPI.Events.SaveEvents.AfterSave += SaveEvents_AfterSave;
         
            if (!Directory.Exists(Path.Combine(ModCore.ModHelper.DirectoryPath, Utilities.EntensionsFolderName)))
            {
                Directory.CreateDirectory(Path.Combine(ModCore.ModHelper.DirectoryPath, Utilities.EntensionsFolderName));
            }


            StardustCore.ModCore.SerializationManager.acceptedTypes.Add("AdditionalCropsFramework.PlanterBox", new SerializerDataNode(new SerializerDataNode.SerializingFunction(PlanterBox.Serialize), new SerializerDataNode.ParsingFunction(PlanterBox.ParseIntoInventory), new SerializerDataNode.WorldParsingFunction(PlanterBox.SerializeFromWorld),new SerializerDataNode.SerializingToContainerFunction(PlanterBox.Serialize))); //need serialize, deserialize, and world deserialize functions.
            StardustCore.ModCore.SerializationManager.acceptedTypes.Add("AdditionalCropsFramework.ModularCropObject", new SerializerDataNode(new SerializerDataNode.SerializingFunction(ModularCropObject.Serialize), new SerializerDataNode.ParsingFunction(ModularCropObject.ParseIntoInventory), new SerializerDataNode.WorldParsingFunction(ModularCropObject.SerializeFromWorld),new SerializerDataNode.SerializingToContainerFunction(ModularCropObject.Serialize)));
            StardustCore.ModCore.SerializationManager.acceptedTypes.Add("AdditionalCropsFramework.ModularSeeds", new SerializerDataNode(new SerializerDataNode.SerializingFunction(ModularSeeds.Serialize), new SerializerDataNode.ParsingFunction(ModularSeeds.ParseIntoInventory), new SerializerDataNode.WorldParsingFunction(ModularSeeds.SerializeFromWorld),new SerializerDataNode.SerializingToContainerFunction(ModularSeeds.Serialize)));
            // StardewModdingAPI.Events.GameEvents.UpdateTick += GameEvents_UpdateTick;
            this.shippingList = new List<Item>();

          ModConfig=  helper.ReadConfig<Framework.Config>();
        }

        public void dailyUpdates()
        {
            foreach (var v in StardustCore.ModCore.SerializationManager.trackedObjectList)
            {
                if (v is PlanterBox)
                {
                    (v as PlanterBox).dayUpdate();
                }
            }
        }

        private void SaveEvents_AfterSave(object sender, EventArgs e)
        {
            dailyUpdates();
        }



        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
           
            dailyUpdates();
        }
        }
    }
