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
    }

    public class LunarDisturbancesAPI : ILunarDisturbancesAPI
    {
        private readonly SDVMoon IntMoon;

        public LunarDisturbancesAPI(SDVMoon OurMoon)
        {
            IntMoon = OurMoon;
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
          return LunarDisturbances.IsEclipse;
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
    }
}

