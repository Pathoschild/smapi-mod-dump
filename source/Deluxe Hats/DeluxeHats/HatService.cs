using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using DeluxeHats.Hats;
using Harmony;
using System.Collections.Generic;
using StardewValley.Objects;

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

        public static bool LoadDisplayFields_Prefix(ref Hat __instance, ref bool __result)
        {
            try
            {
                if (__instance.Name == null) 
                {
                    __result = false;
                    return false;
                }
                foreach (KeyValuePair<int, string> keyValuePair in Game1.content.Load<Dictionary<int, string>>("Data\\hats"))
                {
                    string[] strArray = keyValuePair.Value.Split('/');
                    if (strArray[0] == __instance.Name)
                    {
                        __instance.displayName = __instance.Name;
                        if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
                            __instance.displayName = strArray[strArray.Length - 1];
                        __instance.description = $"{strArray[1]}\n\nDeluxe Hats:\n";
                        __result = true;
                        switch (strArray[0])
                        {
                            case CowboyHat.Name:
                                __instance.description += CowboyHat.Description;
                                break;
                            case BowlerHat.Name:
                                __instance.description += BowlerHat.Description;
                                break;
                            case TopHat.Name:
                                __instance.description += TopHat.Description;
                                break;
                            case Sombrero.Name:
                                __instance.description += Sombrero.Description;
                                break;
                            case StrawHat.Name:
                                __instance.description += StrawHat.Description;
                                break;
                            case OfficialCap.Name:
                                __instance.description += OfficialCap.Description;
                                break;
                            case BlueBonnet.Name:
                                __instance.description += BlueBonnet.Description;
                                break;
                            case PlumChapeau.Name:
                                __instance.description += PlumChapeau.Description;
                                break;
                            case SkeletonMask.Name:
                                __instance.description += SkeletonMask.Description;
                                break;
                            case GoblinMask.Name:
                                __instance.description += GoblinMask.Description;
                                break;
                            case ChickenMask.Name:
                                __instance.description += ChickenMask.Description;
                                break;
                            case Earmuffs.Name:
                                __instance.description += Earmuffs.Description;
                                break;
                            case DelicateBow.Name:
                                __instance.description += DelicateBow.Description;
                                break;
                            case Tropiclip.Name:
                                __instance.description += Tropiclip.Description;
                                break;
                            case ButterflyBow.Name:
                                __instance.description += ButterflyBow.Description;
                                break;
                            case HuntersCap.Name:
                                __instance.description += HuntersCap.Description;
                                break;
                            case TruckerHat.Name:
                                __instance.description += TruckerHat.Description;
                                break;
                            case SailorsCap.Name:
                                __instance.description += SailorsCap.Description;
                                break;
                            case GoodOlCap.Name:
                                __instance.description += GoodOlCap.Description;
                                break;
                            case Fedora.Name:
                                __instance.description += Fedora.Description;
                                break;
                            case CoolCap.Name:
                                __instance.description += CoolCap.Description;
                                break;
                            case LuckyBow.Name:
                                __instance.description += LuckyBow.Description;
                                break;
                            case PolkaBow.Name:
                                __instance.description += PolkaBow.Description;
                                break;
                            case GnomesCap.Name:
                                __instance.description += GnomesCap.Description;
                                break;
                            case EyePatch.Name:
                                __instance.description += EyePatch.Description;
                                break;
                            case SantaHat.Name:
                                __instance.description += SantaHat.Description;
                                break;
                            case Tiara.Name:
                                __instance.description += Tiara.Description;
                                break;
                            case HardHat.Name:
                                __instance.description += HardHat.Description;
                                break;
                            case Souwester.Name:
                                __instance.description += Souwester.Description;
                                break;
                            case Daisy.Name:
                                __instance.description += Daisy.Description;
                                break;
                            case WatermelonBand.Name:
                                __instance.description += WatermelonBand.Description;
                                break;
                            case MouseEars.Name:
                                __instance.description += MouseEars.Description;
                                break;
                            case CatEars.Name:
                                __instance.description += CatEars.Description;
                                break;
                            case CowgalHat.Name:
                                __instance.description += CowgalHat.Description;
                                break;
                            case CowpokeHat.Name:
                                __instance.description += CowpokeHat.Description;
                                break;
                            case ArchersCap.Name:
                                __instance.description += ArchersCap.Description;
                                break;
                            case PandaHat.Name:
                                __instance.description += PandaHat.Description;
                                break;
                            case BlueCowboyHat.Name:
                                __instance.description += BlueCowboyHat.Description;
                                break;
                            case RedCowboyHat.Name:
                                __instance.description += RedCowboyHat.Description;
                                break;
                            case ConeHat.Name:
                                __instance.description += ConeHat.Description;
                                break;
                            case LivingHat.Name:
                                __instance.description += LivingHat.Description;
                                break;
                            case EmilysMagicHat.Name:
                                __instance.description += EmilysMagicHat.Description;
                                break;
                            case MushroomCap.Name:
                                __instance.description += MushroomCap.Description;
                                break;
                            case DinosaurHat.Name:
                                __instance.description += DinosaurHat.Description;
                                break;
                            case TotemMask.Name:
                                __instance.description += TotemMask.Description;
                                break;
                            case LogoCap.Name:
                                __instance.description += LogoCap.Description;
                                break;
                            case WearableDwarfHelm.Name:
                                __instance.description += WearableDwarfHelm.Description;
                                break;
                            case FashionHat.Name:
                                __instance.description += FashionHat.Description;
                                break;
                            case PumpkinMask.Name:
                                __instance.description += PumpkinMask.Description;
                                break;
                            case HairBone.Name:
                                __instance.description += HairBone.Description;
                                break;
                            case KnightsHelmet.Name:
                                __instance.description += KnightsHelmet.Description;
                                break;
                            case SquiresHelmet.Name:
                                __instance.description += SquiresHelmet.Description;
                                break;
                            case SpottedHeadscarf.Name:
                                __instance.description += SpottedHeadscarf.Description;
                                break;
                            case Beanie.Name:
                                __instance.description += Beanie.Description;
                                break;
                            case FishingHat.Name:
                                __instance.description += FishingHat.Description;
                                break;
                            case BlobfishMask.Name:
                                __instance.description += BlobfishMask.Description;
                                break;
                            case PartyHat.Name:
                                __instance.description += PartyHat.Description;
                                break;
                            case ArcaneHat.Name:
                                __instance.description += ArcaneHat.Description;
                                break;
                            case ChefHat.Name:
                                __instance.description += ChefHat.Description;
                                break;
                            case PirateHat.Name:
                                __instance.description += PirateHat.Description;
                                break;
                            case FlatToppedHat.Name:
                                __instance.description += FlatToppedHat.Description;
                                break;
                            case ElegantTurban.Name:
                                __instance.description += ElegantTurban.Description;
                                break;
                            case WhiteTurban.Name:
                                __instance.description += WhiteTurban.Description;
                                break;
                            case GarbageHat.Name:
                                __instance.description += GarbageHat.Description;
                                break;
                            case GoldenMask.Name:
                                __instance.description += GoldenMask.Description;
                                break;
                            case PropellerHat.Name:
                                __instance.description += PropellerHat.Description;
                                break;
                            case BridalVeil.Name:
                                __instance.description += BridalVeil.Description;
                                break;
                            case WitchHat.Name:
                                __instance.description += WitchHat.Description;
                                break;
                            case CopperPan.Name:
                                __instance.description += CopperPan.Description;
                                break;
                            case GreenTurban.Name:
                                __instance.description += GreenTurban.Description;
                                break;
                            case MagicCowboyHat.Name:
                                __instance.description += MagicCowboyHat.Description;
                                break;
                            case MagicTurban.Name:
                                __instance.description += MagicTurban.Description;
                                break;
                            default:
                                __instance.description += "No effect.";
                                break;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                HatService.Monitor.Log($"Failed in {nameof(LoadDisplayFields_Prefix)}:\n{ex}");
                return true;
            }

        }
    }
}
