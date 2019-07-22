namespace BetterTrainLoot.Framework
{
    public interface IWeighted
    {
        double GetWeight();
        bool GetEnabled();
        bool IsValid();
    }
}
