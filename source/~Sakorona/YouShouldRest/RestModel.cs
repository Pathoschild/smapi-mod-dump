namespace TwilightShards.YouShouldRest
{
    public class RestModel
    {
        public string Conditions;
        public string Dialogue;

        public RestModel()
        {

        }

        public RestModel(string c, string d)
        {
            Conditions = c;
            Dialogue = d;
        }
    }
}
