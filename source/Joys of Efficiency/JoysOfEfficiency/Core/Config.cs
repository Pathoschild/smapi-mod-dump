using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace JoysOfEfficiency.Core
{
    internal class Config
    {

        public bool MineInfoGui { get; set; } = true;

        public bool AutoWaterNearbyCrops { get; set; } = true;
        public int AutoWaterRadius { get; set; } = 1;
        public bool FindCanFromInventory { get; set; } = true;

        public bool GiftInformation { get; set; } = true;

        public bool AutoPetNearbyAnimals { get; set; } = false;
        public bool AutoPetNearbyPets { get; set; } = false;
        public int AutoPetRadius { get; set; } = 1;

        public bool AutoDigArtifactSpot { get; set; } = false;
        public int AutoDigRadius { get; set; } = 1;
        public bool FindHoeFromInventory { get; set; } = true;

        public bool AutoAnimalDoor { get; set; } = true;

        public bool AutoFishing { get; set; } = false;
        public float CpuThresholdFishing { get; set; } = 0.2f;

        public bool AutoReelRod { get; set; } = true;

        public bool FishingInfo { get; set; } = true;

        public bool AutoGate { get; set; } = true;

        public bool AutoEat { get; set; } = false;
        public float StaminaToEatRatio { get; set; } = 0.2f;
        public float HealthToEatRatio { get; set; } = 0.2f;

        public bool AutoHarvest { get; set; } = true;
        public int AutoHarvestRadius { get; set; } = 1;
        public bool ProtectNectarProducingFlower { get; set; } = true;
        public List<int> HarvestException { get; set; } = new List<int>();
        public SButton ButtonToggleBlackList { get; set; } = Keys.F2.ToSButton();

        public bool AutoDestroyDeadCrops { get; set; } = true;

        public bool AutoRefillWateringCan { get; set; } = true;

        public SButton ButtonShowMenu { get; set; } = Keys.R.ToSButton();
        public bool FilterBackgroundInMenu { get; set; } = true;
        public bool ShowMousePositionWhenAssigningLocation { get; set; } = true;

        public bool AutoCollectCollectibles { get; set; } = false;
        public int AutoCollectRadius { get; set; } = 1;

        public bool AutoShakeFruitedPlants { get; set; } = true;
        public int AutoShakeRadius { get; set; } = 1;

        public bool BalancedMode { get; set; } = true;

        public bool AutoDepositIngredient { get; set; } = false;
        public bool AutoPullMachineResult { get; set; } = true;
        public int MachineRadius { get; set; } = 1;

        //Fishing Probabilities
        public bool FishingProbabilitiesInfo { get; set; } = false;
        public Point ProbBoxCoordinates { get; set; } = new Point(100, 400);
        public bool MorePreciseProbabilities { get; set; } = true;
        public int TrialOfExamine { get; set; } = 10;

        public bool EstimateShippingPrice { get; set; } = true;
        public Point PriceBoxCoordinates { get; set; } = new Point(100, 100);

        public bool UnifyFlowerColors { get; set; } = false;
        public Color JazzColor { get; set; } = Color.Aqua;
        public Color TulipColor { get; set; } = Color.Red;
        public Color PoppyColor { get; set; } = Color.OrangeRed;
        public Color SummerSpangleColor { get; set; } = Color.Gold;
        public Color FairyRoseColor { get; set; } = Color.Thistle;

        public SButton ButtonToggleFlowerColorUnification { get; set; } = SButton.L;
        public Dictionary<int, Color> CustomizedFlowerColors { get; set; } = new Dictionary<int, Color>();

        public bool AutoLootTreasures { get; set; } = true;
        public bool CloseTreasureWhenAllLooted { get; set; } = false;

        public bool CollectLetterAttachmentsAndQuests { get; set; } = false;

        public bool PauseWhenIdle { get; set; } = false;
        public int IdleTimeout { get; set; } = 180;
        public int PauseNotificationX { get; set; } = 100;
        public int PauseNotificationY { get; set; } = 700;

        public bool AutoPickUpTrash { get; set; } = false;
        public int ScavengingRadius { get; set; } = 2;

        public bool AutoShearingAndMilking { get; set; } = true;
        public int AnimalHarvestRadius { get; set; } = 1;

        public int RadiusFarmCleanup { get; set; } = 1;
        public bool CutWeeds { get; set; } = false;
        public bool BreakRocks { get; set; } = false;
        public bool ChopTwigs { get; set; } = false;

        public bool DisableConfigLimitation { get; set; } = false;
    }
}
