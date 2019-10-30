using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.AI.Controller
{
    internal interface IController : Internal.IUpdateable
    {
        bool IsIdle { get; }
        void Activate();
        void Deactivate();
    }
}
