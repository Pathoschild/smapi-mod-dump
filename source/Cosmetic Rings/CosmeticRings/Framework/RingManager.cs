/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CosmeticRings
**
*************************************************/

using CosmeticRings.Framework.Rings;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmeticRings.Framework
{
    internal enum RingType
    {
        Unknown,
        PetalRing,
        ButterflyRing,
        FairyRing,
        RaindropRing,
        BunnyRing,
        JunimoRing,
        SlimeRing,
        FireflyRing,
        FrogRing,
        DustRing,
        BatRing
    }

    internal static class RingManager
    {
        private static readonly string _ringNamePrefix = "PeacefulEnd.Rings";
        internal static List<CustomRing> wornCustomRings = new List<CustomRing>();

        internal static List<string> GetRingNames()
        {
            List<string> ringNames = new List<string>();
            foreach (var ringType in Enum.GetValues(typeof(RingType)))
            {
                ringNames.Add(String.Concat(_ringNamePrefix, ".", ringType));
            }

            return ringNames;
        }

        internal static bool IsCosmeticRing(string ringName)
        {
            return GetRingNames().Contains(ringName);
        }

        internal static bool HasCosmeticRingEquipped(Farmer who)
        {
            return wornCustomRings.Any();
        }

        internal static void LoadWornRings(Farmer who, GameLocation location, IEnumerable<Ring> rings)
        {
            wornCustomRings = new List<CustomRing>();
            foreach (Ring ring in rings.Where(r => r != null && IsCosmeticRing(r.Name)))
            {
                HandleEquip(who, location, ring);
            }
        }

        internal static CustomRing CreateCustomRing(Ring ring)
        {
            CustomRing customRing = null;
            switch (GetRingTypeFromName(ring.Name))
            {
                case RingType.PetalRing:
                    customRing = new PetalRing(ring);
                    break;
                case RingType.ButterflyRing:
                    customRing = new ButterflyRing(ring);
                    break;
                case RingType.FairyRing:
                    customRing = new FairyRing(ring);
                    break;
                case RingType.RaindropRing:
                    customRing = new RaindropRing(ring);
                    break;
                case RingType.BunnyRing:
                    customRing = new BunnyRing(ring);
                    break;
                case RingType.JunimoRing:
                    customRing = new JunimoRing(ring);
                    break;
                case RingType.SlimeRing:
                    customRing = new SlimeRing(ring);
                    break;
                case RingType.FireflyRing:
                    customRing = new FireflyRing(ring);
                    break;
                case RingType.FrogRing:
                    customRing = new FrogRing(ring);
                    break;
                case RingType.DustRing:
                    customRing = new DustRing(ring);
                    break;
                case RingType.BatRing:
                    customRing = new BatRing(ring);
                    break;
                default:
                    // Do nothing, though we should never reach here as Unknown isn't handled
                    break;
            }

            return customRing;
        }

        internal static void UpdateRingEffects(Farmer who, GameLocation location)
        {
            foreach (CustomRing ring in wornCustomRings)
            {
                ring.Update(who, location);
            }
        }

        internal static void HandleEquip(Farmer who, GameLocation location, Ring ring)
        {
            CustomRing customRing = CreateCustomRing(ring);
            if (customRing != null)
            {
                customRing.HandleEquip(who, location);
                wornCustomRings.Add(customRing);
            }
        }

        internal static void HandleUnequip(Farmer who, GameLocation location, Ring ring)
        {
            CustomRing customRing = wornCustomRings.FirstOrDefault(r => r.RingObject == ring);
            if (customRing != null)
            {
                customRing.HandleUnequip(who, location);
                wornCustomRings.Remove(customRing);
            }
        }

        internal static void HandleNewLocation(Farmer who, GameLocation location, Ring ring)
        {
            // This will handle save loading
            CustomRing customRing = wornCustomRings.FirstOrDefault(r => r.RingObject == ring);
            if (customRing != null)
            {
                customRing.HandleNewLocation(who, location);
            }
        }

        internal static void HandleLeaveLocation(Farmer who, GameLocation location, Ring ring)
        {
            CustomRing customRing = wornCustomRings.FirstOrDefault(r => r.RingObject == ring);
            if (customRing != null)
            {
                customRing.HandleLeaveLocation(who, location);
            }
        }

        private static RingType GetRingTypeFromName(string ringName)
        {
            switch (ringName.Replace(String.Concat(_ringNamePrefix, "."), ""))
            {
                case nameof(RingType.PetalRing):
                    return RingType.PetalRing;
                case nameof(RingType.ButterflyRing):
                    return RingType.ButterflyRing;
                case nameof(RingType.FairyRing):
                    return RingType.FairyRing;
                case nameof(RingType.RaindropRing):
                    return RingType.RaindropRing;
                case nameof(RingType.BunnyRing):
                    return RingType.BunnyRing;
                case nameof(RingType.JunimoRing):
                    return RingType.JunimoRing;
                case nameof(RingType.SlimeRing):
                    return RingType.SlimeRing;
                case nameof(RingType.FireflyRing):
                    return RingType.FireflyRing;
                case nameof(RingType.FrogRing):
                    return RingType.FrogRing;
                case nameof(RingType.DustRing):
                    return RingType.DustRing;
                case nameof(RingType.BatRing):
                    return RingType.BatRing;
                default:
                    return RingType.Unknown;
            }
        }
    }
}
