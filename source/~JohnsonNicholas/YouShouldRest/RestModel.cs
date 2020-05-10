using System.Collections.Generic;

namespace TwilightShards.YouShouldRest
{
    public class RestModel
    {
        public Dictionary<string, string> Keys;

        public RestModel()
        {

        }

        public RestModel(Dictionary<string,string> otherKeys)
        {
            foreach (var k in otherKeys)
            {
                Keys.Add(k.Key, k.Value);
            }
        }
    }
}
