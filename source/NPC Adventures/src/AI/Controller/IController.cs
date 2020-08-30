using PurrplingCore.Internal;

namespace NpcAdventure.AI.Controller
{
    internal interface IController : IUpdateable
    {
        bool IsIdle { get; }
        void Activate();
        void Deactivate();
    }
}
