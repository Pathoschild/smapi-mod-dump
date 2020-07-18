using StardewModdingAPI;
using StardewValley.Tools;
using System;

namespace SvFishingMod
{
    public sealed partial class FishingMod : Mod
    {
        private IReflectedField<int> maxFishingBiteTimeField = null;

        private IReflectedField<int> minFishingBiteTimeField = null;

        private int bobberBarHeight // Hardcoded Max: 568
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<int>(_fishMenu, nameof(bobberBarHeight), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<int>(_fishMenu, nameof(bobberBarHeight), true).SetValue(value);
            }
        }

        private bool bossFish
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<bool>(_fishMenu, nameof(bossFish), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<bool>(_fishMenu, nameof(bossFish), true).SetValue(value);
            }
        }
        private float difficulty
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<float>(_fishMenu, nameof(difficulty), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<float>(_fishMenu, nameof(difficulty), true).SetValue(value);
            }
        }

        private float distanceFromCatching
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<float>(_fishMenu, nameof(distanceFromCatching), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<float>(_fishMenu, nameof(distanceFromCatching), true).SetValue(value);
            }
        }

        private bool fadeOut
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<bool>(_fishMenu, nameof(fadeOut), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<bool>(_fishMenu, nameof(fadeOut), true).SetValue(value);
            }
        }

        private int fishQuality
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<int>(_fishMenu, nameof(fishQuality), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<int>(_fishMenu, nameof(fishQuality), true).SetValue(value);
            }
        }

        private int fishSize
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<int>(_fishMenu, nameof(fishSize), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<int>(_fishMenu, nameof(fishSize), true).SetValue(value);
            }
        }

        private bool fromFishPond
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<bool>(_fishMenu, nameof(fromFishPond), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<bool>(_fishMenu, nameof(fromFishPond), true).SetValue(value);
            }
        }

        private bool handledFishResult
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<bool>(_fishMenu, nameof(handledFishResult), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<bool>(_fishMenu, nameof(handledFishResult), true).SetValue(value);
            }
        }

        private int maxFishingBiteTime
        {
            get
            {
                if (maxFishingBiteTimeField == null)
                    maxFishingBiteTimeField = Helper.Reflection.GetField<int>(typeof(FishingRod), nameof(maxFishingBiteTime), true);

                return maxFishingBiteTimeField.GetValue();
            }
            set
            {
                if (maxFishingBiteTimeField == null)
                    maxFishingBiteTimeField = Helper.Reflection.GetField<int>(typeof(FishingRod), nameof(maxFishingBiteTime), true);

                maxFishingBiteTimeField.SetValue(value);
            }
        }

        private int maxFishSize
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<int>(_fishMenu, nameof(maxFishSize), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<int>(_fishMenu, nameof(maxFishSize), true).SetValue(value);
            }
        }

        private int minFishingBiteTime
        {
            get
            {
                if (minFishingBiteTimeField == null)
                    minFishingBiteTimeField = Helper.Reflection.GetField<int>(typeof(FishingRod), nameof(minFishingBiteTime), true);

                return minFishingBiteTimeField.GetValue();
            }
            set
            {
                if (minFishingBiteTimeField == null)
                    minFishingBiteTimeField = Helper.Reflection.GetField<int>(typeof(FishingRod), nameof(minFishingBiteTime), true);

                minFishingBiteTimeField.SetValue(value);
            }
        }
        private bool perfect
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<bool>(_fishMenu, nameof(perfect), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<bool>(_fishMenu, nameof(perfect), true).SetValue(value);
            }
        }

        private bool treasure
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<bool>(_fishMenu, nameof(treasure), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<bool>(_fishMenu, nameof(treasure), true).SetValue(value);
            }
        }

        private bool treasureCaught
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<bool>(_fishMenu, nameof(treasureCaught), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<bool>(_fishMenu, nameof(treasureCaught), true).SetValue(value);
            }
        }

        private int whichFish
        {
            get
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                return Helper.Reflection.GetField<int>(_fishMenu, nameof(whichFish), true).GetValue();
            }
            set
            {
                if (_fishMenu == null) throw new NullReferenceException(nameof(_fishMenu));
                Helper.Reflection.GetField<int>(_fishMenu, nameof(whichFish), true).SetValue(value);
            }
        }
    }
}