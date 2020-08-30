namespace TwilightShards.LunarDisturbances
{
    public interface ILunarDisturbancesAPI
    {
        string GetCurrentMoonPhase();
        string GetPlainCurrMoonPhase();
        bool IsSolarEclipse();
        int GetMoonRise();
        int GetMoonSet();
        bool IsMoonUp(int time);
        int GetCycleLength();
        float GetBrightnessQuotient();
        int GetMoonZenith();
        float GetTrackPosition();
        void AlterEclipseOdds(float val);
        void ForceEclipse();
        void ForceEclipseTomorrow();
        bool IsEclipseTomorrow();
    }

    public class LunarDisturbancesAPI : ILunarDisturbancesAPI
    {
        private readonly SDVMoon IntMoon;

        public LunarDisturbancesAPI(SDVMoon OurMoon)
        {
            IntMoon = OurMoon;
        }

        public int GetMoonZenith()
        {
            return IntMoon.GetMoonZenith();
        }

        public float GetTrackPosition()
        {
            return IntMoon.GetTrackPosition();
        }

        public string GetPlainCurrMoonPhase()
        {
            return IntMoon.SimpleMoonPhase();
        }

        public string GetCurrentMoonPhase()
        {
            return IntMoon.DescribeMoonPhase();
        }

        public bool IsSolarEclipse()
        {
          return IntMoon.IsEclipse;
        }

        public int GetMoonRise()
        {
             return IntMoon.GetMoonRiseTime();
        }

        public int GetMoonSet()
        {
            return IntMoon.GetMoonSetTime();
        }

        public bool IsMoonUp(int time)
        {
            return IntMoon.IsMoonUp(time);
        }

        public int GetCycleLength() => IntMoon.GetMoonCycleLength;

        public float GetBrightnessQuotient()
        {
            return IntMoon.GetBrightnessQuotient();
        }

        public void AlterEclipseOdds(float val)
        {
            IntMoon.EclipseMods += val;
        }

        public void ForceEclipse()
        {
            IntMoon.TurnEclipseOn();
        }

        public void ForceEclipseTomorrow()
        {
            IntMoon.MoonTracker.IsEclipseTomorrow = true;
        }

        public bool IsEclipseTomorrow()
        {
            return IntMoon.MoonTracker.IsEclipseTomorrow;
        }
    }
}

