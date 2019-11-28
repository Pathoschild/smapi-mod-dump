namespace ClimatesOfFerngillRebuild.Integrations
{
    public interface ILunarDisturbancesAPI
    {
        string GetCurrentMoonPhase();
        bool IsSolarEclipse();
        int GetMoonRise();
        int GetMoonSet();
        bool IsMoonUp(int time);
    }
}
