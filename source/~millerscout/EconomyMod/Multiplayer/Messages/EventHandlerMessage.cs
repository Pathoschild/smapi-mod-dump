namespace EconomyMod.Multiplayer.Messages
{
    public class EventHandlerMessage
    {
        public int tax;
        public bool isMale;

        public EventHandlerMessage(int tax, bool isMale)
        {
            this.tax = tax;
            this.isMale = isMale;
        }
    }
}
