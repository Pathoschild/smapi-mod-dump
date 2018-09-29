using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterRanching
{
    public static class GameConstants
    {
        public struct Tools
        {
            public const string MilkPail = "Milk Pail";
            public const string Shears = "Shears";
        }
    }

    public enum RanchType
    {
        Milking,
        Shearing
    }
}
