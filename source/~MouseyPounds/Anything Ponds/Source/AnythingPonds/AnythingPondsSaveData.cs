using System.Collections.Generic;

namespace AnythingPonds
{
    class AnythingPondsSaveData
    {
        public IDictionary<string,int> EmptyPonds { get; set; }

        public AnythingPondsSaveData()
        {
            this.EmptyPonds = new Dictionary<string, int>();
        }
    }
}
