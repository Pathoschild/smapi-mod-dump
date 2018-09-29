using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.cooking
{
    public class CookingData
    {
        public CookingItem Meatloaf;
        public CookingItem OrangeChicken;
        public CookingItem MonteCristo;
        public CookingItem BaconCheeseburger;
        public CookingItem RoastDuck;
        public CookingItem RabbitAuVin;
        public CookingItem SteakFajitas;
        public CookingItem GlazedHam;
        public CookingItem SummerSausage;
        public CookingItem SweetAndSourPork;
        public CookingItem RabbitStew;
        public CookingItem WinterDuck;
        public CookingItem SteakWithMushrooms;
        public CookingItem CowboyDinner;
        public CookingItem Bacon;

        public CookingData()
        {
            Meatloaf = new CookingItem(370,90,0,0,0,1,0,0,0,1,1,0,1440, "639 2 256 1 248 1 -5 1 399 1",1);
            OrangeChicken = new CookingItem(250,65,0,0,2,2,0,0,0,0,0,0,600, "641 1 635 1", 1);
            MonteCristo = new CookingItem(620,120,0,0,0,3,6,0,64,2,0,0,780, "216 1 640 1 424 1 -5 1", 1);
            BaconCheeseburger = new CookingItem(660,130,0,0,0,0,0,100,0,0,0,0,960, "216 1 639 1 424 1 666 1", 1);
            RoastDuck = new CookingItem(410,100,0,0,0,0,0,0,0,1,6,0,780, "642 1", 1);
            RabbitAuVin = new CookingItem(570,110,0,0,0,4,0,0,64,2,4,4,1440, "643 1 348 1 404 1 399 1", 1);
            SteakFajitas = new CookingItem(415,100,0,0,0,0,0,0,0,2,0,0,300, "639 2 247 1 260 2 399 2 229 1", 1);
            GlazedHam = new CookingItem(550,105,0,0,6,3,0,0,64,1,0,0,960, "640 1 340 1 724 1 245 1", 1);
            SummerSausage = new CookingItem(360,90,0,0,0,0,0,0,0,0,2,2,780, "639 2 666 1 248 1", 1);
            SweetAndSourPork = new CookingItem(450,105,6,0,0,3,0,0,32,2,0,0,780, "640 1 419 1 245 1 247 1", 1);
            RabbitStew = new CookingItem(360,90,0,0,4,4,0,0,64,2,4,0,1440, "643 1 192 1 256 1 20 1 78 1", 1);
            WinterDuck = new CookingItem(360,90,0,0,0,6,0,0,0,0,0,0,1440, "642 1 637 1 250 1 416 1", 1);
            SteakWithMushrooms = new CookingItem(510,105,0,0,0,0,0,0,0,2,3,6,1440, "644 1 404 1 257 1 281 1 432 1", 1);
            CowboyDinner = new CookingItem(305,80,4,0,4,3,4,50,0,1,0,0,960, "644 1 207 1 194 1 270 1 426 1", 1);
            Bacon = new CookingItem(300,75,0,0,0,0,0,50,0,0,0,0,780, "640 1", 4);
        }

        public CookingItem getCookingItem(Cooking cooking)
        {
            switch (cooking)
            {
                case Cooking.Meatloaf:
                    return Meatloaf;
                case Cooking.OrangeChicken:
                    return OrangeChicken;
                case Cooking.MonteCristo:
                    return MonteCristo;
                case Cooking.BaconCheeseburger:
                    return BaconCheeseburger;
                case Cooking.RoastDuck:
                    return RoastDuck;
                case Cooking.RabbitAuVin:
                    return RabbitAuVin;
                case Cooking.SteakFajitas:
                    return SteakFajitas;
                case Cooking.GlazedHam:
                    return GlazedHam;
                case Cooking.SummerSausage:
                    return SummerSausage;
                case Cooking.SweetAndSourPork:
                    return SweetAndSourPork;
                case Cooking.RabbitStew:
                    return RabbitStew;
                case Cooking.WinterDuck:
                    return WinterDuck;
                case Cooking.SteakWithMushrooms:
                    return SteakWithMushrooms;
                case Cooking.CowboyDinner:
                    return CowboyDinner;
                case Cooking.Bacon:
                    return Bacon;
                default:
                    throw new ArgumentException("Invalid Cooking");
            }
        }

        public void CloneRecipeAndAmount(CookingData cookingData)
        {
            Meatloaf.CopyRecipeAndAmount(cookingData.Meatloaf);
            OrangeChicken.CopyRecipeAndAmount(cookingData.OrangeChicken);
            MonteCristo.CopyRecipeAndAmount(cookingData.MonteCristo);
            BaconCheeseburger.CopyRecipeAndAmount(cookingData.BaconCheeseburger);
            RoastDuck.CopyRecipeAndAmount(cookingData.RoastDuck);
            RabbitAuVin.CopyRecipeAndAmount(cookingData.RabbitAuVin);
            SteakFajitas.CopyRecipeAndAmount(cookingData.SteakFajitas);
            GlazedHam.CopyRecipeAndAmount(cookingData.GlazedHam);
            SummerSausage.CopyRecipeAndAmount(cookingData.SummerSausage);
            SweetAndSourPork.CopyRecipeAndAmount(cookingData.SweetAndSourPork);
            RabbitStew.CopyRecipeAndAmount(cookingData.RabbitStew);
            WinterDuck.CopyRecipeAndAmount(cookingData.WinterDuck);
            SteakWithMushrooms.CopyRecipeAndAmount(cookingData.SteakWithMushrooms);
            CowboyDinner.CopyRecipeAndAmount(cookingData.CowboyDinner);
            Bacon.CopyRecipeAndAmount(cookingData.Bacon);
    }
    }
}
