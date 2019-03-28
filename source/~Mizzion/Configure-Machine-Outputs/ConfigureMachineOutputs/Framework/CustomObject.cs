/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;

namespace ConfigureMachineOutputs.Framework
{
    internal class CustomObject : StardewValley.Object
    {

        private StardewValley.Object obje;

        public CustomObject(StardewValley.Object obj, IModHelper helper)
        {
            obje = obj;
            
        }
        public CustomObject(Vector2 tileLocation, int parentSheetIndex, bool isRecipe = false)
            : base(tileLocation, parentSheetIndex, isRecipe = false)
        {
            this.isRecipe.Value = isRecipe;
            this.tileLocation.Value = tileLocation;
            this.ParentSheetIndex = parentSheetIndex;
            this.canBeSetDown.Value = true;
            this.bigCraftable.Value = true;
            string str;
            Game1.bigCraftablesInformation.TryGetValue(parentSheetIndex, out str);
            if (str != null)
            {
                string[] strArray1 = str.Split('/');
                this.name = strArray1[0];
                this.price.Value = Convert.ToInt32(strArray1[1]);
                this.edibility.Value = Convert.ToInt32(strArray1[2]);
                string[] strArray2 = strArray1[3].Split(' ');
                this.type.Value = strArray2[0];
                if (strArray2.Length > 1)
                    this.Category = Convert.ToInt32(strArray2[1]);
                this.setOutdoors.Value = Convert.ToBoolean(strArray1[5]);
                this.setIndoors.Value = Convert.ToBoolean(strArray1[6]);
                this.fragility.Value = Convert.ToInt32(strArray1[7]);
                this.isLamp.Value = strArray1.Length > 8 && strArray1[8].Equals("true");
            }
            this.initializeLightSource((Vector2)((NetFieldBase<Vector2, NetVector2>)this.tileLocation), false);
            this.boundingBox.Value = new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);
        }

        public CustomObject(int parentSheetIndex, int initialStack, bool isRecipe = false, int price = -1, int quality = 0)
            : base(Vector2.Zero, parentSheetIndex, initialStack)
        {
            this.isRecipe.Value = isRecipe;
            if (price != -1)
                this.price.Value = price;
            this.quality.Value = quality;
        }

        public CustomObject(Vector2 tileLocation, int parentSheetIndex, int initialStack)
            : base(tileLocation, parentSheetIndex, (string)null, true, true, false, false)
        {
            this.stack.Value = initialStack;
        }

        public CustomObject(
            Vector2 tileLocation,
            int parentSheetIndex,
            string Givenname,
            bool canBeSetDown,
            bool canBeGrabbed,
            bool isHoedirt,
            bool isSpawnedObject)
            : base()
        {
            this.tileLocation.Value = tileLocation;
            this.ParentSheetIndex = parentSheetIndex;
            string str;
            Game1.objectInformation.TryGetValue(parentSheetIndex, out str);
            try
            {
                if (str != null)
                {
                    string[] strArray1 = str.Split('/');
                    this.name = strArray1[0];
                    this.price.Value = Convert.ToInt32(strArray1[1]);
                    this.edibility.Value = Convert.ToInt32(strArray1[2]);
                    string[] strArray2 = strArray1[3].Split(' ');
                    this.type.Value = strArray2[0];
                    if (strArray2.Length > 1)
                        this.Category = Convert.ToInt32(strArray2[1]);
                }
            }
            catch (Exception ex)
            {
            }
            if (this.name == null && Givenname != null)
                this.name = Givenname;
            else if (this.name == null)
                this.name = "Error Item";
            this.canBeSetDown.Value = canBeSetDown;
            this.canBeGrabbed.Value = canBeGrabbed;
            this.isHoedirt.Value = isHoedirt;
            this.isSpawnedObject.Value = isSpawnedObject;
            if (Game1.random.NextDouble() < 0.5 && parentSheetIndex > 52 && (parentSheetIndex < 8 || parentSheetIndex > 15) && (parentSheetIndex < 384 || parentSheetIndex > 391))
                this.flipped.Value = true;
            if (this.name.Contains("Block"))
                this.scale = new Vector2(1f, 1f);
            if (parentSheetIndex == 449 || this.name.Contains("Weed") || this.name.Contains("Twig"))
                this.fragility.Value = 2;
            else if (this.name.Contains("Fence"))
            {
                this.scale = new Vector2(10f, 0.0f);
                canBeSetDown = false;
            }
            else if (this.name.Contains("Stone"))
            {
                switch (parentSheetIndex)
                {
                    case 8:
                        this.minutesUntilReady.Value = 4;
                        break;
                    case 10:
                        this.minutesUntilReady.Value = 8;
                        break;
                    case 12:
                        this.minutesUntilReady.Value = 16;
                        break;
                    case 14:
                        this.minutesUntilReady.Value = 12;
                        break;
                    default:
                        this.minutesUntilReady.Value = 1;
                        break;
                }
            }
            if (parentSheetIndex >= 75 && parentSheetIndex <= 77)
                isSpawnedObject = false;
            this.initializeLightSource((Vector2)((NetFieldBase<Vector2, NetVector2>)this.tileLocation), false);
            if (this.Category == -22)
                this.scale.Y = 1f;
            this.boundingBox.Value = new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);
        }

        public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            Game1.showGlobalMessage("Test");
            return false;
        }
    }
}*/
