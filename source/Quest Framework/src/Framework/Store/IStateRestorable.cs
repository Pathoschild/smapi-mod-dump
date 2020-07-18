using QuestFramework.Framework;

namespace QuestFramework.Framework.Store
{
    internal interface IStateRestorable : IStatefull
    {
        void RestoreState(StatePayload payload);
    }
}