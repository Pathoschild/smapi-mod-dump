using System;

namespace EconomyMod
{
    public class ModConfig
    {
        public DayOfWeek DayOfPaymentWeekly = DayOfWeek.Friday;
        public int LotValue { get; set; } = 800000;
        public TaxPaymentType TaxPaymentType { get; set; }
        public byte ThresholdInPercentageToAskAboutPayment { get; set; } = 60;
        public bool IncludeGreenhouseOnLotValue { get; set; } = true;
        public bool IncludeOwnedObjectsOnLotValue { get; set; } = true;
        public int GreenhouseValue { get; set; } = 35000; //value taken from Joja restoration.
        public bool TaxAfterFirstYear { get; set; } = true;

        public int[] ListOfDepreciationObjects = new int[] {
            746, //Jack O Lantern
            747, //RottenPlant
            748, //RottenPlant
            784, //Weeds
            785, //Weeds
            786, //Weeds
            674, //Weeds
            675, //Weeds
            676, //Weeds
            677, //Weeds
            678, //Weeds
            679, //Weeds,
            295, //Twig
            450, //Stone
        };
    }
}
