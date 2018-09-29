using System.Collections.Generic;

namespace Denifia.Stardew.SendItems.Domain
{
    public class Farmer : BasePerson
    {
        public List<Friend> Friends { get; set; }

        public Farmer()
        {
            Friends = new List<Friend>();
        }
    }
}
