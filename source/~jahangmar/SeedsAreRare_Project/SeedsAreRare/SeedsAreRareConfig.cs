using System;
namespace SeedsAreRare
{
    public class SeedsAreRareConfig     
    {
        public bool exclude_rare_seed { get; set; } = false;
        public bool exclude_traveling_merchant { get; set; } = false;
        public bool exclude_oasis { get; set; } = false;
        public bool exclude_pierre { get; set; } = false;
        public bool exclude_night_market { get; set; } = false;
        public bool exclude_egg_festival { get; set; } = false;
    }
}
