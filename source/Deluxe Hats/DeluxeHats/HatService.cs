using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using DeluxeHats.Hats;
using Harmony;

namespace DeluxeHats
{
    public static class HatService
    {
        public static int BuffId = 6284;
        public static string HarmonyId;


        public static IMonitor Monitor;
        public static IModHelper Helper;
        public static HarmonyInstance Harmony;

        public delegate void OnUpdateTickedDelegate(UpdateTickedEventArgs e);
        public static OnUpdateTickedDelegate OnUpdateTicked;

        public delegate void OnTimeChangedDelegate(TimeChangedEventArgs e);
        public static OnTimeChangedDelegate OnTimeChanged;

        public delegate void OnInventoryChangedDelegate(InventoryChangedEventArgs e);
        public static OnInventoryChangedDelegate OnInventoryChanged;

        public delegate void OnButtonPressedDelegate(ButtonPressedEventArgs e);
        public static OnButtonPressedDelegate OnButtonPressed;

        public delegate void OnDayStartDelegate(DayStartedEventArgs e);
        public static OnDayStartDelegate OnDayStarted;

        public delegate void OnDayEndingDelegate(DayEndingEventArgs e);
        public static OnDayEndingDelegate OnDayEnding;

        private delegate void DisableHatDelegate();
        private static DisableHatDelegate DisableHat;

        public static void CleanUp()
        {
            DisableHat?.Invoke();
            DisableHat = null;
            OnUpdateTicked = null;
            OnTimeChanged = null;
            OnInventoryChanged = null;
            OnButtonPressed = null;
            OnDayStarted = null;
            OnDayEnding = null;

        }

        public static void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            OnUpdateTicked?.Invoke(e);
        }

        public static void TimeChanged(object sender, TimeChangedEventArgs e)
        {
            OnTimeChanged?.Invoke(e);
        }

        public static void InventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (e.IsLocalPlayer)
            {
                OnInventoryChanged?.Invoke(e);
            }
        }

        public static void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            OnButtonPressed?.Invoke(e);
        }

        public static void DayStarted(object sender, DayStartedEventArgs e)
        {
            ApplyHatEffect();
            OnDayStarted?.Invoke(e);
        }

        public static void DayEnding(object sender, DayEndingEventArgs e)
        {
            OnDayEnding?.Invoke(e);
        }

        public static void HatChanged(Netcode.NetRef<StardewValley.Objects.Hat> field, StardewValley.Objects.Hat oldValue, StardewValley.Objects.Hat newValue)
        {
            ApplyHatEffect();
        }

        private static void ApplyHatEffect()
        {
            CleanUp();
            if (Game1.player.hat.Value != null)
            {
                Monitor.Log($"Hat Equipped: {Game1.player.hat.Value.Name}", LogLevel.Trace);
                switch (Game1.player.hat.Value.Name)
                {
                    case CowboyHat.Name:
                        CowboyHat.Activate();
                        DisableHat = CowboyHat.Disable;
                        break;
                    case BowlerHat.Name:
                        BowlerHat.Activate();
                        DisableHat = BowlerHat.Disable;
                        break;
                    case TopHat.Name:
                        TopHat.Activate();
                        DisableHat = TopHat.Disable;
                        break;
                    case Sombrero.Name:
                        Sombrero.Activate();
                        DisableHat = Sombrero.Disable;
                        break;
                    case StrawHat.Name:
                        StrawHat.Activate();
                        DisableHat = StrawHat.Disable;
                        break;
                    case OfficialCap.Name:
                        OfficialCap.Activate();
                        DisableHat = OfficialCap.Disable;
                        break;
                    case BlueBonnet.Name:
                        BlueBonnet.Activate();
                        DisableHat = BlueBonnet.Disable;
                        break;
                    case PlumChapeau.Name:
                        PlumChapeau.Activate();
                        DisableHat = PlumChapeau.Disable;
                        break;
                    case SkeletonMask.Name:
                        SkeletonMask.Activate();
                        DisableHat = SkeletonMask.Disable;
                        break;
                    case GoblinMask.Name:
                        GoblinMask.Activate();
                        DisableHat = GoblinMask.Disable;
                        break;
                    case ChickenMask.Name:
                        ChickenMask.Activate();
                        DisableHat = ChickenMask.Disable;
                        break;
                    case Earmuffs.Name:
                        Earmuffs.Activate();
                        DisableHat = Earmuffs.Disable;
                        break;
                    case DelicateBow.Name:
                        DelicateBow.Activate();
                        DisableHat = DelicateBow.Disable;
                        break;
                    case Tropiclip.Name:
                        Tropiclip.Activate();
                        DisableHat = Tropiclip.Disable;
                        break;
                    case ButterflyBow.Name:
                        ButterflyBow.Activate();
                        DisableHat = ButterflyBow.Disable;
                        break;
                    case HuntersCap.Name:
                        HuntersCap.Activate();
                        DisableHat = HuntersCap.Disable;
                        break;
                    case TruckerHat.Name:
                        TruckerHat.Activate();
                        DisableHat = TruckerHat.Disable;
                        break;
                    case SailorsCap.Name:
                        SailorsCap.Activate();
                        DisableHat = SailorsCap.Disable;
                        break;
                    case GoodOlCap.Name:
                        GoodOlCap.Activate();
                        DisableHat = GoodOlCap.Disable;
                        break;
                    case Fedora.Name:
                        Fedora.Activate();
                        DisableHat = Fedora.Disable;
                        break;
                    case CoolCap.Name:
                        CoolCap.Activate();
                        DisableHat = CoolCap.Disable;
                        break;
                    case LuckyBow.Name:
                        LuckyBow.Activate();
                        DisableHat = LuckyBow.Disable;
                        break;
                    case PolkaBow.Name:
                        PolkaBow.Activate();
                        DisableHat = PolkaBow.Disable;
                        break;
                    case GnomesCap.Name:
                        GnomesCap.Activate();
                        DisableHat = GnomesCap.Disable;
                        break;
                    case EyePatch.Name:
                        EyePatch.Activate();
                        DisableHat = EyePatch.Disable;
                        break;
                    case SantaHat.Name:
                        SantaHat.Activate();
                        DisableHat = SantaHat.Disable;
                        break;
                    case Tiara.Name:
                        Tiara.Activate();
                        DisableHat = Tiara.Disable;
                        break;
                    case HardHat.Name:
                        HardHat.Activate();
                        DisableHat = HardHat.Disable;
                        break;
                    case Souwester.Name:
                        Souwester.Activate();
                        DisableHat = Souwester.Disable;
                        break;
                    case Daisy.Name:
                        Daisy.Activate();
                        DisableHat = Daisy.Disable;
                        break;
                    case WatermelonBand.Name:
                        WatermelonBand.Activate();
                        DisableHat = WatermelonBand.Disable;
                        break;
                    case MouseEars.Name:
                        MouseEars.Activate();
                        DisableHat = MouseEars.Disable;
                        break;
                    case CatEars.Name:
                        CatEars.Activate();
                        DisableHat = CatEars.Disable;
                        break;
                    case CowgalHat.Name:
                        CowgalHat.Activate();
                        DisableHat = CowgalHat.Disable;
                        break;
                    case CowpokeHat.Name:
                        CowpokeHat.Activate();
                        DisableHat = CowpokeHat.Disable;
                        break;
                    case ArchersCap.Name:
                        ArchersCap.Activate();
                        DisableHat = ArchersCap.Disable;
                        break;
                    case PandaHat.Name:
                        PandaHat.Activate();
                        DisableHat = PandaHat.Disable;
                        break;
                    case BlueCowboyHat.Name:
                        BlueCowboyHat.Activate();
                        DisableHat = BlueCowboyHat.Disable;
                        break;
                    case RedCowboyHat.Name:
                        RedCowboyHat.Activate();
                        DisableHat = RedCowboyHat.Disable;
                        break;
                    case ConeHat.Name:
                        ConeHat.Activate();
                        DisableHat = ConeHat.Disable;
                        break;
                    case LivingHat.Name:
                        LivingHat.Activate();
                        DisableHat = LivingHat.Disable;
                        break;
                    case EmilysMagicHat.Name:
                        EmilysMagicHat.Activate();
                        DisableHat = EmilysMagicHat.Disable;
                        break;
                    case MushroomCap.Name:
                        MushroomCap.Activate();
                        DisableHat = MushroomCap.Disable;
                        break;
                    case DinosaurHat.Name:
                        DinosaurHat.Activate();
                        DisableHat = DinosaurHat.Disable;
                        break;
                    case TotemMask.Name:
                        TotemMask.Activate();
                        DisableHat = TotemMask.Disable;
                        break;
                    case LogoCap.Name:
                        LogoCap.Activate();
                        DisableHat = LogoCap.Disable;
                        break;
                    case WearableDwarfHelm.Name:
                        WearableDwarfHelm.Activate();
                        DisableHat = WearableDwarfHelm.Disable;
                        break;
                    case FashionHat.Name:
                        FashionHat.Activate();
                        DisableHat = FashionHat.Disable;
                        break;
                    case PumpkinMask.Name:
                        PumpkinMask.Activate();
                        DisableHat = PumpkinMask.Disable;
                        break;
                    case HairBone.Name:
                        HairBone.Activate();
                        DisableHat = HairBone.Disable;
                        break;
                    case KnightsHelmet.Name:
                        KnightsHelmet.Activate();
                        DisableHat = KnightsHelmet.Disable;
                        break;
                    case SquiresHelmet.Name:
                        SquiresHelmet.Activate();
                        DisableHat = SquiresHelmet.Disable;
                        break;
                    case SpottedHeadscarf.Name:
                        SpottedHeadscarf.Activate();
                        DisableHat = SpottedHeadscarf.Disable;
                        break;
                    case Beanie.Name:
                        Beanie.Activate();
                        DisableHat = Beanie.Disable;
                        break;
                    case FishingHat.Name:
                        FishingHat.Activate();
                        DisableHat = FishingHat.Disable;
                        break;
                    case BlobfishMask.Name:
                        BlobfishMask.Activate();
                        DisableHat = BlobfishMask.Disable;
                        break;
                    case PartyHat.Name:
                        PartyHat.Activate();
                        DisableHat = PartyHat.Disable;
                        break;
                    case ArcaneHat.Name:
                        ArcaneHat.Activate();
                        DisableHat = ArcaneHat.Disable;
                        break;
                    case ChefHat.Name:
                        ChefHat.Activate();
                        DisableHat = ChefHat.Disable;
                        break;
                    case PirateHat.Name:
                        PirateHat.Activate();
                        DisableHat = PirateHat.Disable;
                        break;
                    case FlatToppedHat.Name:
                        FlatToppedHat.Activate();
                        DisableHat = FlatToppedHat.Disable;
                        break;
                    case ElegantTurban.Name:
                        ElegantTurban.Activate();
                        DisableHat = ElegantTurban.Disable;
                        break;
                    case WhiteTurban.Name:
                        WhiteTurban.Activate();
                        DisableHat = WhiteTurban.Disable;
                        break;
                    case GarbageHat.Name:
                        GarbageHat.Activate();
                        DisableHat = GarbageHat.Disable;
                        break;
                    case GoldenMask.Name:
                        GoldenMask.Activate();
                        DisableHat = GoldenMask.Disable;
                        break;
                    case PropellerHat.Name:
                        PropellerHat.Activate();
                        DisableHat = PropellerHat.Disable;
                        break;
                    case BridalVeil.Name:
                        BridalVeil.Activate();
                        DisableHat = BridalVeil.Disable;
                        break;
                    case WitchHat.Name:
                        WitchHat.Activate();
                        DisableHat = WitchHat.Disable;
                        break;
                    case CopperPan.Name:
                        CopperPan.Activate();
                        DisableHat = CopperPan.Disable;
                        break;
                    case GreenTurban.Name:
                        GreenTurban.Activate();
                        DisableHat = GreenTurban.Disable;
                        break;
                    case MagicCowboyHat.Name:
                        MagicCowboyHat.Activate();
                        DisableHat = MagicCowboyHat.Disable;
                        break;
                    case MagicTurban.Name:
                        MagicTurban.Activate();
                        DisableHat = MagicTurban.Disable;
                        break;
                    default:
                        Monitor.Log($"Hat not found: {Game1.player.hat.Value.Name}", LogLevel.Warn);
                        break;
                }
            }
        }
    }
}
